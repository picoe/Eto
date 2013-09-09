using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using System.Diagnostics;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class TableLayoutHandler : WpfLayout<swc.Grid, TableLayout>, ITableLayout
	{
		Eto.Drawing.Size spacing;
		Control[,] controls;
		bool[] columnScale;
		bool lastColumnScale;
		bool[] rowScale;
		bool lastRowScale;

		public Size Adjust { get; set; }

		public override sw.Size GetPreferredSize (sw.Size? constraint)
		{
			var size = constraint ?? new sw.Size (double.PositiveInfinity, double.PositiveInfinity);
			var padding = new sw.Size ((columnScale.Length - 1) * Spacing.Width + Padding.Horizontal, (rowScale.Length - 1) * Spacing.Height + Padding.Vertical);
			size = new sw.Size (Math.Max(0, size.Width - padding.Width), Math.Max(0, size.Height - padding.Height));
			double[] widths = new double[columnScale.Length];
			double[] heights = new double[rowScale.Length];
			for (int y = 0; y < heights.Length; y++)
				for (int x = 0; x < widths.Length; x++) {
					var preferredSize = controls[x, y].GetPreferredSize (size);
					widths[x] = Math.Max (widths[x], preferredSize.Width);
					heights[y] = Math.Max (heights[y], preferredSize.Height);
				}

			return new sw.Size (widths.Sum () + padding.Width, heights.Sum () + padding.Height);
		}

		public void CreateControl (int cols, int rows)
		{
			controls = new Control[cols, rows];
			columnScale = new bool[cols];
			rowScale = new bool[rows];
			lastColumnScale = true;
			lastRowScale = true;
			Control = new swc.Grid {
				SnapsToDevicePixels = true
			};
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add (new swc.ColumnDefinition {
				Width = GetColumnWidth(i)
			});
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add (new swc.RowDefinition {
				Height = GetRowHeight(i)
			});
			Spacing = TableLayout.DefaultSpacing;
			Padding = TableLayout.DefaultPadding;
			Control.SizeChanged += delegate {
				SetSizes ();
			};

			for (int y = 0; y < rows; y++)
				for (int x = 0; x < cols; x++)
					Control.Children.Add (EmptyCell (x, y));
		}

		sw.FrameworkElement EmptyCell (int x, int y)
		{
			var empty = new sw.FrameworkElement { };
			swc.Grid.SetColumn (empty, x);
			swc.Grid.SetRow (empty, y);
			SetMargins (empty, x, y);
			return empty;
		}

		void SetSizes ()
		{
			if (!Widget.Loaded) return;
			for (int x = 0; x < Control.ColumnDefinitions.Count; x++) {

				var max = Control.ColumnDefinitions[x].ActualWidth;
				if (x == Control.ColumnDefinitions.Count - 1)
					max += Adjust.Width;
				foreach (var child in ColumnControls(x))
				{
					if (!double.IsNaN(child.Width))
						child.Width = Math.Max(0, max - child.Margin.Horizontal ());
				}
			}
			for (int y = 0; y < Control.RowDefinitions.Count; y++) {
				var max = Control.RowDefinitions[y].ActualHeight;
				if (y == Control.RowDefinitions.Count - 1)
					max += Adjust.Height;
				foreach (var child in RowControls (y)) {
					if (!double.IsNaN (child.Height))
						child.Height = Math.Max(0, max - child.Margin.Vertical ());
				}
			}
		}

		void SetSizes (sw.FrameworkElement control, int col, int row)
		{
			if (!Widget.Loaded) return;
			var maxWidth = double.IsNaN (control.Width) ? 0 : control.Width;
			var maxHeight = double.IsNaN (control.Height) ? 0 : control.Height;
			for (int x = 0; x < Control.ColumnDefinitions.Count; x++) {

				var max = Control.ColumnDefinitions[x].ActualWidth;
				if (x == col && max < maxWidth) max = maxWidth;
				foreach (var child in ColumnControls (x)) {
					if (!double.IsNaN (child.Width))
						child.Width = Math.Max(0, max - child.Margin.Horizontal ());
				}
			}
			for (int y = 0; y < Control.RowDefinitions.Count; y++) {
				var max = Control.RowDefinitions[y].ActualHeight;
				if (y == row && max < maxHeight) max = maxHeight;
				foreach (var child in RowControls (y)) {
					if (!double.IsNaN (child.Height))
						child.Height = Math.Max(0, max - child.Margin.Vertical ());
				}
			}
		}

		void SetMargins ()
		{
			foreach (var child in Control.Children.OfType<sw.FrameworkElement> ()) {
				var x = swc.Grid.GetColumn (child);
				var y = swc.Grid.GetRow (child);
				SetMargins (child, x, y);
			}
		}

		sw.GridLength GetColumnWidth (int column)
		{
			var scale = columnScale[column];
			if (column == columnScale.Length - 1)
				scale |= lastColumnScale;
			return new System.Windows.GridLength (1, scale ? sw.GridUnitType.Star : sw.GridUnitType.Auto);
		}

		sw.GridLength GetRowHeight (int row)
		{
			var scale = rowScale[row];
			if (row == rowScale.Length - 1)
				scale |= lastRowScale;
			return new System.Windows.GridLength (1, scale ? sw.GridUnitType.Star : sw.GridUnitType.Auto);
		}

		public void SetColumnScale (int column, bool scale)
		{
			columnScale[column] = scale;
			var lastScale = columnScale.Length == 1 || columnScale.Take (columnScale.Length - 1).All (r => !r);
			Control.ColumnDefinitions[column].Width = GetColumnWidth (column);
			if (lastScale != lastColumnScale)
			{
				lastColumnScale = lastScale;
				Control.ColumnDefinitions[columnScale.Length - 1].Width = GetColumnWidth (columnScale.Length - 1);
			}
			SetSizes ();
		}

		public bool GetColumnScale (int column)
		{
			return columnScale[column];
		}

		public void SetRowScale (int row, bool scale)
		{
			rowScale[row] = scale;
			var lastScale = rowScale.Length == 1 || rowScale.Take (rowScale.Length - 1).All (r => !r);
			Control.RowDefinitions[row].Height = GetRowHeight (row);
			if (lastScale != lastRowScale)
			{
				lastRowScale = lastScale;
				Control.RowDefinitions[rowScale.Length - 1].Height = GetRowHeight (rowScale.Length - 1);
			}
			SetSizes ();
		}

		public bool GetRowScale (int row)
		{
			return rowScale[row];
		}

		public Eto.Drawing.Size Spacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				SetMargins ();
			}
		}

		IEnumerable<sw.FrameworkElement> ColumnControls (int x)
		{
			return Control.Children.OfType<sw.FrameworkElement> ().Where (r => swc.Grid.GetColumn (r) == x);
		}

		IEnumerable<sw.FrameworkElement> RowControls (int y)
		{
			return Control.Children.OfType<sw.FrameworkElement> ().Where (r => swc.Grid.GetRow (r) == y);
		}

		void SetMargins (sw.FrameworkElement c, int x, int y)
		{
			var margin = new sw.Thickness ();
			if (x > 0) margin.Left = spacing.Width / 2;
			if (x < Control.ColumnDefinitions.Count - 1) margin.Right = spacing.Width / 2;
			if (y > 0) margin.Top = spacing.Height / 2;
			if (y < Control.RowDefinitions.Count - 1) margin.Bottom = spacing.Height / 2;
			c.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			c.VerticalAlignment = sw.VerticalAlignment.Stretch;

			c.Margin = margin;
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Control.Margin.ToEto (); }
			set { Control.Margin = value.ToWpf (); }
		}

        void Remove(int x, int y)
        {
			var control = controls[x, y];
			controls[x, y] = null;
			if (control != null)
				Control.Children.Remove (control.GetContainerControl ());
        }

		public void Add (Control child, int x, int y)
		{
            Remove(x, y);
            if (child == null)
            {
                Control.Children.Add(EmptyCell(x, y));
            }
            else
            {
				var control = child.GetContainerControl ();
				controls[x, y] = child;
				control.SetValue (swc.Grid.ColumnProperty, x);
				control.SetValue (swc.Grid.RowProperty, y);
				SetMargins (control, x, y);
				Control.Children.Add (control);
				SetSizes (control, x, y);
			}
		}

		public void Move (Control child, int x, int y)
		{
			var control = child.GetContainerControl ();
			var oldx = swc.Grid.GetColumn (control);
			var oldy = swc.Grid.GetRow (control);

			Remove (x, y);

 			control.SetValue (swc.Grid.ColumnProperty, x);
 			control.SetValue (swc.Grid.RowProperty, y);
 			SetMargins (control, x, y);
			SetSizes (control, x, y);
			SetSizes (control, x, y);
			Control.Children.Add (EmptyCell (oldx, oldy));
			controls[x, y] = child;
		}

		public void Remove (Control child)
		{
			Remove (child.GetContainerControl ());
		}

		public override void Remove (sw.FrameworkElement control)
		{
			var x = swc.Grid.GetColumn (control);
			var y = swc.Grid.GetRow (control);
			Control.Children.Remove (control);
			controls[x, y] = null;
			Control.Children.Add (EmptyCell (x, y));
			SetSizes ();
		}
	}
}
