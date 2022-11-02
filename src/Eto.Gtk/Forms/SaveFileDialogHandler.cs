using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class SaveFileDialogHandler : GtkFileDialog<Gtk.FileChooserNative, SaveFileDialog>, SaveFileDialog.IHandler
	{
		public SaveFileDialogHandler()
		{
			Control = new Gtk.FileChooserNative(string.Empty, null, Gtk.FileChooserAction.Save, null, null);
			Control.DoOverwriteConfirmation = true;
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
		}
	}
}