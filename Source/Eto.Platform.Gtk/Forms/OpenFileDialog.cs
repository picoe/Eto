using System;
using System.IO;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, OpenFileDialog>, IOpenFileDialog
	{
		public OpenFileDialogHandler()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Open);
			Control.SetCurrentFolder(Directory.GetCurrentDirectory());
			
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
		}

	}
}
