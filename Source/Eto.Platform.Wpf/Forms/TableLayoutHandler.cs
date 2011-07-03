using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class TableLayoutHandler : WidgetHandler<System.Windows.Controls.Grid, TableLayout>, ITableLayout
	{
		public void CreateControl (int cols, int rows)
		{
			Control = new System.Windows.Controls.Grid ();
			for (int i = 0; i < cols; i++) Control.ColumnDefinitions.Add (new System.Windows.Controls.ColumnDefinition {
				Width = new System.Windows.GridLength (1, System.Windows.GridUnitType.Auto)
			});
			for (int i = 0; i < rows; i++) Control.RowDefinitions.Add (new System.Windows.Controls.RowDefinition { 
				Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto)
			});
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
			get; set; 
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Generator.Convert (Control.Margin); }
			set { Control.Margin = Generator.Convert (value); }
		}

		public void Update ()
		{
		}

		public void Add (Control child, int x, int y)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			Control.Children.Add (control);
			control.SetValue (System.Windows.Controls.Grid.ColumnProperty, x);
			control.SetValue (System.Windows.Controls.Grid.RowProperty, y);
		}

		public void Move (Control child, int x, int y)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			control.SetValue (System.Windows.Controls.Grid.ColumnProperty, x);
			control.SetValue (System.Windows.Controls.Grid.RowProperty, y);
		}

		public void Remove (Control child)
		{
			var control = (System.Windows.UIElement)child.ControlObject;
			Control.Children.Remove (control);
		}
	}
}
