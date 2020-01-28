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
using MonoDevelop.Ide.Editor.Extension;
using System.Threading.Tasks;
using System.Threading;

namespace Eto.Addin.MonoDevelop.Editor
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

		protected override void Initialize()
		{
			base.Initialize();

			// ensure highlighting works (shouldn't be necessary, but is in VS for Mac???)
			// also set in EditorView.Save as the mime type gets reset there
			Editor.MimeType = "application/xml";
			DocumentContext.ReparseDocument();
		}

		protected override async Task<CompletionDataList> GetElementCompletions (System.Threading.CancellationToken token)
		{
			var list = await base.GetElementCompletions (token);
			var currentPath = GetCurrentPath ();
			var path = GetPath (currentPath);
			var namespaces = GetNamespaces (currentPath);
			var completions = Completion.GetCompletions (namespaces).ToList ();
			var filter = completions.Select (r => r.GetFilter (path)).FirstOrDefault (r => r != null);
			foreach (var completion in completions) {
				foreach (var item in completion.GetClasses (path, filter)) {
					var xmlCompletion = new XmlCompletionData (item.Name, item.Description, XmlCompletionData.DataType.XmlElement);
					xmlCompletion.Icon = Stock.Class;
					list.Add (xmlCompletion);
				}
			}
			AddMiscBeginTags (list);
			return list;
		}

		public override bool KeyPress(KeyDescriptor descriptor)
		{
			var buffer = Editor;
			var keyChar = descriptor.KeyChar;
			var key = descriptor.SpecialKey;
			var modifier = descriptor.ModifierKeys;
			if (CompletionWindowManager.IsVisible)
			{
				// do some things to minimize keystrokes with code completion
#if MD_7_0
				if (keyChar == '=')
				{
					// we're in an attribute completion, so automatically add the quote and show completions (if available)
					var ret = base.KeyPress(descriptor);
					if (!ret)
					{
						base.KeyPress(KeyDescriptor.FromGtk((Gdk.Key)0, '"', Gdk.ModifierType.None));
						buffer.InsertText(buffer.CaretOffset, "\"");
						buffer.CaretOffset--;
					}
					return ret;
				}
#endif
				if (key == SpecialKey.Return
					&& modifier == ModifierKeys.None
					&& isParameterValueCompletion)
				{
					// finish completion
					base.KeyPress(descriptor);
					// if double quote already exists, skip it!
					if (buffer.GetCharAt(buffer.CaretOffset) == '"')
					{
						buffer.CaretOffset++;
						return false;
					}
					// no double quote yet, so add it
					return base.KeyPress(KeyDescriptor.FromGtk((Gdk.Key)0, '"', Gdk.ModifierType.None));
				}
				if (keyChar == '"' || keyChar == '\'')
				{
					// finish completion with double quote
					base.KeyPress(KeyDescriptor.FromGtk(Gdk.Key.Return, '\0', Gdk.ModifierType.None));
					// if double quote already exists, skip it!
					if (buffer.GetCharAt(buffer.CaretOffset) == keyChar)
					{
						buffer.CaretOffset++;
						return false;
					}
					return base.KeyPress(descriptor);
				}
			}
			if (keyChar == '>')
			{
				// finish completion first
				if (CompletionWindowManager.IsVisible)
					base.KeyPress(KeyDescriptor.FromGtk(Gdk.Key.Return, '\0', Gdk.ModifierType.None));

				// add self-closing tag if there is no content for the control
				if (!HasContentAtCurrentElement())
				{
#if MD_7_0
					base.KeyPress(KeyDescriptor.FromGtk((Gdk.Key)0, '/', Gdk.ModifierType.None));
#elif MD_8_0
					// md 8.x inserts the closing '>' automatically, so we just need to insert the foward slash
					descriptor = KeyDescriptor.FromGtk((Gdk.Key)0, '/', Gdk.ModifierType.None);
#endif
					return base.KeyPress(descriptor);
				}
			}
			if (keyChar == '"' || keyChar == '\'')
			{
				// if double quote already exists, skip it!
				if (buffer.GetCharAt(buffer.CaretOffset) == keyChar)
				{
					buffer.CaretOffset++;
					return false;
				}
			}
			if (keyChar == '.' || ((key == SpecialKey.Return || key == SpecialKey.Tab) && CompletionWindowManager.IsVisible))
			{
				// provide completions for <Control.Property> elements
				var isNewCompletion = keyChar == '.' && !CompletionWindowManager.IsVisible;
				var completionContext = CurrentCompletionContext;

				var result = base.KeyPress(descriptor);

				if (isNewCompletion) completionContext = CurrentCompletionContext;
				var offset = completionContext.TriggerOffset - 1;

				// using reflection here as these have been made internal as of XS 6.0.  Why? who knows.  Alternative? reflection of course.
				var completionWidget = GetType().GetProperty("CompletionWidget", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
				var getCharMethod = completionWidget?.GetType()?.GetMethod("GetChar", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (getCharMethod != null)
				{
					Func<int, char> getChar = (arg) =>
					{
						return (char)getCharMethod.Invoke(completionWidget, new object[] { arg });
					};

					var ch = getChar(offset);
					while (ch != '\0' && (XmlConvert.IsNCNameChar(ch) || ch == ':') && offset > 0)
					{
						offset--;
						ch = getChar(offset);
					}
					if (ch != '\0' && ch != '<')
						return result;
				}
				offset++;
				var end = isNewCompletion ? completionContext.TriggerOffset - 1 : buffer.CaretOffset;

				var name = Editor.GetTextAt(offset, end - offset);

				if (!isNewCompletion && !name.EndsWith(".", StringComparison.Ordinal))
					return result;

				try
				{
					XmlConvert.VerifyName(name);
				}
				catch (XmlException)
				{
					// not a valid xml name, so just return
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
				var task = GetAttributeCompletions(xobject, attributeDictionary, CancellationToken.None, XmlCompletionData.DataType.XmlElement, true);

				task.ContinueWith(t =>
				{
					// need to show completion within the main task scheduler, otherwise it doesn't show
					var completions = t.Result;
					if (completions != null)
					{
						ShowCompletion(completions, 0, '.');
					}
				}, Runtime.MainTaskScheduler);
				return false;
			}
			return base.KeyPress (descriptor);
		}

		protected override async Task<CompletionDataList> GetEntityCompletions (System.Threading.CancellationToken token)
		{
			var list = await base.GetEntityCompletions (token);

			return list;
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

		protected override Task<CompletionDataList> GetAttributeCompletions (IAttributedXObject attributedOb, Dictionary<string, string> existingAtts, CancellationToken token)
		{
			return GetAttributeCompletions (attributedOb, existingAtts, token, XmlCompletionData.DataType.XmlAttribute, false);
		}

		async Task<CompletionDataList> GetAttributeCompletions (IAttributedXObject attributedOb, Dictionary<string, string> existingAtts, CancellationToken token, XmlCompletionData.DataType type, bool elementNamespacesOnly)
		{
			var list = await base.GetAttributeCompletions (attributedOb, existingAtts, token) ?? new CompletionDataList ();
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

		protected override async Task<ICompletionDataList> HandleCodeCompletion (CodeCompletionContext completionContext, bool forced, CancellationToken token)
		{
			isParameterValueCompletion = false;
			return await base.HandleCodeCompletion (completionContext, forced, token);
		}

		protected override async Task<CompletionDataList> GetAttributeValueCompletions (IAttributedXObject attributedOb, XAttribute att, CancellationToken token)
		{
			isParameterValueCompletion = true;
			var list = await base.GetAttributeValueCompletions (attributedOb, att, token);
			var currentPath = GetCurrentPath ();
			var path = GetPath (currentPath);
			var namespaces = GetNamespaces (currentPath);
			var objectName = attributedOb.Name.FullName;
			foreach (var completion in Completion.GetCompletions (namespaces)) {
				foreach (var item in completion.GetPropertyValues (objectName, att.Name.FullName, path)) {
					var xmlCompletion = new XmlCompletionData (item.Name, item.Description, XmlCompletionData.DataType.XmlAttributeValue);
					xmlCompletion.Icon = GetIcon (item.Type);
					list.Add (xmlCompletion);
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

