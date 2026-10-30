using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
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
	/// Objects with an <c>x:Name</c> or <c>ID</c> get a private field unless the code-behind already declares one.
	/// Set <c>x:FieldModifier</c> on an object, or <c>EtoXamlFieldModifier</c> in the project, to change its access.
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
			=> IsXamlFile(file.Path) && IsCompiled(fileOptions, globalOptions);

		internal static bool IsXamlFile(string path) => path.EndsWith(".xeto", StringComparison.OrdinalIgnoreCase) || IsJson(path);

		internal static bool IsCompiled(AnalyzerConfigOptions fileOptions, AnalyzerConfigOptions globalOptions)
		{
			if (fileOptions.TryGetValue("build_metadata.AdditionalFiles.XamlCodeGen", out var fileValue) && !string.IsNullOrEmpty(fileValue))
				return IsTrue(fileValue);
			return globalOptions.TryGetValue("build_property.EtoXamlCodeGen", out var value) && IsTrue(value);
		}

		static bool IsTrue(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

		internal static bool IsJson(string path) => path?.EndsWith(".jeto", StringComparison.OrdinalIgnoreCase) == true;

		protected override void ConfigureOptions(XamlCodeGenOptions options, AdditionalText file, AnalyzerConfigOptions fileOptions, AnalyzerConfigOptions globalOptions)
		{
			base.ConfigureOptions(options, file, fileOptions, globalOptions);
			ConfigureEtoOptions(options, file, globalOptions);
		}

		internal static void ConfigureEtoOptions(XamlCodeGenOptions options, AdditionalText file, AnalyzerConfigOptions globalOptions)
		{
			options.MethodName = MethodName;
			options.MethodModifiers = "private";
			options.Namespaces[string.Empty] = EtoFormsNamespace;
			options.Namespaces["x"] = XamlLanguage.Xaml2006Namespace;
			options.IgnoredNamespaces.Add(DesignNamespace);
			globalOptions.TryGetValue("build_property.EtoXamlFieldModifier", out var fieldModifier);
			options.DefaultFieldModifier = string.IsNullOrWhiteSpace(fieldModifier) ? "private" : fieldModifier.Trim().ToLowerInvariant();

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

		readonly EtoXamlFieldWriter _fieldWriter = new EtoXamlFieldWriter();

		// keeps the fields of a file that fails to compile, so the code-behind doesn't break while it is being edited
		protected override void AfterGenerate(SourceProductionContext context, Compilation compilation, IReadOnlyList<XamlSourceFile> files)
		{
			EtoXamlCodeGenerator generator = null;
			var hintNames = new HashSet<string>();
			foreach (var file in files)
			{
				if (file.Result == null)
					continue;
				if (file.Result.Success)
				{
					_fieldWriter.Remember(file.File.Path, file.Text);
					continue;
				}
				generator = generator ?? (EtoXamlCodeGenerator)CreateCodeGenerator(compilation);
				_fieldWriter.Write(context, generator, file.File.Path, file.Text, file.Options, hintNames);
			}
		}
	}

	/// <summary>
	/// Declares fields for the named objects in .xeto and .jeto files that aren't compiled, which <c>XamlReader</c> sets when it parses them.
	/// </summary>
	[Generator(LanguageNames.CSharp)]
	public class EtoXamlFieldGenerator : IIncrementalGenerator
	{
		readonly EtoXamlFieldWriter _fieldWriter = new EtoXamlFieldWriter();

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var files = context.AdditionalTextsProvider
				.Combine(context.AnalyzerConfigOptionsProvider)
				.Where(pair => EtoXamlSourceGenerator.IsXamlFile(pair.Left.Path) && !EtoXamlSourceGenerator.IsCompiled(pair.Right.GetOptions(pair.Left), pair.Right.GlobalOptions))
				.Select((pair, token) =>
				{
					var options = new XamlCodeGenOptions { FilePath = pair.Left.Path };
					EtoXamlSourceGenerator.ConfigureEtoOptions(options, pair.Left, pair.Right.GlobalOptions);
					return (path: pair.Left.Path, text: pair.Left.GetText(token)?.ToString(), options);
				});

			context.RegisterSourceOutput(files.Collect().Combine(context.CompilationProvider), (spc, data) =>
			{
				if (data.Left.IsDefaultOrEmpty)
					return;
				var generator = new EtoXamlCodeGenerator(data.Right);
				var hintNames = new HashSet<string>();
				foreach (var file in data.Left)
				{
					spc.CancellationToken.ThrowIfCancellationRequested();
					if (file.text != null)
						_fieldWriter.Write(spc, generator, file.path, file.text, file.options, hintNames);
				}
			});
		}
	}

	/// <summary>Adds the fields-only source for a file, from the last version that could be read when it can't be now.</summary>
	class EtoXamlFieldWriter
	{
		// the IDE runs generators on every edit, so a file that can't be read yet keeps the fields from when it last could
		readonly ConcurrentDictionary<string, string> _lastReadable = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public void Remember(string path, string text) => _lastReadable[path] = text;

		public void Write(SourceProductionContext context, EtoXamlCodeGenerator generator, string path, string text, XamlCodeGenOptions options, HashSet<string> hintNames)
		{
			var fields = generator.GenerateFields(text, options);
			if (fields.source != null)
				Remember(path, text);
			else if (_lastReadable.TryGetValue(path, out var lastText))
				fields = generator.GenerateFields(lastText, options);
			if (fields.source == null)
				return;

			var baseName = fields.rootClass.ToDisplayString().Replace('<', '[').Replace('>', ']');
			var hintName = baseName;
			for (int i = 1; !hintNames.Add(hintName); i++)
				hintName = baseName + i;
			context.AddSource(hintName + ".xaml.fields.g.cs", SourceText.From(fields.source, Encoding.UTF8));
		}
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

		/// <summary>Declares fields for the named objects in the file, without the method that creates them, or null when it can't be read.</summary>
		public (INamedTypeSymbol rootClass, string source) GenerateFields(string text, XamlCodeGenOptions options)
		{
			XamlObjectNode root;
			try
			{
				PrepareOptions(options);
				root = Read(text, options);
			}
			catch (Exception)
			{
				return default;
			}
			var rootClass = options.RootClassName != null ? Compilation.GetTypeByMetadataName(options.RootClassName) : null;
			if (root == null || rootClass == null || !IsPartial(rootClass))
				return default;

			var fields = new List<string>();
			AddFields(root, rootClass, true, new HashSet<string>(), fields, options);
			return (rootClass, BuildFieldsSource(rootClass, fields));
		}

		// matches the fields XamlCodeEmitter declares, skipping objects whose type isn't known yet
		void AddFields(XamlObjectNode node, INamedTypeSymbol rootClass, bool isRoot, HashSet<string> names, List<string> fields, XamlCodeGenOptions options)
		{
			if (node.Type != null && !isRoot && !node.Type.IsMarkupExtension)
			{
				var alias = node.Type.GetAliasedProperty(XamlLanguage.Name);
				var name = node.GetText(XamlLanguage.Name) ?? (alias != null ? node.GetText(alias) : null);
				var type = SchemaContext.GetSymbol(node.Type);
				if (name != null && type != null && SyntaxFacts.IsValidIdentifier(name) && names.Add(name) && !HasMember(rootClass, name))
				{
					var modifier = node.GetText(XamlLanguage.FieldModifier);
					if (modifier == null)
						modifier = options.DefaultFieldModifier;
					else if (string.Equals(modifier, "NotPublic", StringComparison.OrdinalIgnoreCase))
						modifier = "internal";
					else
						modifier = modifier.ToLowerInvariant();
					fields.Add($"{modifier} {XamlCodeEmitter.TypeName(type)} {Identifier(name)};");
				}
			}
			foreach (var member in node.Members)
				foreach (var value in member.Values)
					if (value.Object != null)
						AddFields(value.Object, rootClass, false, names, fields, options);
		}

		bool HasMember(INamedTypeSymbol rootClass, string name)
		{
			for (ITypeSymbol type = rootClass; type != null; type = type.BaseType)
				if (type.GetMembers(name).Any(m => !m.IsStatic && (m is IFieldSymbol || m is IPropertySymbol) && Compilation.IsSymbolAccessibleWithin(m, rootClass)))
					return true;
			return false;
		}

		static bool IsPartial(INamedTypeSymbol type)
		{
			for (var t = type; t != null; t = t.ContainingType)
				if (!t.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is TypeDeclarationSyntax syntax && syntax.Modifiers.Any(SyntaxKind.PartialKeyword)))
					return false;
			return true;
		}

		static string Identifier(string name) => SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name;

		static string BuildFieldsSource(INamedTypeSymbol rootClass, List<string> fields)
		{
			var types = new List<INamedTypeSymbol>();
			for (var t = rootClass; t != null; t = t.ContainingType)
				types.Insert(0, t);

			var sb = new StringBuilder();
			sb.AppendLine("// <auto-generated/>");
			sb.AppendLine("#pragma warning disable");
			sb.AppendLine("#nullable disable");
			sb.AppendLine();
			var indent = string.Empty;
			var ns = rootClass.ContainingNamespace;
			if (ns != null && !ns.IsGlobalNamespace)
			{
				sb.AppendLine($"namespace {ns.ToDisplayString()}");
				sb.AppendLine("{");
				indent += "\t";
			}
			foreach (var type in types)
			{
				var kind = type.IsRecord ? "record" : type.TypeKind == TypeKind.Struct ? "struct" : "class";
				var typeParameters = type.TypeParameters.Length > 0 ? "<" + string.Join(", ", type.TypeParameters.Select(r => r.Name)) + ">" : string.Empty;
				sb.AppendLine($"{indent}partial {kind} {type.Name}{typeParameters}");
				sb.AppendLine($"{indent}{{");
				indent += "\t";
			}
			foreach (var field in fields)
				sb.AppendLine(indent + field);
			while (indent.Length > 0)
			{
				indent = indent.Substring(1);
				sb.AppendLine($"{indent}}}");
			}
			return sb.ToString();
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
