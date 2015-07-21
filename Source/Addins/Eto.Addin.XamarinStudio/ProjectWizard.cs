using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using MonoDevelop.Projects;
using System.Text;
using System.Linq;
using Eto.Addin.Shared;

namespace Eto.Addin.XamarinStudio
{
	public class ProjectWizard : TemplateWizard
	{
		static ProjectWizard()
		{
			EtoInitializer.Initialize();
		}

		public override void ConfigureWizard()
		{
			Parameters["BaseProjectName"] = Parameters["ProjectName"].Trim();
			base.ConfigureWizard();
		}

		public override string Id
		{
			get { return "Eto.Addin.XamarinStudio.ProjectWizard"; }
		}

		public override int TotalPages
		{
			get
			{
				var model = new ProjectWizardPageModel(new ParameterSource(this));
				return model.RequiresInput ? 1 : 0;
			}
		}

		public override WizardPage GetPage(int pageNumber)
		{
			return new ProjectWizardPage(this);
		}
	}
}

