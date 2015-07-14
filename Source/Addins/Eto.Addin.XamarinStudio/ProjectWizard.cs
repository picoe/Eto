using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using MonoDevelop.Projects;
using System.Text;
using System.Linq;

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

		public bool ShowGenerateCombined
		{
			get { return IsSupportedParameter("GenerateCombined"); }
		}

		public bool ShowSharedCode
		{
			get { return IsSupportedParameter("SharedCode"); }
		}

		public bool SupportsPCL
		{
			get { return IsSupportedParameter("SupportsPCL"); }
		}

		public override WizardPage GetPage(int pageNumber)
		{
			return new ProjectWizardPage(this);
		}
	}
}

