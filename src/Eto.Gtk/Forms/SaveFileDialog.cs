using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class SaveFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, SaveFileDialog>, SaveFileDialog.IHandler
	{
		public SaveFileDialogHandler()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Save);
			Control.DoOverwriteConfirmation = true;
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
			
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
			Control.DefaultResponse = Gtk.ResponseType.Ok;
		}
	}
}
