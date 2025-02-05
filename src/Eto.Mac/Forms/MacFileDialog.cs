namespace Eto.Mac.Forms
{
	interface IMacFileDialog
	{
		List<string> MacFilters { get; }

		string GetDefaultExtension();

		int CurrentFilterIndex { get; }
	}

	class SavePanelDelegate : NSOpenSavePanelDelegate
	{
		WeakReference handler;

		public IMacFileDialog Handler { get { return (IMacFileDialog)handler.Target; } set { handler = new WeakReference(value); } }

		public override bool ShouldEnableUrl(NSSavePanel panel, NSUrl url)
		{
			if (Directory.Exists(url.Path))
				return true;

			var extension = Path.GetExtension(url.Path).TrimStart(new[] { '.' });
			if (Handler.MacFilters == null || Handler.MacFilters.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
				return true;
			return false;
		}
		
	}

	public abstract class MacFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler, IMacFileDialog
     where TControl: NSSavePanel
     where TWidget: FileDialog
	{
		List<string> macfilters;
		readonly DropDown fileTypes;

		protected MacFileDialog()
		{
			fileTypes = new DropDown();
			fileTypes.SelectedIndexChanged += (sender, e) => OnFileTypeChanged();
		}

		void Create()
		{
			if (Widget.Filters.Count > 0)
			{
				if (Control.AccessoryView != null)
					return;

				var layout = new TableLayout { Padding = 15, Spacing = new Size(2, 2) };
				layout.Rows.Add(new TableRow(null, fileTypes, null));

				Control.AccessoryView = layout.ToNative(true);
				SetAllowedFileTypes();
			}
			else
				Control.AccessoryView = null;
		}

		protected virtual void OnFileTypeChanged()
		{
			SetAllowedFileTypes();
			Control.ValidateVisibleColumns();
			Control.Update();
		}

		string fileName;

		public virtual string FileName
		{
			get => Control.Url?.Path ?? fileName;
			set => fileName = value;
		}

		public Uri Directory
		{
			get => (Uri)Control.DirectoryUrl;
			set => Control.DirectoryUrl = new NSUrl(value.AbsoluteUri);
		}

		public string GetDefaultExtension()
		{
			var filter = Widget.CurrentFilter;
			if (filter != null)
			{
				string ext = filter.Extensions.FirstOrDefault();
				if (!string.IsNullOrEmpty(ext))
				{
					return ext.TrimStart('*', '.');
				}
			}
			return null;
		}

		public List<string> MacFilters => macfilters;

		internal void SetAllowedFileTypes()
		{
			var filters = Widget.CurrentFilter?.Extensions?.Select(r => r.TrimStart('*', '.')).ToList();
			if (filters != null && (filters.Count == 0 || filters.Contains("")))
			{
				filters = null;
			}
			
			if (macfilters == filters)
				return;
			macfilters = filters;

#if MACOS_NET
			Control.AllowedContentTypes = macfilters.Distinct().Select(UniformTypeIdentifiers.UTType.CreateFromExtension).ToArray()
				?? Array.Empty<UniformTypeIdentifiers.UTType>();
#else			

			Control.AllowedFileTypes = macfilters?.Distinct().ToArray();
#endif
		}

		public int CurrentFilterIndex
		{
			get => fileTypes.SelectedIndex;
			set => fileTypes.SelectedIndex = value;
		}

		public bool CheckFileExists
		{
			get { return true; }
			set { }
		}

		public string Title
		{
			get { return Control.Message; }
			set { Control.Message = value ?? string.Empty; }
		}
		
		public virtual DialogResult ShowDialog(Window parent)
		{
			//Control.AllowsOtherFileTypes = false;
			Control.Delegate = new SavePanelDelegate{ Handler = this };
			Create();

			int ret = MacModal.Run(Control, parent);
			
			if (ret == 1)
				fileName = null;

			return ret == 1 ? DialogResult.Ok : DialogResult.Cancel;
		}

		public void InsertFilter(int index, FileFilter filter)
		{
			fileTypes.Items.Insert(index, new ListItem { Text = filter.Name } );
			if (fileTypes.SelectedIndex == -1)
				fileTypes.SelectedIndex = index;
		}

		public void RemoveFilter(int index)
		{
			fileTypes.Items.RemoveAt(index);
		}

		public void ClearFilters()
		{
			fileTypes.Items.Clear();
		}
	}
}
