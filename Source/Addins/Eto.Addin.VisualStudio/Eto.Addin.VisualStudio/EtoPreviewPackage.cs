using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Eto.Addin.VisualStudio
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", Constants.VersionString)]
	/*[ProvideXmlEditorChooserDesignerView("Eto.Forms preview", ".xeto", LogicalViewID.Designer, 0x60,
		//DesignerLogicalViewEditor = typeof(EditorFactory),
		DesignerLogicalViewEditor = typeof(EditorFactory),
		CodeLogicalViewEditor = typeof(EditorFactory), //VSConstants.VsEditorFactoryGuid.TextEditor_string,
		MatchExtensionAndNamespace = false)]*/
	//[ProvideEditorFactory(typeof(EditorFactory), 106)]
	//[ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.Designer_string)]
	[ProvideEditorExtension(typeof(EditorFactory), ".cs", 0x10, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".vb", 0x10, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".jeto", 0x32, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".xeto", 0x10, NameResourceID = 106)]

	//[ProvideLanguageExtension(VSConstants.VsLanguageServiceGuid.HtmlLanguageService_string, ".jeto")]

	[Guid(Constants.EtoPreviewPackagePkg_string)]
	public sealed class EtoPreviewPackage : Package
	{
		public EtoPreviewPackage()
		{
			Instance = this;
		}

		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

			//Create Editor Factory.
			base.RegisterEditorFactory(new EditorFactory(this));
		}

		public static EtoPreviewPackage Instance { get; private set; }

		IComponentModel componentModel;
		public IComponentModel ComponentModel
		{
			get
			{
				if (componentModel == null)
					componentModel = (IComponentModel)GetGlobalService(typeof(SComponentModel));
				return componentModel;
			}
		}

	}
}
