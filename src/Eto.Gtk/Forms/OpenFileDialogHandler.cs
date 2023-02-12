using Eto.Forms;
using System.Collections.Generic;
using System.IO;

namespace Eto.GtkSharp.Forms
{
	#if GTKCORE
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserNative, OpenFileDialog>, OpenFileDialog.IHandler
#else
	public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, OpenFileDialog>, OpenFileDialog.IHandler
#endif
	{
		string fileName;

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

		public override string FileName
		{
			get => base.FileName ?? fileName;
			set => base.FileName = fileName = value;
		}

		public override DialogResult ShowDialog(Window parent)
		{
			var result = base.ShowDialog(parent);
			
			// When cancelling, Control.Filename all of the sudden returns a value but is just the folder.
			// so, combine it with the desired file name, if one was set.
			if (result == DialogResult.Ok)
				fileName = null;
			else if (!string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
				Control.SetFilename(Path.Combine(base.FileName, fileName));
			return result;
		}

	}
}