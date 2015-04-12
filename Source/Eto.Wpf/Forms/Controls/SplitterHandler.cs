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
		SplitterOrientation orientation;
		SplitterFixedPanel fixedPanel;
		SplitterPositionMode mode;
		int? position;
		int splitterWidth = 5;
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

			bool horiz = Orientation == SplitterOrientation.Horizontal;
			var size = (horiz ? Control.ActualWidth : Control.ActualHeight) - splitterWidth;

			// use preferred size if no position set
			// note: shall we rather use GetPreferredSize(OurSize)?
			if (position == null)
			{
				switch (FixedPanel)
				{
					case SplitterFixedPanel.Panel1:
						var size1 = panel1.GetPreferredSize(Conversions.PositiveInfinitySize);
						SetPosition(horiz ? size1.Width : size1.Height, SplitterPositionMode.Near);
						break;
					case SplitterFixedPanel.Panel2:
						var size2 = panel2.GetPreferredSize(Conversions.PositiveInfinitySize);
						SetPosition(horiz ? size2.Width : size2.Height, SplitterPositionMode.Far);
						break;
					default:
						var sone = panel1.GetPreferredSize(Conversions.PositiveInfinitySize);
						var stwo = panel2.GetPreferredSize(Conversions.PositiveInfinitySize);
						var one = horiz ? sone.Width : sone.Height;
						var two = horiz ? stwo.Width : stwo.Height;
						SetPosition(one * size / (one + two), SplitterPositionMode.Near);
						break;
				}
			}
			else
			{
				double pos = position.Value;
				if (mode != SplitterPositionMode.Percent &&
					(FixedPanel == SplitterFixedPanel.None ||
					(FixedPanel == SplitterFixedPanel.Panel1) != (mode == SplitterPositionMode.Near)))
				{
					var want = (horiz ? PreferredSize.Width : PreferredSize.Height) - splitterWidth;
					var diff = size - want;
					if (diff != 0 && !double.IsNaN(diff))
					{
						if (FixedPanel == SplitterFixedPanel.None)
							pos = pos * size / want;
						else
							pos += mode == SplitterPositionMode.Near ? diff : -diff;
					}
				}
				SetPosition(pos, mode);
			}
		}

		static void SetStretch(Control panel)
		{
			if (panel != null)
			{
				var control = panel.GetContainerControl();
				control.VerticalAlignment = sw.VerticalAlignment.Stretch;
				control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			}
		}

		void UpdateOrientation()
		{
			if (orientation == SplitterOrientation.Horizontal)
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
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		void UpdateColumnSizing(bool updatePosition = true)
		{
			var pos = updatePosition ? Control.IsLoaded ? GetPositon(mode) : position ?? 0 : 0;
			var fix = Control.IsLoaded ? fixedPanel : SplitterFixedPanel.None;
			switch (Orientation)
			{
				case SplitterOrientation.Horizontal:
					var col1 = Control.ColumnDefinitions[0];
					var col2 = Control.ColumnDefinitions[2];
					switch (fix)
					{
						case SplitterFixedPanel.None:
							col1.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							col2.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel1:
							col2.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel2:
							col1.Width = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
					}
					break;
				case SplitterOrientation.Vertical:
					var row1 = Control.RowDefinitions[0];
					var row2 = Control.RowDefinitions[2];
					switch (fix)
					{
						case SplitterFixedPanel.None:
							row1.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							row2.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel1:
							row2.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel2:
							row1.Height = new sw.GridLength(1, sw.GridUnitType.Star);
							break;
					}
					break;
				default:
					throw new NotSupportedException();
			}
			if(updatePosition)
				SetPosition(pos, mode);
		}

		public SplitterOrientation Orientation
		{
			get
			{
				switch (splitter.ResizeDirection)
				{
					case swc.GridResizeDirection.Columns:
						return SplitterOrientation.Horizontal;
					case swc.GridResizeDirection.Rows:
						return SplitterOrientation.Vertical;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				orientation = value;
				UpdateOrientation();
				UpdateColumnSizing();
			}
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				fixedPanel = value;
				if(Control.IsLoaded)
					UpdateColumnSizing();
			}
		}

		public int Position
		{
			get
			{
				if (!Widget.Loaded)
					return position ?? 0;
				return (int)Math.Round(GetPositon(mode));
			}
			set
			{
				if (!Widget.Loaded)
					position = value;
				SetPosition(value, mode);
			}
		}

		public double GetPositon(SplitterPositionMode mode)
		{
			double pos;
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
			{
				if (mode == SplitterPositionMode.Far)
					pos = Control.ColumnDefinitions[2].ActualWidth;
				else
				{
					pos = Control.ColumnDefinitions[0].ActualWidth;
					if (mode == SplitterPositionMode.Percent)
						pos = pos * 100 / (pos + Control.ColumnDefinitions[2].ActualWidth);
				}
			}
			else
			{
				if (mode == SplitterPositionMode.Far)
					pos = Control.RowDefinitions[2].ActualHeight;
				else
				{
					pos = Control.RowDefinitions[0].ActualHeight;
					if (mode == SplitterPositionMode.Percent)
						pos = pos * 100 / (pos + Control.RowDefinitions[2].ActualHeight);
				}
			}
			return pos;
		}

		public void SetPosition(double pos, SplitterPositionMode mode = SplitterPositionMode.Near)
		{
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
			{
				var col1 = Control.ColumnDefinitions[0];
				var col2 = Control.ColumnDefinitions[2];

				if(!Control.IsLoaded)
				{
					if (mode == SplitterPositionMode.Near)
						col1.Width = new sw.GridLength(pos, sw.GridUnitType.Pixel);
					else if (mode == SplitterPositionMode.Far)
						col2.Width = new sw.GridLength(pos, sw.GridUnitType.Pixel);
					return;
				}

				var width = Control.ActualWidth;

				if (mode == SplitterPositionMode.Far)
					pos = width - splitterWidth - pos;
				else if (mode == SplitterPositionMode.Percent)
					pos = (width - splitterWidth) * pos * 0.01;
				if (!(pos >= 0))
					pos = 0.0;

				if (fixedPanel == SplitterFixedPanel.Panel1)
					col1.Width = new sw.GridLength(Math.Max(0,
						pos), sw.GridUnitType.Pixel);
				else if (fixedPanel == SplitterFixedPanel.Panel2)
					col2.Width = new sw.GridLength(Math.Max(0,
						width - splitter.Width - pos), sw.GridUnitType.Pixel);
				else
				{
					col1.Width = new sw.GridLength(Math.Max(0,
						pos), sw.GridUnitType.Star);
					col2.Width = new sw.GridLength(Math.Max(0,
						width - splitter.Width - pos), sw.GridUnitType.Star);
				}
			}
			else
			{
				var row1 = Control.RowDefinitions[0];
				var row2 = Control.RowDefinitions[2];

				if (!Control.IsLoaded)
				{
					if (mode == SplitterPositionMode.Near)
						row1.Height = new sw.GridLength(pos, sw.GridUnitType.Pixel);
					else if (mode == SplitterPositionMode.Far)
						row2.Height = new sw.GridLength(pos, sw.GridUnitType.Pixel);
					return;
				}

				var height = Control.ActualHeight;

				if (mode == SplitterPositionMode.Far)
					pos = height - splitterWidth - pos;
				else if (mode == SplitterPositionMode.Percent)
					pos = (height - splitterWidth) * pos * 0.01;
				if (!(pos >= 0))
					pos = 0.0;

				if (fixedPanel == SplitterFixedPanel.Panel1)
					row1.Height = new sw.GridLength(pos, sw.GridUnitType.Pixel);
				else if (fixedPanel == SplitterFixedPanel.Panel2)
					row2.Height = new sw.GridLength(Math.Max(0,
						height - splitter.Height - pos), sw.GridUnitType.Pixel);
				else
				{
					row1.Height = new sw.GridLength(pos, sw.GridUnitType.Star);
					row2.Height = new sw.GridLength(Math.Max(0,
						height - splitter.Height - pos), sw.GridUnitType.Star);
				}
			}
		}

		public SplitterPositionMode PositionMode
		{
			get { return mode; }
			set {
				mode = value;
				if (!Control.IsLoaded)
					UpdateColumnSizing();
			}
		}

		public int SplitterWidth
		{
			get { return splitterWidth; }
			set {
				if (splitterWidth == value)
					return;
				splitterWidth = value;
				if (orientation == SplitterOrientation.Horizontal)
					splitter.Width = value;
				else
					splitter.Height = value;
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

