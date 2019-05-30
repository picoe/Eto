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

	public enum CompletionMode
	{
		None,
		Class,
		Property,
		Value
	}

	public class CompletionPathNode
	{
		public List<CompletionNamespace> Namespaces { get; set; }
		List<string> attributes;
		public List<string> Attributes { get { return attributes ?? (attributes = new List<string>()); } }
		public string LocalName { get; set; }
		public string Prefix { get; set; }
		public CompletionMode Mode { get; set; }

		public string Name
		{
			get {
				return (Prefix ?? "") + LocalName;
			}
		}

		public CompletionPathNode(string prefix, string localName, CompletionMode mode)
		{
			if (!string.IsNullOrEmpty(prefix) && prefix.EndsWith(":"))
				prefix = prefix.TrimEnd(':');
			Prefix = prefix;
			LocalName = localName;
			Mode = mode;
		}
	}

	public abstract class Completion
	{
		public string Prefix { get; set; }

		public virtual Func<Type, bool> GetFilter(IEnumerable<string> path)
		{
			return null;
		}

		public abstract IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path, Func<Type, bool> filter);

		public abstract IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path);

		public virtual IEnumerable<CompletionItem> GetPropertyValues(string objectName, string propertyName, IEnumerable<string> path)
		{
			yield break;
		}

		/// <summary>
		/// Determine whether the specified objectName has content, or null if not known by this completion handler.
		/// </summary>
		public virtual bool? HasContent(string objectName, IEnumerable<string> path)
		{
			return null;
		}

		public string PrefixWithColon
		{
			get { return string.IsNullOrEmpty(Prefix) ? string.Empty : Prefix + ":"; }
		}

		public virtual bool HandlesPrefix(string prefix)
		{
			return prefix == Prefix || string.IsNullOrEmpty(prefix) == string.IsNullOrEmpty(Prefix);
		}

		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string XamlNamespace2006 = "http://schemas.microsoft.com/winfx/2006/xaml";

		public static IEnumerable<CompletionItem> GetCompletionItems(IEnumerable<CompletionNamespace> namespaces, CompletionMode mode, IEnumerable<string> path, CompletionPathNode context)
		{
			if (mode == CompletionMode.None)
				return Enumerable.Empty<CompletionItem>();
			var completions = GetCompletions(namespaces);
			IEnumerable<CompletionItem> items;
			if (mode == CompletionMode.Property && context != null)
			{
				var contextName = context.Name;
				if (contextName.EndsWith("."))
				{
					// if it contains a dot it is a property element. only show completions for the current namespace.
					contextName = contextName.TrimEnd('.');
					completions = completions.Where(r => r.Prefix == context.Prefix).ToList();
				}
				items = completions
					.SelectMany(r => r.GetProperties(contextName, path))
					.Where(r => !context.Attributes.Contains(r.Name));
			}
			else if (mode == CompletionMode.Value && context != null && context.Mode == CompletionMode.Property)
				items = completions.SelectMany(r => r.GetPropertyValues(path.Last(), context.LocalName, path));
			else
			{
				var filter = completions.Select(r => r.GetFilter(path)).FirstOrDefault(r => r != null);
				items = completions.SelectMany(r => r.GetClasses(path, filter));
			}
			return items;
		}

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
				if (ns.Namespace == XamlNamespace2006)
				{
					yield return new XamlCompletion { Prefix = ns.Prefix };
				}
			}
		}
	}
}