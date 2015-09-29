using System;
using MonoDevelop.Xml.Editor;
using System.Linq;
using MonoDevelop.Xml.Completion;
using System.Reflection;
using System.Collections.Generic;
using MonoDevelop.Xml.Dom;
using MonoDevelop.Ide.CodeCompletion;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using Eto.Designer;

namespace Eto.Addin.XamarinStudio.Editor
{
	public class XetoTextEditorExtension : BaseXmlEditorExtension
	{
		static IEnumerable<CompletionNamespace> GetNamespaces(List<XObject> p)
		{
			var root = p.FirstOrDefault() as XElement;
			var namespaces = root.Attributes.Where(r => r.Name.Name == "xmlns" || r.Name.Prefix == "xmlns").Select(r => new CompletionNamespace {
				Prefix = r.Name.Name == "xmlns" ? "" : r.Name.Name + ":",
				Namespace = r.Value
			});
			return namespaces;
		}

		static IEnumerable<string> GetPath(List<XObject> p)
		{
			return p.OfType<XElement>().Select(r => r.Name.FullName);
		}

		protected override void GetElementCompletions(CompletionDataList list)
		{
			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);

			foreach (var completion in Completion.GetCompletions(namespaces))
			{
				foreach (var item in completion.GetElements(path))
				{
					var xmlCompletion = new XmlCompletionData(item.Name, item.Description, XmlCompletionData.DataType.XmlAttribute);
					xmlCompletion.Icon = Stock.Class;
					list.Add(xmlCompletion);
				}
			}
			BaseXmlEditorExtension.AddMiscBeginTags(list);
			base.GetElementCompletions(list);
		}

		protected override CompletionDataList GetAttributeCompletions(IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			var list = base.GetAttributeCompletions(attributedOb, existingAtts) ?? new CompletionDataList();

			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);
			var objectName = attributedOb.Name.Name;
			foreach (var completion in Completion.GetCompletions(namespaces))
			{
				foreach (var item in completion.GetAttributes(objectName, path).Where(r => !existingAtts.ContainsKey(r.Name)))
				{
					var xmlCompletion = new XmlCompletionData(item.Name, item.Description, XmlCompletionData.DataType.XmlAttribute);
					xmlCompletion.Icon = GetIcon(item.Type);
					list.Add(xmlCompletion);
				}
			}
			return list;
		}

		IconId GetIcon(CompletionType type)
		{
			switch (type)
			{
				case CompletionType.Class:
					return Stock.Class;
				case CompletionType.Property:
					return Stock.Property;
				case CompletionType.Event:
					return Stock.Event;
				case CompletionType.Attribute:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}

