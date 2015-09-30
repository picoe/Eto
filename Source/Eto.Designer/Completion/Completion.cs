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
	public class CompletionNamespace
	{
		public string Prefix { get; set; }

		public string Namespace { get; set; }
	}

	public abstract class Completion
	{
		public string Prefix { get; set; }

		public abstract IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path);

		public abstract IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path);

		public virtual IEnumerable<CompletionItem> GetPropertyValues(string objectName, string propertyName, IEnumerable<string> path)
		{
			yield break;
		}

		public virtual bool HandlesPrefix(string prefix)
		{
			return prefix == Prefix || string.IsNullOrEmpty(prefix) == string.IsNullOrEmpty(Prefix);
		}

		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string XamlNamespace2009 = "http://schemas.microsoft.com/winfx/2009/xaml";
		public const string XamlNamespace2006 = "http://schemas.microsoft.com/winfx/2006/xaml";

		public static IEnumerable<Completion> GetCompletions(IEnumerable<CompletionNamespace> namespaces)
		{
			yield return new GeneralCompletion();

			foreach (var ns in namespaces)
			{
				if (ns.Namespace == EtoFormsNamespace)
				{
					yield return new TypeCompletion
					{
						Prefix = ns.Prefix,
						Assembly = typeof(Eto.Widget).Assembly,
						Namespace = "Eto.Forms"
					};
				}
				if (ns.Namespace == XamlNamespace2009 || ns.Namespace == XamlNamespace2006)
				{
					yield return new XamlCompletion { Prefix = ns.Prefix };
				}
			}
		}
		public static IEnumerable<Completion> GetCompletions(IEnumerable<CompletionNamespace> namespaces, string prefix)
		{
			return GetCompletions(namespaces).Where(r => r.HandlesPrefix(prefix));
		}
	}
}