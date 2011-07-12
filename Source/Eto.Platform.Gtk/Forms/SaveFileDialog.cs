using System;
using System.IO;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SaveFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, SaveFileDialog>, ISaveFileDialog
	{
		public SaveFileDialogHandler()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Save);
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
			
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
		}
	}
}
