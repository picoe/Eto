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
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Eto.Addin.VisualStudio
{
	[ComVisible(true)]
	public sealed class EtoPreviewPane : Microsoft.VisualStudio.Shell.WindowPane,
		IVsWindowPane,
		IVsTextBufferDataEvents,
		IVsTextLinesEvents
	{
		IVsTextLines textBuffer;
		BuilderInfo builderInfo;
		EtoPreviewPackage package;
		Panel editorControl;
		Panel previewControl;
		UITimer timer;
		ElementHost host = new ElementHost();
		uint dataEventsCookie;
		uint linesEventsCookie;
		IInterfaceBuilder builder;

		/*void IVsWindowPane.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider site)
		{
			this.site = site;
		}*/

		void RegisterIndependentView(bool subscribe)
		{
			var textManager = (IVsTextManager)GetService(typeof(SVsTextManager));
			if (textManager != null)
			{
				if (subscribe)
					textManager.RegisterIndependentView((IVsWindowPane)this, textBuffer);
				else
					textManager.UnregisterIndependentView((IVsWindowPane)this, textBuffer);
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
		public EtoPreviewPane(EtoPreviewPackage package, string fileName, IVsTextLines textBuffer)
			: base(package)
		{
			this.package = package;
			this.textBuffer = textBuffer;
			FileName = fileName;

			builderInfo = BuilderInfo.Find(fileName);
			if (builderInfo == null)
				throw new InvalidOperationException(string.Format("Could not find builder for file {0}", fileName));
			builder = builderInfo.CreateBuilder();

			SetupCommands();


			editorControl = new Panel();


			previewControl = new Panel();

			var split = new Splitter { Panel1 = previewControl, Panel2 = editorControl, Orientation = SplitterOrientation.Vertical };
			host.Child = split.ToNative(true);
		}

		protected override bool PreProcessMessage(ref System.Windows.Forms.Message m)
		{
			if (viewAdapter != null)
			{
				// copy the Message into a MSG[] array, so we can pass
				// it along to the active core editor's IVsWindowPane.TranslateAccelerator
				var pMsg = new MSG[1];
				pMsg[0].hwnd = m.HWnd;
				pMsg[0].message = (uint)m.Msg;
				pMsg[0].wParam = m.WParam;
				pMsg[0].lParam = m.LParam;

				var vsWindowPane = (IVsWindowPane)viewAdapter;
				return vsWindowPane.TranslateAccelerator(pMsg) == 0;
			}
			return base.PreProcessMessage(ref m);
		}
		

		IVsTextView viewAdapter;
		protected override void Initialize()
		{
			base.Initialize();

			RegisterIndependentView(true);

			timer = new UITimer { Interval = 0.2 };
			timer.Elapsed += timer_Elapsed;

			TriggerUpdate();

			var initView = new[] { new INITVIEW() };
			initView[0].fSelectionMargin = 0;
			initView[0].fWidgetMargin = 0;
			initView[0].fVirtualSpace = 0;
			initView[0].fDragDropMove = 1;

			uint flags =
				(uint)TextViewInitFlags.VIF_SET_SELECTION_MARGIN |
				(uint)TextViewInitFlags.VIF_SET_DRAGDROPMOVE |
				(uint)TextViewInitFlags2.VIF_SUPPRESS_STATUS_BAR_UPDATE |
				(uint)TextViewInitFlags2.VIF_SUPPRESSBORDER |
				//(uint)TextViewInitFlags2.VIF_SUPPRESSTRACKCHANGES |
				//(uint)TextViewInitFlags2.VIF_SUPPRESSTRACKGOBACK |
				(uint)TextViewInitFlags.VIF_HSCROLL |
				(uint)TextViewInitFlags.VIF_VSCROLL |
				(uint)TextViewInitFlags3.VIF_NO_HWND_SUPPORT/* |
				readOnlyValue*/;

			//SetReadOnly(false);
			var editorSvc = Services.GetComponentService<IVsEditorAdaptersFactoryService>();
			var textSvc = Services.GetComponentService<ITextDocumentFactoryService>();
			var textEditorSvc = Services.GetComponentService<ITextEditorFactoryService>();

			//var itd = textSvc.CreateAndLoadTextDocument();
			
			var roles = textEditorSvc.CreateTextViewRoleSet(
				PredefinedTextViewRoles.Interactive,
				PredefinedTextViewRoles.Editable,
				PredefinedTextViewRoles.Document,
				PredefinedTextViewRoles.PrimaryDocument);

			viewAdapter = editorSvc.CreateVsTextViewAdapter(Services.ServiceProvider, roles);
			((IVsWindowPane)viewAdapter).SetSite(Services.ServiceProvider);

			viewAdapter.Initialize(textBuffer, IntPtr.Zero, flags, initView);

			var textManager = (IVsTextManager)GetService(typeof(SVsTextManager));
			textManager.RegisterIndependentView((IVsWindowPane)viewAdapter, textBuffer);

			//IntPtr hwnd;
			//((IVsWindowPane)viewAdapter).CreatePaneWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0, 0, 100, 100, out hwnd);
			var wpfView = editorSvc.GetWpfTextView(viewAdapter); // need to get first so host will not be null
			var wpfViewHost = editorSvc.GetWpfTextViewHost(viewAdapter);

			if (wpfViewHost != null)
				editorControl.Content = wpfViewHost.HostControl.ToEto();
			else
				editorControl.Content = new Label { Text = "Could not load editor" };

		}

		public void SetReadOnly(bool isReadOnly)
		{
			uint flags;
			textBuffer.GetStateFlags(out flags);
			var newFlags = isReadOnly
			  ? flags | (uint)BUFFERSTATEFLAGS.BSF_USER_READONLY
			  : flags & ~(uint)BUFFERSTATEFLAGS.BSF_USER_READONLY;
			textBuffer.SetStateFlags(newFlags);
		}
		void timer_Elapsed(object sender, EventArgs e)
		{
			timer.Stop();
			UpdateView();
		}

		private IConnectionPoint GetConnectionPoint<T>()
		{
			var container = textBuffer as IConnectionPointContainer;
			var guid = typeof(T).GUID;
			IConnectionPoint cp;
			container.FindConnectionPoint(ref guid, out cp);
			return cp;
		}

		public override System.Windows.Forms.IWin32Window Window { get { return host; } }

		#endregion


		/// <summary>
		/// returns the name of the file currently loaded
		/// </summary>
		public string FileName { get; private set; }

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (timer != null)
					{
						timer.Stop();
						timer.Dispose();
						timer = null;
					}

					RegisterIndependentView(false);

					if (editorControl != null)
					{
						editorControl.Dispose();
						editorControl = null;
					}

					if (host != null)
					{
						host.Dispose();
						host = null;
					}
					var builderDispose = builder as IDisposable;
					if (builderDispose != null)
						builderDispose.Dispose();

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
				mcs.AddCommand(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.ViewForm, ViewTextDesign);
				mcs.AddCommand(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.ViewCode, ViewCode);
			}
		}

		void ViewCode()
		{
			// Open the referenced document using the standard text editor.
			var codeFile = builderInfo.GetCodeFile(FileName);

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

		void ViewTextDesign()
		{
			// Open the referenced document using the standard text editor.
			IVsWindowFrame frame;
			IVsUIHierarchy hierarchy;
			uint itemid;
			if (!VsShellUtilities.IsDocumentOpen(this, FileName, VSConstants.LOGVIEWID.Code_guid, out hierarchy, out itemid, out frame)
				&& !VsShellUtilities.IsDocumentOpen(this, FileName, VSConstants.LOGVIEWID.TextView_guid, out hierarchy, out itemid, out frame))
			{
				VsShellUtilities.OpenDocumentWithSpecificEditor(this, FileName, VSConstants.VsEditorFactoryGuid.TextEditor_guid, VSConstants.LOGVIEWID.Code_guid, out hierarchy, out itemid, out frame);
			}
			ErrorHandler.ThrowOnFailure(frame.Show());
		}

		#endregion


		void UpdateView()
		{
			var text = textBuffer.GetText();
			if (!string.IsNullOrEmpty(text))
			{
				builder.Create(text, SetChild, error =>
				{
					previewControl.Content = new Label { Text = error };
				});
			}
		}

		void SetChild(Control control)
		{
			var child = control;
			var window = child as Eto.Forms.Window;
			if (window != null)
			{
				// swap out window for a panel so we can add it as a child
				var content = window.Content;
				window.Content = null;
				child = new Panel { Content = content, Padding = window.Padding };
			}
			previewControl.Content = child;
		}


		void IVsTextBufferDataEvents.OnFileChanged(uint grfChange, uint dwFileAttrs)
		{
			TriggerUpdate();
		}

		int IVsTextBufferDataEvents.OnLoadCompleted(int fReload)
		{
			TriggerUpdate();
			return VSConstants.S_OK;
		}

		void IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine)
		{
		}

		void IVsTextLinesEvents.OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
		{
			TriggerUpdate();
		}

		void TriggerUpdate()
		{
			timer.Stop();
			timer.Interval = 0.5;
			timer.Start();
		}
	}
}
