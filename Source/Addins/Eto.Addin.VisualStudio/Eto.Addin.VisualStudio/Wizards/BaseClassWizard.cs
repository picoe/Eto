using Eto.Forms;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Wizards
{
	public abstract class BaseClassWizard : BaseWizard
	{
		protected abstract string TypeName { get; }

		protected abstract IEnumerable<BaseClassDefinition> Definitions { get; }

		public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
		{
			if (runKind == WizardRunKind.AsNewItem)
			{
				var form = new BaseClassDialog(Definitions, TypeName);
				var definition = form.ShowModal(Helpers.MainWindow);
				if (definition != null)
				{
					replacementsDictionary.Add("$BaseClassName$", definition.Name);
					replacementsDictionary.Add("$Methods$", definition.Methods);
					replacementsDictionary.Add("$CodeContent$", definition.CodeContent);
					replacementsDictionary.Add("$JsonContent$", definition.JsonContent);
					replacementsDictionary.Add("$XamlContent$", definition.XamlContent);
					replacementsDictionary.Add("$XamlAttributes$", definition.XamlAttributes);
				}
				else throw new WizardCancelledException();
			}
		}
	}
}
