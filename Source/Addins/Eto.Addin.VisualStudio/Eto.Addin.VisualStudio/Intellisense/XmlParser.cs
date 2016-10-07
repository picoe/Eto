using Eto.Designer.Completion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Eto.Addin.VisualStudio.Intellisense
{
	class XmlParseInfo
	{
		public IEnumerable<CompletionPathNode> Nodes { get; set; }
		public CompletionMode Mode { get; set; }
	}

	static class XmlParser
	{
		static Stream GetStream(string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
		static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase;
		static Regex valueReg = new Regex(@"(?<=\w+\s*=\s*)(('[^']*)|(""[^""]*))?$", opts);
		static Regex propertyReg = new Regex(@"([<]\w+\s+)([^<]*)?(?<!/|([/][>])|[>])$", opts);
		static Regex classReg = new Regex(@"([<]\w*)$", opts);

		public static XmlParseInfo Read(string text)
		{
			var nodes = new List<CompletionPathNode>();
			var info = new XmlParseInfo { Nodes = nodes, Mode = CompletionMode.None };

			// check the last part of the xml to see what type of completion we are in
			// and complete it so we can parse the property or class name.
			var m = valueReg.Match(text);
			if (m.Success)
			{
				info.Mode = CompletionMode.Value;
				text = text.Substring(0, m.Index) + "''>";
			}
			else
			{
				m = classReg.Match(text);
				if (m.Success)
					info.Mode = CompletionMode.Class;
				else
				{
					m = propertyReg.Match(text);
					if (m.Success)
						info.Mode = CompletionMode.Property;
				}
				text += ">";
			}


			CompletionPathNode current = null;
			CompletionPathNode attribute = null;
			try
			{
				using (var stream = GetStream(text))
				using (var reader = XmlReader.Create(stream))
				{
					while (reader.Read())
					{
						attribute = null;
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								if (reader.IsEmptyElement)
									continue;
								nodes.Add(current = new CompletionPathNode(reader.Prefix, reader.LocalName, CompletionMode.Class));
								if (reader.HasAttributes && reader.MoveToFirstAttribute())
								{
									do
									{
										if (reader.Name == "xmlns" || reader.Prefix == "xmlns")
										{
											// record namespaces
											if (current.Namespaces == null)
												current.Namespaces = new List<CompletionNamespace>();
											var prefix = reader.Prefix == "xmlns" ? reader.LocalName : string.Empty;
											current.Namespaces.Add(new CompletionNamespace { Prefix = prefix, Namespace = reader.Value });
										}
										else
										{
											current.Attributes.Add(reader.Name);
											attribute = new CompletionPathNode(reader.Prefix, reader.LocalName, CompletionMode.Property);
										}
									} while (reader.MoveToNextAttribute());
								}
								break;
							case XmlNodeType.EndElement:
								if (nodes.Count > 0)
									nodes.RemoveAt(nodes.Count - 1);
								current = nodes.Count > 0 ? nodes[nodes.Count - 1] : null;
								break;
							default:
								break;
						}
					}
				}
			}
			catch (XmlException)
			{
				// ignore errors, since xml will usually be incomplete
			}

			if (info.Mode == CompletionMode.Value && attribute != null)
				nodes.Add(attribute);

			return info;
		}
	}
}
