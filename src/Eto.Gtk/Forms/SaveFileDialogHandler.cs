using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
#if GTKCORE
	public class SaveFileDialogHandler : GtkFileDialog<Gtk.FileChooserNative, SaveFileDialog>, SaveFileDialog.IHandler
#else
	public class SaveFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, SaveFileDialog>, SaveFileDialog.IHandler
#endif
	{
		public SaveFileDialogHandler()
		{
#if GTKCORE
			Control = new Gtk.FileChooserNative(string.Empty, null, Gtk.FileChooserAction.Save, null, null);
#else
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Save);
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
			Control.DefaultResponse = Gtk.ResponseType.Ok;
#endif
			Control.DoOverwriteConfirmation = true;
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
		}
	}
}