using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<Gtk.FileChooserDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public SelectFolderDialogHandler()
		{
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.SelectFolder);
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());

			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
			Control.DefaultResponse = Gtk.ResponseType.Ok;
		}


		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null) Control.TransientFor = (Gtk.Window)parent.ControlObject;

			int result = Control.Run();

			Control.Hide();
			Control.Unrealize();

			DialogResult response = ((Gtk.ResponseType)result).ToEto();
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
			get { return Control.Filename ?? Control.CurrentFolder; }
			set { Control.SetCurrentFolder(value); }
		}
	}
}

