using Eto.Drawing;
using Eto.Forms;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Eto.VisualStudioWizards
{
	public enum ProjectType
	{
		None = 0,
		Pcl,
		Full,
		Sal
	}

	public class ProjectTypeDefinition
	{
		public string Name { get; set; }

		public ProjectType Type { get; set; }

		public string Description { get; set; }

		public Image Image { get; set; }

		public static IEnumerable<ProjectTypeDefinition> Definitions
		{
			get
			{
				var icon = Icon.FromResource("Eto.VisualStudioWizards.Eto.Project.ico");
				yield return new ProjectTypeDefinition { Type = ProjectType.Pcl, Name = "Portable Class Library", Description = "Create a portable class library for your shared UI for maximum portability", Image = icon };
				yield return new ProjectTypeDefinition { Type = ProjectType.Full, Name = "Full .NET", Description = "Use the full .NET framework for your shared UI, for maximum compatibility with 3rd party libraries and Xaml support", Image = icon };
				yield return new ProjectTypeDefinition { Type = ProjectType.Sal, Name = "Shared Asset Library", Description = "Share the code and use #IFDEF's for platform-specific functionality", Image = icon };
			}
		}
	}

	public class ProjectTypeDialog : Dialog<ProjectType>
	{
		public ProjectTypeDialog(ProjectType[] types = null)
		{
			Title = "Select project type";
			ClientSize = new Size(400, -1);

			var layout = new TableLayout { Spacing = new Size(5, 5) };
			foreach (var def in ProjectTypeDefinition.Definitions)
			{
				if (types != null && types.Length > 0 && !types.Contains(def.Type))
					continue;
				var currentDef = def;
				var button = new Button { Size = new Size(-1, 100), Text = def.Name, Image = def.Image, ImagePosition = ButtonImagePosition.Above };
				button.Click += (sender, e) => Close(currentDef.Type);
				layout.Rows.Add(new TableRow (button, new Label { Text = def.Description, VerticalAlignment = VerticalAlignment.Center }));
			}

			Content = new TableLayout
			{
				Padding = new Padding(10),
				Spacing = new Size(10, 10),
				Rows =
				{
					new Label { Text = "Select the type of project to create for your shared UI:"},
					layout
				}
			};
		}
	}

	public class ProjectTypeWizard : IWizard
	{
		static ProjectTypeWizard()
		{
			EtoInitializer.Initialize();
		}

		public bool ShouldAddProjectItem(string filePath)
		{
			return true;
		}

		public void RunFinished()
		{
		}

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;
			var types = new List<ProjectType>();
			var projectTypes = doc.Root.Element(ns + "ProjectTypes");
			if (projectTypes != null)
			{
				var typesString = (string)projectTypes.Attribute("supported");
				var typesArray = typesString.Split(',');
				foreach (var type in typesArray)
					types.Add((ProjectType)Enum.Parse(typeof(ProjectType), type, true));
			}
	
			var dlg = new ProjectTypeDialog(types.ToArray());

			var result = dlg.ShowModal(Helpers.MainWindow);
			if (result == ProjectType.None)
				throw new WizardCancelledException();

			replacementsDictionary["$projectType$"] = result.ToString();
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