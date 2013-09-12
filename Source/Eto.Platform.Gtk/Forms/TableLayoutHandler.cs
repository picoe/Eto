using System;
using System.Collections;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.GtkSharp
{
	public class TableLayoutHandler : GtkContainer<Gtk.Table, TableLayout>, ITableLayout
	{
		const Gtk.AttachOptions SCALED_OPTIONS = Gtk.AttachOptions.Expand | Gtk.AttachOptions.Shrink | Gtk.AttachOptions.Fill;
		Gtk.Alignment align;
		bool[] columnScale;
		bool lastColumnScale;
		bool[] rowScale;
		bool lastRowScale;
		Control[,] controls;
		Gtk.Widget[,] blank;

		public override Gtk.Widget ContainerControl
		{
			get { return align; }
		}

		public Size Spacing
		{
			get { return new Size((int)Control.ColumnSpacing, (int)Control.RowSpacing); }
			set
			{ 
				Control.ColumnSpacing = (uint)value.Width;
				Control.RowSpacing = (uint)value.Height;
			}
		}

		public Padding Padding
		{
			get
			{
				uint top, bottom, left, right;
				align.GetPadding(out top, out bottom, out left, out right);
				return new Padding((int)left, (int)top, (int)right, (int)bottom);
			}
			set
			{
				align.SetPadding((uint)value.Top, (uint)value.Bottom, (uint)value.Left, (uint)value.Right);
			}
		}

		public void Add(Control child, int x, int y)
		{
			Attach(child, x, y);
			if (child != null)
			{
				var widget = child.GetContainerWidget();
				widget.ShowAll();
			}
		}

		public void Move(Control child, int x, int y)
		{
			Attach(child, x, y);
		}

		Gtk.AttachOptions GetColumnOptions(int column)
		{
			var scale = columnScale[column];
			if (column == columnScale.Length - 1)
				scale |= lastColumnScale;
			return scale ? SCALED_OPTIONS : Gtk.AttachOptions.Fill;
		}

		Gtk.AttachOptions GetRowOptions(int row)
		{
			var scale = rowScale[row];
			if (row == rowScale.Length - 1)
				scale |= lastRowScale;
			return scale ? SCALED_OPTIONS : Gtk.AttachOptions.Fill;
		}

		public void CreateControl(int cols, int rows)
		{
			columnScale = new bool[cols];
			lastColumnScale = true;
			rowScale = new bool[rows];
			lastRowScale = true;
			align = new Gtk.Alignment(0, 0, 1.0F, 1.0F);
			Control = new Gtk.Table((uint)rows, (uint)cols, false);
			controls = new Control[rows, cols];
			blank = new Gtk.Widget[rows, cols];
			align.Add(Control);

			this.Spacing = TableLayout.DefaultSpacing;
			this.Padding = TableLayout.DefaultPadding;
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			var lastScale = columnScale.Length == 1 || columnScale.Take(columnScale.Length - 1).All(r => !r);
			AttachColumn(column);
			if (lastScale != lastColumnScale && column < columnScale.Length - 1)
			{
				lastColumnScale = lastScale;
				AttachColumn(columnScale.Length - 1);
			}
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		void AttachColumn(int column)
		{
			for (int y = 0; y < controls.GetLength (0); y++)
			{
				Attach(controls[y, column], column, y);
			}
		}

		void AttachRow(int row)
		{
			for (int x = 0; x < controls.GetLength (1); x++)
			{
				Attach(controls[row, x], x, row);
			}
		}

		bool Attach(Control child, int x, int y)
		{
			var old = controls[y, x];
			if (old != null && old != child)
			{
				if (old != null)
					Control.Remove(old.GetContainerWidget());
			}

			var blankWidget = blank[y, x];
			if (blankWidget != null)
			{
				Control.Remove(blankWidget);
				blank[y, x] = null;
			}
			if (child != null)
			{
				controls[y, x] = child;
				var widget = child.GetContainerWidget();
				if (widget.Parent != null)
					((Gtk.Container)widget.Parent).Remove(widget);
				Control.Attach(widget, (uint)x, (uint)x + 1, (uint)y, (uint)y + 1, GetColumnOptions(x), GetRowOptions(y), 0, 0);
				widget.ShowAll();
				return true;
			}
			else
			{
				controls[y, x] = null;
				blankWidget = blank[y, x] = new Gtk.VBox();
				Control.Attach(blankWidget, (uint)x, (uint)x + 1, (uint)y, (uint)y + 1, GetColumnOptions(x), GetRowOptions(y), 0, 0);
			}
			return false;
		}

		public void Remove(Control child)
		{
			bool done = false;
			for (int y = 0; y<controls.GetLength(0); y++)
			{
				for (int x = 0; x<controls.GetLength(1); x++)
				{
					if (Object.ReferenceEquals(controls[y, x], child))
					{
						controls[y, x] = null;
						done = true;
						break;
					}
				}
				if (done)
					break;
			}
			Control.Remove((Gtk.Widget)child.ControlObject);
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			var lastScale = rowScale.Length == 1 || rowScale.Take(rowScale.Length - 1).All(r => !r);
			AttachRow(row);
			if (lastScale != lastRowScale && row < rowScale.Length - 1)
			{
				lastRowScale = lastScale;
				AttachRow(rowScale.Length - 1);
			}
		}

		public bool GetRowScale(int row)
		{
			return rowScale[row];
		}

		public void Update()
		{
			Control.ResizeChildren();
		}
	}
}
