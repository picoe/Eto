using Eto.Forms;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms
{
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, OpenFileDialog>, OpenFileDialog.IHandler
	{
		public OpenFileDialogHandler()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Open);
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
			
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
			Control.DefaultResponse = Gtk.ResponseType.Ok;
		}

		public bool MultiSelect
		{
			get { return Control.SelectMultiple; }
			set { Control.SelectMultiple = value; }	
		}

		public IEnumerable<string> Filenames
		{
			get { return Control.Filenames; }
		}

	}
}
