using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;

namespace Eto.Addin.XamarinStudio
{
	public class LibraryWizard : TemplateWizard
	{
		static LibraryWizard()
		{
			EtoInitializer.Initialize();
		}

		public override string Id
		{
			get { return "Eto.Addin.XamarinStudio.LibraryWizard"; }
		}

		public bool ShowLibraryType
		{
			get { return IsSupportedParameter("LibraryType"); }
		}

		public override int TotalPages
		{
			get { return ShowLibraryType ? 1 : 0; }
		}

		public override WizardPage GetPage(int pageNumber)
		{
			return new LibraryWizardPage(this);
		}
	}
}

