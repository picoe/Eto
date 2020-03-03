using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using MonoDevelop.Projects;
using System.Text;
using System.Linq;
using Eto.Addin.Shared;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using System.IO;
using MonoDevelop.Ide.Gui;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Eto.Addin.MonoDevelop
{
	public class ProjectWizard : TemplateWizard
	{
		static ProjectWizard()
		{
			EtoInitializer.Initialize();
		}

		public override void ConfigureWizard()
		{
			//Parameters["BaseProjectName"] = Parameters["ProjectName"].Trim();
			base.ConfigureWizard();
		}

		public override string Id => "Eto.Addin.MonoDevelop.ProjectWizard";

		public override int TotalPages => Model.RequiresInput ? 1 : 0;

		ProjectWizardPageModel Model => new ProjectWizardPageModel(new ParameterSource(this), null);

		public override WizardPage GetPage(int pageNumber) => new ProjectWizardPage(this);

		public override void ItemsCreated(System.Collections.Generic.IEnumerable<IWorkspaceFileObject> items)
		{
			base.ItemsCreated(items);

#if OLD
			var model = new ProjectWizardPageModel(new ParameterSource(this), null);

			// hard coded as we can't get at any custom data out of the template..
			// at least try to be a little generic here..
			string fileName = null;
			string extension = null;
			if (model.UseXeto)
				extension = ".xeto";
			else if (model.UseJeto)
				extension = ".jeto";
			else if (model.UseCodePreview)
				extension = ".eto.cs";
			else
				fileName = model.IsLibrary ? "MyPanel.cs" : "MainForm.cs";

			IEnumerable<Project> projects = items.OfType<Solution>().SelectMany(r => r.GetAllProjects()).ToList();
			if (!projects.Any())
				projects = items.OfType<Project>();

			/*
			var item = items.SelectMany(r => r.GetItemFiles(false)).FirstOrDefault(r =>
				{
					if (extension != null && r.FileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
						return true;
					if (fileName != null && r.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
						return true;
					return false;

				});
			if (item != null)
			{
				
				IdeApp.Workbench.OpenDocument(item);
			}*/
			foreach (var proj in projects)
			{
				var item = proj.Files.FirstOrDefault(r => {
					if (extension != null && r.FilePath.FileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
						return true;
					if (fileName != null && r.FilePath.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
						return true;
					return false;
				});
				if (item != null)
					IdeApp.Workbench.OpenDocument(item.FilePath, proj);
			}
#endif

		}
	}
}