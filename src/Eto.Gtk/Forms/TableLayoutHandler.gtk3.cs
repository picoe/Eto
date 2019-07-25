#if GTK3
using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms
{
	public class TableLayoutHandler : GtkContainer<Gtk.Grid, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		Gtk.Alignment align;
		Gtk.EventBox box;
		bool[] columnScale;
		int lastColumnScale;
		bool[] rowScale;
		int lastRowScale;
		Control[,] controls;
		Gtk.Widget[,] blank;

		public override Gtk.Widget ContainerControl => box;

		public override Gtk.Widget EventControl => ContainerControl;

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
			get => new Padding((int)align.LeftPadding, (int)align.TopPadding, (int)align.RightPadding, (int)align.BottomPadding);
			set
			{
				align.LeftPadding = (uint)value.Left;
				align.RightPadding = (uint)value.Right;
				align.TopPadding = (uint)value.Top;
				align.BottomPadding = (uint)value.Bottom;
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

		public TableLayoutHandler()
		{
			align = new Gtk.Alignment(0, 0, 1.0F, 1.0F);
			align.Hexpand = false;
			align.Vexpand = false;
			box = new EtoEventBox { Child = align, Handler = this };
			Control = new Gtk.Grid();
			align.Add(Control);
		}

		public void CreateControl(int cols, int rows)
		{
			columnScale = new bool[cols];
			lastColumnScale = cols - 1;
			rowScale = new bool[rows];
			lastRowScale = rows - 1;
			controls = new Control[rows, cols];
			blank = new Gtk.Widget[rows, cols];
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			var lastScale = lastColumnScale;
			lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
			SetExpandColumn(column);
			if (lastScale != lastColumnScale && column != columnScale.Length - 1)
			{
				SetExpandColumn(columnScale.Length - 1);
			}
		}

		public bool GetColumnScale(int column) => columnScale[column];

		void SetExpandColumn(int column)
		{
			for (int y = 0; y < controls.GetLength(0); y++)
			{
				SetExpand(Control.GetChildAt(column, y), column, y);
			}
		}

		void SetExpandRow(int row)
		{
			for (int x = 0; x < controls.GetLength(1); x++)
			{
				SetExpand(Control.GetChildAt(x, row), x, row);
			}
		}

		bool Attach(Control child, int x, int y)
		{
			var old = controls[y, x];
			if (old != null && !object.ReferenceEquals(old, child))
			{
				var widget = old.GetContainerWidget();
				if (widget.Parent != null)
					Control.Remove(widget);
			}

			if (child != null)
			{
				var blankWidget = blank[y, x];
				if (blankWidget != null)
				{
					if (blankWidget.Parent != null)
						Control.Remove(blankWidget);
					blank[y, x] = null;
				}

				controls[y, x] = child;
				var widget = child.GetContainerWidget();
				if (widget.Parent != null)
					((Gtk.Container)widget.Parent).Remove(widget);

				SetExpand(widget, x, y);
				Control.Attach(widget, x, y, 1, 1);
				widget.ShowAll();
				return true;
			}
			else
			{
				controls[y, x] = null;
				var blankWidget = blank[y, x];
				if (blankWidget == null)
					blankWidget = blank[y, x] = new Gtk.VBox();
				else if (blankWidget.Parent != null)
					Control.Remove(blankWidget);
				SetExpand(blankWidget, x, y);
				Control.Attach(blankWidget, x, y, 1, 1);
			}
			return false;
		}

		public void Remove(Control child)
		{
			for (int y = 0; y < controls.GetLength(0); y++)
			{
				for (int x = 0; x < controls.GetLength(1); x++)
				{
					if (ReferenceEquals(controls[y, x], child))
					{
						controls[y, x] = null;
						var widget = child.GetContainerWidget();
						Control.Remove(widget);

						var blankWidget = blank[y, x] = new Gtk.VBox();
						SetExpand(blankWidget, x, y);
						Control.Attach(blankWidget, x, y, 1, 1);
						return;
					}
				}
			}
		}

		void SetExpand(Gtk.Widget widget, int x, int y)
		{
			if (widget == null)
			{
				var blankWidget = blank[y, x];
				if (blankWidget == null)
				{
					blankWidget = blank[y, x] = new Gtk.VBox();
					Control.Attach(blankWidget, x, y, 1, 1);
				}
				else if (blankWidget.Parent != null)
					Control.Remove(blankWidget);
				widget = blankWidget;
			}
			widget.Hexpand = columnScale[x] || x == lastColumnScale;
			widget.Vexpand = rowScale[y] || y == lastRowScale;
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			var lastScale = lastRowScale;
			lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;
			SetExpandRow(row);
			if (lastScale != lastRowScale && row != rowScale.Length - 1)
			{
				SetExpandRow(rowScale.Length - 1);
			}
		}

		public bool GetRowScale(int row) => rowScale[row];

		public void Update() => Control.QueueResize();

		public override void OnLoadComplete(System.EventArgs e)
		{
			base.OnLoadComplete(e);
			SetFocusChain();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.ShownEvent:
					Control.Mapped += Connector.MappedEvent;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

#endif