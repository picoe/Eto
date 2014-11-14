#if TODO_XAML
using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = Windows.UI.Xaml;

namespace Eto.WinRT.Forms
{
	public abstract class WpfFileDialog<TControl, TWidget> : WpfCommonDialog<TControl, TWidget>, IFileDialog
		where TControl: mw.FileDialog
		where TWidget: FileDialog
	{
		IFileDialogFilter[] filters;

		public string FileName
		{
			get { return Control.FileName; }
			set { Control.FileName = value; }
		}

		public IEnumerable<IFileDialogFilter> Filters
		{
			get { return filters; }
			set
			{
				filters = value.ToArray ();
				var filterValues = from f in filters
								   select string.Format ("{0}|{1}",
									   f.Name,
									   string.Join (";",
										   from ex in f.Extensions
										   select "*" + ex
									   )
								   );
				Control.Filter = string.Join ("|", filterValues);
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
			get { return new Uri(Control.InitialDirectory); }
			set
			{
				Control.InitialDirectory = value.LocalPath;
			}
		}
	}
}
#endif