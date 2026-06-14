namespace Eto.WinForms.Forms
{
	public abstract class WindowsFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler, CommonDialog.ICancellableHandler
		where TControl : swf.FileDialog
		where TWidget : FileDialog
	{
		readonly Win32.CancellableModalDialog _cancellable = new Win32.CancellableModalDialog();

		// A WH_CBT hook captured the native dialog's window handle when it was shown (see CancellableModalDialog),
		// allowing the async ShowDialogAsync to dismiss exactly this dialog when its cancellation token is signalled.
		public void CancelDialog() => _cancellable.Cancel();

		public string FileName
		{
			get { return Control.FileName; }
			set
			{
				var dir = Path.GetDirectoryName(value);
				if (!string.IsNullOrEmpty(dir))
					Control.InitialDirectory = dir;
				Control.FileName = Path.GetFileName(value);
			}
		}

		public Uri Directory
		{
			get { return new Uri(Control.InitialDirectory); }
			set { Control.InitialDirectory = value.AbsoluteUri; }
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

		public void SetFilters()
		{
			var filterValues = from f in Widget.Filters
							   select string.Format("{0}|{1}",
								   f.Name.Replace("|", " "),
								   string.Join(";",
									   from ex in f.Extensions
									   select "*" + ex.Replace(";", " ")
								   )
							   );
			Control.Filter = string.Join("|", filterValues);
		}

		public FileFilter CurrentFilter
		{
			get
			{
				if (CurrentFilterIndex == -1) return null;
				return Widget.Filters[CurrentFilterIndex];
			}
			set
			{
				CurrentFilterIndex = Widget.Filters.IndexOf(value);
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

		public DialogResult ShowDialog(Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();

			SetFilters();

			var dr = _cancellable.Show(() => parent != null
				? Control.ShowDialog((swf.Control)parent.ControlObject)
				: Control.ShowDialog());
			return dr.ToEto();
		}
	}
}
