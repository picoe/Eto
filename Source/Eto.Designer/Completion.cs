using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;

namespace Eto.Designer
{
	public enum CompletionType
	{
		Class,
		Property,
		Event,
		Attribute
	}

	public class CompletionItem
	{
		public CompletionType Type { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}

	public static class XmlComments
	{
		static Dictionary<string, string> lookup;

		static string GetNiceName(Type type)
		{
			if (type == null)
				return null;
			if (!type.IsGenericType)
				return type.Name;

			var arguments = type.GetGenericArguments().Select(GetNiceName);
			var typeName = type.Name;
			typeName = typeName.Substring(0, typeName.IndexOf("`"));
			return string.Format("{0}<{1}>", typeName, string.Join(",", arguments));
		}

		static bool EnsureDoc()
		{
			if (lookup != null)
				return true;
			lookup = new Dictionary<string, string>();
			var dllPath = typeof(Eto.Widget).Assembly.Location;
			var xmlPath = Path.Combine(Path.GetDirectoryName(dllPath), Path.GetFileNameWithoutExtension(dllPath) + ".xml");
			if (!File.Exists(xmlPath))
				return false;
			var doc = new XmlDocument();
			doc.Load(xmlPath);
			foreach (var node in doc.SelectNodes("//member").OfType<XmlElement>())
			{
				var nodeName = node.GetAttribute("name");
				var typeName = nodeName.Substring(nodeName.IndexOf(':') + 1);
				string description;
				if (nodeName.StartsWith("P:") || nodeName.StartsWith("E:"))
				{
					var lastDot = typeName.LastIndexOf('.');
					var propName = typeName.Substring(lastDot + 1);
					typeName = typeName.Substring(0, lastDot);
					var type = typeof(Eto.Widget).Assembly.GetType(typeName);
					if (nodeName.StartsWith("E:"))
					{
						var evt = type?.GetEvent(propName);
						description = GetDescription(GetNiceName(evt?.EventHandlerType), node);
					}
					else if (nodeName.StartsWith("P:"))
					{
						var property = type?.GetProperty(propName);
						description = GetDescription(property != null ? string.Format("{0} : {1}", property.Name, GetNiceName(property.PropertyType)) : null, node);
					}
					else
						description = GetDescription(null, node);
				}
				else
				{
					description = GetDescription(null, node);
				}
				lookup[nodeName] = description;
			}
			return true;
		}

		public static string GetSummary(Type type)
		{
			EnsureDoc();
			string path = "T:" + type.FullName;
			return GetSummary(path);
		}

		public static string GetSummary(PropertyInfo property)
		{
			EnsureDoc();
			string path = "P:" + property.DeclaringType.FullName + "." + property.Name;
			return GetSummary(path);
		}

		public static string GetSummary(EventInfo evt)
		{
			EnsureDoc();
			string path = "E:" + evt.DeclaringType.FullName + "." + evt.Name;
			return GetSummary(path);
		}

		static string GetSummary(string path)
		{
			EnsureDoc();
			string result;
			if (lookup.TryGetValue(path, out result))
				return result;
			return null;
		}

		static string GetDescription(string name, XmlNode node)
		{
			var sb = new StringBuilder();
			if (name != null)
			{
				sb.AppendLine(name);
				sb.AppendLine();
			}

			sb.AppendLine("Summary");
			GetDescription(node, sb);
			return System.Net.WebUtility.HtmlEncode(sb.ToString());
		}

		static void GetDescription(XmlNode node, StringBuilder sb)
		{
			if (node == null)
				return;
			var summary = node.SelectSingleNode("summary")?.InnerText.Trim();
			var details = node.SelectSingleNode("remarks")?.InnerText.Trim();
			sb.Append(summary);
			if (!string.IsNullOrEmpty(details))
			{
				sb.AppendLine();
				sb.AppendLine();
				var reader = new StringReader(details);
				string line;
				var newline = true;
				while ((line = reader.ReadLine()) != null)
				{
					var val = line.Trim();
					if (string.IsNullOrEmpty(val))
					{
						if (!newline)
						{
							sb.AppendLine();
							sb.AppendLine();
							newline = true;
						}
					}
					else
					{
						sb.Append(val);
						sb.Append(" ");
						newline = false;
					}
				}
			}
		}
	}

	public class CompletionNamespace
	{
		public string Prefix { get; set; }

		public string Namespace { get; set; }
	}

	public abstract class Completion
	{
		public string Prefix { get; set; }

		public abstract IEnumerable<CompletionItem> GetElements(IEnumerable<string> path);

		public abstract IEnumerable<CompletionItem> GetAttributes(string objectName, IEnumerable<string> path);

		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2009/xaml";

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
				if (ns.Namespace == XamlNamespace)
				{
					yield return new XamlCompletion { Prefix = ns.Prefix };
				}
			}
		}
	}

	class GeneralCompletion : Completion
	{
		public override IEnumerable<CompletionItem> GetElements(IEnumerable<string> path)
		{
			yield break;
		}

		public override IEnumerable<CompletionItem> GetAttributes(string objectName, IEnumerable<string> path)
		{
			yield return new CompletionItem { Name = "xmlns", Type = CompletionType.Attribute };
			if (path.Count() == 1)
			{
				yield return new CompletionItem { Name = "xmlns:custom=\"clr-namespace:[namespace];assembly=[assembly]\"", Type = CompletionType.Attribute };
			}
		}
	}

	class XamlCompletion : Completion
	{
		public override IEnumerable<CompletionItem> GetElements(IEnumerable<string> path)
		{
			yield break;
		}

		public override IEnumerable<CompletionItem> GetAttributes(string objectName, IEnumerable<string> path)
		{
			var prefix = Prefix ?? "";
			if (path.Count() == 1)
			{
				yield return new CompletionItem { Name = prefix + "Class", Type = CompletionType.Attribute };
			}
			yield return new CompletionItem { Name = prefix + "Name", Type = CompletionType.Attribute };
		}
	}

	class TypeCompletion : Completion
	{
		public Assembly Assembly { get; set; }

		public string Namespace { get; set; }

		public override IEnumerable<CompletionItem> GetElements(IEnumerable<string> path)
		{
			var prefix = Prefix ?? "";
			var types = Assembly.ExportedTypes;
			var results = types.Where(r => r.Namespace == Namespace && typeof(Eto.Forms.Control).IsAssignableFrom(r));
			results = results.Where(r => !r.IsGenericType && !r.IsAbstract);

			foreach (var result in results)
			{
				yield return new CompletionItem
				{ 
					Name = prefix + result.Name, 
					Description = XmlComments.GetSummary(result),
					Type = CompletionType.Class
				}; 
			}
		}

		public override IEnumerable<CompletionItem> GetAttributes(string objectName, IEnumerable<string> path)
		{
			var fullName = Namespace + "." + objectName;
			var type = Assembly.GetType(fullName, false);
			if (type != null)
			{
				foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					if (prop.SetMethod == null || !prop.SetMethod.IsPublic)
						continue;
					yield return new CompletionItem
					{ 
						Name = prop.Name,
						Description = XmlComments.GetSummary(prop),
						Type = CompletionType.Property 
					};
				}
				foreach (var evt in type.GetEvents(BindingFlags.Public | BindingFlags.Instance))
				{
					yield return new CompletionItem
					{ 
						Name = evt.Name,
						Description = XmlComments.GetSummary(evt),
						Type = CompletionType.Event 
					};
				}
			}
		}
	}
}

