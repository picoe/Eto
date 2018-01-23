using System;
using System.Linq;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class TableLayoutHandler : WpfLayout<swc.Grid, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		readonly swc.Border border;
		Size spacing;
		Control[,] controls;
		bool[] columnScale;
		bool[] rowScale;
		int lastColumnScale;
		int lastRowScale;
		bool inGroupBox;

		public TableLayoutHandler()
		{
			Control = new swc.Grid { SnapsToDevicePixels = true };
			border = new EtoBorder { Handler = this };
			border.Padding = Padding.Empty.ToWpf();
		}

		public Size Adjust { get; set; }

		public override sw.FrameworkElement ContainerControl
		{
			get { return border; }
		}

		public override Color BackgroundColor
		{
			get { return border.Background.ToEtoColor(); }
			set { border.Background = value.ToWpfBrush(Control.Background); }
		}

        bool IsColumnScaled(int column)
        {
            return columnScale[column] || column == lastColumnScale;
        }

        bool IsRowScaled(int row)
        {
            return rowScale[row] || row == lastRowScale;
        }

		public void CreateControl(int cols, int rows)
		{
			controls = new Control[cols, rows];
			columnScale = new bool[cols];
			rowScale = new bool[rows];
			lastColumnScale = cols - 1;
			lastRowScale = rows - 1;
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = GetColumnWidth(i) });
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add(new swc.RowDefinition { Height = GetRowHeight(i) });

			for (int y = 0; y < rows; y++)
				for (int x = 0; x < cols; x++)
					Control.Children.Add(EmptyCell(x, y));

			border.Child = Control;

			Control.LayoutUpdated += (sender, args) => SetChildrenSizes();
		}

		sw.FrameworkElement EmptyCell(int x, int y)
		{
			var empty = new sw.FrameworkElement();
			swc.Grid.SetColumn(empty, x);
			swc.Grid.SetRow(empty, y);
			SetMargins(empty, x, y);
			return empty;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			inGroupBox = Widget.FindParent<GroupBox>() != null;
			SetMargins();
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetChildrenSizes();
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetScale();
		}

		void SetScale()
		{
			if (Control == null || !Widget.Loaded)
				return;
			for (int y = 0; y < Control.RowDefinitions.Count; y++)
			{
				for (int x = 0; x < Control.ColumnDefinitions.Count; x++)
				{
					var handler = controls[x, y].GetWpfFrameworkElement();
					if (handler != null)
						SetScale(handler, x, y);
				}
			}
		}

		public override void UpdatePreferredSize()
		{
			base.UpdatePreferredSize();
			for (int y = 0; y < Control.RowDefinitions.Count; y++)
			{
				for (int x = 0; x < Control.ColumnDefinitions.Count; x++)
				{
					var child = controls[x, y];
					var childControl = child.GetWpfFrameworkElement();
					if (childControl != null)
					{
						childControl.ParentMinimumSize = sw.Size.Empty;
					}
				}
			}
			if (Widget.Loaded)
				Control.UpdateLayout();
			SetChildrenSizes();
		}

		void SetChildrenSizes()
		{
			var inGroupBoxCurrent = inGroupBox;
			for (int y = 0; y < Control.RowDefinitions.Count; y++)
			{
				var rowdef = Control.RowDefinitions[y];
				var maxy = rowdef.ActualHeight;
				if (inGroupBoxCurrent && rowdef.Height.IsStar)
 				{
					maxy -= 1;
					inGroupBoxCurrent = false;
				}
				for (int x = 0; x < Control.ColumnDefinitions.Count; x++)
				{
					var coldef = Control.ColumnDefinitions[x];
					var maxx = coldef.ActualWidth;
					var child = controls[x, y];
					var childControl = child.GetWpfFrameworkElement();
					if (childControl != null)
					{
						var margin = childControl.ContainerControl.Margin;
						childControl.ParentMinimumSize = new sw.Size(Math.Max(0, maxx - margin.Horizontal()), Math.Max(0, maxy - margin.Vertical()));
					}
				}
			}
		}

		void SetScale(IWpfFrameworkElement handler, int x, int y)
		{
			handler.SetScale(XScale && (columnScale[x] || lastColumnScale == x), YScale && (rowScale[y] || lastRowScale == y));
		}

		void SetMargins()
		{
			if (Widget.Loaded)
			{
				foreach (var child in Control.Children.OfType<sw.FrameworkElement>())
				{
					var x = swc.Grid.GetColumn(child);
					var y = swc.Grid.GetRow(child);
					SetMargins(child, x, y);
				}
			}
		}

		sw.GridLength GetColumnWidth(int column)
		{
			var scale = columnScale[column] || column == lastColumnScale;
			return new System.Windows.GridLength(1, scale ? sw.GridUnitType.Star : sw.GridUnitType.Auto);
		}

		sw.GridLength GetRowHeight(int row)
		{
			var scale = rowScale[row] || lastRowScale == row;
			return new System.Windows.GridLength(1, scale ? sw.GridUnitType.Star : sw.GridUnitType.Auto);
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			var lastScale = lastColumnScale;
			lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
			Control.ColumnDefinitions[column].Width = GetColumnWidth(column);
			if (lastScale != lastColumnScale)
			{
				Control.ColumnDefinitions[columnScale.Length - 1].Width = GetColumnWidth(columnScale.Length - 1);
			}
			SetScale();
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			var lastScale = lastRowScale;
			lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;
			Control.RowDefinitions[row].Height = GetRowHeight(row);
			if (lastScale != lastRowScale)
			{
				Control.RowDefinitions[rowScale.Length - 1].Height = GetRowHeight(rowScale.Length - 1);
			}
			SetScale();
		}

		public bool GetRowScale(int row)
		{
			return rowScale[row];
		}

		public Size Spacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				SetMargins();
			}
		}

        sw.Thickness GetMargins(int x, int y)
        {
            var margin = new sw.Thickness();
            if (x > 0) margin.Left = Math.Floor(spacing.Width / 2.0);
            if (x < Control.ColumnDefinitions.Count - 1) margin.Right = Math.Ceiling(spacing.Width / 2.0);
            if (y > 0) margin.Top = Math.Floor(spacing.Height / 2.0);
            if (y < Control.RowDefinitions.Count - 1) margin.Bottom = Math.Ceiling(spacing.Height / 2.0);
            return margin;
        }

        void SetMargins(sw.FrameworkElement c, int x, int y)
		{
			c.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			c.VerticalAlignment = sw.VerticalAlignment.Stretch;
			c.Margin = GetMargins(x, y);
		}

		public Padding Padding
		{
			get { return border.Padding.ToEto(); }
			set { border.Padding = value.ToWpf(); }
		}

		void Remove(int x, int y)
		{
			var control = controls[x, y];
			controls[x, y] = null;
			if (control != null)
				Control.Children.Remove(control.GetContainerControl());
		}

		public void Add(Control child, int x, int y)
		{
			Remove(x, y);
			if (child == null)
			{
				Control.Children.Add(EmptyCell(x, y));
			}
			else
			{
				var handler = child.GetWpfFrameworkElement();
				var control = handler.ContainerControl;
				controls[x, y] = child;
				control.SetValue(swc.Grid.ColumnProperty, x);
				control.SetValue(swc.Grid.RowProperty, y);
				SetMargins(control, x, y);
				if (Widget.Loaded)
					SetScale(handler, x, y);
				Control.Children.Add(control);
			}
			UpdatePreferredSize();
		}

		public void Move(Control child, int x, int y)
		{
			var handler = child.GetWpfFrameworkElement();
			var control = handler.ContainerControl;
			var oldx = swc.Grid.GetColumn(control);
			var oldy = swc.Grid.GetRow(control);

			Remove(x, y);

			control.SetValue(swc.Grid.ColumnProperty, x);
			control.SetValue(swc.Grid.RowProperty, y);
			SetMargins(control, x, y);
			SetScale(handler, x, y);
			Control.Children.Add(EmptyCell(oldx, oldy));
			controls[x, y] = child;
			UpdatePreferredSize();
		}

		public void Remove(Control child)
		{
			Remove(child.GetContainerControl());
		}

		public override void Remove(sw.FrameworkElement child)
		{
			var x = swc.Grid.GetColumn(child);
			var y = swc.Grid.GetRow(child);
			Control.Children.Remove(child);
			controls[x, y] = null;
			Control.Children.Add(EmptyCell(x, y));
			UpdatePreferredSize();
		}
	}
}
