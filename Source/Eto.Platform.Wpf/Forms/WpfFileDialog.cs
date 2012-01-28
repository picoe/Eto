using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using mw = Microsoft.Win32;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfFileDialog<T, W> : WpfCommonDialog<T, W>, IFileDialog
		where T: mw.FileDialog
		where W: FileDialog
	{
		public string FileName
		{
			get { return Control.FileName; }
			set { Control.FileName = value; }
		}

		public IEnumerable<IFileDialogFilter> Filters
		{
			get; set;
		}

		public int CurrentFilterIndex
		{
			get { return Control.FilterIndex; }
			set { Control.FilterIndex = value; }
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
				Control.InitialDirectory = value.AbsolutePath;
			}
		}
	}
}
