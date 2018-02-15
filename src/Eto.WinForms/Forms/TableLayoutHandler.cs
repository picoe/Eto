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

		public class EtoTableLayoutPanel : swf.TableLayoutPanel
		{
			public TableLayoutHandler Handler { get; set; }

			sd.Size GetMinSize()
			{
				var columnScale = Handler.columnScale;
				var lastColumnScale = Handler.lastColumnScale;
				var rowScale = Handler.rowScale;
				var lastRowScale = Handler.lastRowScale;
				if (columnScale == null || rowScale == null || !Handler.Widget.Loaded || !IsHandleCreated)
					return sd.Size.Empty;
				// ensure that our width doesn't get smaller than the non-scaled child controls
				// to make it so the child controls are left-justified when the container
				// is smaller than all the children
				var widths = GetColumnWidths();
				var heights = GetRowHeights();
                if (widths.Length != columnScale.Length || heights.Length != rowScale.Length)
                    return sd.Size.Empty;
				var curSize = sd.Size.Empty;
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
				return curSize;
			}

			protected override void SetBoundsCore(int x, int y, int width, int height, swf.BoundsSpecified specified)
			{
				// ensure that the controls are left-aligned when the space available is smaller than the minimum size 
				// of this control
				var curSize = GetMinSize();
				width = Math.Max(curSize.Width, width);
				height = Math.Max(curSize.Height, height);
				base.SetBoundsCore(x, y, width, height, specified);
			}

			// optimization especially for content on drawable
			protected override void OnBackColorChanged( EventArgs e )
			{
				SetStyle
					( swf.ControlStyles.AllPaintingInWmPaint
					| swf.ControlStyles.DoubleBuffer
					, BackColor.A != 255 );
				base.OnBackColorChanged( e );
			}
			protected override void OnParentBackColorChanged( EventArgs e )
			{
				SetStyle
					( swf.ControlStyles.AllPaintingInWmPaint
					| swf.ControlStyles.DoubleBuffer
					, BackColor.A != 255 );
				base.OnParentBackColorChanged( e );
			}
		}

		public TableLayoutHandler()
		{
			Control = new EtoTableLayoutPanel
			{
				Handler = this,
				Margin = swf.Padding.Empty,
				Dock = swf.DockStyle.Fill,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
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
							control.Margin = GetMargin(x, y);
					}
				}
			}
		}

		swf.Padding GetMargin(int x, int y)
		{
			var margin = new swf.Padding();
			if (x > 0) margin.Left = (int)Math.Floor(spacing.Width / 2.0);
			if (x < columnScale.Length - 1) margin.Right = (int)Math.Ceiling(spacing.Width / 2.0);
			if (y > 0) margin.Top = (int)Math.Floor(spacing.Height / 2.0);
			if (y < rowScale.Length - 1) margin.Bottom = (int)Math.Ceiling(spacing.Height / 2.0);
			return margin;
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
				old.GetContainerControl().Parent = null;
			else
				RemoveEmptyCell(x, y);
			views[x, y] = child;
			if (child != null)
			{
				var childHandler = child.GetWindowsHandler();
				var childControl = childHandler.ContainerControl;
				childControl.Parent = null;
				childControl.Dock = childHandler.DockStyle;
				childControl.Margin = GetMargin(x, y);
				SetScale(child, x, y);
				childHandler.BeforeAddControl(Widget.Loaded);

				Control.Controls.Add(childControl, x, y);
			}
			else
				SetEmptyCell(x, y);
			if (Widget.Loaded)
				ResumeLayout();
		}

		public void Move(Control child, int x, int y)
		{
			if (Widget.Loaded)
				SuspendLayout();
			var old = views[x, y];
			if (old != null)
				old.GetContainerControl().Parent = null;
			else
				RemoveEmptyCell(x, y);

			swf.Control childControl = child.GetContainerControl();
			Control.SetCellPosition(childControl, new swf.TableLayoutPanelCellPosition(x, y));
			views[x, y] = child;
			SetScale(child, x, y);
			if (Widget.Loaded)
				ResumeLayout();
		}

		public override void OnLoad(EventArgs e)
		{
            SetMinimumSize(useCache: true); // when created during pre-load, we need this to ensure the scale is set on the children properly
			base.OnLoad(e);
			FillEmptyCells();
		}

		class EmptyCell : swf.Control
		{
			protected override void CreateHandle()
			{
				base.CreateHandle();
				SetStyle(System.Windows.Forms.ControlStyles.Selectable, false);
			}

		}

		swf.Control CreateEmptyCell(int x, int y)
		{
			var c = new EmptyCell();
			c.Margin = GetMargin(x, y);
			return c;
		}

		void RemoveEmptyCell(int x, int y)
		{
			if (x == 0 || y == 0)
			{
				var ctl = Control.GetControlFromPosition(x, y);
				if (ctl is EmptyCell)
				{
					Control.Controls.Remove(ctl);
				}
			}
		}

		void SetEmptyCell(int x, int y)
		{
			if (x == 0 || y == 0)
			{
				Control.Controls.Add(CreateEmptyCell(x, y), x, y);
			}
		}

		void FillEmptyCells()
		{
			for (int x = 1; x < views.GetLength(0); x++)
			{
				var ctl = Control.GetControlFromPosition(x, 0);
				if (ctl == null)
					Control.Controls.Add(CreateEmptyCell(x, 0), x, 0);
			}
			for (int y = 0; y < views.GetLength(1); y++)
			{
				var ctl = Control.GetControlFromPosition(0, y);
				if (ctl == null)
					Control.Controls.Add(CreateEmptyCell(0, y), 0, y);
			}
		}

		public void Remove(Control child)
		{
			swf.Control childControl = child.GetContainerControl();
			if (childControl.Parent == Control)
			{
				var pos = Control.GetCellPosition(childControl);
				views[pos.Column, pos.Row] = null;
				childControl.Parent = null;
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
				control.SetScale(xscale, i == lastRowScale || rowScale[i]);
			}
		}

		void ResetRowScale(int row)
		{
			var yscale = rowScale[row] || row == lastRowScale;
			for (int i = 0; i < columnScale.Length; i++)
			{
				var control = views[i, row];
				control.SetScale(i == lastColumnScale || columnScale[i], yscale);
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
