namespace Eto.GtkSharp.Forms
{
	// Remembers the last folder used by any file/folder dialog so subsequent dialogs
	// open where the user left off, without modifying the process working directory.
	static class GtkFileDialogHelper
	{
		public static string LastUsedFolder;

		public static string InitialFolder => LastUsedFolder ?? System.IO.Directory.GetCurrentDirectory();
	}

	public abstract class GtkFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler
#if GTKCORE
		where TControl: Gtk.FileChooserNative
#else
		where TControl: Gtk.FileChooserDialog
#endif
		where TWidget: FileDialog
	{
		// Cache the full path and folder so FileName/Directory round-trip them: a FileChooserNative
		// reports nothing back until it has been shown. Cleared on OK so the chosen path wins.
		string fileName;
		Uri directory;

		public virtual string FileName
		{
#if GTKCORE
			get => fileName ?? Control.Filename ?? Control.CurrentName;
#else
			get => fileName ?? Control.Filename;
#endif
			set
			{
				fileName = string.IsNullOrEmpty(value) ? null : value;
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
					{
						Control.SetCurrentFolder(dir);
						directory = new Uri(dir);
					}
					if (File.Exists(value))
						Control.SetFilename(value);
				}
				Control.CurrentName = Path.GetFileName(value) ?? string.Empty;
			}
		}
		
		public Uri Directory {
			get {
				return directory ?? (Control.CurrentFolderUri != null ? new Uri(Control.CurrentFolderUri) : null);
			}
			set {
				Control.SetCurrentFolderUri(value.AbsoluteUri);
				directory = value;
				// Last one wins: move a full-path FileName into the new folder so it matches what the dialog shows.
				if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
					fileName = Path.Combine(value.LocalPath, Path.GetFileName(fileName));
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
			if (response == DialogResult.Ok)
			{
				fileName = null;
				directory = null;
				if (!string.IsNullOrEmpty(Control.CurrentFolder))
					GtkFileDialogHelper.LastUsedFolder = Control.CurrentFolder;
			}

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