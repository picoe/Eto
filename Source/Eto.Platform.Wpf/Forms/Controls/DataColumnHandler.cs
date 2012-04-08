using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DataColumnHandler<C, W> : WidgetHandler<C, W>, IDataColumn
		where C: swc.DataGridColumn
		where W: DataColumn
	{
		Cell dataCell;

		public override void Initialize ()
		{
			base.Initialize ();
			DataCell = new TextBoxCell (Widget.Generator);
			Editable = false;
		}

		public string HeaderText
		{
			get { return Control.Header as string; }
			set { Control.Header = value;  }
		}

		public bool Resizable
		{
			get { return Control.CanUserResize; }
			set { Control.CanUserResize = value; }
		}

		public bool Sortable
		{
			get { return Control.CanUserSort; }
			set { Control.CanUserSort = value; }
		}

		public bool AutoSize
		{
			get { return Control.Width.IsAuto; }
			set { Control.Width = value ? new swc.DataGridLength(1, swc.DataGridLengthUnitType.Auto) : new swc.DataGridLength(100); }
		}

		public int Width
		{
			get
			{
				return (int)Control.Width.Value;
			}
			set
			{
				Control.Width = new swc.DataGridLength (value);
			}
		}

		void CopyValues (Cell oldCell)
		{
			if (oldCell == null) return;
			var handler = (ICellHandler)oldCell.Handler;
			var oldControl = handler.Control;
			Control.Header = oldControl.Header;
			Control.CanUserResize = oldControl.CanUserResize;
			Control.CanUserSort = oldControl.CanUserSort;
			Control.Width = oldControl.Width;
			Control.MinWidth = oldControl.MinWidth;
			Control.MaxWidth = oldControl.MaxWidth;
			Control.Visibility = oldControl.Visibility;
			Control.IsReadOnly = oldControl.IsReadOnly;
		}

		public Cell DataCell
		{
			get { return dataCell; }
			set
			{
				var oldCell = dataCell;
				dataCell = value;
				Control = (C)((ICellHandler)dataCell.Handler).Control;
				CopyValues (oldCell);
			}
		}

		public bool Editable
		{
			get { return !Control.IsReadOnly; }
			set { Control.IsReadOnly = !value; }
		}

		public bool Visible
		{
			get { return Control.Visibility == sw.Visibility.Visible; }
			set { Control.Visibility = (value) ? sw.Visibility.Visible : sw.Visibility.Hidden; }
		}

		public void Bind (int col)
		{
			if (dataCell != null) {
				var cell = (ICellHandler)dataCell.Handler;
				cell.Bind (col);
			}
		}
	}
}
