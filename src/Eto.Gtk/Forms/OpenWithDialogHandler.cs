#if GTK3
namespace Eto.GtkSharp.Forms
{
	public class OpenWithDialogHandler : WidgetHandler<Gtk.Dialog, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler, CommonDialog.ICancellableHandler
	{
		Gtk.AppChooserDialog adialog;

		public string FilePath { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			#if GTKCORE
			adialog = new Gtk.AppChooserDialog(
				parent == null ? null : (parent.ControlObject as Gtk.Window),
				Gtk.DialogFlags.UseHeaderBar | Gtk.DialogFlags.DestroyWithParent,
				GLib.FileFactory.NewForPath(FilePath)
			);
			#else
			var handle = parent == null ? IntPtr.Zero : (parent.ControlObject as Gtk.Window).Handle;
			var adialoghandle = NativeMethods.gtk_app_chooser_dialog_new(handle, 5, NativeMethods.g_file_new_for_path(FilePath));
			adialog = new Gtk.AppChooserDialog(adialoghandle);
			#endif

			var response = (Gtk.ResponseType)adialog.Run();
			if (response == Gtk.ResponseType.Ok)
				Process.Start(adialog.AppInfo.Executable, "\"" + FilePath + "\"");
#if GTKCORE
			adialog.Dispose();
#else
			adialog.Destroy();
#endif
			adialog = null;

			return response == Gtk.ResponseType.Ok ? DialogResult.Ok : DialogResult.Cancel;
		}

		public void CancelDialog() => adialog?.Respond((int)Gtk.ResponseType.Cancel);
	}
}
#endif
