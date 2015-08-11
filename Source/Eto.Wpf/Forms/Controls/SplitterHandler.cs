using System;
using System.Linq;
using Eto.Forms;
using swm = System.Windows.Media;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class SplitterHandler : WpfContainer<swc.Grid, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		readonly swc.GridSplitter splitter;
		readonly swc.DockPanel pane1;
		readonly swc.DockPanel pane2;
		Orientation orientation;
		SplitterFixedPanel fixedPanel;
		int? position;
		int splitterWidth = 5;
		double relative = double.NaN;
		readonly sw.Style style;

		Control panel1;
		Control panel2;

		public SplitterHandler()
		{
			Control = new swc.Grid();
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition());
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = sw.GridLength.Auto });
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition());
			Control.RowDefinitions.Add(new swc.RowDefinition());
			Control.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
			Control.RowDefinitions.Add(new swc.RowDefinition());

			splitter = new swc.GridSplitter
			{
				//Background = sw.SystemColors.ControlLightLightBrush,
				ResizeBehavior = swc.GridResizeBehavior.PreviousAndNext
			};
			pane1 = new swc.DockPanel { LastChildFill = true };
			pane2 = new swc.DockPanel { LastChildFill = true };


			Control.Children.Add(pane1);
			Control.Children.Add(splitter);
			Control.Children.Add(pane2);

			style = new sw.Style();
			style.Setters.Add(new sw.Setter(sw.FrameworkElement.VerticalAlignmentProperty, sw.VerticalAlignment.Stretch));
			style.Setters.Add(new sw.Setter(sw.FrameworkElement.HorizontalAlignmentProperty, sw.HorizontalAlignment.Stretch));

			UpdateOrientation();
			Control.Loaded += Control_Loaded;
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// controls should be stretched to fit panels
			SetStretch(panel1);
			SetStretch(panel2);
			UpdateColumnSizing(false);

			if (position != null)
			{
				var pos = position.Value;
				if (fixedPanel != SplitterFixedPanel.Panel1)
				{
					var size = GetAvailableSize(false);
					var want = GetAvailableSize(true);
					if (size != want)
					{
						if (FixedPanel == SplitterFixedPanel.Panel2)
							pos += (int)Math.Round(size - want);
						else
						{
							SetRelative(pos / (double)want);
							return;
						}
					}

				}
				SetPosition(pos);
			}
			else if (!double.IsNaN(relative))
			{
				SetRelative(relative);
			}
			else if (fixedPanel == SplitterFixedPanel.Panel1)
			{
				var size1 = panel1.GetPreferredSize(WpfConversions.PositiveInfinitySize);
				SetRelative(orientation == Orientation.Horizontal ? size1.Width : size1.Height);
			}
			else if (fixedPanel == SplitterFixedPanel.Panel2)
			{
				var size2 = panel2.GetPreferredSize(WpfConversions.PositiveInfinitySize);
				SetRelative(orientation == Orientation.Horizontal ? size2.Width : size2.Height);
			}
			else
			{
				var size1 = panel1.GetPreferredSize(WpfConversions.PositiveInfinitySize);
				var size2 = panel2.GetPreferredSize(WpfConversions.PositiveInfinitySize);
				SetRelative(orientation == Orientation.Horizontal
					? size1.Width / (double)(size1.Width + size2.Width)
					: size1.Height / (double)(size1.Height + size2.Height));
			}
		}

		static void SetStretch(Control panel)
		{
			if (panel != null)
			{
				var control = panel.GetContainerControl();
				control.VerticalAlignment = sw.VerticalAlignment.Stretch;
				control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				/*
				((sw.FrameworkElement)panel.ControlObject).Width = double.NaN;
				((sw.FrameworkElement)panel.ControlObject).Height = double.NaN;
				 * */
			}
		}

		void UpdateOrientation()
		{
			if (orientation == Orientation.Horizontal)
			{

				splitter.ResizeDirection = swc.GridResizeDirection.Columns;
				splitter.HorizontalAlignment = sw.HorizontalAlignment.Left;
				splitter.VerticalAlignment = sw.VerticalAlignment.Stretch;

				splitter.SetValue(swc.Grid.RowSpanProperty, 3);
				pane1.SetValue(swc.Grid.RowSpanProperty, 3);
				pane2.SetValue(swc.Grid.RowSpanProperty, 3);

				splitter.SetValue(swc.Grid.ColumnSpanProperty, 1);
				pane1.SetValue(swc.Grid.ColumnSpanProperty, 1);
				pane2.SetValue(swc.Grid.ColumnSpanProperty, 1);

				swc.Grid.SetColumn(splitter, 1);
				swc.Grid.SetRow(splitter, 0);
				swc.Grid.SetColumn(pane2, 2);
				swc.Grid.SetRow(pane2, 0);

				splitter.Width = splitterWidth;
				splitter.Height = double.NaN;
			}
			else
			{
				splitter.ResizeDirection = swc.GridResizeDirection.Rows;
				splitter.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				splitter.VerticalAlignment = sw.VerticalAlignment.Top;
				pane2.VerticalAlignment = sw.VerticalAlignment.Stretch;

				splitter.SetValue(swc.Grid.RowSpanProperty, 1);
				pane1.SetValue(swc.Grid.RowSpanProperty, 1);
				pane2.SetValue(swc.Grid.RowSpanProperty, 1);

				splitter.SetValue(swc.Grid.ColumnSpanProperty, 3);
				pane1.SetValue(swc.Grid.ColumnSpanProperty, 3);
				pane2.SetValue(swc.Grid.ColumnSpanProperty, 3);

				swc.Grid.SetColumn(splitter, 0);
				swc.Grid.SetRow(splitter, 1);
				swc.Grid.SetColumn(pane2, 0);
				swc.Grid.SetRow(pane2, 2);

				splitter.Width = double.NaN;
				splitter.Height = splitterWidth;
			}
			UpdateColumnSizing(position.HasValue || !double.IsNaN(relative));
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		void UpdateColumnSizing(bool updatePosition)
		{
			if (updatePosition && position == null && double.IsNaN(relative))
				UpdateRelative();
			switch (Orientation)
			{
				case Orientation.Horizontal:
					var col1 = Control.ColumnDefinitions[0];
					var col2 = Control.ColumnDefinitions[2];
					switch (fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							col2.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel2:
							col1.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.None:
							col1.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							col2.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
					}
					break;
				case Orientation.Vertical:
					var row1 = Control.RowDefinitions[0];
					var row2 = Control.RowDefinitions[2];
					switch (fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							row2.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel2:
							row1.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.None:
							row1.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							row2.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
					}
					break;
				default:
					throw new NotSupportedException();
			}
			if (updatePosition)
			{
				if (position.HasValue)
					SetPosition(position.Value);
				else if (!double.IsNaN(relative))
					SetRelative(relative);
			}
		}

		public Orientation Orientation
		{
			get
			{
				switch (splitter.ResizeDirection)
				{
					case swc.GridResizeDirection.Columns:
						return Orientation.Horizontal;
					case swc.GridResizeDirection.Rows:
						return Orientation.Vertical;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				orientation = value;
				UpdateOrientation();
			}
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				fixedPanel = value;
				UpdateColumnSizing(Control.IsLoaded);
			}
		}

		public int Position
		{
			get
			{
				if (!Control.IsLoaded)
					return position ?? 0;
				if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
					return (int)Math.Round(Control.ColumnDefinitions[0].ActualWidth);
				return (int)Math.Round(Control.RowDefinitions[0].ActualHeight);
			}
			set
			{
				SetPosition(value);
			}
		}

		public int SplitterWidth
		{
			get { return splitterWidth; }
			set
			{
				if (splitterWidth == value)
					return;
				splitterWidth = value;
				if (orientation == Orientation.Horizontal)
					splitter.Width = value;
				else
					splitter.Height = value;
			}
		}

		double GetAvailableSize()
		{
			return GetAvailableSize(!Control.IsLoaded);
		}
		double GetAvailableSize(bool desired)
		{
			if (desired)
			{
				var size = PreferredSize;
				var pick = Orientation == Orientation.Horizontal ? size.Width : size.Height;
				if (pick >= 0)
					return pick - splitterWidth;
			}
			var width = Orientation == Orientation.Horizontal ? Control.ActualWidth : Control.ActualHeight;
			if (double.IsNaN(width))
				width = Orientation == Orientation.Horizontal ? Control.Width : Control.Height;
			return width - splitterWidth;
		}

		void UpdateRelative()
		{
			var pos = Position;
			if (fixedPanel == SplitterFixedPanel.Panel1)
				relative = pos;
			else
			{
				var sz = GetAvailableSize();
				if (fixedPanel == SplitterFixedPanel.Panel2)
					relative = sz <= 0 ? 0 : sz - pos;
				else
					relative = sz <= 0 ? 0.5 : pos / (double)sz;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(relative))
					UpdateRelative();
				return relative;
			}
			set
			{
				if (relative == value)
					return;
				SetRelative(value);
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		void SetPosition(int newPosition)
		{
			if (!Control.IsLoaded)
			{
				position = newPosition;
				relative = double.NaN;
				if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				{
					Control.ColumnDefinitions[0].Width = new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel);
					Control.ColumnDefinitions[2].Width = new sw.GridLength(1, sw.GridUnitType.Star);
				}
				else
				{
					Control.RowDefinitions[0].Height = new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel);
					Control.RowDefinitions[2].Height = new sw.GridLength(1, sw.GridUnitType.Star);
				}
				return;
			}

			position = null;
			var size = GetAvailableSize(false);
			relative = fixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, newPosition)
				: fixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, size - newPosition)
				: size <= 0 ? 0.5 : Math.Max(0.0, Math.Min(1.0, newPosition / (double)size));
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
			{
				var col1 = Control.ColumnDefinitions[0];
				var col2 = Control.ColumnDefinitions[2];
				if (fixedPanel == SplitterFixedPanel.Panel1)
					col1.Width = new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel);
				else if (fixedPanel == SplitterFixedPanel.Panel2)
					col2.Width = new sw.GridLength(Math.Max(0, size - newPosition), sw.GridUnitType.Pixel);
				else
				{
					col1.Width = new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Star);
					col2.Width = new sw.GridLength(Math.Max(0, size - newPosition), sw.GridUnitType.Star);
				}
			}
			else
			{
				var row1 = Control.RowDefinitions[0];
				var row2 = Control.RowDefinitions[2];
				if (fixedPanel == SplitterFixedPanel.Panel1)
					row1.Height = new sw.GridLength(Math.Max(0,
						newPosition), sw.GridUnitType.Pixel);
				else if (fixedPanel == SplitterFixedPanel.Panel2)
					row2.Height = new sw.GridLength(Math.Max(0,
						size - newPosition), sw.GridUnitType.Pixel);
				else
				{
					row1.Height = new sw.GridLength(Math.Max(0,
						newPosition), sw.GridUnitType.Star);
					row2.Height = new sw.GridLength(Math.Max(0,
						size - newPosition), sw.GridUnitType.Star);
				}
			}
		}
		void SetRelative(double newRelative)
		{
			position = null;
			relative = newRelative;
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
			{
				var col1 = Control.ColumnDefinitions[0];
				var col2 = Control.ColumnDefinitions[2];
				if (fixedPanel == SplitterFixedPanel.Panel1)
					col1.Width = new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Pixel);
				else if (fixedPanel == SplitterFixedPanel.Panel2)
					col2.Width = new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Pixel);
				else
				{
					col1.Width = new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Star);
					col2.Width = new sw.GridLength(Math.Max(0, 1 - relative), sw.GridUnitType.Star);
				}
			}
			else
			{
				var row1 = Control.RowDefinitions[0];
				var row2 = Control.RowDefinitions[2];
				if (fixedPanel == SplitterFixedPanel.Panel1)
					row1.Height = new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Pixel);
				else if (fixedPanel == SplitterFixedPanel.Panel2)
					row2.Height = new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Pixel);
				else
				{
					row1.Height = new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Star);
					row2.Height = new sw.GridLength(Math.Max(0, 1 - relative), sw.GridUnitType.Star);
				}
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			var control = panel1.GetWpfFrameworkElement();
			if (control != null)
				control.SetScale(true, true);
			control = panel2.GetWpfFrameworkElement();
			if (control != null)
				control.SetScale(true, true);
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				panel1 = value;
				pane1.Children.Clear();
				if (panel1 != null)
				{
					var control = panel1.GetWpfFrameworkElement();
					control.ContainerControl.Style = style;
					//swc.DockPanel.SetDock (control, swc.Dock.Top);
					//control.Height = double.NaN;
					SetStretch(panel1);
					if (Widget.Loaded)
						control.SetScale(true, true);
					pane1.Children.Add(control.ContainerControl);
				}
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				panel2 = value;
				pane2.Children.Clear();
				if (panel2 != null)
				{
					var control = panel2.GetWpfFrameworkElement();
					control.ContainerControl.Style = style;
					//swc.DockPanel.SetDock (control, swc.Dock.Top);
					//control.Height = double.NaN;
					SetStretch(panel2);
					if (Widget.Loaded)
						control.SetScale(true, true);
					pane2.Children.Add(control.ContainerControl);
				}
			}
		}

		public override void Remove(sw.FrameworkElement child)
		{
			if (pane1.Children.Contains(child))
			{
				panel1 = null;
				pane1.Children.Remove(child);
			}
			else if (pane2.Children.Contains(child))
			{
				panel2 = null;
				pane2.Children.Remove(child);
			}
		}
	}
}

