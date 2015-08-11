using System;
using MonoDevelop.Ide.Templates;
using System.Linq;
using System.Xml;
using MonoDevelop.Core.StringParsing;
using System.Collections.Generic;

namespace Eto.Addin.XamarinStudio
{
	public class ParameterizedFile : TextFileDescriptionTemplate
	{
		public Dictionary<string, Replacement> Replacements { get; set; }

		public class Replacement
		{
			public string Name { get; set; }

			public string Value { get; set; }

			public Replacement(XmlElement element)
			{
				Name = element.GetAttribute("name");
				Value = element.GetAttribute("value");
			}
		}

		public override void Load(System.Xml.XmlElement filenode, MonoDevelop.Core.FilePath baseDirectory)
		{
			var replacements = filenode.SelectNodes("Replacements/Replacement");
			Replacements = replacements.OfType<XmlElement>().Select(r => new Replacement(r)).ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);
			base.Load(filenode, baseDirectory);
		}

		class TagModel : IStringTagModel
		{
			public IStringTagModel Inner { get;set; }

			public ParameterizedFile File { get; set; }

			public object GetValue(string name)
			{
				Replacement parameter;
				if (File.Replacements.TryGetValue(name, out parameter))
					return parameter.Value;
				return Inner.GetValue(name);
			}
		}

		protected override string ProcessContent(string content, IStringTagModel tags)
		{
			tags = new TagModel { Inner = tags, File = this };
			return base.ProcessContent(content, tags);
		}
	}
}

