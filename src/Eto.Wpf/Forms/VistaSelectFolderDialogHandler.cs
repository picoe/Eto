using Eto.Forms;
using System;
using cp = Microsoft.WindowsAPICodePack.Dialogs;

#if WINFORMS
namespace Eto.WinForms.Forms
#elif WPF
namespace Eto.Wpf.Forms
#endif
{
	public class VistaSelectFolderDialogHandler : WidgetHandler<cp.CommonOpenFileDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public VistaSelectFolderDialogHandler()
		{
			Control = new cp.CommonOpenFileDialog
			{
				IsFolderPicker = true
			};
		}

		public DialogResult ShowDialog(Window parent)
		{
#if WINFORMS
			// use reflection since adding a parameter requires us to reference PresentationFramework which we don't want in winforms
			cp.CommonFileDialogResult result;
			var handle = parent.ToNative()?.Handle;
			if (handle == null)
			{
				result = Control.ShowDialog();
			}
			else
			{
				var showDialogMethod = Control.GetType().GetMethod("ShowDialog", new[] { typeof(IntPtr) });
				result = (cp.CommonFileDialogResult)showDialogMethod.Invoke(Control, new object[] { handle.Value });
			}
#elif WPF
			// don't use WPF window, parent might be a HwndFormHandler
			var wpfParent = parent?.NativeHandle;
			var result = wpfParent != null ? Control.ShowDialog(wpfParent.Value) : Control.ShowDialog();
#endif
			switch (result)
			{
				case cp.CommonFileDialogResult.Cancel:
					return DialogResult.Cancel;
				case cp.CommonFileDialogResult.Ok:
					return DialogResult.Ok;
				default:
				case cp.CommonFileDialogResult.None:
					return DialogResult.None;
			}
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public string Directory
		{
			get { return Control.FileName; }
			set { Control.InitialDirectory = value; }
		}
	}
}