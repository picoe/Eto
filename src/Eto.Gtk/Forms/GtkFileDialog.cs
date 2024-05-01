namespace Eto.GtkSharp.Forms
{
	public abstract class GtkFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler
#if GTKCORE
		where TControl: Gtk.FileChooserNative
#else
		where TControl: Gtk.FileChooserDialog
#endif
		where TWidget: FileDialog
	{
		public virtual string FileName
		{
#if GTKCORE
			get => Control.Filename ?? Control.CurrentName;
#else
			get => Control.Filename;
#endif
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Control.UnselectAll();
					if (!string.IsNullOrEmpty(Control.Filename))
						Control.UnselectFilename(Control.Filename);
				}
				else
				{
					var dir = Path.GetDirectoryName(value);
					if (!string.IsNullOrEmpty(dir))
						Control.SetCurrentFolder(dir);
					if (File.Exists(value))
						Control.SetFilename(value);
				}
				Control.CurrentName = Path.GetFileName(value) ?? string.Empty;
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
		
		
		public void SetFilters()
		{
			var list = Control.Filters.ToArray();
			foreach (Gtk.FileFilter filter in list)
			{
				Control.RemoveFilter(filter);
			}
			
			foreach (var val in Widget.Filters)
			{
				var filter = new Gtk.FileFilter();
				filter.Name = val.Name;
				foreach (string pattern in val.Extensions) filter.AddPattern("*" + pattern);
				Control.AddFilter(filter);
			}
		}

		public int CurrentFilterIndex
		{
			get
			{
				Gtk.FileFilter[] filters = Control.Filters;
				for (int i=0; i<filters.Length; i++)
				{
					if (filters[i] == Control.Filter) return i;
				}
				return -1;
			}
			set
			{
				Gtk.FileFilter[] filters = Control.Filters;
				Control.Filter = filters[value];
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


		public virtual DialogResult ShowDialog(Window parent)
		{
			SetFilters();
#if !GTKCORE
			if (parent != null) Control.TransientFor = (Gtk.Window)parent.ControlObject;
#endif

			int result = Control.Run();

			Control.Hide();
#if !GTKCORE
			Control.Unrealize();
#endif

			DialogResult response = ((Gtk.ResponseType)result).ToEto ();
			if (response == DialogResult.Ok && !string.IsNullOrEmpty(Control.CurrentFolder))
				System.IO.Directory.SetCurrentDirectory(Control.CurrentFolder);
			
			return response;
		}

		public void InsertFilter(int index, FileFilter filter)
		{
			var gtkFilter = new Gtk.FileFilter();
			gtkFilter.Name = filter.Name;
			foreach (var extension in filter.Extensions)
				gtkFilter.AddPattern(extension);

			var filters = new List<Gtk.FileFilter>(Control.Filters);
			if (index < filters.Count)
			{
				for (int i = 0; i < filters.Count; i++)
					Control.RemoveFilter(filters[i]);
				filters.Insert(index, gtkFilter);
				for (int i = 0; i < filters.Count; i++)
					Control.AddFilter(filters[i]);
			}
			else
				Control.AddFilter(gtkFilter);
		}

		public void RemoveFilter(int index)
		{
			Control.RemoveFilter(Control.Filters[index]);
		}

		public void ClearFilters()
		{
			for (int i = 0; i < Control.Filters.Length; i++)
				Control.RemoveFilter(Control.Filters[0]);
		}
	}
}