using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Eto.Designer.Completion
{

	public static class XmlComments
	{
		static Dictionary<string, XmlElement> lookup;
		static Dictionary<string, string> lookupTranslated;
		public static bool EncodeHtml = true;

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
			lookup = new Dictionary<string, XmlElement>();
			lookupTranslated = new Dictionary<string, string>();
			var dllPath = typeof(Eto.Widget).Assembly.Location;
			var xmlPath = Path.Combine(Path.GetDirectoryName(dllPath), Path.GetFileNameWithoutExtension(dllPath) + ".xml");
			if (!File.Exists(xmlPath))
				return false;
			var doc = new XmlDocument();
			doc.Load(xmlPath);
			foreach (var node in doc.DocumentElement.SelectNodes("members/member").OfType<XmlElement>())
			{
				var nodeName = node.GetAttribute("name");
				if (!string.IsNullOrEmpty(nodeName))
					lookup[nodeName] = node;
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

		static string GetSummary(string nodeName)
		{
			EnsureDoc();
			string description;
			if (lookupTranslated.TryGetValue(nodeName, out description))
				return description;

			XmlElement node;
			if (lookup.TryGetValue(nodeName, out node))
			{
				var typeName = nodeName.Substring(nodeName.IndexOf(':') + 1);
				if (nodeName.StartsWith("P:") || nodeName.StartsWith("E:"))
				{
					var lastDot = typeName.LastIndexOf('.');
					var propName = typeName.Substring(lastDot + 1);
					typeName = typeName.Substring(0, lastDot);
					var type = typeof(Eto.Widget).Assembly.GetType(typeName);
					if (nodeName.StartsWith("E:"))
					{
						var evt = type?.GetEvents().FirstOrDefault(r => r.Name == propName);
						description = GetDescription(GetNiceName(evt?.EventHandlerType), node);
					}
					else if (nodeName.StartsWith("P:"))
					{
						var property = type?.GetProperties().FirstOrDefault(r => r.Name == propName);
						description = GetDescription(property != null ? string.Format("{0} : {1}", property.Name, GetNiceName(property.PropertyType)) : null, node);
					}
					else
						description = GetDescription(null, node);
				}
				else
				{
					description = GetDescription(null, node);
				}
				lookup.Remove(nodeName);
				lookupTranslated[nodeName] = description;
				return description;
			}
			return null;
		}

		static string GetDescription(string name, XmlNode node)
		{
			var sb = new StringBuilder();
			if (name != null)
			{
				name = EncodeHtml ? System.Net.WebUtility.HtmlEncode(name) : name;
				sb.AppendLine(name);
				sb.AppendLine();
			}

			sb.AppendLine("Summary");
			GetDescription(node, sb);
			return sb.ToString();
		}

		static Regex regReferences;
		static Regex regPrefix = new Regex(@"^\w\:(Eto.Forms.)?", RegexOptions.Compiled);

		static string TranslateCref(string name)
		{
			if (EncodeHtml)
			{
				if (regReferences == null)
					regReferences = new Regex(@"&lt;\w+\s+\w+\s*=\s*((&quot;(?<c>.*?)&quot;)|('(?<c>.*?)'))\s*/&gt;", RegexOptions.Compiled);
				name = System.Net.WebUtility.HtmlEncode(name);
				return regReferences.Replace(name, m => "<i>" + regPrefix.Replace(m.Groups["c"].Value, "") + "</i>");

			}
			else
			{
				if (regReferences == null)
					regReferences = new Regex(@"<\w+\s+\w+\s*=\s*((""(?<c>.*?)"")|('(?<c>.*?)'))\s*/>", RegexOptions.Compiled);
				return regReferences.Replace(name, m => regPrefix.Replace(m.Groups["c"].Value, ""));

			}
		}

		static void GetDescription(XmlNode node, StringBuilder sb)
		{
			if (node == null)
				return;
			var summary = node.SelectSingleNode("summary")?.InnerXml.Trim();
			var details = node.SelectSingleNode("remarks")?.InnerXml.Trim();
			sb.Append(TranslateCref(summary));
			if (!string.IsNullOrEmpty(details))
			{
				sb.AppendLine();
				sb.AppendLine();
				var reader = new StringReader(TranslateCref(details));
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
