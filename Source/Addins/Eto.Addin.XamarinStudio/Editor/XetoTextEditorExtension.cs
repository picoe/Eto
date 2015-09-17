using System;
using MonoDevelop.Xml.Editor;
using System.Linq;
using MonoDevelop.Xml.Completion;
using System.Reflection;
using System.Collections.Generic;
using MonoDevelop.Xml.Dom;
using MonoDevelop.Ide.CodeCompletion;

namespace Eto.Addin.XamarinStudio.Editor
{
	public class XetoTextEditorExtension : BaseXmlEditorExtension
	{
		protected override void GetElementCompletions(CompletionDataList list)
		{
			var path = GetCurrentPath();
			foreach (var completion in Completion.GetCompletions(path))
			{
				completion.GetElements(path, list);
			}
			BaseXmlEditorExtension.AddMiscBeginTags(list);
			base.GetElementCompletions(list);
		}

		protected override CompletionDataList GetAttributeCompletions(IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			var list = base.GetAttributeCompletions(attributedOb, existingAtts) ?? new CompletionDataList();

			var path = GetCurrentPath();
			foreach (var completion in Completion.GetCompletions(path))
			{
				completion.GetAttributes(list, attributedOb, path, existingAtts);
			}
			return list;
		}
	}
}

