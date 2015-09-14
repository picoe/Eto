using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TemplateWizard;
using Eto.Addin.Shared;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class OptionsWizard : BaseWizard
	{
		public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			var doc = Helpers.LoadWizardXml(replacementsDictionary);
			var ns = Helpers.WizardNamespace;

			var model = new OptionsPageModel(doc.Root.Elements(ns + "Options").FirstOrDefault());

			var view = new OptionsPageView(model);
			var dialog = new BaseDialog { Content = view, Title = model.Title };
			if (dialog.ShowModal(Helpers.MainWindow))
			{
				foreach (var option in model.Options)
				{
					var selected = option.Selected;
					if (selected != null)
					{
						foreach (var replacement in selected.Replacements)
						{
							if (replacementsDictionary.MatchesCondition(replacement.Condition))
							{
								replacementsDictionary[replacement.Name] = replacement.Content;
							}
						}
					}
				}
			}
			else
				throw new WizardBackoutException();
			
		}
	}
}
