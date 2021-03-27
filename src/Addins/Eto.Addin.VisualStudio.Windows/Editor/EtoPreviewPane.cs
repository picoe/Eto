using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

using Eto.Forms;
using System.Linq;
using System.Windows.Forms.Integration;
using System.Text;
using Eto.Designer;
using Eto.Designer.Builders;
using System.Windows.Threading;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Media;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Collections.Generic;

namespace Eto.Addin.VisualStudio.Windows.Editor
{
	[ComVisible(true)]
	public sealed class EtoPreviewPane : Microsoft.VisualStudio.Shell.WindowPane,
		IVsWindowPane,
		IVsTextBufferDataEvents,
		IVsTextLinesEvents,
		IOleCommandTarget,
		IVsCodeWindow, // support setting breakpoints
		IVsCodeWindowEx
	{
		IVsTextLines textBuffer;
		CodeEditorHost editor;
		EtoAddinPackage package;
		PreviewEditorView preview;
		PreviewEditorViewSplitter previewSplitter;
		Panel editorControl;
		uint dataEventsCookie;
		uint linesEventsCookie;

		IWpfTextView WpfTextView => editor.textViewHost?.TextView;

		bool CodeEditorHasFocus => WpfTextView?.VisualElement?.IsKeyboardFocused == true;

		void RegisterIndependentView(bool subscribe)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			var textManager = (IVsTextManager)GetService(typeof(SVsTextManager));
			if (textManager != null)
			{
				if (subscribe)
					textManager.RegisterIndependentView(this, textBuffer);
				else
					textManager.UnregisterIndependentView(this, textBuffer);
			}

			var dataEvents = GetConnectionPoint<IVsTextBufferDataEvents>();
			if (dataEvents != null)
			{
				if (subscribe)
					dataEvents.Advise(this, out dataEventsCookie);
				else if (dataEventsCookie != 0)
				{
					dataEvents.Unadvise(dataEventsCookie);
					dataEventsCookie = 0;
				}
			}

			var linesEvents = GetConnectionPoint<IVsTextLinesEvents>();
			if (linesEvents != null)
			{
				if (subscribe)
					linesEvents.Advise(this, out linesEventsCookie);
				else if (linesEventsCookie != 0)
				{
					linesEvents.Unadvise(linesEventsCookie);
					linesEventsCookie = 0;
				}
			}

		}

		#region "Window.Pane Overrides"
		/// <summary>
		/// Constructor that calls the Microsoft.VisualStudio.Shell.WindowPane constructor then
		/// our initialization functions.
		/// </summary>
		/// <param name="package">Our Package instance.</param>
		public EtoPreviewPane(EtoAddinPackage package, string fileName, IVsTextLines textBuffer, string mainAssembly, IEnumerable<string> references, CodeEditorHost codeEditor)
			: base(package)
		{
			this.editor = codeEditor;
			this.package = package;
			this.textBuffer = textBuffer;
			FileName = fileName;

			editorControl = new Panel();
			editorControl.Content = editor.wpfElement.ToEto();

			previewSplitter = new PreviewEditorViewSplitter(editorControl, mainAssembly, references, () => textBuffer?.GetText());
			previewSplitter.GotFocus += (sender, e) =>
			{
				WpfTextView?.VisualElement?.Focus();
			};
			preview = previewSplitter.Preview;

			var content = previewSplitter.ToNative(true);

			if (!preview.SetBuilder(fileName))
				throw new InvalidOperationException(string.Format("Could not find builder for file {0}", fileName));
			Content = content;

		}

		protected override bool PreProcessMessage(ref System.Windows.Forms.Message m)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// copy the Message into a MSG[] array, so we can pass
			// it along to the active core editor's IVsWindowPane.TranslateAccelerator
			var pMsg = new MSG[1];
			pMsg[0].hwnd = m.HWnd;
			pMsg[0].message = (uint)m.Msg;
			pMsg[0].wParam = m.WParam;
			pMsg[0].lParam = m.LParam;

			var filterKeys2 = Services.GetService<SVsFilterKeys, IVsFilterKeys2>();
			if (filterKeys2 != null)
			{
				// support global keyboard shortcuts

				int hr = filterKeys2.TranslateAcceleratorEx(pMsg,
					(uint)__VSTRANSACCELEXFLAGS.VSTAEXF_UseGlobalKBScope,
					0,
					null,
					out _,
					out _,
					out _,
					out _);
				if (hr == 0)
					return true;
			}

			if (editor.viewAdapter != null)
			{
				var vsWindowPane = (IVsWindowPane)editor.viewAdapter;

				return vsWindowPane.TranslateAccelerator(pMsg) == 0;
			}
			return base.PreProcessMessage(ref m);
		}

		const int NotSupported = (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			var hr = NotSupported;
			if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97 && nCmdID == (int)VSConstants.VSStd97CmdID.ViewCode)
			{
				ViewCode();
				return VSConstants.S_OK;
			}
			if (CodeEditorHasFocus && editor.viewAdapter != null)
			{
				var cmdTarget = (IOleCommandTarget)editor.viewAdapter;
				hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
			}
			return hr;
		}

		int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]prgCmds, IntPtr pCmdText)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			var hr = NotSupported;
			if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
			{
				for (int i = 0; i < prgCmds.Length; i++)
				{
					if (prgCmds[i].cmdID == (int)VSConstants.VSStd97CmdID.ViewCode)
					{
						prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
						return VSConstants.S_OK;
                    }
				}
			}

			if (CodeEditorHasFocus && editor.viewAdapter != null)
			{
				var cmdTarget = (IOleCommandTarget)editor.viewAdapter;
				hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
			}
			return hr;
		}

		protected override void Initialize()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			base.Initialize();

			RegisterIndependentView(true);

			SetupCommands();

			InheritKeyBindings();

			preview.Update();
		}

		void InheritKeyBindings()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// allow text editor keyboard shortcuts to be used in our embedded editor
			var frame = (IVsWindowFrame)GetService(typeof(SVsWindowFrame));
			if (frame != null)
			{
				Guid commandUiGuid = VSConstants.GUID_TextEditorFactory;
				frame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref commandUiGuid);
			}
		}

		IConnectionPoint GetConnectionPoint<T>()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			var container = textBuffer as IConnectionPointContainer;
			var guid = typeof(T).GUID;
			IConnectionPoint cp;
			container.FindConnectionPoint(ref guid, out cp);
			return cp;
		}

