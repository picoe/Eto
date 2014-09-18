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
		swc.Border border = new swc.Border();
		Size spacing;
		Control[,] controls;
		bool[] columnScale;
		bool[] rowScale;
		int lastColumnScale;
		int lastRowScale;
		bool inGroupBox;

		public TableLayoutHandler()
		{
			spacing = TableLayout.DefaultSpacing;
			border.Padding = TableLayout.DefaultPadding.ToWpf();
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

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			var widths = new double[columnScale.Length];
			double height = 0;
			for (int y = 0; y < rowScale.Length; y++)
			{
				double maxHeight = 0;
				for (int x = 0; x < widths.Length; x++)
				{
					var childControl = controls[x, y].GetWpfFrameworkElement();
					if (childControl != null)
					{
						var preferredSize = childControl.GetPreferredSize(Conversions.PositiveInfinitySize);
						var margin = childControl.ContainerControl.Margin;
						widths[x] = Math.Max(widths[x], preferredSize.Width + margin.Horizontal());
						maxHeight = Math.Max(maxHeight, preferredSize.Height + margin.Vertical());
					}
				}
				height += maxHeight;
			}

			return new sw.Size(widths.Sum() + border.Padding.Horizontal(), height + border.Padding.Vertical());
		}

		public void CreateControl(int cols, int rows)
		{
			controls = new Control[cols, rows];
			columnScale = new bool[cols];
			rowScale = new bool[rows];
			lastColumnScale = cols - 1;
			lastRowScale = rows - 1;
			Control = new swc.Grid { SnapsToDevicePixels = true };
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = GetColumnWidth(i) });
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add(new swc.RowDefinition { Height = GetRowHeight(i) });

			for (int y = 0; y < rows; y++)
				for (int x = 0; x < cols; x++)
					Control.Children.Add(EmptyCell(x, y));

			border.Child = Control;

			Control.SizeChanged += Control_SizeChanged;
			Control.Loaded += Control_SizeChanged;
		}

		void Control_SizeChanged(object sender, EventArgs e)
		{
			SetChildrenSizes();
		}

		sw.FrameworkElement EmptyCell(int x, int y)
		{
			var empty = new sw.FrameworkElement();
			swc.Grid.SetColumn(empty, x);
			swc.Grid.SetRow(empty, y);
			SetMargins(empty, x, y);
			return empty;
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			inGroupBox = Widget.FindParent<GroupBox>() != null;
			SetMargins();

			if (Control.IsLoaded)
				SetScale();
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			if (Widget.Loaded)
			{
				base.SetScale(xscale, yscale);
				SetScale();
			}
		}

		void SetScale()
		{
			if (Control == null)
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
			SetChildrenSizes();
		}

		void SetChildrenSizes()
		{
			var inGroupBoxCurrent = inGroupBox;
			var widths = new double[Control.ColumnDefinitions.Count];
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

		void SetMargins(sw.FrameworkElement c, int x, int y)
		{
			var margin = new sw.Thickness();
			if (x > 0) margin.Left = spacing.Width / 2;
			if (x < Control.ColumnDefinitions.Count - 1) margin.Right = spacing.Width / 2;
			if (y > 0) margin.Top = spacing.Height / 2;
			if (y < Control.RowDefinitions.Count - 1) margin.Bottom = spacing.Height / 2;
			c.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			c.VerticalAlignment = sw.VerticalAlignment.Stretch;
			c.Margin = margin;
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
