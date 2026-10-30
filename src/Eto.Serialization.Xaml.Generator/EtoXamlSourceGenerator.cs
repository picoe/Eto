using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Portable.Xaml;
using Portable.Xaml.CodeGen;
using Portable.Xaml.CodeGen.Schema;
using Portable.Xaml.Json;
using Eto.Serialization.Xaml;

namespace Eto.Serialization.Xaml.Generator
{
	/// <summary>
	/// Compiles .xeto and .jeto files into the code-behind class when the project sets <c>EtoXamlCodeGen</c> to true.
	/// </summary>
	/// <remarks>
	/// <c>XamlReader.Load(this)</c> (or <c>JetoReader.Load(this)</c>) then runs the generated method instead of parsing the .xeto at runtime.
	/// </remarks>
	[Generator(LanguageNames.CSharp)]
	public class EtoXamlSourceGenerator : XamlSourceGenerator
	{
		public const string EtoFormsNamespace = EtoJsonReader.EtoFormsNamespace;
		public const string DesignNamespace = EtoJsonReader.DesignNamespace;

		/// <summary>Name of the generated method, which XamlReader looks up.</summary>
		public const string MethodName = "InitializeXeto";

		protected override string DiagnosticPrefix => "ETOXAML";

		protected override bool ShouldGenerate(AdditionalText file, AnalyzerConfigOptions fileOptions, AnalyzerConfigOptions globalOptions)
		{
			if (!file.Path.EndsWith(".xeto", StringComparison.OrdinalIgnoreCase) && !IsJson(file.Path))
				return false;
			if (fileOptions.TryGetValue("build_metadata.AdditionalFiles.XamlCodeGen", out var fileValue) && !string.IsNullOrEmpty(fileValue))
				return IsTrue(fileValue);
			return globalOptions.TryGetValue("build_property.EtoXamlCodeGen", out var value) && IsTrue(value);
		}

		static bool IsTrue(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

		internal static bool IsJson(string path) => path?.EndsWith(".jeto", StringComparison.OrdinalIgnoreCase) == true;

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

		protected override XamlReader CreateReader(string text, XamlCodeGenOptions options)
		{
			if (!EtoXamlSourceGenerator.IsJson(options.FilePath))
				return base.CreateReader(text, options);

			var settings = new XamlJsonReaderSettings { ProvideLineInfo = true };
			EtoJsonReader.AddNamespaces(settings);
			foreach (var ns in options.Namespaces)
				settings.AddNamespace(ns.Key, ns.Value);
			// .jeto files don't name their root type, so it is the class they're loaded into
			var rootClass = options.RootClassName != null ? Compilation.GetTypeByMetadataName(options.RootClassName) : null;
			if (rootClass != null)
				settings.RootType = SchemaContext.GetXamlType(rootClass);
			return new EtoJsonReader(new StringReader(text), SchemaContext, settings);
		}

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
			if (TryEmitIndirectBinding(node, target, out result))
				return true;
			return base.TryEmitMarkupExtension(node, expectedType, target, out result);
		}

		protected override void EmitMember(XamlObjectNode node, XamlExpression target, XamlMemberNode memberNode)
		{
			if (!TryEmitDirectBinding(target, memberNode))
				base.EmitMember(node, target, memberNode);
		}

		const string BindingExtensionName = "Eto.Serialization.Xaml.Extensions.BindingExtension";
		static readonly string[] s_bindingMembers = { "Path", "Converter", "ConverterParameter", "ConverterCulture", "Mode" };

		// the parts of a {Binding}, or null when it uses something only BindingExtension at runtime handles
		Dictionary<string, XamlValueNode> GetBindingParts(XamlObjectNode node)
		{
			if (node == null || Context.GetSymbol(node.Type)?.ToDisplayString() != BindingExtensionName)
				return null;
			var parts = new Dictionary<string, XamlValueNode>();
			foreach (var member in node.Members)
			{
				var name = member.Member == XamlLanguage.PositionalParameters ? "Path" : member.Member.Name;
				if (member.Values.Count != 1 || !s_bindingMembers.Contains(name) || parts.ContainsKey(name))
					return null;
				parts[name] = member.Values[0];
			}
			var path = parts.TryGetValue("Path", out var p) ? p.Text : null;
			if (string.IsNullOrEmpty(path) || (parts.TryGetValue("Converter", out var converter) && converter.Object == null))
				return null;
			return parts;
		}

