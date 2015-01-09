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
	public class ChildWizard : IWizard
	{
		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			foreach (var replacement in RootWizard.Globals)
				replacementsDictionary["$root." + replacement.Key.TrimStart('$')] = replacement.Value;
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
