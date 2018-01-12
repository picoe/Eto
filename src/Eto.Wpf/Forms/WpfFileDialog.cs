using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public abstract class WpfFileDialog<TControl, TWidget> : WpfCommonDialog<TControl, TWidget>, FileDialog.IHandler
		where TControl: mw.FileDialog
		where TWidget: FileDialog
	{
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
			SetFilters();
			return base.ShowDialog(parent);
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