		string EmitBindingPart(XamlObjectNode node, Dictionary<string, XamlValueNode> parts, string name)
		{
			if (!parts.TryGetValue(name, out var value))
				return null;
			var member = (SymbolXamlMember)node.Type.GetMember(name);
			return EmitValue(value, member.ValueType, member, null, node).Code;
		}

		// e.g. ItemTextBinding="{Binding Name}" creates the binding instead of BindingExtension doing it by reflection
		bool TryEmitIndirectBinding(XamlObjectNode node, XamlValueTarget target, out XamlExpression result)
		{
			result = default;
			var propertyType = target?.Member?.Property?.Type as INamedTypeSymbol;
			if (propertyType == null || !propertyType.IsGenericType || !IsEtoFormsType(propertyType.OriginalDefinition, "IIndirectBinding`1"))
				return false;
			var parts = GetBindingParts(node);
			if (parts == null)
				return false;

			var valueType = propertyType.TypeArguments[0];
			var path = Literal(parts["Path"].Text);
			var converter = EmitBindingPart(node, parts, "Converter");
			if (converter == null)
			{
				var bindingType = Compilation.GetTypeByMetadataName("Eto.Forms.PropertyBinding`1").Construct(valueType);
				result = new XamlExpression($"new {TypeName(bindingType)}({path}, false)", bindingType);
			}
			else
			{
				var parameter = EmitBindingPart(node, parts, "ConverterParameter") ?? "null";
				var culture = EmitBindingPart(node, parts, "ConverterCulture") ?? "null";
				var bindingType = Compilation.GetTypeByMetadataName("Eto.Forms.IndirectBinding`1").Construct(valueType);
				result = new XamlExpression($"global::Eto.Forms.Binding.Property<object>({path}).Convert<{TypeName(valueType)}>({converter}, {parameter}, {culture})", bindingType);
			}
			return true;
		}

		// e.g. Text="{Binding Name}" binds the control directly, instead of BindingExtension binding it by reflection
		// and returning the current value to set the property to again
		bool TryEmitDirectBinding(XamlExpression target, XamlMemberNode memberNode)
		{
			if (!(memberNode.Member is SymbolXamlMember member) || member.Property == null || member.IsAttachable || memberNode.Values.Count != 1)
				return false;
			var node = memberNode.Values[0].Object;
			var propertyType = member.Property.Type;
			if (target.Type == null || !IsOrImplements(target.Type, "IBindable") || IsOrImplements(propertyType, "IBinding"))
				return false;
			var parts = GetBindingParts(node);
			if (parts == null)
				return false;

			var path = Literal(parts["Path"].Text);
			var mode = EmitBindingPart(node, parts, "Mode");
			var modeArg = mode != null ? ", " + mode : string.Empty;
			var converter = EmitBindingPart(node, parts, "Converter");
			var typedBinding = converter == null ? FindTypedBinding(target.Type, member.Property) : null;
			if (typedBinding != null)
			{
				MarkLine(memberNode.LineInfo);
				// e.g. TextBox.TextBinding, which uses the control's own events instead of reflection
				WriteLine($"{target.Code}.{typedBinding.Name}.BindDataContext(global::Eto.Forms.Binding.Property<{TypeName(propertyType)}>({path}){modeArg});");
				return true;
			}

			var source = $"global::Eto.Forms.Binding.Property<object>({path})";
			if (converter != null)
			{
				var parameter = EmitBindingPart(node, parts, "ConverterParameter") ?? "null";
				var culture = EmitBindingPart(node, parts, "ConverterCulture") ?? "null";
				source += $".Convert({converter}, typeof({TypeName(propertyType)}), {parameter}, {culture})";
			}
			MarkLine(memberNode.LineInfo);
			WriteLine($"global::Eto.Forms.BindableExtensions.BindDataContext<object>({target.Code}, global::Eto.Forms.Binding.Property<object>({Literal(member.Name)}), {source}{modeArg});");
			return true;
		}

