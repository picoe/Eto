﻿using Eto.Addin.Shared;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public static Dictionary<string, string> Globals = new Dictionary<string, string>();
		static Dictionary<string, string> cachedGlobals;

		public class ProjectDefinition
		{
			public ProjectDefinition(XElement element, Dictionary<string, string> replacementsDictionary)
			{
				Name = Helpers.ReplaceProperties((string)element.Attribute("name"), replacementsDictionary);
				Startup = string.Equals((string)element.Attribute("startup"), "true", StringComparison.InvariantCultureIgnoreCase);
				Condition = (string)element.Attribute("condition");
				Path = Helpers.ReplaceProperties((string)element.Attribute("path") ?? element.Value, replacementsDictionary);
				Replacements = ReplacementGroup.LoadXml(element).ToList();
			}
			public string Name { get; set; }
			public string Path { get; set; }
			public bool Startup { get; set; }
			public string Condition { get; set; }
			public List<ReplacementGroup> Replacements { get; private set; }
		}

		public void RunFinished()
		{
			try
			{
				Directory.Delete(defaultDestinationFolder, true);
				foreach (var definition in definitions)
				{
					AddProject(definition.Path, definition.Name, definition.Replacements);
				}
				var startupProject = definitions.FirstOrDefault(r => r.Startup);
				if (startupProject != null)
					dte.Solution.Properties.Item("StartupProject").Value = startupProject.Name;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error adding projects\n{0}", ex);
				throw;
			}
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

			cachedGlobals = null;
			Globals.Clear();
			foreach (var replacement in replacementsDictionary)
				Globals[replacement.Key] = replacement.Value;
		}

		public bool ShouldAddProjectItem(string filePath)
		{
			return true;
		}

		public void AddProject(string templatePath, string name, IEnumerable<ReplacementGroup> replacements)
		{
			var solution = dte.Solution;
			if (replacements != null)
			{
				var projectGlobals = new Dictionary<string, string>(cachedGlobals ?? (cachedGlobals = Globals));

				replacements.SetMatchedItems(projectGlobals);
				Globals = projectGlobals;
			}

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
