using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

using Eto.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Eto.Designer;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Eto.Addin.VisualStudio.Wizards;

namespace Eto.Addin.VisualStudio.Editor
{
	[Guid(Constants.EtoPreviewEditorFactory_string)]
	public sealed class EditorFactory : IVsEditorFactory, IDisposable
	{
		EtoAddinPackage editorPackage;
		ServiceProvider vsServiceProvider;

		static EditorFactory()
		{
			EtoInitializer.Initialize();
		}

		public EditorFactory(EtoAddinPackage package)
		{
			this.editorPackage = package;
		}

		/// <summary>
		/// Since we create a ServiceProvider which implements IDisposable we
		/// also need to implement IDisposable to make sure that the ServiceProvider's
		/// Dispose method gets called.
		/// </summary>
		public void Dispose()
		{
			if (vsServiceProvider != null)
			{
				vsServiceProvider.Dispose();
			}
		}

		#region IVsEditorFactory Members

		public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
		{
			vsServiceProvider = new ServiceProvider(psp);
			return VSConstants.S_OK;
		}

		public object GetService(Type serviceType)
		{
			return vsServiceProvider.GetService(serviceType);
		}

		// This method is called by the Environment (inside IVsUIShellOpenDocument::
		// OpenStandardEditor and OpenSpecificEditor) to map a LOGICAL view to a 
		// PHYSICAL view. A LOGICAL view identifies the purpose of the view that is
		// desired (e.g. a view appropriate for Debugging [LOGVIEWID_Debugging], or a 
		// view appropriate for text view manipulation as by navigating to a find
		// result [LOGVIEWID_TextView]). A PHYSICAL view identifies an actual type 
		// of view implementation that an IVsEditorFactory can create. 
		//
		// NOTE: Physical views are identified by a string of your choice with the 
		// one constraint that the default/primary physical view for an editor  
		// *MUST* use a NULL string as its physical view name (*pbstrPhysicalView = NULL).
		//
		// NOTE: It is essential that the implementation of MapLogicalView properly
		// validates that the LogicalView desired is actually supported by the editor.
		// If an unsupported LogicalView is requested then E_NOTIMPL must be returned.
		//
		// NOTE: The special Logical Views supported by an Editor Factory must also 
		// be registered in the local registry hive. LOGVIEWID_Primary is implicitly 
		// supported by all editor types and does not need to be registered.
		// For example, an editor that supports a ViewCode/ViewDesigner scenario
		// might register something like the following:
		//        HKLM\Software\Microsoft\VisualStudio\<version>\Editors\
		//            {...guidEditor...}\
		//                LogicalViews\
		//                    {...LOGVIEWID_TextView...} = s ''
		//                    {...LOGVIEWID_Code...} = s ''
		//                    {...LOGVIEWID_Debugging...} = s ''
		//                    {...LOGVIEWID_Designer...} = s 'Form'
		//
		public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
		{
			pbstrPhysicalView = null;    // initialize out parameter

			// we support only a single physical view
			if (
				rguidLogicalView == VSConstants.LOGVIEWID.Primary_guid
				//|| rguidLogicalView == VSConstants.LOGVIEWID.Code_guid
				|| rguidLogicalView == VSConstants.LOGVIEWID.TextView_guid
				|| rguidLogicalView == VSConstants.LOGVIEWID.Designer_guid
			)
			{
				//pbstrPhysicalView = "design";
				return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
			}
			return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
		}

		public int Close()
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Used by the editor factory to create an editor instance. the environment first determines the 
		/// editor factory with the highest priority for opening the file and then calls 
		/// IVsEditorFactory.CreateEditorInstance. If the environment is unable to instantiate the document data 
		/// in that editor, it will find the editor with the next highest priority and attempt to so that same 
		/// thing. 
		/// NOTE: The priority of our editor is 32 as mentioned in the attributes on the package class.
		/// 
		/// Since our editor supports opening only a single view for an instance of the document data, if we 
		/// are requested to open document data that is already instantiated in another editor, or even our 
		/// editor, we return a value VS_E_INCOMPATIBLEDOCDATA.
		/// </summary>
		/// <param name="grfCreateDoc">Flags determining when to create the editor. Only open and silent flags 
		/// are valid
		/// </param>
		/// <param name="pszMkDocument">path to the file to be opened</param>
		/// <param name="pszPhysicalView">name of the physical view</param>
		/// <param name="pvHier">pointer to the IVsHierarchy interface</param>
		/// <param name="itemid">Item identifier of this editor instance</param>
		/// <param name="punkDocDataExisting">This parameter is used to determine if a document buffer 
		/// (DocData object) has already been created
		/// </param>
		/// <param name="ppunkDocView">Pointer to the IUnknown interface for the DocView object</param>
		/// <param name="ppunkDocData">Pointer to the IUnknown interface for the DocData object</param>
		/// <param name="pbstrEditorCaption">Caption mentioned by the editor for the doc window</param>
		/// <param name="pguidCmdUI">the Command UI Guid. Any UI element that is visible in the editor has 
		/// to use this GUID. This is specified in the .vsct file
		/// </param>
		/// <param name="pgrfCDW">Flags for CreateDocumentWindow</param>
		/// <returns></returns>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public int CreateEditorInstance(
						uint grfCreateDoc,
						string pszMkDocument,
						string pszPhysicalView,
						IVsHierarchy pvHier,
						uint itemid,
						System.IntPtr punkDocDataExisting,
						out System.IntPtr ppunkDocView,
						out System.IntPtr ppunkDocData,
						out string pbstrEditorCaption,
						out Guid pguidCmdUI,
						out int pgrfCDW)
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering {0} CreateEditorInstace()", this.ToString()));

