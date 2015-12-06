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
using MonoDevelop.Ide.Gui.Content;
using System.Xml;

namespace Eto.Addin.XamarinStudio.Editor
{
	public class XetoTextEditorExtension : BaseXmlEditorExtension
	{
		static IEnumerable<CompletionNamespace> GetNamespaces(IEnumerable<XObject> p)
		{
			return p.OfType<XElement>()
				.SelectMany(e => e.Attributes
					.Where(r => r.Name.Name == "xmlns" || r.Name.Prefix == "xmlns")
					.Select(r => new CompletionNamespace
					{
						Prefix = r.Name.Name == "xmlns" ? "" : r.Name.Name,
						Namespace = r.Value
					}));
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
			var completions = Completion.GetCompletions(namespaces).ToList();
			var filter = completions.Select(r => r.GetFilter(path)).FirstOrDefault(r => r != null);
			foreach (var completion in completions)
			{
				foreach (var item in completion.GetClasses(path, filter))
				{
					var xmlCompletion = new XmlCompletionData(item.Name, item.Description, XmlCompletionData.DataType.XmlElement);
					xmlCompletion.Icon = Stock.Class;
					list.Add(xmlCompletion);
				}
			}
			BaseXmlEditorExtension.AddMiscBeginTags(list);
			base.GetElementCompletions(list);
		}

		public override bool KeyPress(Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			var buffer = EditableBuffer;
			if (CompletionWindowManager.IsVisible)
			{
				// do some things to minimize keystrokes with code completion
				if (keyChar == '=')
				{
					// we're in an attribute completion, so automatically add the quote and show completions (if available)
					var ret = base.KeyPress(key, keyChar, modifier);
					if (!ret)
					{
						base.KeyPress((Gdk.Key)0, '"', Gdk.ModifierType.None);
						buffer.InsertText(buffer.CursorPosition, "\"");
						buffer.CursorPosition--;
					}
					return ret;
				}
				if (key == Gdk.Key.Return 
					&& modifier == Gdk.ModifierType.None
					&& isParameterValueCompletion)
				{
					// finish completion
					base.KeyPress(key, keyChar, modifier);
					// if double quote already exists, skip it!
					if (buffer.GetCharAt(buffer.CursorPosition) == '"')
					{
						buffer.CursorPosition++;
						return false;
					}
					// no double quote yet, so add it
					return base.KeyPress((Gdk.Key)0, '"', Gdk.ModifierType.None);
				}
				if (keyChar == '"' || keyChar == '\'')
				{
					// finish completion with double quote
					base.KeyPress(Gdk.Key.Return, '\0', Gdk.ModifierType.None);
					// if double quote already exists, skip it!
					if (buffer.GetCharAt(buffer.CursorPosition) == keyChar)
					{
						buffer.CursorPosition++;
						return false;
					}
					return base.KeyPress(key, keyChar, modifier);
				}
			}
			if (keyChar == '>')
			{
				// finish completion first
				if (CompletionWindowManager.IsVisible)
					base.KeyPress(Gdk.Key.Return, '\0', Gdk.ModifierType.None);

				// add self-closing tag if there is no content for the control
				if (!HasContentAtCurrentElement())
				{
					base.KeyPress((Gdk.Key)0, '/', Gdk.ModifierType.None);
					//buffer.InsertText(buffer.CursorPosition++, "/");
					return base.KeyPress(key, keyChar, modifier);
				}
			}
			if (keyChar == '"' || keyChar == '\'')
			{
				// if double quote already exists, skip it!
				if (buffer.GetCharAt(buffer.CursorPosition) == keyChar)
				{
					buffer.CursorPosition++;
					return false;
				}
			}
			if (keyChar == '.')
			{
				var result = base.KeyPress(key, keyChar, modifier);
				// provide completions for <Control.Property> elements
				var completionContext = CompletionWidget.CurrentCodeCompletionContext;
				var offset = completionContext.TriggerOffset - 1;
				var ch = CompletionWidget.GetChar(offset);
				while (ch != '\0' && (XmlConvert.IsNCNameChar(ch) || ch == ':') && offset > 0)
				{
					offset--;
					ch = CompletionWidget.GetChar(offset);
				}
				if (ch != '\0' && ch != '<')
					return result;
				offset++;
				var len = completionContext.TriggerOffset - offset;
				var name = Editor.GetTextAt(offset, len);
				try
				{
					XmlConvert.VerifyName(name);
				}
				catch (XmlException)
				{
					// not a valid xml name
					return result;
				}

				var xobject = Tracker.Engine.Nodes.Peek(1) as IAttributedXObject;
				if (xobject == null)
				{
					string prefix = null;
					var nsidx = name.IndexOf(':');
					if (nsidx > 0)
					{
						prefix = name.Substring(0, nsidx);
						name = name.Substring(nsidx + 1);
					}
					name = name.TrimEnd('.');
					xobject = new XElement(Tracker.Engine.Location, new XName(prefix, name));
				}
				
				var attributeDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				if (xobject.Attributes != null)
				{
					foreach (XAttribute current in xobject.Attributes)
					{
						attributeDictionary[current.Name.FullName] = current.Value ?? string.Empty;
					}
				}
				var completions = GetAttributeCompletions(xobject, attributeDictionary, XmlCompletionData.DataType.XmlElement, true);

				if (completions != null)
				{
					ShowCompletion(completions);
					return false;
				}
			}
			return base.KeyPress(key, keyChar, modifier);
		}

