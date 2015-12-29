using Eto.Designer.Completion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
		public static XmlParseInfo Read(string text)
		{
			var nodes = new List<CompletionPathNode>();
			var info = new XmlParseInfo { Nodes = nodes, Mode = CompletionMode.Class };

			// complete the last node and/or attribute so the reader can find it.
			string supplement = null;
			bool hadLetter = false;
			char last = char.MinValue;
			for (int i = text.Length - 1; i >= 0; i--)
			{
				var ch = text[i];
				if (ch == '<')
				{
					if (i == text.Length - 1)
						break;
					if (last == '/') // we're in an ending tag
						info.Mode = CompletionMode.None;
					if (supplement != null)
						text += supplement;
					text += ">";
                    break;
				}
				if (ch == '>')
				{
					info.Mode = CompletionMode.Class;
                    break;
				}
				if (info.Mode == CompletionMode.Class)
				{
					if (ch == '=')
					{
						info.Mode = CompletionMode.Value;
						text = text.Substring(0, i + 1);
						supplement = "''";
					}
					if (ch == '.')
					{
						info.Mode = CompletionMode.Property;
					}
					if (ch == ' ')
					{
						info.Mode = CompletionMode.Property;
						if (hadLetter)
							supplement = "=''";
					}
					if (char.IsLetterOrDigit(ch))
						hadLetter = true;
				}
				last = ch;
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
