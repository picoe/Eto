using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class SplitterHandler : WpfPanel<System.Windows.Controls.Grid, Splitter>, ISplitter
	{
		System.Windows.Controls.GridSplitter splitter;
		System.Windows.Controls.DockPanel pane1;
		System.Windows.Controls.DockPanel pane2;
		SplitterOrientation orientation;

		Control panel1;
		Control panel2;

		public SplitterHandler ()
		{
			Control = new System.Windows.Controls.Grid ();
			Control.ColumnDefinitions.Add (new System.Windows.Controls.ColumnDefinition ());
			Control.ColumnDefinitions.Add (new System.Windows.Controls.ColumnDefinition ());
			Control.RowDefinitions.Add (new System.Windows.Controls.RowDefinition ());
			Control.RowDefinitions.Add (new System.Windows.Controls.RowDefinition ());

			splitter = new System.Windows.Controls.GridSplitter ();
			pane1 = new System.Windows.Controls.DockPanel ();
			pane2 = new System.Windows.Controls.DockPanel ();

			Control.Children.Add (pane1);
			Control.Children.Add (splitter);
			Control.Children.Add (pane2);

			UpdateOrientation ();
		}

		void UpdateOrientation ()
		{
			if (orientation == SplitterOrientation.Horizontal) {

				splitter.ResizeDirection = System.Windows.Controls.GridResizeDirection.Columns;
				splitter.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
				splitter.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

				splitter.SetValue (System.Windows.Controls.Grid.RowSpanProperty, 2);
				pane1.SetValue (System.Windows.Controls.Grid.RowSpanProperty, 2);
				pane2.SetValue (System.Windows.Controls.Grid.RowSpanProperty, 2);

				splitter.SetValue (System.Windows.Controls.Grid.ColumnSpanProperty, 1);
				pane1.SetValue (System.Windows.Controls.Grid.ColumnSpanProperty, 1);
				pane2.SetValue (System.Windows.Controls.Grid.ColumnSpanProperty, 1);

				pane2.SetValue (System.Windows.Controls.Grid.ColumnProperty, 1);
				pane2.SetValue (System.Windows.Controls.Grid.RowProperty, 0);
				splitter.Width = 6;
				splitter.Height = double.NaN;
			}
			else {
				splitter.ResizeDirection = System.Windows.Controls.GridResizeDirection.Rows;
				splitter.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
				splitter.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

				splitter.SetValue (System.Windows.Controls.Grid.RowSpanProperty, 1);
				pane1.SetValue (System.Windows.Controls.Grid.RowSpanProperty, 1);
				pane2.SetValue (System.Windows.Controls.Grid.RowSpanProperty, 1);

				splitter.SetValue (System.Windows.Controls.Grid.ColumnSpanProperty, 2);
				pane1.SetValue (System.Windows.Controls.Grid.ColumnSpanProperty, 2);
				pane2.SetValue (System.Windows.Controls.Grid.ColumnSpanProperty, 2);

				pane2.SetValue (System.Windows.Controls.Grid.ColumnProperty, 0);
				pane2.SetValue (System.Windows.Controls.Grid.RowProperty, 1);
				splitter.Width = double.NaN;
				splitter.Height = 6;
			}
		}

		public SplitterOrientation Orientation {
			get {
				switch (splitter.ResizeDirection) {
					case System.Windows.Controls.GridResizeDirection.Columns:
						return SplitterOrientation.Horizontal;
					case System.Windows.Controls.GridResizeDirection.Rows:
						return SplitterOrientation.Vertical;
					default:
						throw new NotSupportedException ();
				}
			}
			set {
				orientation = value;
				UpdateOrientation ();
			}
		}

		public int Position {
			get {
				if (splitter.ResizeDirection == System.Windows.Controls.GridResizeDirection.Columns)
					return (int)Control.ColumnDefinitions[0].Width.Value;
				else
					return (int)Control.RowDefinitions[0].Height.Value;
			}
			set {
				if (splitter.ResizeDirection == System.Windows.Controls.GridResizeDirection.Columns)
					Control.ColumnDefinitions[0].Width = new System.Windows.GridLength (value);
				else
					Control.RowDefinitions[0].Height = new System.Windows.GridLength (value);
			}
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				panel1 = value;
				pane1.Children.Clear ();
				if (panel1 != null) {
					pane1.Children.Add ((System.Windows.UIElement)panel1.ControlObject);
				}
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				panel2 = value;
				pane2.Children.Clear ();
				if (panel2 != null) {
					pane2.Children.Add ((System.Windows.UIElement)panel2.ControlObject);
				}
			}
		}

	}
}

