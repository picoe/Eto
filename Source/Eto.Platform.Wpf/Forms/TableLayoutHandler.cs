using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;

namespace Eto.Platform.Wpf.Forms
{
	public class TableLayoutHandler : WpfLayout<swc.Grid, TableLayout>, ITableLayout
	{
		Eto.Drawing.Size spacing;
		bool lastColumnSet;
		bool lastRowSet;

		public void CreateControl (int cols, int rows)
		{
			Control = new swc.Grid {
				SnapsToDevicePixels = true
			};
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add (new swc.ColumnDefinition {
				Width = new System.Windows.GridLength (1, i == cols - 1 ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto)
			});
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add (new swc.RowDefinition {
				Height = new System.Windows.GridLength (1, i == rows - 1 ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto)
			});
			Spacing = TableLayout.DefaultSpacing;
			Padding = TableLayout.DefaultPadding;
			Control.SizeChanged += delegate {
				SetSizes ();
			};
		}

		void SetSizes ()
		{
			if (!Widget.Loaded) return;
			for (int x = 0; x < Control.ColumnDefinitions.Count; x++) {

				var max = Control.ColumnDefinitions[x].ActualWidth;
				foreach (var child in ColumnControls (x)) {
					if (!double.IsNaN(child.Width))
						child.Width = max - child.Margin.Left - child.Margin.Right;
				}
			}
			for (int y = 0; y < Control.RowDefinitions.Count; y++) {
				var max = Control.RowDefinitions[y].ActualHeight;
				foreach (var child in RowControls (y)) {
					if (!double.IsNaN (child.Height))
						child.Height = max - child.Margin.Top - child.Margin.Bottom;
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
						child.Width = Math.Max(0, max - child.Margin.Left - child.Margin.Right);
				}
			}
			for (int y = 0; y < Control.RowDefinitions.Count; y++) {
				var max = Control.RowDefinitions[y].ActualHeight;
				if (y == row && max < maxHeight) max = maxHeight;
				foreach (var child in RowControls (y)) {
					if (!double.IsNaN (child.Height))
						child.Height = Math.Max(0, max - child.Margin.Top - child.Margin.Bottom);
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

		public void SetColumnScale (int column, bool scale)
		{
			bool isLastColumn = column == Widget.Size.Width - 1;
			Control.ColumnDefinitions[column].Width = new System.Windows.GridLength (1, scale ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto);
			if (scale) {
				if (!lastColumnSet && !isLastColumn) {
					SetColumnScale (Widget.Size.Width - 1, false);
				}
				else if (isLastColumn)
					lastColumnSet = true;

			}
			SetSizes ();
		}

		public void SetRowScale (int row, bool scale)
		{
			Control.RowDefinitions[row].Height = new System.Windows.GridLength (1, scale ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto);
			if (scale) {
				bool isLastRow = row == Widget.Size.Height - 1;
				if (!lastRowSet && !isLastRow) {
					SetRowScale (Widget.Size.Height - 1, false);
				}
				else if (isLastRow)
					lastRowSet = true;

			}
			SetSizes ();
		}

		public Eto.Drawing.Size Spacing
		{
			get
			{
				return spacing;
			}
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
			get { return Generator.Convert (Control.Margin); }
			set { Control.Margin = Generator.Convert (value); }
		}

		public void Add (Control child, int x, int y)
		{
			if (child == null) {
				foreach (sw.UIElement element in Control.Children) {
					var col = swc.Grid.GetColumn (element);
					if (x != col) continue;
					var row = swc.Grid.GetRow (element);
					if (y != row) continue;
					Control.Children.Remove (element);
					break;
				}
			}
			else {
				var control = (sw.FrameworkElement)child.ControlObject;
				control.SetValue (swc.Grid.ColumnProperty, x);
				control.SetValue (swc.Grid.RowProperty, y);
				SetMargins (control, x, y);
				Control.Children.Add (control);
				SetSizes (control, x, y);
			}
		}

		public void Move (Control child, int x, int y)
		{
			var control = (sw.FrameworkElement)child.ControlObject;
			control.SetValue (swc.Grid.ColumnProperty, x);
			control.SetValue (swc.Grid.RowProperty, y);
			SetMargins (control, x, y);
			SetSizes (control, x, y);
		}

		public void Remove (Control child)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			Control.Children.Remove (control);
			SetSizes ();
		}
	}
}
