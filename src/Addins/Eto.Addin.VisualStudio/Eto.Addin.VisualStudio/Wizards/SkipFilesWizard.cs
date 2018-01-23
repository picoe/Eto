using Eto.Addin.Shared;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class SkipFilesWizard : IWizard
	{
		public class SkipGroup
		{
			public string Condition { get; set; }
			public List<SkipItem> Items { get; } = new List<SkipItem>();

			public static IEnumerable<SkipGroup> LoadXml(XElement element)
			{
				if (element == null)
					yield break;
				var ns = element.GetDefaultNamespace();
				foreach (var child in element.Elements(ns + "SkipGroup"))
				{
					yield return new SkipGroup(child);
				}
			}

			public SkipGroup(XElement element)
			{
				Condition = (string)element.Attribute("condition");
				Items.AddRange(SkipItem.LoadXml(element));
			}
		}

		public enum SkipMode
		{
			Include,
			Exclude,
			Iteration
		}

		public class SkipItem
		{
			public Regex FileMaskRegex { get; set; }

			public string Condition { get; set; }

			public SkipMode Mode { get; set; }

			public string FileMask { get; set; }

			public int Count { get; set; }
			public int CurrentCount { get; set; }

			public static IEnumerable<SkipItem> LoadXml(XElement element)
			{
				var ns = element.GetDefaultNamespace();
				foreach (var child in element.Elements())
				{
					yield return new SkipItem(child);
				}
			}

			public SkipItem(XElement element)
			{
				Condition = (string)element.Attribute("condition");
				FileMask = (string)element.Value;
				FileMaskRegex = new Regex(FileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", ".").Replace("\\", @"\\"));
				var name = element.Name.LocalName;
				SkipMode mode;
				Enum.TryParse<SkipMode>(name, out mode);
				Mode = mode;
				if (mode == SkipMode.Iteration)
				{
					int count;
					int.TryParse((string)element.Attribute("count"), out count);
					Count = count;
				}
			}
		}

		List<SkipGroup> skipFiles;

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;

			skipFiles = SkipGroup.LoadXml(doc.Root.Elements(ns + "SkipFiles").FirstOrDefault())
				.Where(r => replacementsDictionary.MatchesCondition(r.Condition))
				.ToList();
		}

		public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
		{
		}

		public void ProjectFinishedGenerating(EnvDTE.Project project)
		{
		}

		public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
		{
		}

		public void RunFinished()
		{
		}

		public bool ShouldAddProjectItem(string filePath)
		{
			bool include = true;
			if (skipFiles != null)
			{
				foreach (var skipGroup in skipFiles)
				{
					foreach (var item in skipGroup.Items)
					{
						if (!item.FileMaskRegex.IsMatch(filePath))
							continue;

						switch (item.Mode)
						{
							case SkipMode.Include:
								include = true;
								break;
							case SkipMode.Exclude:
								include = false;
								break;
							case SkipMode.Iteration:
								include = item.Count == item.CurrentCount;
								item.CurrentCount++;
								break;
						}
					}
				}
			}
			return include;
		}
	}
}