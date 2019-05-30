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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Eto.Addin.VisualStudio.Wizards;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Xml;

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
				|| rguidLogicalView == VSConstants.LOGVIEWID.Code_guid
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

			object prjItemObject;
			var projectItemId = VSConstants.VSITEMID_ROOT;
			pvHier.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjItemObject);
			var proj = prjItemObject as EnvDTE.Project;

			// Get or open text buffer
			var textBuffer = GetTextBuffer(punkDocDataExisting, pszMkDocument, ToVsProject(proj));
			
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


			var outputFile = GetAssemblyPath(proj);
			var references = GetReferences(proj).ToList();
			//var outputDir = Path.GetDirectoryName(outputFile);

			// Create the Document (editor)
			var editor = new EtoPreviewPane(editorPackage, pszMkDocument, textBuffer, outputFile, references);
			ppunkDocView = Marshal.GetIUnknownForObject(editor);
			//pbstrEditorCaption = " [Preview]";
			return VSConstants.S_OK;
		}

		public IVsHierarchy ToHierarchy(EnvDTE.Project project)
		{
			if (project == null) throw new ArgumentNullException("project");

			var solutionService = Services.GetService<SVsSolution, IVsSolution>();
			IVsHierarchy projectHierarchy = null;
			if (solutionService.GetProjectOfUniqueName(project.UniqueName, out projectHierarchy) == VSConstants.S_OK)
			{
				if (projectHierarchy != null)
				{
					return projectHierarchy;
				}
			}
			return null;
		}

		public IVsProject ToVsProject(EnvDTE.Project project)
		{
			if (project == null) throw new ArgumentNullException("project");
			IVsProject vsProject = ToHierarchy(project) as IVsProject;
			if (vsProject == null)
			{
				throw new ArgumentException("Project is not a VS project.");
			}
			return vsProject;
		}

		public static IEnumerable<string> GetReferences(EnvDTE.Project project)
		{
			var vsproject = project.Object as VSLangProj.VSProject;
			// note: you could also try casting to VsWebSite.VSWebSite

			foreach (VSLangProj.Reference reference in vsproject.References)
			{
				if (reference.SourceProject == null)
				{
					// skip framework assemblies
					if (((dynamic)reference).AutoReferenced)
						continue;
					// This is an assembly reference
					if (reference.Path != null)
						yield return reference.Path;
				}
				else
				{
					// This is a project reference
					var path = GetAssemblyPath(reference.SourceProject);
					if (path != null)
						yield return path;
				}
			}
		}
		static string GetAssemblyPath(EnvDTE.Project vsProject)
		{
			string fullPath = vsProject.Properties.Item("FullPath").Value.ToString();
			string outputPath = vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
			string outputDir = Path.Combine(fullPath, outputPath);
			string outputFileName = vsProject.Properties.Item("OutputFileName").Value.ToString();
			string assemblyPath = Path.Combine(outputDir, outputFileName);
			return assemblyPath;
		}

		IVsTextLines GetTextBuffer(IntPtr punkDocDataExisting, string fileName, IVsProject project)
		{
			IVsTextLines textBuffer = null;
			if (punkDocDataExisting == IntPtr.Zero)
			{
				// load file using invisible editor
				var invisibleEditorManager = Services.GetService<SVsInvisibleEditorManager, IVsInvisibleEditorManager>();
				IVsInvisibleEditor invisibleEditor;
				var result = invisibleEditorManager.RegisterInvisibleEditor(
					fileName
					, pProject: project
					, dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
					, pFactory: null
					, ppEditor: out invisibleEditor);
				if (invisibleEditor == null) // sometimes when closing files this will be null?
					return null;
				IntPtr docDataPointer;
				var guidIVsTextLines = typeof(IVsTextLines).GUID;
				result = invisibleEditor.GetDocData(
					fEnsureWritable: 1
					, riid: ref guidIVsTextLines
					, ppDocData: out docDataPointer);
				var docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

				/* set site for the doc data?
				var objWSite = docData as IObjectWithSite;
				if (objWSite != null)
				{
					var oleServiceProvider = (IOleServiceProvider)GetService(typeof(IOleServiceProvider));
					objWSite.SetSite(oleServiceProvider);
				}*/

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
