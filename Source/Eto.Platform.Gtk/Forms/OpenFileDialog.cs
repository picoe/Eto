using System;
using System.IO;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp
{
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, OpenFileDialog>, IOpenFileDialog
	{
		public OpenFileDialogHandler()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Open);
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
			
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
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
