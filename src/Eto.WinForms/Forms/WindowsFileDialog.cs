using System;
using System.IO;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.WinForms.Forms
{
	public abstract class WindowsFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler
		where TControl : swf.FileDialog
		where TWidget : FileDialog
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
			SetFilters();

			swf.DialogResult dr;
			if (parent != null)
				dr = Control.ShowDialog((swf.Control)parent.ControlObject);
			else
				dr = Control.ShowDialog();
			return dr.ToEto();
		}
	}
}