#endregion


		/// <summary>
		/// returns the name of the file currently loaded
		/// </summary>
		public string FileName { get; private set; }

		protected override void Dispose(bool disposing)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				if (disposing)
				{

					RegisterIndependentView(false);

					editorControl?.Dispose();
					editorControl = null;

					previewSplitter?.Dispose();
					previewSplitter = null;

					GC.SuppressFinalize(this);
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

#region Command Handling Functions

		void SetupCommands()
		{
			var mcs = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (mcs != null)
			{
				//mcs.AddCommand(new MenuCommand((sender, e) => ViewCode(), new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.ViewCode)));
				mcs.AddCommand(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.ViewCode, ViewCode);
			}
		}

		void ViewCode()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// Open the referenced document using the standard text editor.
			var codeFile = preview.GetCodeFile(FileName);

			IVsWindowFrame frame;
			IVsUIHierarchy hierarchy;
			uint itemid;
			if (!VsShellUtilities.IsDocumentOpen(this, codeFile, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame)
				&& !VsShellUtilities.IsDocumentOpen(this, codeFile, VSConstants.LOGVIEWID.TextView_guid, out hierarchy, out itemid, out frame))
			{
				VsShellUtilities.OpenDocumentWithSpecificEditor(this, codeFile, VSConstants.VsEditorFactoryGuid.TextEditor_guid, VSConstants.LOGVIEWID.Primary_guid, out hierarchy, out itemid, out frame);
			}
			ErrorHandler.ThrowOnFailure(frame.Show());
		}

#endregion


		void IVsTextBufferDataEvents.OnFileChanged(uint grfChange, uint dwFileAttrs)
		{
			preview.Update();
		}

		int IVsTextBufferDataEvents.OnLoadCompleted(int fReload)
		{
			preview.Update();
			return VSConstants.S_OK;
		}

		void IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine)
		{
		}

		void IVsTextLinesEvents.OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
		{
			preview.Update();
		}

		public override int SaveUIState(out Stream stateStream)
		{
			var ret = base.SaveUIState(out stateStream);
			stateStream = stateStream ?? new MemoryStream();
			using (var bw = new BinaryWriter(stateStream, Encoding.UTF8, true))
				bw.Write(previewSplitter.RelativePosition);
			return 0;
		}

		public override int LoadUIState(Stream stateStream)
		{
			var ret = base.LoadUIState(stateStream);
			if (stateStream != null)
			{
				using (var br = new BinaryReader(stateStream, Encoding.UTF8, true))
					previewSplitter.RelativePosition = br.ReadDouble();
			}
			return 0;
		}

		public int SetBuffer(IVsTextLines pBuffer) => editor.codeWindow.SetBuffer(pBuffer);
		public int GetBuffer(out IVsTextLines ppBuffer) => editor.codeWindow.GetBuffer(out ppBuffer);
		public int GetPrimaryView(out IVsTextView ppView) => editor.codeWindow.GetPrimaryView(out ppView);
		public int GetSecondaryView(out IVsTextView ppView) => editor.codeWindow.GetSecondaryView(out ppView);
		public int SetViewClassID(ref Guid clsidView) => editor.codeWindow.SetViewClassID(ref clsidView);
		public int GetViewClassID(out Guid pclsidView) => editor.codeWindow.GetViewClassID(out pclsidView);
		public int SetBaseEditorCaption(string[] pszBaseEditorCaption) => editor.codeWindow.SetBaseEditorCaption(pszBaseEditorCaption);
		public int GetEditorCaption(READONLYSTATUS dwReadOnly, out string pbstrEditorCaption) => editor.codeWindow.GetEditorCaption(dwReadOnly, out pbstrEditorCaption);
		public int Close() => editor.codeWindow.Close();
		public int GetLastActiveView(out IVsTextView ppView) => editor.codeWindow.GetLastActiveView(out ppView);

		int IVsCodeWindowEx.Initialize(uint grfCodeWindowBehaviorFlags, VSUSERCONTEXTATTRIBUTEUSAGE usageAuxUserContext, string szNameAuxUserContext, string szValueAuxUserContext, uint InitViewFlags, INITVIEW[] pInitView) => ((IVsCodeWindowEx)editor.codeWindow).Initialize(grfCodeWindowBehaviorFlags, usageAuxUserContext, szNameAuxUserContext, szValueAuxUserContext, InitViewFlags, pInitView);
		int IVsCodeWindowEx.IsReadOnly() => ((IVsCodeWindowEx)editor.codeWindow).IsReadOnly();
	}
}
