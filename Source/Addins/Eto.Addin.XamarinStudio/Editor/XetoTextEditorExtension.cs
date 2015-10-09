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
using Eto.Designer.Completion;

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
			return p.OfType<XElement>().Select(r => r.Name.Name);
		}

		protected override void GetElementCompletions(CompletionDataList list)
		{
			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);
			var prefix = currentPath.OfType<XElement>().Select(r => r.Name.Prefix).LastOrDefault();
			foreach (var completion in Completion.GetCompletions(namespaces, prefix))
			{
				foreach (var item in completion.GetClasses(path))
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
			foreach (var completion in Completion.GetCompletions(namespaces, attributedOb.Name.Prefix))
			{
				foreach (var item in completion.GetProperties(objectName, path).Where(r => !existingAtts.ContainsKey(r.Name)))
				{
					var xmlCompletion = new XmlCompletionData(item.Name, item.Description, XmlCompletionData.DataType.XmlAttribute);
					xmlCompletion.Icon = GetIcon(item.Type);
					list.Add(xmlCompletion);
				}
			}
			return list;
		}

		protected override CompletionDataList GetAttributeValueCompletions(IAttributedXObject attributedOb, XAttribute att)
		{
			var list = base.GetAttributeValueCompletions(attributedOb, att) ?? new CompletionDataList();

			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);
			var objectName = attributedOb.Name.Name;
			foreach (var completion in Completion.GetCompletions(namespaces, attributedOb.Name.Prefix))
			{
				foreach (var item in completion.GetPropertyValues(objectName, att.Name.FullName, path))
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
				case CompletionType.Field:
					return Stock.Field;
				case CompletionType.Literal:
					return Stock.Literal;
				case CompletionType.Attribute:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}

