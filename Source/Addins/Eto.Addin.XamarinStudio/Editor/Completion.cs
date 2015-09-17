using System;
using System.Collections.Generic;
using MonoDevelop.Xml.Dom;
using MonoDevelop.Ide.CodeCompletion;
using System.Linq;
using MonoDevelop.Xml.Completion;
using System.Reflection;

namespace Eto.Addin.XamarinStudio.Editor
{
	abstract class Completion
	{
		public string Prefix { get; set; }

		public abstract void GetElements(List<XObject> path, CompletionDataList list);

		public abstract void GetAttributes(CompletionDataList list, IAttributedXObject attributedOb, List<XObject> path, Dictionary<string, string> existingAtts);

		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2009/xaml";

		public static IEnumerable<Completion> GetCompletions(List<XObject> path)
		{
			var root = path.FirstOrDefault() as XElement;
			if (root == null)
				yield break;

			foreach (var attrib in root.Attributes.Where(r => r.Name.Name == "xmlns" || r.Name.Prefix == "xmlns"))
			{
				var prefix = attrib.Name.Name == "xmlns" ? "" : attrib.Name.Name + ":";
				if (attrib.Value == EtoFormsNamespace)
				{
					yield return new TypeCompletion
					{
						Prefix = prefix,
						Assembly = typeof(Eto.Widget).Assembly,
						Namespace = "Eto.Forms"
					};
				}
				if (attrib.Value == XamlNamespace)
				{
					yield return new XamlCompletion { Prefix = prefix };
				}
			}
		}
	}

	class XamlCompletion : Completion
	{
		public override void GetElements(List<XObject> path, CompletionDataList list)
		{
		}
		public override void GetAttributes(CompletionDataList list, IAttributedXObject attributedOb, List<XObject> path, Dictionary<string, string> existingAtts)
		{
			var prefix = Prefix ?? "";
			if (path.OfType<XElement>().Count() == 1)
				list.Add(new XmlCompletionData(prefix + "Class"));
			list.Add(new XmlCompletionData(prefix + "Name"));
		}
	}

	class TypeCompletion : Completion
	{
		public Assembly Assembly { get; set; }

		public string Namespace { get; set; }

		public override void GetElements(List<XObject> path, CompletionDataList list)
		{
			var prefix = Prefix ?? "";
			var types = Assembly.ExportedTypes;
			var results = types.Where(r => r.Namespace == Namespace && typeof(Eto.Forms.Control).IsAssignableFrom(r));
			results = results.Where(r => !r.IsGenericType && !r.IsAbstract);

			foreach (var result in results)
			{
				list.Add(new XmlCompletionData(prefix + result.Name, XmlCompletionData.DataType.XmlElement));
			}
		}

		public override void GetAttributes(CompletionDataList list, IAttributedXObject attributedOb, List<XObject> path, Dictionary<string, string> existingAtts)
		{
			var fullName = Namespace + "." + attributedOb.Name.Name;
			var type = Assembly.GetType(fullName, false);
			if (type != null)
			{
				foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					if (existingAtts.ContainsKey(prop.Name))
						continue;
					list.Add(new XmlCompletionData(prop.Name, XmlCompletionData.DataType.XmlAttribute));
				}
				foreach (var evt in type.GetEvents(BindingFlags.Public | BindingFlags.Instance))
				{
					if (existingAtts.ContainsKey(evt.Name))
						continue;
					list.Add(new XmlCompletionData(evt.Name, XmlCompletionData.DataType.XmlAttribute));
				}
			}
		}
	}
}

