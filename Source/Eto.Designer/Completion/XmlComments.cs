using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;

namespace Eto.Designer.Completion
{

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

		public static string GetEnum(PropertyInfo property, string name)
		{
			EnsureDoc();
			string path = "F:" + property.PropertyType.FullName + "." + name;
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
	
}
