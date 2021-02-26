using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Windows.Wizards
{
	public class BaseWizard : IWizard
	{

		static BaseWizard()
		{
			EtoInitializer.Initialize();
		}

		public virtual bool ShouldAddProjectItem(string filePath)
		{
			return true;
		}

		public virtual void RunFinished()
		{
		}

		public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			if (replacementsDictionary.TryGetValue("SupportedParameters", out var supportedParameters))
			{
				var parametersArray = supportedParameters.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var parameter in parametersArray)
					replacementsDictionary.SetSupportedParameter(parameter, true);
            }
		}

		public virtual void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
		{
		}

		public virtual void ProjectFinishedGenerating(EnvDTE.Project project)
		{
		}

		public virtual void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
		{
		}
	}
}