		protected override CompletionDataList GetAttributeCompletions(IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			return GetAttributeCompletions(attributedOb, existingAtts, XmlCompletionData.DataType.XmlAttribute, false);
		}

		bool HasContentAtCurrentElement()
		{
			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);
			var attributedOb = currentPath.OfType<XElement>().LastOrDefault();
			if (attributedOb == null)
				return true;
				
			var objectName = attributedOb.Name.FullName;
			return Completion.GetCompletions(namespaces).All(r => r.HasContent(objectName, path) != false);
		}
		

		CompletionDataList GetAttributeCompletions(IAttributedXObject attributedOb, Dictionary<string, string> existingAtts, XmlCompletionData.DataType type, bool elementNamespacesOnly)
		{
			var list = base.GetAttributeCompletions(attributedOb, existingAtts) ?? new CompletionDataList();
			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);
			var objectName = attributedOb.Name.FullName;
			if (elementNamespacesOnly)
				namespaces = namespaces.Where(r => (r.Prefix ?? string.Empty) == (attributedOb.Name.Prefix ?? string.Empty)).ToList();
			foreach (var completion in Completion.GetCompletions(namespaces))
			{
				foreach (var item in completion.GetProperties(objectName, path).Where(r => !existingAtts.ContainsKey(r.Name)))
				{
					var xmlCompletion = new XmlCompletionData(item.Name, item.Description, type);
					xmlCompletion.Icon = GetIcon(item.Type);
					list.Add(xmlCompletion);
				}
			}
			return list;
		}

		bool isParameterValueCompletion;

		protected override ICompletionDataList HandleCodeCompletion(CodeCompletionContext completionContext, bool forced, ref int triggerWordLength)
		{
			isParameterValueCompletion = false;
			return base.HandleCodeCompletion(completionContext, forced, ref triggerWordLength);
		}

		protected override CompletionDataList GetAttributeValueCompletions(IAttributedXObject attributedOb, XAttribute att)
		{
			isParameterValueCompletion = true;
			var list = base.GetAttributeValueCompletions(attributedOb, att) ?? new CompletionDataList();

			var currentPath = GetCurrentPath();
			var path = GetPath(currentPath);
			var namespaces = GetNamespaces(currentPath);
			var objectName = attributedOb.Name.FullName;
			foreach (var completion in Completion.GetCompletions(namespaces))
			{
				foreach (var item in completion.GetPropertyValues(objectName, att.Name.FullName, path))
				{
					var xmlCompletion = new XmlCompletionData(item.Name, item.Description, XmlCompletionData.DataType.XmlAttributeValue);
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

