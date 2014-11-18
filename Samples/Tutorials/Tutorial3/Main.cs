using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial3
{
	public class MyForm : Form
	{
		public MyForm()
		{
			ClientSize = new Size(600, 400);
			Title = "Table Layout";

			// The main layout mechanism for Eto.Forms is a TableLayout.
			// This is recommended to allow controls to keep their natural platform-specific size.
			// You can layout your controls declaratively using rows and columns as below, or add to the TableLayout.Rows and TableRow.Cell directly.

			Content = new TableLayout(
				new TableRow(
					new TableCell { Control = new Label { Text = "First Column" }, ScaleWidth = true }, // Scale width to share space among all columns
					new TableCell { Control = new Label { Text = "Second Column" }, ScaleWidth = true },
					new TableCell { Control = new Label { Text = "Third Column" }, ScaleWidth = true }
				),
				new TableRow(
				// A Control can be implicitly converted to a TableCell or TableRow, so we can omit creating a TableCell when we don't have to set scaling
					new TextBox { Text = "Second Row, First Column" },
					new ComboBox { DataStore = new ListItemCollection { new ListItem { Text = "Second Row, Second Column" } } },
					new CheckBox { Text = "Second Row, Third Column" }
				),
				// by default, the last row & column will get scaled. This adds a row at the end to take the extra space of the form.
				// otherwise, the above row will get scaled and stretch the TextBox/ComboBox/CheckBox to fill the remaining height.
				new TableRow { ScaleHeight = true }
			)
			{
				Spacing = new Size(5, 5), // space between each cell
				Padding = new Padding(10, 10, 10, 10) // space around the table's sides
			};

			// This creates the following layout:
			//  --------------------------------
			// |First     |Second    |Third     |
			//  --------------------------------
			// |<TextBox> |<ComboBox>|<CheckBox>|
			//  --------------------------------
			// |          |          |          |
			// |          |          |          |
			//  --------------------------------
			//
			// Some notes:
			//  1. When scaling the width of a cell, it applies to all cells in the same column.
			//  2. When scaling the height of a row, it applies to the entire row.
			//  3. Scaling a row/column makes it share all remaining space with other scaled rows/columns.
			//  4. If a row/column is not scaled, it will be the size of the largest control in that row/column.
			//  5. A Control can be implicitly converted to a TableCell or TableRow to make the layout more concise.

			Menu = new MenuBar
			{
				QuitItem = new Command((sender, e) => Application.Instance.Quit())
				{
					MenuText = "Quit",
					Shortcut = Application.Instance.CommonModifier | Keys.Q
				}
			};
		}
	}

	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application().Run(new MyForm());
		}
	}
}
