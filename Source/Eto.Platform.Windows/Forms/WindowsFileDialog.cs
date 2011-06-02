using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public abstract class WindowsFileDialog<T, W> : WidgetHandler<T, W>, IFileDialog
		where T: SWF.FileDialog
		where W: FileDialog
	{

		#region IFileDialog Members

		public string FileName
		{
			get { return Control.FileName; }
			set { Control.FileName = value; }
		}

		public string[] Filters
		{
			get { return Control.Filter.Split('|'); }
			set { Control.Filter = String.Join("|", value); }
		}

		public int CurrentFilterIndex
		{
			get { return (Control.FilterIndex > 0) ? Control.FilterIndex-1 : 0; }
			set { Control.FilterIndex = value+1; }
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

		public Eto.Forms.DialogResult ShowDialog(Window parent)
		{
			SWF.DialogResult dr;
			if (parent != null) dr = Control.ShowDialog((SWF.Control)parent.ControlObject);
			else dr = Control.ShowDialog();
			return Generator.Convert(dr);
		}

		#endregion



	}
}
