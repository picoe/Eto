using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class RootWizard : IWizard
	{
		public static Dictionary<string, string> Globals = new Dictionary<string, string>();

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			foreach (var replacement in replacementsDictionary)
				Globals[replacement.Key] = replacement.Value;
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
