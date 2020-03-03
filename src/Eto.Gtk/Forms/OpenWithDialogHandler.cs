#if GTK3
using System;
using System.Diagnostics;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class OpenWithDialogHandler : WidgetHandler<Gtk.Dialog, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
	{
		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			#if GTKCORE
			var adialog = new Gtk.AppChooserDialog(
				parent == null ? null : (parent.ControlObject as Gtk.Window),
				Gtk.DialogFlags.UseHeaderBar | Gtk.DialogFlags.DestroyWithParent,
				GLib.FileFactory.NewForPath(FilePath)
			);
			#else
			var handle = parent == null ? IntPtr.Zero : (parent.ControlObject as Gtk.Window).Handle;
			var adialoghandle = NativeMethods.gtk_app_chooser_dialog_new(handle, 5, NativeMethods.g_file_new_for_path(FilePath));
			var adialog = new Gtk.AppChooserDialog(adialoghandle);
			#endif

			if (adialog.Run() == (int)Gtk.ResponseType.Ok)
				Process.Start(adialog.AppInfo.Executable, "\"" + FilePath + "\"");
#if GTKCORE
			adialog.Dispose();
#else
			adialog.Destroy();
#endif

			return DialogResult.Ok;
		}
	}
}
#endif
