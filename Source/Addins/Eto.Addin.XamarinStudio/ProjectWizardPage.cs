using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;

namespace Eto.Addin.XamarinStudio
{
	public class ProjectWizardPage : WizardPage
	{
		public ProjectWizard Wizard { get; private set; }
		ProjectWizardPageUI view;

		public ProjectWizardPage(ProjectWizard wizard)
		{
			this.Wizard = wizard;
			GenerateCombined = true;
			UseSAL = false;
			UseNET = !wizard.SupportsPCL;
			UsePCL = wizard.SupportsPCL;
			Validate();
		}

		string appName;
		public string AppName
		{
			get { return appName; }
			set
			{
				appName = value;
				Wizard.Parameters["AppName"] = value;
				Wizard.Parameters["ProjectName"] = value;
				Validate();
			}
		}

		bool generateCombined;
		public bool GenerateCombined
		{
			get { return generateCombined; }
			set
			{
				generateCombined = value;
				Wizard.Parameters["GenerateCombined"] = value.ToString();
			}
		}

		bool usePCL;
		public bool UsePCL
		{
			get { return usePCL; }
			set
			{
				usePCL = value;
				Wizard.Parameters["UsePCL"] = value.ToString();
			}
		}
		bool useSAL;
		public bool UseSAL
		{
			get { return useSAL; }
			set
			{
				useSAL = value;
				Wizard.Parameters["UseSAL"] = value.ToString();
			}
		}
		bool useNET;
		public bool UseNET
		{
			get { return useNET; }
			set
			{
				useNET = value;
				Wizard.Parameters["UseNET"] = value.ToString();
			}
		}

		public override string Title
		{
			get { return "Eto.Forms Application Properties"; }
		}

		protected override object CreateNativeWidget()
		{
			return (view ?? (view = new ProjectWizardPageUI(this))).ToNative(true);
		}

		public void Validate()
		{
			CanMoveToNextPage = !string.IsNullOrWhiteSpace(AppName);
		}
	}
}

