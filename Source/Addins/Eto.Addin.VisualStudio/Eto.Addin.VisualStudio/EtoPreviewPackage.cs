using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace Eto.Addin.VisualStudio
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", Constants.VersionString)]
	[ProvideXmlEditorChooserDesignerView("Eto.Forms preview", ".xeto", LogicalViewID.Designer, 0x60,
		DesignerLogicalViewEditor = typeof(EditorFactory),
		TextLogicalViewEditor = VSConstants.VsEditorFactoryGuid.TextEditor_string,
		MatchExtensionAndNamespace = false)]
	[ProvideEditorExtension(typeof(EditorFactory), ".cs", 0x10, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".vb", 0x10, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".jeto", 0x10, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".xeto", 0x10, NameResourceID = 106)]

	//[ProvideLanguageExtension(VSConstants.VsLanguageServiceGuid.HtmlLanguageService_string, ".jeto")]

	[Guid(Constants.EtoPreviewPackagePkg_string)]
	public sealed class EtoPreviewPackage : Package
	{
		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

			//Create Editor Factory.
			base.RegisterEditorFactory(new EditorFactory(this));
		}
	}
}
