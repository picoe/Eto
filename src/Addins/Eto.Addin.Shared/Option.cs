using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eto.Addin.Shared
{
	public class OptionValue
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public List<ReplacementGroup> Replacements { get; } = new List<ReplacementGroup>();

		public static IEnumerable<OptionValue> LoadXml(XElement element)
		{
			if (element == null)
				yield break;
			var ns = element.GetDefaultNamespace();
			foreach (var child in element.Elements(ns + "Value"))
			{
				yield return new OptionValue(child);
			}
		}

		public OptionValue(XElement element)
		{
			Name = (string)element.Attribute("name");
			Description = (string)element.Attribute("description");
			Replacements.AddRange(ReplacementGroup.LoadXml(element));
		}
	}

	public class Option
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public OptionValue Selected { get; set; }

		public static IEnumerable<Option> LoadXml(XElement element)
		{
			if (element == null)
				yield break;
			var ns = element.GetDefaultNamespace();
			foreach (var child in element.Elements(ns + "Option"))
			{
				yield return new Option(child);
			}
		}

		public List<OptionValue> Values { get; } = new List<OptionValue>();

		public Option(XElement element)
		{
			Name = (string)element.Attribute("name");
			Description = (string)element.Attribute("description");
			Values.AddRange(OptionValue.LoadXml(element));
		}
	}
}
