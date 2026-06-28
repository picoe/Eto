using Eto.Forms;
using Eto.Drawing;
using Eto.GirCore.Forms;
using Gtk;
using System;
using System.Linq;

namespace Eto.GirCore.Forms
{
	public class TableLayoutHandler : GirContainer<Gtk.Grid, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		private bool[] columnScale;
		private int lastColumnScale;
		private bool[] rowScale;
		private int lastRowScale;
		private Control?[,] controls;
		private Gtk.Widget?[,] blank;

		public TableLayoutHandler()
		{
			Control = Gtk.Grid.New();
		}

		public Size Spacing
		{
			get => new Size((int)Control.ColumnSpacing, (int)Control.RowSpacing);
			set
			{
				Control.ColumnSpacing = value.Width;
				Control.RowSpacing = value.Height;
			}
		}

		public Padding Padding
		{
			get => new Padding(0); // Gtk.Grid does not support padding directly
			set { /* Optionally implement with a container if needed */ }
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

		public void Add(Control child, int x, int y)
		{
			Attach(child, x, y);
			if (child != null)
			{
				var widget = child.GetContainerWidget();
				widget.Show();
				Update();
			}
		}

		public void Move(Control child, int x, int y)
		{
			Attach(child, x, y);
		}

		private bool Attach(Control child, int x, int y)
		{
			var old = controls[y, x];
			if (old != null && !ReferenceEquals(old, child))
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
					widget.SetParent(null);

				SetExpand(widget, x, y);
				Control.Attach(widget, x, y, 1, 1);
				widget.Show();
				Update();
				return true;
			}
			else
			{
				controls[y, x] = null;
				var blankWidget = blank[y, x];
				if (blankWidget == null)
					blankWidget = blank[y, x] = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
				else if (blankWidget.Parent != null)
					Control.Remove(blankWidget);
				SetExpand(blankWidget, x, y);
				Control.Attach(blankWidget, x, y, 1, 1);
			}
			Update();
			return false;
		}

		public void Remove(Control child)
		{
			if (controls == null)
				return;

			for (int y = 0; y < controls.GetLength(0); y++)
			{
				for (int x = 0; x < controls.GetLength(1); x++)
				{
					if (ReferenceEquals(controls[y, x], child))
					{
						controls[y, x] = null;
						var widget = child.GetContainerWidget();
						Control.Remove(widget);

						var blankWidget = blank[y, x] = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
						SetExpand(blankWidget, x, y);
						Control.Attach(blankWidget, x, y, 1, 1);

						Update();
						return;
					}
				}
			}
		}

		private void SetExpand(Gtk.Widget widget, int x, int y)
		{
			if (widget == null)
			{
				var blankWidget = blank[y, x];
				if (blankWidget == null)
				{
					blankWidget = blank[y, x] = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
					Control.Attach(blankWidget, x, y, 1, 1);
				}
				else if (blankWidget.Parent != null)
					Control.Remove(blankWidget);
				widget = blankWidget;
			}
			widget.Hexpand = columnScale[x] || x == lastColumnScale;
			widget.Vexpand = rowScale[y] || y == lastRowScale;
			Update();
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

		private void SetExpandColumn(int column)
		{
			for (int y = 0; y < controls.GetLength(0); y++)
			{
				SetExpand(Control.GetChildAt(column, y), column, y);
			}
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
			Update();
		}

		public bool GetRowScale(int row) => rowScale[row];

		private void SetExpandRow(int row)
		{
			for (int x = 0; x < controls.GetLength(1); x++)
			{
				SetExpand(Control.GetChildAt(x, row), x, row);
			}
		}

		public void Update() => Control.QueueResize();

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			// Optionally set focus chain or other post-load logic
		}
	}
}
