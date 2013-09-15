using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swm = System.Windows.Media;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class SplitterHandler : WpfContainer<swc.Grid, Splitter>, ISplitter
	{
		swc.GridSplitter splitter;
		swc.DockPanel pane1;
		swc.DockPanel pane2;
		SplitterOrientation orientation;
		SplitterFixedPanel fixedPanel;
		int? position;
		int? initialWidth;
		int? initialHeight;
		sw.Style style;

		Control panel1;
		Control panel2;

		public SplitterHandler ()
		{
			Control = new swc.Grid ();
			Control.ColumnDefinitions.Add (new swc.ColumnDefinition ());
			Control.ColumnDefinitions.Add (new swc.ColumnDefinition { Width = sw.GridLength.Auto });
			Control.ColumnDefinitions.Add (new swc.ColumnDefinition ());
			Control.RowDefinitions.Add (new swc.RowDefinition ());
			Control.RowDefinitions.Add (new swc.RowDefinition { Height = sw.GridLength.Auto });
			Control.RowDefinitions.Add (new swc.RowDefinition ());

			splitter = new swc.GridSplitter {
				//Background = sw.SystemColors.ControlLightLightBrush,
				ResizeBehavior = swc.GridResizeBehavior.PreviousAndNext
			};
			pane1 = new swc.DockPanel { LastChildFill = true };
			pane2 = new swc.DockPanel { LastChildFill = true };


			Control.Children.Add (pane1);
			Control.Children.Add (splitter);
			Control.Children.Add (pane2);

			style = new sw.Style ();
			style.Setters.Add (new sw.Setter (sw.FrameworkElement.VerticalAlignmentProperty, sw.VerticalAlignment.Stretch));
			style.Setters.Add (new sw.Setter (sw.FrameworkElement.HorizontalAlignmentProperty, sw.HorizontalAlignment.Stretch));


			UpdateOrientation ();
			Control.Loaded += delegate {
				// controls should be stretched to fit panels
				SetStretch (panel1);
				SetStretch (panel2);
				UpdateColumnSizing ();
				if (FixedPanel == SplitterFixedPanel.Panel2) {
					if (Orientation == SplitterOrientation.Horizontal && initialWidth != null && initialWidth.Value > 0)
						Position = this.Size.Width - (initialWidth.Value - position.Value);
					else if (Orientation == SplitterOrientation.Vertical && initialHeight != null && initialHeight.Value > 0)
						Position = this.Size.Height - (initialHeight.Value - position.Value);
					else if (position != null)
						Position = position.Value;
				}
				else if (position != null)
					Position = position.Value;
			};
		}

		void SetStretch (Control panel)
		{
			if (panel != null)
			{
				var control = panel.GetContainerControl ();
				control.VerticalAlignment = sw.VerticalAlignment.Stretch;
				control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				/*
				((sw.FrameworkElement)panel.ControlObject).Width = double.NaN;
				((sw.FrameworkElement)panel.ControlObject).Height = double.NaN;
				 * */
			}
		}

		void UpdateOrientation ()
		{
			if (orientation == SplitterOrientation.Horizontal) {

				splitter.ResizeDirection = swc.GridResizeDirection.Columns;
				splitter.HorizontalAlignment = sw.HorizontalAlignment.Left;
				splitter.VerticalAlignment = sw.VerticalAlignment.Stretch;

				splitter.SetValue (swc.Grid.RowSpanProperty, 3);
				pane1.SetValue (swc.Grid.RowSpanProperty, 3);
				pane2.SetValue (swc.Grid.RowSpanProperty, 3);

				splitter.SetValue (swc.Grid.ColumnSpanProperty, 1);
				pane1.SetValue (swc.Grid.ColumnSpanProperty, 1);
				pane2.SetValue (swc.Grid.ColumnSpanProperty, 1);

				swc.Grid.SetColumn (splitter, 1);
				swc.Grid.SetRow (splitter, 0);
				swc.Grid.SetColumn (pane2, 2);
				swc.Grid.SetRow (pane2, 0);

				splitter.Width = 5;
				splitter.Height = double.NaN;
			}
			else {
				splitter.ResizeDirection = swc.GridResizeDirection.Rows;
				splitter.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				splitter.VerticalAlignment = sw.VerticalAlignment.Top;
				pane2.VerticalAlignment = sw.VerticalAlignment.Stretch;

				splitter.SetValue (swc.Grid.RowSpanProperty, 1);
				pane1.SetValue (swc.Grid.RowSpanProperty, 1);
				pane2.SetValue (swc.Grid.RowSpanProperty, 1);

				splitter.SetValue (swc.Grid.ColumnSpanProperty, 3);
				pane1.SetValue (swc.Grid.ColumnSpanProperty, 3);
				pane2.SetValue (swc.Grid.ColumnSpanProperty, 3);

				swc.Grid.SetColumn (splitter, 0);
				swc.Grid.SetRow (splitter, 1);
				swc.Grid.SetColumn (pane2, 0);
				swc.Grid.SetRow (pane2, 2);

				splitter.Width = double.NaN;
				splitter.Height = 5;
			}
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		void UpdateColumnSizing ()
		{
			var pos = this.Position;
			switch (Orientation) {
				case SplitterOrientation.Horizontal:
					var col1 = Control.ColumnDefinitions[0];
					var col2 = Control.ColumnDefinitions[2];
					switch (fixedPanel) {
						case SplitterFixedPanel.None:
							col1.Width = new sw.GridLength (1, sw.GridUnitType.Star);
							col2.Width = new sw.GridLength (1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel1:
							col2.Width = new sw.GridLength (1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel2:
							col1.Width = new sw.GridLength (1, sw.GridUnitType.Star);
							break;
					}
					break;
				case SplitterOrientation.Vertical:
					var row1 = Control.RowDefinitions[0];
					var row2 = Control.RowDefinitions[2];
					switch (fixedPanel) {
						case SplitterFixedPanel.None:
							row1.Height = new sw.GridLength (1, sw.GridUnitType.Star);
							row2.Height = new sw.GridLength (1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel1:
							row2.Height = new sw.GridLength (1, sw.GridUnitType.Star);
							break;
						case SplitterFixedPanel.Panel2:
							row1.Height = new sw.GridLength (1, sw.GridUnitType.Star);
							break;
					}
					break;
				default:
					throw new NotSupportedException ();
			}
			Position = pos;
		}

		public SplitterOrientation Orientation {
			get {
				switch (splitter.ResizeDirection) {
					case swc.GridResizeDirection.Columns:
						return SplitterOrientation.Horizontal;
					case swc.GridResizeDirection.Rows:
						return SplitterOrientation.Vertical;
					default:
						throw new NotSupportedException ();
				}
			}
			set {
				orientation = value;
				UpdateOrientation ();
				UpdateColumnSizing ();
			}
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set	{
				fixedPanel = value;
				UpdateColumnSizing ();
			}
		}

		public int Position
		{
			get
			{
				if (!Widget.Loaded)
					return position ?? 0;
				if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
					return (int)Control.ColumnDefinitions[0].ActualWidth;
				else
					return (int)Control.RowDefinitions[0].ActualHeight;
			}
			set
			{
				if (!Widget.Loaded) {
					position = value;
					initialWidth = this.Size.Width;
					initialHeight = this.Size.Height;
					return;
				}
				if (splitter.ResizeDirection == swc.GridResizeDirection.Columns) {
					var col1 = Control.ColumnDefinitions[0];
					var col2 = Control.ColumnDefinitions[2];
					var controlWidth = Control.IsLoaded ? Control.ActualWidth : Control.Width;
					switch (fixedPanel) {
					case SplitterFixedPanel.None:
						var scale = (col1.Width.Value + col2.Width.Value) / controlWidth;
						col1.Width = new sw.GridLength (value * scale, sw.GridUnitType.Star);
						break;
					case SplitterFixedPanel.Panel1:
						col1.Width = new sw.GridLength (value, sw.GridUnitType.Pixel);
						break;
					case SplitterFixedPanel.Panel2:
						col2.Width = new sw.GridLength (controlWidth - value, sw.GridUnitType.Pixel);
						break;
					}
				}
				else {
					var row1 = Control.RowDefinitions[0];
					var row2 = Control.RowDefinitions[2];
					var controlHeight = Control.IsLoaded ? Control.ActualHeight : Control.Height;
					switch (fixedPanel) {
					case SplitterFixedPanel.None:
						var scale = (row1.Height.Value + row2.Height.Value) / controlHeight;
						row1.Height = new sw.GridLength (value * scale, sw.GridUnitType.Star);
						break;
					case SplitterFixedPanel.Panel1:
						row1.Height = new sw.GridLength (value, sw.GridUnitType.Pixel);
						break;
					case SplitterFixedPanel.Panel2:
						row2.Height = new sw.GridLength (controlHeight - value, sw.GridUnitType.Pixel);
						break;
					}
				}
			}
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				panel1 = value;
				pane1.Children.Clear ();
				if (panel1 != null) {
					var control = panel1.GetContainerControl();
					control.Style = style;
					//swc.DockPanel.SetDock (control, swc.Dock.Top);
					//control.Height = double.NaN;
					SetStretch (panel1);
					pane1.Children.Add (control);
				}
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				panel2 = value;
				pane2.Children.Clear ();
				if (panel2 != null) {
					var control = panel2.GetContainerControl();
					control.Style = style;
					//swc.DockPanel.SetDock (control, swc.Dock.Top);
					//control.Height = double.NaN;
					SetStretch (panel2);
					pane2.Children.Add (control);
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