		// the control's BindableBinding for the property, e.g. TextBinding for Text
		IPropertySymbol FindTypedBinding(ITypeSymbol controlType, IPropertySymbol property)
		{
			IPropertySymbol binding = null;
			for (var type = controlType; type != null && binding == null; type = type.BaseType)
				binding = type.GetMembers(property.Name + "Binding").OfType<IPropertySymbol>()
					.FirstOrDefault(r => !r.IsStatic && Compilation.IsSymbolAccessibleWithin(r, RootClass));
			var bindingType = binding?.Type as INamedTypeSymbol;
			if (bindingType == null || !bindingType.IsGenericType || !IsEtoFormsType(bindingType.OriginalDefinition, "BindableBinding`2"))
				return null;
			return SymbolEqualityComparer.Default.Equals(bindingType.TypeArguments[1], property.Type) ? binding : null;
		}

		static bool IsEtoFormsType(ITypeSymbol type, string metadataName)
			=> type.MetadataName == metadataName && type.ContainingNamespace?.ToDisplayString() == "Eto.Forms";

		static bool IsOrImplements(ITypeSymbol type, string interfaceName)
			=> IsEtoFormsType(type, interfaceName) || type.AllInterfaces.Any(r => IsEtoFormsType(r, interfaceName));

		// type name, form shown in errors, valid component counts, and whether components are float
		static readonly (string name, string form, int[] counts, bool isFloat)[] s_geometryTypes =
		{
			("Size", "'size' or 'width, height'", new[] { 1, 2 }, false),
			("SizeF", "'size' or 'width, height'", new[] { 1, 2 }, true),
			("Point", "'x, y'", new[] { 2 }, false),
			("PointF", "'x, y'", new[] { 2 }, true),
			("Rectangle", "'x, y, width, height'", new[] { 4 }, false),
			("RectangleF", "'x, y, width, height'", new[] { 4 }, true),
			("Padding", "'all', 'horizontal, vertical' or 'left, top, right, bottom'", new[] { 1, 2, 4 }, false),
			("PaddingF", "'all', 'horizontal, vertical' or 'left, top, right, bottom'", new[] { 1, 2, 4 }, true),
		};

		public override XamlExpression ConvertText(string text, ITypeSymbol type, SymbolXamlMember member, XamlObjectNode scope, XamlLineInfo lineInfo, bool allowAsIs = false)
		{
			if (type?.ContainingNamespace?.ToDisplayString() == "Eto.Drawing"
				&& !HasTypeConverter(member?.Property)
				&& TryConvertGeometry(text, type, lineInfo, out var result))
				return result;
			return base.ConvertText(text, type, member, scope, lineInfo, allowAsIs);
		}

		// a converter on the property itself wins over the type's, like the runtime
		static bool HasTypeConverter(ISymbol property)
			=> property?.GetAttributes().Any(r => r.AttributeClass?.ToDisplayString() == "System.ComponentModel.TypeConverterAttribute") == true;

		// builds the value at compile time, instead of parsing it with XamlConverters when loaded
		bool TryConvertGeometry(string text, ITypeSymbol type, XamlLineInfo lineInfo, out XamlExpression result)
		{
			result = default;
			var info = s_geometryTypes.FirstOrDefault(r => r.name == type.Name);
			if (info.name == null)
				return false;

			var parts = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (Array.IndexOf(info.counts, parts.Length) < 0)
				throw Invalid(text, info.name, info.form, lineInfo);

			var args = new string[parts.Length];
			for (int i = 0; i < parts.Length; i++)
			{
				if (info.isFloat && float.TryParse(parts[i], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var f))
					args[i] = FloatLiteral(f);
				else if (!info.isFloat && int.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
					args[i] = n.ToString(CultureInfo.InvariantCulture);
				else
					throw Invalid(text, info.name, info.form, lineInfo);
			}
			result = new XamlExpression($"new {TypeName(type)}({string.Join(", ", args)})", type);
			return true;
		}

		XamlCodeGenException Invalid(string text, string typeName, string form, XamlLineInfo lineInfo)
			=> Error($"Cannot parse value '{text}' as {typeName}. Should be in the form of {form}", lineInfo);

		static string FloatLiteral(float value)
		{
			if (float.IsNaN(value))
				return "float.NaN";
			if (float.IsPositiveInfinity(value))
				return "float.PositiveInfinity";
			if (float.IsNegativeInfinity(value))
				return "float.NegativeInfinity";
			return value.ToString("R", CultureInfo.InvariantCulture) + "f";
		}
	}
}
