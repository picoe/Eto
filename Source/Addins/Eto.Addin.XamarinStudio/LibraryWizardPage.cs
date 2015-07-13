using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;

namespace Eto.Addin.XamarinStudio
{
	public class LibraryWizardPage : WizardPage
	{
		LibraryWizardPageUI view;

		public LibraryWizard Wizard { get; private set; }

		public LibraryWizardPage(LibraryWizard wizard)
		{
			this.Wizard = wizard;
			UseNET = false;
			UsePCL = true;
			Validate();
		}

		public string LibraryName
		{
			get { return Wizard.Parameters["ProjectName"]; }
			set
			{
				Wizard.Parameters["ProjectName"] = value;
				Validate();
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
			get { return "Eto.Forms Library Properties"; }
		}

		protected override object CreateNativeWidget()
		{
			return (view ?? (view = new LibraryWizardPageUI(this))).ToNative(true);
		}

		public void Validate()
		{
			CanMoveToNextPage = !string.IsNullOrWhiteSpace(LibraryName);
		}
	}
}

