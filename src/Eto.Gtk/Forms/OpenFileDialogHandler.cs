namespace Eto.GtkSharp.Forms
{
	#if GTKCORE
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserNative, OpenFileDialog>, OpenFileDialog.IHandler
#else
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, OpenFileDialog>, OpenFileDialog.IHandler
#endif
	{
		public OpenFileDialogHandler()
		{
#if GTKCORE
			Control = new Gtk.FileChooserNative(string.Empty, null, Gtk.FileChooserAction.Open, null, null);
#else
			Control = new Gtk.FileChooserDialog(string.Empty, null, Gtk.FileChooserAction.Open);
			Control.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			Control.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
			Control.DefaultResponse = Gtk.ResponseType.Ok;
#endif
			Control.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory());
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