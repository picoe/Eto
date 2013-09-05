using System;
using System.Linq;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows
{
	public abstract class WindowsFileDialog<T, W> : WidgetHandler<T, W>, IFileDialog
		where T: System.Windows.Forms.FileDialog
		where W: FileDialog
	{
		IFileDialogFilter[] filters;

		#region IFileDialog Members

		public string FileName {
			get { return Control.FileName; }
			set { Control.FileName = value; }
		}
		
		public Uri Directory {
			get {
				return new Uri (this.Control.InitialDirectory);
			}
			set {
				this.Control.InitialDirectory = value.AbsoluteUri;
			}
		}
		
		public IEnumerable<IFileDialogFilter> Filters {
			get { return filters; }
			set { 
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

		public int CurrentFilterIndex {
			get { return (Control.FilterIndex > 0) ? Control.FilterIndex - 1 : 0; }
			set { Control.FilterIndex = value + 1; }
		}

		public bool CheckFileExists {
			get { return Control.CheckFileExists; }
			set { Control.CheckFileExists = value; }
		}

		public string Title {
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public Eto.Forms.DialogResult ShowDialog (Window parent)
		{
			SWF.DialogResult dr;
			if (parent != null)
				dr = Control.ShowDialog ((SWF.Control)parent.ControlObject);
			else
				dr = Control.ShowDialog ();
			return dr.ToEto ();
		}

		#endregion



	}
}
