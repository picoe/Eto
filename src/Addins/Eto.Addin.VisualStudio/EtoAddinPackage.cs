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
using Eto.Addin.VisualStudio.Editor;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Eto.Addin.VisualStudio
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", Constants.VersionString)]
	/*
	[ProvideXmlEditorChooserDesignerView("Eto.Forms preview", ".xeto", LogicalViewID.Designer, 0x1000,
		DesignerLogicalViewEditor = Constants.EtoPreviewEditorFactory_string)]
	[ProvideXmlEditorChooserDesignerView("Eto.Forms preview", ".xaml", LogicalViewID.Designer, 0x1000,
		DesignerLogicalViewEditor = typeof(EditorFactory),
		Namespace = "http://schema.picoe.ca/eto.forms",
		MatchExtensionAndNamespace = true)]
	*/
	[ProvideEditorExtension(typeof(EditorFactory), ".cs", 0x100, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".vb", 0x100, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".fs", 0x100, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".jeto", 0x1000, NameResourceID = 106)]
	[ProvideEditorExtension(typeof(EditorFactory), ".xeto", 0x1000, NameResourceID = 106)]
	[ProvideEditorFactory(typeof(EditorFactory), 106, 
		CommonPhysicalViewAttributes = (int)__VSPHYSICALVIEWATTRIBUTES.PVA_SupportsPreview, 
		TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
	[ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.Any_string)]
	[ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.Code_string)]
	[ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.TextView_string)]
	[ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.Designer_string)]


	//[ProvideLanguageExtension(VSConstants.VsLanguageServiceGuid.HtmlLanguageService_string, ".jeto")]

	[Guid(Constants.EtoPreviewPackagePkg_string)]
	public sealed class EtoAddinPackage : Package
	{
		public EtoAddinPackage()
		{
			Instance = this;
			// reference Portable.Xaml so it gets included in output
			Portable.Xaml.XamlSchemaContext sc = new Portable.Xaml.XamlSchemaContext();

		}

		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

			//Create Editor Factory.
			base.RegisterEditorFactory(new EditorFactory(this));
		}

		public static EtoAddinPackage Instance { get; private set; }

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
