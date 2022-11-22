using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
#if GTKCORE
	public class SelectFolderDialogHandler : WidgetHandler<Gtk.FileChooserNative, SelectFolderDialog>, SelectFolderDialog.IHandler
#else
	public class SelectFolderDialogHandler : WidgetHandler<Gtk.FileChooserDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
#endif
	{
		public SelectFolderDialogHandler()
		{
#if GTKCORE
			Control = new Gtk.FileChooserNative(string.Empty, null, Gtk.FileChooserAction.SelectFolder, null, null);
#else
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.SelectFolder);
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
			Control.DefaultResponse = Gtk.ResponseType.Ok;
#endif
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
		}


		public DialogResult ShowDialog(Window parent)
		{
#if !GTKCORE
			if (parent != null) Control.TransientFor = (Gtk.Window)parent.ControlObject;
#endif
			int result = Control.Run();

			Control.Hide();
#if !GTKCORE
			Control.Unrealize();
#endif

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