using mw = Microsoft.Win32;

namespace Eto.Wpf.Forms
{
	public abstract class WpfFileDialog<TControl, TWidget> : WpfCommonDialog<TControl, TWidget>, FileDialog.IHandler
		where TControl: mw.FileDialog
		where TWidget: FileDialog
	{
		protected override void Initialize()
		{
			base.Initialize();
			Control.RestoreDirectory = true;
		}

		// Cache the full path so FileName round-trips it; cleared on OK so the chosen path wins.
		string fileName;

		public string FileName
		{
			get { return fileName ?? Control.FileName; }
			set
			{
				fileName = string.IsNullOrEmpty(value) ? null : value;
				var dir = Path.GetDirectoryName(value);
				if (!string.IsNullOrEmpty(dir))
					Control.InitialDirectory = dir;

				// A full path here would show verbatim in the dialog's file name box and override Directory.
				Control.FileName = Path.GetFileName(value);
			}
		}

		public int CurrentFilterIndex
		{
			get { return (Control.FilterIndex > 0) ? Control.FilterIndex - 1 : 0; }
			set { Control.FilterIndex = value + 1; }
		}

		public bool CheckFileExists
		{
			get { return Control.CheckFileExists; }
			set { Control.CheckFileExists = value; }
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public Uri Directory
		{
			get { return string.IsNullOrEmpty(Control.InitialDirectory) ? null : new Uri(Control.InitialDirectory); }
			set
			{
				Control.InitialDirectory = value.LocalPath;
				// Last one wins: move a full-path FileName into the new folder so it matches what the dialog shows.
				if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
					fileName = Path.Combine(value.LocalPath, Path.GetFileName(fileName));
			}
		}

		void SetFilters()
		{
			var filterValues = from f in Widget.Filters
							   select string.Format("{0}|{1}",
								   f.Name,
								   string.Join(";",
									   from ex in f.Extensions
									   select "*" + ex
								   )
							   );
			Control.Filter = string.Join("|", filterValues);
		}

		public override DialogResult ShowDialog(Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();
			SetFilters();
			var result = base.ShowDialog(parent);
			if (result == DialogResult.Ok)
				fileName = null;
			return result;
		}

		public void InsertFilter(int index, FileFilter filter)
		{
		}

		public void RemoveFilter(int index)
		{
		}

		public void ClearFilters()
		{
		}
	}
}
