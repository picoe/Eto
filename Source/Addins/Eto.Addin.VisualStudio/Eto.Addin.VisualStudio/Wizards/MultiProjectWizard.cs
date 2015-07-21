using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Eto.Addin.VisualStudio.Wizards
{
	/// <summary>
	/// Wizard to create projects at the root folder instead of in a subfolder
	/// </summary>
	/// <remarks>
	/// Also, supports setting the startup project
	/// </remarks>
	public class MultiProjectWizard : IWizard
	{
		string defaultDestinationFolder;
		string basePath;
		string safeProjectName;
		EnvDTE.DTE dte;
		List<ProjectDefinition> definitions = new List<ProjectDefinition>();

		public class ProjectDefinition
		{
			public ProjectDefinition(XElement element, Dictionary<string, string> replacementsDictionary)
			{
				Name = Helpers.ReplaceProperties((string)element.Attribute("name"), replacementsDictionary);
				Startup = string.Equals((string)element.Attribute("startup"), "true", StringComparison.InvariantCultureIgnoreCase);
				Condition = (string)element.Attribute("condition");
				Path = Helpers.ReplaceProperties(element.Value, replacementsDictionary);
			}
			public string Name { get; set; }
			public string Path { get; set; }
			public bool Startup { get; set; }
			public string Condition { get; set; }
		}

		public void RunFinished()
		{
			Directory.Delete(defaultDestinationFolder, true);
			foreach (var definition in definitions)
			{
				AddProject(definition.Path, definition.Name);
			}
			var startupProject = definitions.FirstOrDefault(r => r.Startup);
			if (startupProject != null)
				dte.Solution.Properties.Item("StartupProject").Value = startupProject.Name;
		}

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			dte = automationObject as EnvDTE.DTE;
			defaultDestinationFolder = replacementsDictionary["$destinationdirectory$"];
			basePath = Path.GetDirectoryName((string)customParams[0]);

			safeProjectName = replacementsDictionary["$safeprojectname$"];

			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;
			foreach (var element in doc.Root.Elements(ns + "Projects").Elements(ns + "Project"))
			{
				var definition = new ProjectDefinition(element, replacementsDictionary);
				if (replacementsDictionary.MatchesCondition(definition.Condition))
					definitions.Add(definition);
			}
		}

		public bool ShouldAddProjectItem(string filePath)
		{
			return true;
		}

		public void AddProject(string templatePath, string name)
		{
			var solution = dte.Solution;

			string destinationPath = Path.Combine(Path.GetDirectoryName(defaultDestinationFolder), name);

			var proj = solution.AddFromTemplate(Path.Combine(basePath, templatePath), destinationPath, name, false);
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
	}
}