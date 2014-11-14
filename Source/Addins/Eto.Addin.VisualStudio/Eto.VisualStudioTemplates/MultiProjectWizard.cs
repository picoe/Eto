using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Eto.VisualStudioTemplates
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

		static readonly Regex propertyRegex = new Regex(@"\$\w+\$", RegexOptions.Compiled);
		
		public class ProjectDefinition
		{
			public string Name { get; set; }
			public string Path { get; set; }
			public bool Startup { get; set; }
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

			var wizardData = "<root>" + replacementsDictionary["$wizarddata$"] + "</root>";
			var doc = new XmlDocument();
			doc.LoadXml(wizardData);
			var nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("vs", "http://schemas.microsoft.com/developer/vstemplate/2005");
			foreach (var element in doc.SelectNodes("//vs:Projects/vs:Project", nsmgr).OfType<XmlElement>())
			{
				var name = propertyRegex.Replace(element.GetAttribute("Name"), match => replacementsDictionary[match.Value]);
				var startup = string.Equals(element.GetAttribute("Startup"), "true", StringComparison.InvariantCultureIgnoreCase);
				definitions.Add(new ProjectDefinition { Name = name, Path = element.InnerText, Startup = startup });
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

			solution.AddFromTemplate(Path.Combine(basePath, templatePath), destinationPath, name, false);
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