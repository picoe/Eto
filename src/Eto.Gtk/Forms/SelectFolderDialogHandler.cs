using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<Gtk.FileChooserNative, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public SelectFolderDialogHandler()
		{
			Control = new Gtk.FileChooserNative(string.Empty, null, Gtk.FileChooserAction.SelectFolder, null, null);
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
		}


		public DialogResult ShowDialog(Window parent)
		{
			int result = Control.Run();

			Control.Hide();

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