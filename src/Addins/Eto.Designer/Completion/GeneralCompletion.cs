using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;

namespace Eto.Designer.Completion
{

	class GeneralCompletion : Completion
	{
		public override IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path, Func<Type, bool> filter)
		{
			yield break;
		}

		public override IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path)
		{
			yield return new CompletionItem { Name = "xmlns", Type = CompletionType.Attribute };
			if (path.Count() == 1)
			{
				yield return new CompletionItem { Name = "xmlns:custom", Type = CompletionType.Attribute };
			}
		}

		public override bool HandlesPrefix(string prefix)
		{
			return true;
		}

		public override IEnumerable<CompletionItem> GetPropertyValues(string objectName, string propertyName, IEnumerable<string> path)
		{
			if (propertyName == "xmlns" || propertyName.StartsWith("xmlns:"))
			{
				yield return new CompletionItem { Name = XamlNamespace2006, Type = CompletionType.Literal };
				yield return new CompletionItem { Name = EtoFormsNamespace, Type = CompletionType.Literal };
				yield return new CompletionItem { Name = "clr-namespace:[namespace];assembly=[assembly]", Type = CompletionType.Literal };
			}
		}
	}
}
