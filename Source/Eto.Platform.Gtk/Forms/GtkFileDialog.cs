using System;
using System.Collections;
using System.IO;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.GtkSharp
{
	public abstract class GtkFileDialog<T, W> : WidgetHandler<T, W>, IFileDialog
		where T: Gtk.FileChooserDialog
		where W: FileDialog
	{
		IFileDialogFilter[] filters;



		public string FileName
		{
			get { return Control.Filename; }
			set
			{
				Control.SetCurrentFolder(Path.GetDirectoryName(value));
				Control.SetFilename(value);
			}
		}
		
		public Uri Directory {
			get {
				return new Uri(Control.CurrentFolderUri);
			}
			set {
				Control.SetCurrentFolderUri(value.AbsoluteUri);
			}
		}
		
		
		public IEnumerable<IFileDialogFilter> Filters
		{
			get { return filters; }
			set
			{
				var list = this.Control.Filters.ToArray ();
				foreach (Gtk.FileFilter filter in list)
				{
					this.Control.RemoveFilter(filter);
				}
				
				filters = value.ToArray ();
				foreach (var val in filters)
				{
					Gtk.FileFilter filter = new Gtk.FileFilter();
					filter.Name = val.Name;
					foreach (string pattern in val.Extensions) filter.AddPattern("*" + pattern);
					this.Control.AddFilter(filter);
				}
			}
		}

		public IFileDialogFilter CurrentFilter
		{
			get
			{
				if (CurrentFilterIndex == -1 || filters == null) return null;
				return filters[CurrentFilterIndex];
			}
			set
			{
				CurrentFilterIndex = Array.IndexOf (filters, value);
			}
		}
		
		public int CurrentFilterIndex
		{
			get
			{
				Gtk.FileFilter[] filters = this.Control.Filters;
				for (int i=0; i<filters.Length; i++)
				{
					if (filters[i] == this.Control.Filter) return i;
				}
				return -1;
			}
			set
			{
				Gtk.FileFilter[] filters = this.Control.Filters;
				this.Control.Filter = filters[value];
			}
		}

		public bool CheckFileExists
		{
			get { return false; }
			set {  }
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}


		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null) Control.TransientFor = (Gtk.Window)parent.ControlObject;

			int result = Control.Run();
			
			Control.HideAll();

			DialogResult response = ((Gtk.ResponseType)result).ToEto ();
			if (response == DialogResult.Ok) System.IO.Directory.SetCurrentDirectory(Control.CurrentFolder);
			
			return response;
		}

	}
}
