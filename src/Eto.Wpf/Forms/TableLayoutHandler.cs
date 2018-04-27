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
	public class EtoGrid : swc.Grid
	{
		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			var size = base.MeasureOverride(constraint);

			// WPF has problems with multiple rows or columns with GridUnitType.Star
			// 1) it doesn't autosize correctly to the largest row/column
			// 2) it doesn't position elements correctly until next layout pass
			if (RowDefinitions.Count > 1 && (double.IsInfinity(constraint.Height) || !IsLoaded))
			{
				double maxHeight = 0;
				double totalHeight = 0;
				int totalRows = 0;
				for (int i = 0; i < RowDefinitions.Count; i++)
				{
					var rd = RowDefinitions[i];
					if (rd.Height.IsStar)
					{
						double rowHeight = 0;
						for (int childIndex = 0; childIndex < Children.Count; childIndex++)
						{
							var child = Children[childIndex];
							var row = GetRow(child);
							if (row != i)
								continue;
							//child.Measure(constraint);
							var desired = child.DesiredSize.Height;
							if (!double.IsInfinity(desired))
								rowHeight = Math.Max(rowHeight, desired);
						}
						maxHeight = Math.Max(rowHeight, maxHeight);
						totalHeight += rowHeight;
						totalRows++;
					}
				}
				if (totalRows > 1)
					size.Height = Math.Max(0, size.Height - totalHeight + maxHeight * totalRows);
			}

			if (ColumnDefinitions.Count > 1 && (double.IsInfinity(constraint.Width) || !IsLoaded))
			{
				double maxWidth = 0;
				double totalWidth = 0;
				int totalColumns = 0;
				for (int i = 0; i < ColumnDefinitions.Count; i++)
				{
					var cd = ColumnDefinitions[i];
					if (cd.Width.IsStar)
					{
						double columnWidth = 0;
						for (int childIndex = 0; childIndex < Children.Count; childIndex++)
						{
							var child = Children[childIndex];
							var row = GetColumn(child);
							if (row != i)
								continue;
							//child.Measure(constraint);
							var desired = child.DesiredSize.Width;
							if (!double.IsInfinity(desired))
								columnWidth = Math.Max(columnWidth, desired);
						}
						maxWidth = Math.Max(columnWidth, maxWidth);
						totalWidth += columnWidth;
						totalColumns++;
					}
				}
				if (totalColumns > 1)
					size.Width = Math.Max(0, size.Width - totalWidth + maxWidth * totalColumns);
			}
			return size;
		}

	}

	public class TableLayoutHandler : WpfLayout<EtoGrid, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
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
			Control = new EtoGrid { SnapsToDevicePixels = true };
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

        bool IsColumnScaled(int column) => columnScale[column] || column == lastColumnScale;

        bool IsRowScaled(int row) => rowScale[row] || row == lastRowScale;

		bool IsCreated => columnScale != null && rowScale != null;

		public void CreateControl(int cols, int rows)
		{
			controls = new Control[cols, rows];
			columnScale = new bool[cols];
			rowScale = new bool[rows];
			lastColumnScale = cols - 1;
			lastRowScale = rows - 1;

			// set child here as we get a NRE when using in this in a custom cell if used before created.
			border.Child = Control;

			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add(new swc.ColumnDefinition());
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add(new swc.RowDefinition());

			for (int y = 0; y < rows; y++)
				for (int x = 0; x < cols; x++)
					Control.Children.Add(EmptyCell(x, y));

			if (Widget.Loaded)
			{
				// happens with a blank TableLayout, or if populated during Load event.
				InitializeSizes();
				SetScale(true);
				SetMargins(true);
				SetChildrenSizes(true);
			}

			Control.LayoutUpdated += Control_LayoutUpdated;
		}

		void Control_LayoutUpdated(object sender, EventArgs e) => SetChildrenSizes(false);

		sw.FrameworkElement EmptyCell(int x, int y)
		{
			var empty = new sw.FrameworkElement();
			swc.Grid.SetColumn(empty, x);
			swc.Grid.SetRow(empty, y);
			return empty;
		}

		void InitializeSizes()
		{
			lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
			lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;

			for (int i = 0; i < Control.ColumnDefinitions.Count; i++)
				Control.ColumnDefinitions[i].Width = GetColumnWidth(i);
			for (int i = 0; i < Control.RowDefinitions.Count; i++)
				Control.RowDefinitions[i].Height = GetRowHeight(i);
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			inGroupBox = Widget.FindParent<GroupBox>() != null;
			if (IsCreated)
			{
				// won't be created yet if populated on Load event or if empty.

				InitializeSizes();
				SetScale(true);
				SetMargins(true);
				SetChildrenSizes(true);
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetScale();
		}

		void SetScale(bool force = false)
		{
			if (Control == null || (!Widget.Loaded && !force))
				return;
			for (int y = 0; y < controls.GetLength(1); y++)
			{
				for (int x = 0; x < controls.GetLength(0); x++)
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
			if (!Widget.Loaded)
				return;
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
			Control.UpdateLayout();
		}

		void SetChildrenSizes(bool force)
		{
			if (!Widget.Loaded && !force)
				return;
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

		void SetMargins(bool force = false)
		{
			if (Widget.Loaded || force)
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
			return scale ? new sw.GridLength(1, sw.GridUnitType.Star) : sw.GridLength.Auto;
		}

		sw.GridLength GetRowHeight(int row)
		{
			var scale = rowScale[row] || lastRowScale == row;
			return scale ? new sw.GridLength(1, sw.GridUnitType.Star) : sw.GridLength.Auto;
		}

		public void SetColumnScale(int column, bool scale)
		{
			columnScale[column] = scale;
			if (Widget.Loaded)
			{
				var lastScale = lastColumnScale;
				lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
				Control.ColumnDefinitions[column].Width = GetColumnWidth(column);
				if (lastScale != lastColumnScale)
				{
					Control.ColumnDefinitions[columnScale.Length - 1].Width = GetColumnWidth(columnScale.Length - 1);
				}
				SetScale();
			}
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		public void SetRowScale(int row, bool scale)
		{
			rowScale[row] = scale;
			if (Widget.Loaded)
			{
				var lastScale = lastRowScale;
				lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;
				Control.RowDefinitions[row].Height = GetRowHeight(row);
				if (lastScale != lastRowScale)
				{
					Control.RowDefinitions[rowScale.Length - 1].Height = GetRowHeight(rowScale.Length - 1);
				}
				SetScale();
			}
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
			c.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			c.VerticalAlignment = sw.VerticalAlignment.Stretch;
			var margin = new sw.Thickness();
			if (x > 0) margin.Left = Math.Floor(spacing.Width / 2.0);
			if (x < Control.ColumnDefinitions.Count - 1) margin.Right = Math.Ceiling(spacing.Width / 2.0);
			if (y > 0) margin.Top = Math.Floor(spacing.Height / 2.0);
			if (y < Control.RowDefinitions.Count - 1) margin.Bottom = Math.Ceiling(spacing.Height / 2.0);
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
			if (control != null)
			{
				controls[x, y] = null;
				Control.Children.Remove(control.GetContainerControl());
			}
		}

		public void Add(Control child, int x, int y)
		{
			Remove(x, y);
			if (child != null)
			{
				var handler = child.GetWpfFrameworkElement();
				var control = handler.ContainerControl;
				controls[x, y] = child;
				control.SetValue(swc.Grid.ColumnProperty, x);
				control.SetValue(swc.Grid.RowProperty, y);
				if (Widget.Loaded)
				{
					SetMargins(control, x, y);
					SetScale(handler, x, y);
				}
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

			if (x == oldx && y == oldy)
				return;

			controls[oldx, oldy] = null;

			Remove(x, y);

			control.SetValue(swc.Grid.ColumnProperty, x);
			control.SetValue(swc.Grid.RowProperty, y);
			if (Widget.Loaded)
			{
				SetMargins(control, x, y);
				SetScale(handler, x, y);
			}
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
			UpdatePreferredSize();
		}
	}
}
