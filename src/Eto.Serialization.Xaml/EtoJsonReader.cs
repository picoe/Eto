using System.Collections.Generic;
using System.IO;
using Portable.Xaml;
using Portable.Xaml.Json;
using Portable.Xaml.Schema;

namespace Eto.Serialization.Xaml
{
	/// <summary>
	/// Reads .jeto files, which can leave out the type of layout and list items, or give them as strings or arrays.
	/// </summary>
	/// <remarks>
	/// Also compiled into the source generator, so it only uses Portable.Xaml.
	/// </remarks>
	class EtoJsonReader : XamlJsonReader
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string DesignNamespace = "http://schema.picoe.ca/eto.forms/design";
		public const string DrawingNamespace = "clr-namespace:Eto.Drawing;assembly=Eto";

		public EtoJsonReader(TextReader reader, XamlSchemaContext context, XamlJsonReaderSettings settings)
			: base(reader, context, settings)
		{
		}

		/// <summary>Adds the prefixes .jeto files use without declaring them.</summary>
		public static void AddNamespaces(XamlJsonReaderSettings settings)
		{
			settings.AddNamespace(null, EtoFormsNamespace);
			settings.AddNamespace("x", XamlLanguage.Xaml2006Namespace);
			settings.AddNamespace("d", DesignNamespace);
			settings.AddNamespace("drawing", DrawingNamespace);
		}

		XamlType Get(string name) => SchemaContext.GetXamlType(new XamlTypeName(EtoFormsNamespace, name));

		bool Is(XamlType type, string name) => type == Get(name);

		protected override XamlType GetObjectType(XamlType expectedType, IList<string> propertyNames)
		{
			if (Is(expectedType, "TableCell") || Is(expectedType, "TableRow"))
			{
				if (propertyNames.Contains("Rows"))
					return Get("TableLayout");
				if (propertyNames.Contains("Cells"))
					return Get("TableRow");
				if (propertyNames.Contains("Control"))
					return Get("TableCell");
			}
			else if (Is(expectedType, "DynamicItem"))
			{
				if (propertyNames.Contains("Rows"))
					return Get("DynamicTable");
				if (propertyNames.Contains("Control"))
					return Get("DynamicControl");
				return null;
			}
			else if (Is(expectedType, "IListItem"))
				return Get("ListItem");
			return expectedType;
		}

		protected override XamlMember GetArrayMember(XamlType expectedType, out XamlType objectType)
		{
			objectType = null;
			if (Is(expectedType, "TableCell"))
				objectType = Get("TableLayout");
			else if (Is(expectedType, "DynamicItem"))
				objectType = Get("DynamicTable");
			else if (Is(expectedType, "TableRow"))
			{
				objectType = Get("TableRow");
				return objectType.GetMember("Cells");
			}
			return objectType?.GetMember("Rows");
		}

		protected override XamlMember GetTextMember(XamlType expectedType, out XamlType objectType)
		{
			if (Is(expectedType, "TableCell") || Is(expectedType, "TableRow") || Is(expectedType, "DynamicItem") || Is(expectedType, "DynamicRow"))
				objectType = Get("Label");
			else if (Is(expectedType, "IListItem"))
				objectType = Get("ListItem");
			else
				objectType = null;
			return objectType?.GetMember("Text");
		}
	}
}
