using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eto.Addin.Shared
{
	public class ReplacementGroup
	{
		public string Condition { get; set; }
		public List<ReplacementItem> Replacements { get; } = new List<ReplacementItem>();

		public static IEnumerable<ReplacementGroup> LoadXml(XElement element)
		{
			if (element == null)
				yield break;
			var ns = element.GetDefaultNamespace();
			if (element.Elements(ns + "Replacement").Any())
			{ 
				yield return new ReplacementGroup(element);
			}
			foreach (var child in element.Elements(ns + "ReplacementGroup"))
			{
				yield return new ReplacementGroup(child);
			}
		}

		public ReplacementGroup(XElement element)
		{
			Condition = (string)element.Attribute("condition");
			Replacements.AddRange(ReplacementItem.LoadXml(element));
		}
	}

	public class ReplacementItem
	{
		public string Condition { get; set; }

		public string Name { get; set; }

		public string Content { get; set; }

		public bool ReplaceParameters { get; set; }

		public static IEnumerable<ReplacementItem> LoadXml(XElement element)
		{
			var ns = element.GetDefaultNamespace();
			foreach (var child in element.Elements(ns + "Replacement"))
			{
				yield return new ReplacementItem(child);
			}
		}

		public ReplacementItem(XElement element)
		{
			Condition = (string)element.Attribute("condition");
			Name = (string)element.Attribute("name");
			Content = element.Value.Replace("\r", "").Replace("\n", Environment.NewLine);
			var replaceParametersStr = (string)element.Attribute("replaceParameters");
			bool replaceParameters;
			if (bool.TryParse(replaceParametersStr, out replaceParameters))
				ReplaceParameters = replaceParameters;
		}
	}
}