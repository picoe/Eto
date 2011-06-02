using System;
using System.Collections;
using System.IO;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public abstract class GtkFileDialog<T, W> : WidgetHandler<T, W>, IFileDialog
		where T: Gtk.FileChooserDialog
		where W: FileDialog
	{
		string[] filters = new string[0];



		public string FileName
		{
			get { return Control.Filename; }
			set
			{
				Control.SetCurrentFolder(Path.GetDirectoryName(value));
				Control.SetFilename(value);
			}
		}
		
		
		public string[] Filters
		{
			get { return filters; }
			set
			{
				ArrayList list = new ArrayList(this.Control.Filters);
				foreach (Gtk.FileFilter filter in list)
				{
					this.Control.RemoveFilter(filter);
				}
				
				foreach (string val in value)
				{
					Gtk.FileFilter filter = new Gtk.FileFilter();
					filter.Name = val.Substring(0, val.IndexOf('|'));
					string[] patterns = val.Substring(val.IndexOf('|')+1).Split(';'); 
					foreach (string pattern in patterns) filter.AddPattern(pattern);
					this.Control.AddFilter(filter);
				}
				filters = value;
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

			DialogResult response = Generator.Convert((Gtk.ResponseType)result);
			if (response == DialogResult.Ok) Directory.SetCurrentDirectory(Control.CurrentFolder);
			
			return response;
		}

	}
}
