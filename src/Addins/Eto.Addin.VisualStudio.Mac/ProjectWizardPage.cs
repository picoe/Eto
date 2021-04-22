using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using Eto.Addin.Shared;

namespace Eto.Addin.VisualStudio.Mac
{
	public class ProjectWizardPage : WizardPage
	{
		public ProjectWizard Wizard { get; private set; }

		ProjectWizardPageView view;
		ProjectWizardPageModel model;

		public ProjectWizardPage(ProjectWizard wizard)
		{
			this.Wizard = wizard;
			var source = new ParameterSource(wizard);
			source.ParameterChanged += (name, value) =>
			{
				if (name == "AppName")
				{
					source.SetParameter("ProjectName", value);
					Validate();
				}
			};
			this.model = new ProjectWizardPageModel(source);
			Validate();
		}

		public override string Title
		{
			get { return model.Title; }
		}

		protected override object CreateNativeWidget<T>()
		{
			if (view == null)
				view = new ProjectWizardPageView(model);

			return XamMac2Helpers.ToNative (view, true);
		}

		public void Validate()
		{
			CanMoveToNextPage = model.IsValid;
		}
	}
}

