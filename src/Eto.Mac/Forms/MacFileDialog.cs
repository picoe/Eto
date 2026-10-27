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
			var h = Handler;
			if (h == null || url == null || Directory.Exists(url.Path))
				return true;

			var extension = Path.GetExtension(url.Path)?.TrimStart(new[] { '.' }) ?? string.Empty;
			var macFilters = h.MacFilters;
			if (macFilters == null || macFilters.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
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

		internal virtual void Create()
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

		// Cache the full path so FileName round-trips it; cleared on OK so the chosen path wins.
		string fileName;

		/// <summary>Whether the dialog was shown since FileName was last set.</summary>
		internal bool HasShown { get; private set; }

		public virtual string FileName
		{
			// Before showing, the panel reports its own default (e.g. "Untitled") rather than what was set.
			get => fileName ?? (HasShown ? Control.Url?.Path : null);
			set
			{
				HasShown = false;
				var dir = Path.GetDirectoryName(value);
				if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
					Control.DirectoryUrl = new NSUrl(new Uri(dir).AbsoluteUri);
				fileName = string.IsNullOrEmpty(value) ? null : value;
			}
		}

		public Uri Directory
		{
			// MonoMac's NSUrl conversion dereferences a nil url, so guard before casting.
			get => Control.DirectoryUrl is NSUrl url ? (Uri)url : null;
			set
			{
				Control.DirectoryUrl = new NSUrl(value.AbsoluteUri);
				// Last one wins: move a full-path FileName into the new folder so it matches what the dialog shows.
				if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
					fileName = Path.Combine(value.LocalPath, Path.GetFileName(fileName));
			}
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

			var allowedTypes = GetNativeFileTypes(macfilters);

#if MACOS_NET
			Control.AllowedContentTypes = allowedTypes?.Distinct().Select(UniformTypeIdentifiers.UTType.CreateFromExtension).ToArray()
				?? Array.Empty<UniformTypeIdentifiers.UTType>();
#else

			Control.AllowedFileTypes = allowedTypes?.Distinct().ToArray();
#endif
		}

		/// <summary>
		/// Gets the extensions to restrict the native panel to, which can differ from <see cref="MacFilters"/>
		/// used by <see cref="SavePanelDelegate"/> to enable/disable files in the list.
		/// </summary>
		internal virtual List<string> GetNativeFileTypes(List<string> filters) => filters;

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
			HasShown = true;
			//Control.AllowsOtherFileTypes = false;
			var oldDelegate = Control.Delegate;
			Control.Delegate = new SavePanelDelegate{ Handler = this };
			Create();

			int ret = MacModal.Run(Control, parent);
			Control.Delegate = oldDelegate;
			
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
