using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Portable.Xaml;
using Portable.Xaml.CodeGen;
using Portable.Xaml.CodeGen.Schema;

namespace Eto.Serialization.Xaml.Generator
{
	/// <summary>
	/// Compiles .xeto files into the code-behind class when the project sets <c>EtoXamlCodeGen</c> to true.
	/// </summary>
	/// <remarks>
	/// <c>XamlReader.Load(this)</c> then runs the generated method instead of parsing the .xeto at runtime.
	/// </remarks>
	[Generator(LanguageNames.CSharp)]
	public class EtoXamlSourceGenerator : XamlSourceGenerator
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string DesignNamespace = "http://schema.picoe.ca/eto.forms/design";

		/// <summary>Name of the generated method, which XamlReader looks up.</summary>
		public const string MethodName = "InitializeXeto";

		protected override string DiagnosticPrefix => "ETOXAML";

		protected override bool ShouldGenerate(AdditionalText file, AnalyzerConfigOptions fileOptions, AnalyzerConfigOptions globalOptions)
		{
			if (!file.Path.EndsWith(".xeto", StringComparison.OrdinalIgnoreCase))
				return false;
			if (fileOptions.TryGetValue("build_metadata.AdditionalFiles.XamlCodeGen", out var fileValue) && !string.IsNullOrEmpty(fileValue))
				return IsTrue(fileValue);
			return globalOptions.TryGetValue("build_property.EtoXamlCodeGen", out var value) && IsTrue(value);
		}

		static bool IsTrue(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

		protected override void ConfigureOptions(XamlCodeGenOptions options, AdditionalText file, AnalyzerConfigOptions fileOptions, AnalyzerConfigOptions globalOptions)
		{
			base.ConfigureOptions(options, file, fileOptions, globalOptions);
			options.MethodName = MethodName;
			options.MethodModifiers = "private";
			// XamlReader only sets members that exist in the code-behind
			options.GenerateFields = false;
			options.Namespaces[string.Empty] = EtoFormsNamespace;
			options.Namespaces["x"] = XamlLanguage.Xaml2006Namespace;
			options.IgnoredNamespaces.Add(DesignNamespace);

			if (options.RootClassName == null)
				options.RootClassName = GetResourceClassName(file.Path, globalOptions);
		}

		/// <summary>XamlReader finds the .xeto by the class name, so the class is the manifest resource name without the extension.</summary>
		static string GetResourceClassName(string path, AnalyzerConfigOptions globalOptions)
		{
			globalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
			globalOptions.TryGetValue("build_property.ProjectDir", out var projectDir);
			var name = Path.GetFileNameWithoutExtension(path);
			if (!string.IsNullOrEmpty(projectDir))
			{
				var fullDir = Path.GetFullPath(Path.GetDirectoryName(path));
				var fullProject = Path.GetFullPath(projectDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				if (fullDir.StartsWith(fullProject, StringComparison.OrdinalIgnoreCase))
				{
					var relative = fullDir.Substring(fullProject.Length).Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
					if (relative.Length > 0)
						name = relative.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.') + "." + name;
				}
			}
			return string.IsNullOrEmpty(rootNamespace) ? name : rootNamespace + "." + name;
		}

		protected override XamlCodeGenerator CreateCodeGenerator(Compilation compilation) => new EtoXamlCodeGenerator(compilation);
	}

	class EtoXamlCodeGenerator : XamlCodeGenerator
	{
		public EtoXamlCodeGenerator(Compilation compilation)
			: base(compilation)
		{
		}

		protected override SymbolXamlSchemaContext CreateSchemaContext() => new EtoSymbolXamlSchemaContext(Compilation);

		protected override XamlCodeEmitter CreateEmitter(XamlCodeGenOptions options, INamedTypeSymbol rootClass)
			=> new EtoXamlCodeEmitter(SchemaContext, options, rootClass);

		protected override void PrepareOptions(XamlCodeGenOptions options)
		{
			// a MyForm.xeto.cs code-behind names the class, even when the resource name doesn't match
			var codeBehind = Compilation.SyntaxTrees.FirstOrDefault(r => string.Equals(r.FilePath, options.FilePath + ".cs", StringComparison.OrdinalIgnoreCase));
			var declaration = codeBehind?.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
			if (declaration != null && Compilation.GetSemanticModel(codeBehind).GetDeclaredSymbol(declaration) is INamedTypeSymbol type)
				options.RootClassName = GetMetadataName(type);

			var rootClassName = options.RootClassName;
			var idx = rootClassName?.LastIndexOf('.') ?? -1;
			if (idx > 0 && !options.Namespaces.ContainsKey("local"))
				options.Namespaces["local"] = "clr-namespace:" + rootClassName.Substring(0, idx);
		}

		static string GetMetadataName(INamedTypeSymbol type)
		{
			if (type.ContainingType != null)
				return GetMetadataName(type.ContainingType) + "+" + type.MetadataName;
			var ns = type.ContainingNamespace;
			return ns == null || ns.IsGlobalNamespace ? type.MetadataName : ns.ToDisplayString() + "." + type.MetadataName;
		}
	}

	class EtoSymbolXamlSchemaContext : SymbolXamlSchemaContext
	{
		public EtoSymbolXamlSchemaContext(Compilation compilation)
			: base(compilation)
		{
		}

		// Eto core defines its own ContentProperty and RuntimeNameProperty attributes
		public override bool IsXamlAttribute(INamedTypeSymbol attributeClass, string name)
			=> base.IsXamlAttribute(attributeClass, name)
			|| (attributeClass.Name == name && attributeClass.ContainingNamespace?.ToDisplayString() == "Eto");
	}

	class EtoXamlCodeEmitter : XamlCodeEmitter
	{
		public EtoXamlCodeEmitter(SymbolXamlSchemaContext context, XamlCodeGenOptions options, INamedTypeSymbol rootClass)
			: base(context, options, rootClass)
		{
		}

		protected override bool TryEmitMarkupExtension(XamlObjectNode node, ITypeSymbol expectedType, XamlValueTarget target, out XamlExpression result)
		{
			var type = Context.GetSymbol(node.Type);
			if (type?.ToDisplayString() == "Eto.Serialization.Xaml.Extensions.StaticResourceExtension")
			{
				// resolve against the Properties of this object or its parents, like the runtime ambient lookup
				var key = node.GetMember(XamlLanguage.PositionalParameters)?.Values.FirstOrDefault()?.Text
					?? node.Members.FirstOrDefault(r => r.Member.Name == "ResourceKey")?.Values.FirstOrDefault()?.Text;
				if (key != null && TryFindResource(key, out result))
					return true;
				Warning($"StaticResource '{key}' was not found and will be null", node.LineInfo);
				result = new XamlExpression("null", null);
				return true;
			}
			return base.TryEmitMarkupExtension(node, expectedType, target, out result);
		}
	}
}
