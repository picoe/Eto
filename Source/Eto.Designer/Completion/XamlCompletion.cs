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

	class XamlCompletion : Completion
	{
		public override IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path)
		{
			yield break;
		}

		public override IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path)
		{
			var prefix = Prefix ?? "";
			if (path.Count() == 1)
			{
				yield return new CompletionItem { Name = prefix + "Class", Type = CompletionType.Attribute };
			}
			yield return new CompletionItem { Name = prefix + "Name", Type = CompletionType.Attribute };
		}
	}

}
