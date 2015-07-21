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
	public class ReplacementsWizard : IWizard
	{
		public class ProjectReference
		{
			public string Reference { get; set; }
			public string Guid { get; set; }
			public string Name { get; set; }
			public string Extension { get; set; }
			public ProjectReference(XElement element, Dictionary<string, string> replacementsDictionary)
			{
				Reference = Helpers.ReplaceProperties(element.Value, replacementsDictionary);
				Name = Helpers.ReplaceProperties((string)element.Attribute("name"), replacementsDictionary);
				Guid = Helpers.ReplaceProperties((string)element.Attribute("guid"), replacementsDictionary);
				Extension = Helpers.ReplaceProperties((string)element.Attribute("extension"), replacementsDictionary);
				if (string.IsNullOrEmpty(Extension))
					Extension = "csproj";
			}
		}

		public class ReplacementGroup
		{
			public string Condition { get; set; }
			public List<ReplacementItem> Replacements { get; } = new List<ReplacementItem>();

			public ReplacementGroup(XElement element)
			{
				Condition = (string)element.Attribute("condition");
				foreach (var replacement in element.Elements(element.GetDefaultNamespace() + "Replacement"))
				{
					Replacements.Add(new ReplacementItem(replacement));
				}
			}
		}

		public class ReplacementItem
		{
			public string Condition { get; set; }

			public string Name { get; set; }

			public string Content { get; set; }

			public ReplacementItem(XElement element)
			{
				Condition = (string)element.Attribute("condition");
				Name = (string)element.Attribute("name");
				Content = element.Value;
			}
		}

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;

			foreach (var element in doc.Root.Elements(ns + "Replacements").Elements(ns + "ReplacementGroup"))
			{
				var replacementGroup = new ReplacementGroup(element);
				if (replacementsDictionary.MatchesCondition(replacementGroup.Condition))
				{
					foreach (var replacement in replacementGroup.Replacements)
					{
						if (replacementsDictionary.MatchesCondition(replacement.Condition))
							replacementsDictionary[replacement.Name] = replacement.Content;
					}
				}
			}

			var references = new List<ProjectReference>();
			foreach (var element in doc.Root.Elements(ns + "ProjectReferences").Elements(ns + "ProjectReference"))
			{
				references.Add(new ProjectReference(element, replacementsDictionary));
			}
			const string ProjectImportsKey = "$ProjectImports$";
			const string ProjectReferencesKey = "$ProjectReferences$";
			if (!replacementsDictionary.ContainsKey(ProjectImportsKey))
				replacementsDictionary[ProjectImportsKey] = string.Empty;
			if (!replacementsDictionary.ContainsKey(ProjectReferencesKey))
				replacementsDictionary[ProjectReferencesKey] = string.Empty;

			if (references.Count > 0)
			{
				var isSharedLib = replacementsDictionary.GetParameter("UseSAL").ToBool();

				var projectReferences = new StringBuilder();

				if (isSharedLib)
				{
					const string def = @"
  <Import Project=""{0}.projitems"" Label=""Shared"" Condition=""Exists('{0}.projitems')"" />";
					foreach (var reference in references)
					{
						projectReferences.AppendFormat(def, reference.Reference, reference.Guid, reference.Name, reference.Extension);
					}
					replacementsDictionary[ProjectImportsKey] += projectReferences.ToString();
				}
				else
				{
					const string def = @"
    <ProjectReference Include=""{0}.{3}"">
        <Project>{{{1}}}</Project>
        <Name>{2}</Name>
    </ProjectReference>";
					foreach (var reference in references)
					{
						projectReferences.AppendFormat(def, reference.Reference, reference.Guid, reference.Name, reference.Extension);
					}
					replacementsDictionary[ProjectReferencesKey] += string.Format(@"
  <ItemGroup>{0}
  </ItemGroup>", projectReferences);
				}
			}
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
			return true;
		}
	}
}