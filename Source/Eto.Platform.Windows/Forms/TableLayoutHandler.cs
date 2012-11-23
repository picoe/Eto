using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.Windows
{
	public class TableLayoutHandler : WindowsLayout<System.Windows.Forms.TableLayoutPanel, TableLayout>, ITableLayout
	{
		Size spacing;
		Control[,] views;
		bool[] columnScale;
		bool lastColumnScale;
		bool[] rowScale;
		bool lastRowScale;

		public override object LayoutObject
		{
			get	{ return Control; }
		}

		public override Size DesiredSize
		{
			get { return Control.PreferredSize.ToEto (); }
		}
		
		public TableLayoutHandler()
		{
			Control = new SWF.TableLayoutPanel();
			this.Control.SuspendLayout ();
			this.Control.Margin = SWF.Padding.Empty;
			this.Control.Dock = SWF.DockStyle.Fill;
			this.Control.Size = SD.Size.Empty;
			this.Control.MinimumSize = SD.Size.Empty;
			this.Control.AutoSize = true;
			this.Control.AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink;
			this.Spacing = TableLayout.DefaultSpacing;
			this.Padding = TableLayout.DefaultPadding;
		}

		public override void OnLoad ()
		{
			base.OnLoad ();
			this.Control.ResumeLayout ();
		}
		
		public override void Update ()
		{
			this.Control.Update();
		}
		
		public Size Spacing {
			get {
				return spacing;
			}
			set {
				spacing = value;
				var newpadding = new SWF.Padding(0, 0, spacing.Width, spacing.Height);
				foreach (SWF.Control control in this.Control.Controls)
				{
					control.Margin = newpadding;
				}
			}
		}
		
		SWF.Padding GetPadding(int x, int y)
		{
			return new SWF.Padding(
				x == 0 ? 0 : spacing.Width / 2, 
				y == 0 ? 0 : spacing.Height / 2, 
				x == this.views.GetLength(0)-1 ? 0 : (spacing.Width + 1) / 2, 
				y == this.views.GetLength(1)-1 ? 0 : (spacing.Height + 1) / 2);
		}
		
		public Padding Padding {
			get { return Control.Padding.ToEto (); }
			set { Control.Padding = value.ToSWF (); }
		}

		void SetScale (Control control, int x, int y)
		{
			var xscale = columnScale[x] || (x == columnScale.Length - 1 && lastColumnScale);
			var yscale = rowScale[y] || (y == rowScale.Length - 1 && lastRowScale);
			control.SetScale (this.XScale && xscale, this.YScale && yscale);
		}

		public override void SetScale (bool xscale, bool yscale)
		{
			base.SetScale (xscale, yscale);
			for (int y = 0; y < rowScale.Length; y ++)
				for (int x = 0; x < columnScale.Length; x++)
				{
					SetScale (views[x, y], x, y);
				}
		}
		
		public void Add(Control child, int x, int y)
		{
			Control.SuspendLayout ();
			var old = views[x, y];
			if (old != null)
				Control.Controls.Remove (old.GetContainerControl ());
			views[x, y] = child;
			if (child != null) {
				SWF.Control childControl = child.GetContainerControl ();
				if (childControl.Parent != null) childControl.Parent.Controls.Remove (childControl);
				childControl.Dock = ((IWindowsControl)child.Handler).DockStyle;
				childControl.Margin = GetPadding (x, y);
				SetScale (child, x, y);

				Control.Controls.Add (childControl, x, y);
			}
			Control.ResumeLayout ();
		}
		
		public void Move(Control child, int x, int y)
		{
			SWF.Control childControl = child.GetContainerControl ();
			Control.SetCellPosition(childControl, new SWF.TableLayoutPanelCellPosition(x, y));
			SetScale (child, x, y);
		}
		
		public void Remove (Control child)
		{
			SWF.Control childControl = child.GetContainerControl ();
			if (childControl.Parent != null) childControl.Parent.Controls.Remove(childControl);
			for (int y=0; y<views.GetLength(0); y++)
			for (int x=0; x<views.GetLength(1); x++)
			{
				if (views[y,x] == child) views[y,x] = null;
			}
		}

		public void CreateControl(int cols, int rows)
		{
			views = new Control[cols, rows];
			columnScale = new bool[cols];
			lastColumnScale = true;
			rowScale = new bool[rows];
			lastRowScale = true;
			Control.RowCount = rows;
			Control.ColumnCount = cols;
			Control.ColumnStyles.Clear();
			Control.RowStyles.Clear();
			for (int i = 0; i < cols; i++)
				Control.ColumnStyles.Add(GetColumnStyle(i));
			for (int i = 0; i < rows; i++)
				Control.RowStyles.Add(GetRowStyle(i));
		}

		SWF.ColumnStyle GetColumnStyle (int column)
		{
			var scale = columnScale[column];
			if (column == columnScale.Length - 1)
				scale |= lastColumnScale;
			if (scale)
				return new SWF.ColumnStyle (SWF.SizeType.Percent, 1f);
			else
				return new SWF.ColumnStyle (SWF.SizeType.AutoSize);
		}

		SWF.RowStyle GetRowStyle (int row)
		{
			var scale = rowScale[row];
			if (row == rowScale.Length - 1)
				scale |= lastRowScale;
			if (scale)
				return new SWF.RowStyle (SWF.SizeType.Percent, 1f);
			else
				return new SWF.RowStyle (SWF.SizeType.AutoSize);
		}

		void ResetColumnScale (int column)
		{
			var xscale = columnScale[column];
			if (column == columnScale.Length - 1)
				xscale |= lastColumnScale;
			for (int i = 0; i < rowScale.Length; i++)
			{
				var control = views[column, i];
				control.SetScale (xscale, rowScale[i]);
			}
		}

		void ResetRowScale (int row)
		{
			var yscale = rowScale[row];
			if (row == rowScale.Length - 1)
				yscale |= lastRowScale;
			for (int i = 0; i < columnScale.Length; i++)
			{
				var control = views[i, row];
				control.SetScale (columnScale[i], yscale);
			}
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			var lastScale = columnScale.Length == 1 || columnScale.Take (columnScale.Length - 1).All (r => !r);
			Control.ColumnStyles[column] = GetColumnStyle (column);
			ResetColumnScale (column);
			if (lastScale != lastColumnScale)
			{
				lastColumnScale = lastScale;
				Control.ColumnStyles[columnScale.Length - 1] = GetColumnStyle (columnScale.Length - 1);
				ResetColumnScale (columnScale.Length - 1);
			}
		}

		public bool GetColumnScale (int column)
		{
			return columnScale[column];
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			var lastScale = rowScale[rowScale.Length - 1] || rowScale.Take (rowScale.Length - 1).All (r => !r);
			Control.RowStyles[row] = GetRowStyle (row);
			ResetRowScale (row);
			if (lastScale != lastRowScale)
			{
				lastRowScale = lastScale;
				Control.RowStyles[rowScale.Length - 1] = GetRowStyle (rowScale.Length - 1);
				ResetRowScale (rowScale.Length - 1);
			}
			ResetRowScale (row);
		}

		public bool GetRowScale (int row)
		{
			return rowScale[row];
		}
	}
}
