using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using System.Windows.Media;

namespace Eto.Addin.VisualStudio.Windows.Editor
{
	public class CodeEditorHost
	{
		public IVsCodeWindow codeWindow;
		public System.Windows.FrameworkElement wpfElement;
		public IWpfTextViewHost textViewHost;
		public IVsTextView viewAdapter;

		public CodeEditorHost(IVsTextLines textBuffer)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			var editorSvc = Services.GetComponentService<IVsEditorAdaptersFactoryService>();

			codeWindow = editorSvc.CreateVsCodeWindowAdapter(Services.ServiceProvider);
			// disable splitter since it will cause a crash
			var codeWindowEx = (IVsCodeWindowEx)codeWindow;
			var initView = new INITVIEW[1];
			ErrorHandler.ThrowOnFailure(codeWindowEx.Initialize(
				(uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER,
				VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter,
				szNameAuxUserContext: string.Empty,
				szValueAuxUserContext: string.Empty,
				InitViewFlags: 0,
				pInitView: initView));

			var buffer = editorSvc.GetDataBuffer(textBuffer);
			if (buffer == null)
				ErrorHandler.ThrowOnFailure(VSConstants.VS_E_INCOMPATIBLEDOCDATA);

			ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(textBuffer));

			//needed for xeto/jeto files, not implemented for c# etc so we don't worry about the result
			//Guid clsIdView = VSConstants.LOGVIEWID.TextView_guid;
			//codeWindow.SetViewClassID(ref clsIdView);

			ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out viewAdapter));

			// get the view first so host is created 
			//var wpfView = editorSvc.GetWpfTextView(viewAdapter);

			textViewHost = editorSvc.GetWpfTextViewHost(viewAdapter);

			wpfElement = textViewHost?.HostControl;
			if (wpfElement != null)
			{
				// get real host?
				var parent = VisualTreeHelper.GetParent(wpfElement) as System.Windows.FrameworkElement;
				while (parent != null)
				{
					wpfElement = parent;
					parent = VisualTreeHelper.GetParent(wpfElement) as System.Windows.FrameworkElement;
				}
			}
		}
	}
}
