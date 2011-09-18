using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using WC = System.Windows.Controls;
using W = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class TableLayoutHandler : WpfLayout<WC.Grid, TableLayout>, ITableLayout
	{
		Eto.Drawing.Size spacing;
		W.Style itemStyle;
		public void CreateControl (int cols, int rows)
		{
			Control = new WC.Grid ();
			//Control.Background = System.Windows.Media.Brushes.Blue;
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add (new WC.ColumnDefinition {
				Width = new System.Windows.GridLength (1, i == cols - 1 ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto)
			});
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add (new WC.RowDefinition {
				Height = new System.Windows.GridLength (1, i == rows - 1 ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto)
			});
			Spacing = TableLayout.DefaultSpacing;
			Padding = TableLayout.DefaultPadding;
		}

		public void SetColumnScale (int column, bool scale)
		{
			Control.ColumnDefinitions[column].Width = new System.Windows.GridLength (1, scale ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto);
		}

		public void SetRowScale (int row, bool scale)
		{
			Control.RowDefinitions[row].Height = new System.Windows.GridLength (1, scale ? System.Windows.GridUnitType.Star : System.Windows.GridUnitType.Auto);
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
				if (itemStyle == null) {
					itemStyle = new W.Style (typeof (W.FrameworkElement));
				}
				itemStyle.Setters.Clear ();
				itemStyle.Setters.Add (new W.Setter (W.FrameworkElement.MarginProperty, new W.Thickness (0, 0, value.Width, value.Height)));
				itemStyle.Setters.Add (new W.Setter (W.FrameworkElement.HorizontalAlignmentProperty, W.HorizontalAlignment.Stretch));
			}
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Generator.Convert (Control.Margin); }
			set { Control.Margin = Generator.Convert (value); }
		}

		public void Add (Control child, int x, int y)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			var c = control as W.FrameworkElement;
			if (c != null) c.Style = itemStyle;
			control.SetValue (WC.Grid.ColumnProperty, x);
			control.SetValue (WC.Grid.RowProperty, y);
			Control.Children.Add (control);
		}

		public void Move (Control child, int x, int y)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			control.SetValue (WC.Grid.ColumnProperty, x);
			control.SetValue (WC.Grid.RowProperty, y);
		}

		public void Remove (Control child)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			Control.Children.Remove (control);
		}
	}
}
