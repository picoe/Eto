using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Eto.VisualStudioWizards
{
	public class ProjectTypeReplacementWizard : IWizard
	{
		public class ProjectReference
		{
			public string Reference { get; set; }
			public string Guid { get; set; }
			public string Name { get; set; }
			public ProjectReference(XElement element, Dictionary<string, string> replacementsDictionary)
			{
				Reference = Helpers.ReplaceProperties(element.Value, replacementsDictionary);
				Name = Helpers.ReplaceProperties((string)element.Attribute("name"), replacementsDictionary);
				Guid = Helpers.ReplaceProperties((string)element.Attribute("guid"), replacementsDictionary);
			}
		}

		public class ProjectTypeReplacment
		{
			public ProjectType Type { get; set; }

			public string Name { get; set; }

			public string Content { get; set; }

			public ProjectTypeReplacment(XElement element, Dictionary<string, string> replacementsDictionary)
			{
				Type = (ProjectType)Enum.Parse(typeof(ProjectType), (string)element.Parent.Attribute("type"), true);
				Name = (string)element.Attribute("name");
				Content = element.Value;
			}
		}

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;
			var references = new List<ProjectReference>();
			foreach (var element in doc.Root.Elements(ns + "ProjectReferences").Elements(ns + "ProjectReference"))
			{
				references.Add(new ProjectReference(element, replacementsDictionary));
			}
			replacementsDictionary["$ProjectImports$"] = string.Empty;
			replacementsDictionary["$ProjectReferences$"] = string.Empty;

			var projectType = replacementsDictionary.ContainsKey("$projectType$") ? replacementsDictionary["$projectType$"] : null;
			projectType = projectType ?? (replacementsDictionary.ContainsKey("$root.projectType$") ? replacementsDictionary["$root.projectType$"] : null);

			if (!string.IsNullOrEmpty(projectType))
			{
				foreach (var element in doc.Root.Elements(ns + "ProjectTypes").Elements(ns + "ProjectType").Elements(ns + "Replacement"))
				{
					var replacement = new ProjectTypeReplacment(element, replacementsDictionary);
					if (string.Equals(projectType, replacement.Type.ToString(), StringComparison.OrdinalIgnoreCase))
						replacementsDictionary[replacement.Name] = replacement.Content;
				}

				var projectReferences = new StringBuilder();
				string def;

				var isSharedLib = string.Equals(projectType, ProjectType.Sal.ToString(), StringComparison.OrdinalIgnoreCase);
				if (isSharedLib)
				{
					def = @"
  <Import Project=""{0}.projitems"" Label=""Shared"" Condition=""Exists('{0}.projitems')"" />";
				}
				else
				{
					def = @"
    <ProjectReference Include=""{0}.csproj"">
      <Project>{{{1}}}</Project>
      <Name>{2}</Name>
    </ProjectReference>";
				}

				foreach (var reference in references)
				{
					projectReferences.AppendFormat(def, reference.Reference, reference.Guid, reference.Name);
				}
				if (isSharedLib)
				{
					replacementsDictionary["$ProjectImports$"] = projectReferences.ToString();
				}
				else if (projectReferences.Length > 0)
				{
					replacementsDictionary["$ProjectReferences$"] = string.Format(@"
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
