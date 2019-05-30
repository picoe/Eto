using Eto.Addin.Shared;
using Eto.Drawing;
using Eto.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class ParameterSource : IParameterSource
	{
		readonly Dictionary<string, string> replacements;
        public ParameterSource(Dictionary<string, string> replacements)
		{
			this.replacements = replacements;
        }

		public bool SeparateMac
		{
			get { return false; }
		}

		public Version TargetFrameworkVersion
        {
            get
            {
                Version ver;
                if (Version.TryParse(replacements["$targetframeworkversion$"], out ver))
                    return ver;
                return new Version(4, 5);
            }
        }

        public string GetParameter(string parameter)
		{
			return replacements.GetParameter(parameter);
        }

		public bool IsSupportedParameter(string parameter)
		{
			return replacements.IsSupportedParameter(parameter);
		}

		public void SetParameter(string parameter, string value)
		{
			replacements.SetParameter(parameter, value);
		}
	}

	public class ProjectWizard : BaseWizard
	{

		public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

			var source = new ParameterSource(replacementsDictionary);
			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;

			var model = new ProjectWizardPageModel(source, doc.Root.Elements(ns + "Options").FirstOrDefault());
			if (model.SupportsAppName)
				model.AppName = replacementsDictionary["$projectname$"];
			if (model.RequiresInput)
			{
				var panel = new ProjectWizardPageView(model);
				var dialog = new BaseDialog { Content = panel, Title = model.Title, ClientSize = new Size(-1, 400), Style="themed" };
				if (!dialog.ShowModal(Helpers.MainWindow))
					throw new WizardBackoutException();
			}
		}
	}

}