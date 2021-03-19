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
		public bool IsChildProperty { get; set; }
	}

	static class XmlParser
	{
		const RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase;
		static readonly Regex valueReg = new Regex(@"(?<=\w+\s*=\s*)(('[^']*)|(""[^""]*))?$", opts);
		static readonly Regex propertyReg = new Regex(@"([<]\w+\s+)([^<>]+\s+)?[^<>/]*$", opts);
		//static readonly Regex propertyReg = new Regex(@"([<]\w+\s+)([^<]*)?(?<!(/|([/][>])|[>])[^<]*)$", opts);
		static readonly Regex classReg = new Regex(@"([<]\w*)$", opts);
		static readonly Regex classPropertyReg = new Regex(@"([<]\w*[.])$", opts);

		public static XmlParseInfo Read(string text)
		{
			var nodes = new List<CompletionPathNode>();
			var info = new XmlParseInfo { Nodes = nodes, Mode = CompletionMode.None };

			// check the last part of the xml to see what type of completion we are in
			// and complete it so we can parse the property or class name.
			Match m;
			if ((m = valueReg.Match(text)).Success)
			{
				info.Mode = CompletionMode.Value;
				// without an actual value, xmlns doesn't parse, so put a dummy value here
				text = text.Substring(0, m.Index) + "'value'>";
			}
			else if (classReg.Match(text).Success)
			{
				info.Mode = CompletionMode.Class;
				text += ">";
			}
			else if (propertyReg.Match(text).Success)
			{
				int i = text.Length - 1;
				while (i > 0 && !char.IsWhiteSpace(text[i]) && text[i] != '<')
					i--;
				if (i < text.Length - 1)
					text = text.Substring(0, i + 1);

				info.Mode = CompletionMode.Property;
				text += ">";
			}
			else if (classPropertyReg.Match(text).Success)
			{
				info.Mode = CompletionMode.Property;
				info.IsChildProperty = true;
				text += ">";
			}


			CompletionPathNode current = null;
			CompletionPathNode attribute = null;
			try
			{
				using (var reader = XmlReader.Create(new StringReader(text)))
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
											var name = reader.LocalName;
											if (!string.IsNullOrEmpty(reader.Prefix))
												name = reader.Prefix + ":" + name;
											attribute = new CompletionPathNode(string.Empty, name, CompletionMode.Property);
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

			if (current?.LocalName.Contains(".") == true && info.Mode == CompletionMode.None && current.Mode == CompletionMode.Class)
			{
				current.LocalName = current.LocalName.Substring(current.LocalName.IndexOf('.') + 1);
				current.Mode = CompletionMode.Property;
				info.Mode = CompletionMode.Value;
			}

			if (info.Mode == CompletionMode.Value && attribute != null)
				nodes.Add(attribute);

			return info;
		}
	}
}
