using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.WinForms.Forms
{
	public class TableLayoutHandler : WindowsContainer<swf.TableLayoutPanel, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		Size spacing;
		Control[,] views;
		bool[] columnScale;
		int lastColumnScale;
		bool[] rowScale;
		int lastRowScale;

		protected override bool SetMinimumSize(Size size)
		{
			if (columnScale == null || rowScale == null)
				return base.SetMinimumSize(size);
			// ensure that our width doesn't get smaller than the non-scaled child controls
			// to make it so the child controls are left-justified when the container
			// is smaller than all the children
			var widths = Control.GetColumnWidths();
			var heights = Control.GetRowHeights();
			var curSize = Size.Empty;
			for (int i = 0; i < widths.Length; i++)
			{
				if (!columnScale[i] && i != lastColumnScale)
					curSize.Width += widths[i];
			}
			for (int i = 0; i < heights.Length; i++)
			{
				if (!rowScale[i] && i != lastRowScale)
					curSize.Height += heights[i];
			}
			size = Size.Max(size, curSize);
			return base.SetMinimumSize(size);
		}

		public TableLayoutHandler()
		{
			Control = new swf.TableLayoutPanel
			{
				Margin = swf.Padding.Empty,
				Dock = swf.DockStyle.Fill,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			#pragma warning disable 612,618
			Spacing = TableLayout.DefaultSpacing;
			Padding = TableLayout.DefaultPadding;
			#pragma warning restore 612,618
		}

		public void Update()
		{
			Control.PerformLayout();
		}

		public Size Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				spacing = value;
				SetChildPadding();
			}
		}

		void SetChildPadding()
		{
			if (views != null && rowScale != null && columnScale != null)
			{
				for (int y = 0; y < rowScale.Length; y++)
				{
					for (int x = 0; x < columnScale.Length; x++)
					{
						var control = views[x, y].GetContainerControl();
						if (control != null)
							control.Margin = GetPadding(x, y);
					}
				}
			}
		}

		swf.Padding GetPadding(int x, int y)
		{
			return new swf.Padding(
				x == 0 ? 0 : spacing.Width / 2,
				y == 0 ? 0 : spacing.Height / 2,
				x == views.GetLength(0) - 1 ? 0 : (spacing.Width + 1) / 2,
				y == views.GetLength(1) - 1 ? 0 : (spacing.Height + 1) / 2);
		}

		public Padding Padding
		{
			get { return Control.Padding.ToEto(); }
			set { Control.Padding = value.ToSWF(); }
		}

		void SetScale(Control control, int x, int y)
		{
			var xscale = XScale && (x == lastColumnScale || columnScale[x]);
			var yscale = YScale && (y == lastRowScale || rowScale[y]);
			control.SetScale(xscale, yscale);
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			if (rowScale != null)
			{
				for (int y = 0; y < rowScale.Length; y++)
				{
					var ys = yscale && (y == lastRowScale || rowScale[y]);
					for (int x = 0; x < columnScale.Length; x++)
					{
						var xs = xscale && (x == lastColumnScale || columnScale[x]);
						var control = views[x, y];
						if (control != null)
							control.SetScale(xs, ys);
					}
				}
			}
		}

		public void Add(Control child, int x, int y)
		{
			if (Widget.Loaded)
				SuspendLayout();
			
			var old = views[x, y];
			if (old != null)
			{
				old.GetContainerControl().Parent = null;
			}
			views[x, y] = child;
			if (child != null)
			{
				var childHandler = child.GetWindowsHandler();
				var childControl = childHandler.ContainerControl;
				childControl.Parent = null;
				childControl.Dock = childHandler.DockStyle;
				childControl.Margin = GetPadding(x, y);
				SetScale(child, x, y);
				childHandler.BeforeAddControl(Widget.Loaded);

				Control.Controls.Add(childControl, x, y);
			}
			if (Widget.Loaded)
				ResumeLayout();
		}

		public void Move(Control child, int x, int y)
		{
			if (Widget.Loaded)
				SuspendLayout();
			swf.Control childControl = child.GetContainerControl();
			Control.SetCellPosition(childControl, new swf.TableLayoutPanelCellPosition(x, y));
			SetScale(child, x, y);
			if (Widget.Loaded)
				ResumeLayout();
		}

		public override void OnLoad(EventArgs e)
		{
			SetMinimumSize(useCache: true); // when created during pre-load, we need this to ensure the scale is set on the children properly
			base.OnLoad(e);
		}

		public void Remove(Control child)
		{
			swf.Control childControl = child.GetContainerControl();
			if (childControl.Parent == Control)
			{
				childControl.Parent = null;
				for (int y = 0; y < views.GetLength(0); y++)
					for (int x = 0; x < views.GetLength(1); x++)
					{
						if (object.ReferenceEquals(views[y, x], child)) views[y, x] = null;
					}
			}
		}

		public void CreateControl(int cols, int rows)
		{
			views = new Control[cols, rows];
			columnScale = new bool[cols];
			lastColumnScale = cols - 1;
			rowScale = new bool[rows];
			lastRowScale = rows - 1;
			Control.RowCount = rows;
			Control.ColumnCount = cols;
			Control.ColumnStyles.Clear();
			Control.RowStyles.Clear();
			for (int i = 0; i < cols; i++)
				Control.ColumnStyles.Add(GetColumnStyle(i));
			for (int i = 0; i < rows; i++)
				Control.RowStyles.Add(GetRowStyle(i));
		}

		swf.ColumnStyle GetColumnStyle(int column)
		{
			var scale = columnScale[column] || column == lastColumnScale;
			if (scale)
				return new swf.ColumnStyle(swf.SizeType.Percent, 1f);
			return new swf.ColumnStyle(swf.SizeType.AutoSize);
		}

		swf.RowStyle GetRowStyle(int row)
		{
			var scale = rowScale[row] || row == lastRowScale;
			if (scale)
				return new swf.RowStyle(swf.SizeType.Percent, 1f);
			return new swf.RowStyle(swf.SizeType.AutoSize);
		}

		void ResetColumnScale(int column)
		{
			var xscale = columnScale[column] || column == lastColumnScale;
			for (int i = 0; i < rowScale.Length; i++)
			{
				var control = views[column, i];
				control.SetScale(xscale, rowScale[i]);
			}
		}

		void ResetRowScale(int row)
		{
			var yscale = rowScale[row] || row == lastRowScale;
			for (int i = 0; i < columnScale.Length; i++)
			{
				var control = views[i, row];
				control.SetScale(columnScale[i], yscale);
			}
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			var prev = lastColumnScale;
			lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
			Control.ColumnStyles[column] = GetColumnStyle(column);
			ResetColumnScale(column);
			if (prev != lastColumnScale && column != columnScale.Length - 1)
			{
				Control.ColumnStyles[columnScale.Length - 1] = GetColumnStyle(columnScale.Length - 1);
				ResetColumnScale(columnScale.Length - 1);
			}
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			var prev = lastRowScale;
			lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;
			Control.RowStyles[row] = GetRowStyle(row);
			ResetRowScale(row);
			if (prev != lastRowScale && row != rowScale.Length - 1)
			{
				Control.RowStyles[rowScale.Length - 1] = GetRowStyle(rowScale.Length - 1);
				ResetRowScale(rowScale.Length - 1);
			}
			ResetRowScale(row);
		}

		public bool GetRowScale(int row)
		{
			return rowScale[row];
		}
	}
}