			// Initialize to null
			ppunkDocView = IntPtr.Zero;
			ppunkDocData = IntPtr.Zero;
			pguidCmdUI = Constants.EtoPreviewEditorFactory_guid;
			pgrfCDW = 0;
			pbstrEditorCaption = null;


			if (!BuilderInfo.Supports(pszMkDocument))
				return VSConstants.VS_E_UNSUPPORTEDFORMAT;

			// Validate inputs
			if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
			{
				return VSConstants.E_INVALIDARG;
			}

			// Get or open text buffer
			var textBuffer = GetTextBuffer(punkDocDataExisting, pszMkDocument);
			
			if (textBuffer == null)
				return VSConstants.VS_E_INCOMPATIBLEDOCDATA;

			if (punkDocDataExisting != IntPtr.Zero)
			{
				ppunkDocData = punkDocDataExisting;
				Marshal.AddRef(ppunkDocData);
			}
			else
			{
				ppunkDocData = Marshal.GetIUnknownForObject(textBuffer);
			}

			// Create the Document (editor)
			var editor = new EtoPreviewPane(editorPackage, pszMkDocument, textBuffer);
			ppunkDocView = Marshal.GetIUnknownForObject(editor);
			//pbstrEditorCaption = " [Preview]";
			return VSConstants.S_OK;
		}

		IVsTextLines GetTextBuffer(System.IntPtr punkDocDataExisting, string fileName)
		{
			IVsTextLines textBuffer = null;
			if (punkDocDataExisting == IntPtr.Zero)
			{
				// load file using invisible editor
				var invisibleEditorManager = Services.GetService<SVsInvisibleEditorManager, IVsInvisibleEditorManager>();
				IVsInvisibleEditor invisibleEditor;
				invisibleEditorManager.RegisterInvisibleEditor(
					fileName
					, pProject: null
					, dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
					, pFactory: null
					, ppEditor: out invisibleEditor);
				IntPtr docDataPointer;
				var guidIVsTextLines = typeof(IVsTextLines).GUID;
				invisibleEditor.GetDocData(
					fEnsureWritable: 1
					, riid: ref guidIVsTextLines
					, ppDocData: out docDataPointer);
				var docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

				// assign the right language service for xml/json
				if (fileName.EndsWith(".xeto", StringComparison.OrdinalIgnoreCase))
				{
					Guid langId;
					var textManagerSvc = Services.GetService<SVsTextManager, IVsTextManager>();
					textManagerSvc.MapFilenameToLanguageSID("file.xml", out langId);
					docData.SetLanguageServiceID(ref langId);
				}
				else if (fileName.EndsWith(".jeto", StringComparison.OrdinalIgnoreCase))
				{
					Guid langId;
					var textManagerSvc = Services.GetService<SVsTextManager, IVsTextManager>();
					textManagerSvc.MapFilenameToLanguageSID("file.json", out langId);
					docData.SetLanguageServiceID(ref langId);
				}
				return docData;
			}
			else
			{
				// punkDocDataExisting is *not* null which means the file *is* already open. 
				// We need to verify that the open document is in fact a TextBuffer. If not
				// then we need to return the special error code VS_E_INCOMPATIBLEDOCDATA which
				// causes the user to be prompted to close the open file. If the user closes the
				// file then we will be called again with punkDocDataExisting as null

				// QI existing buffer for text lines
				textBuffer = Marshal.GetObjectForIUnknown(punkDocDataExisting) as IVsTextLines;
				if (textBuffer == null)
				{
					return null;
				}
			}
			return textBuffer;
		}

		#endregion
	}
}
