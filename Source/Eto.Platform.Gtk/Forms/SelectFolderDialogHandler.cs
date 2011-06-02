using System;
using Eto.Forms;
using System.IO;

namespace Eto.Platform.GtkSharp
{
	public class SelectFolderDialogHandler : WidgetHandler<Gtk.FileChooserDialog, SelectFolderDialog>, ISelectFolderDialog
	{
		public SelectFolderDialogHandler ()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.SelectFolder);
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
			
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
		}
	

		public DialogResult ShowDialog (Window parent)
		{
			if (parent != null) Control.TransientFor = (Gtk.Window)parent.ControlObject;

			int result = Control.Run();
			
			Control.HideAll();

			DialogResult response = Generator.Convert((Gtk.ResponseType)result);
			if (response == DialogResult.Ok) System.IO.Directory.SetCurrentDirectory(Control.CurrentFolder);
			
			return response;
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public string Directory
		{
			get { return Control.CurrentFolder; }
			set { Control.SetCurrentFolder(value); }
		}
	}
}

