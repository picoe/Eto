using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using Portable.Xaml;

namespace Eto.Designer.Completion
{

	class XamlCompletion : Completion
	{
		public override IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path, Func<Type, bool> filter)
		{
			var prefix = PrefixWithColon;
			if (filter != null)
			{
				foreach (var xt in XamlLanguage.AllTypes)
				{
					if (filter(xt.UnderlyingType))
						yield return new CompletionItem
						{
							Name = prefix + xt.Name,
							Type = CompletionType.Class
						};
				}
			}

			yield return new CompletionItem {
				Name = prefix + "Reference",
				Type = CompletionType.Class,
				Description = string.Format("Reference another object specified by its {0}Name attribute", Prefix)
			};
		}

		public override bool HandlesPrefix(string prefix)
		{
			return true;
		}

		public override IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path)
		{
			var prefix = PrefixWithColon;
			if (objectName == prefix + "Reference")
			{
				yield return new CompletionItem { Name = "Name", Type = CompletionType.Attribute, Description = "Name of the element to reference" };
				yield break;
			}
			if (path.Count() == 1)
			{
				yield return new CompletionItem { Name = prefix + "Class", Type = CompletionType.Attribute };
			}
			if (!objectName.Contains('.'))
			{
				yield return new CompletionItem
				{
					Name = prefix + "Name",
					Type = CompletionType.Attribute,
					Description = "Sets the name of the object, which will automatically bind to a field or property of the same name in your backing class."
				};
			}
		}
	}

}
