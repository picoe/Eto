The ``TableLayout`` is the main layout for Eto.Forms to organize your controls.  It is far preferred over using a ``PixelLayout`` as it will accomodate various sizes of each control in all platforms.

A TableLayout is somewhat similar to an HTML table, but with a few differences: 

1. A ``TableCell`` can only contain a single control, however that control can be another container, such as a TableLayout.
2. The width of a column or height of a row is determined by the controls added to the column/row
3. If you set the scaling on a column or row, it will share the remaining space of the TableLayout's container with all other scaled columns/rows equally.


## Simple Table

In this tutorial, we will start from [[Tutorial 1 Hello Eto.Forms]].

This shows how to create a simple table with three columns and three rows.

1. First, create a new TableLayout object.
	```c#
	var layout = new TableLayout();
	```

2. Define the padding around the border of the table, and spacing between the cells of the table.

	```c#
	layout.Spacing = new Size(5, 5); // space between each cell
	layout.Padding = new Padding(10, 10, 10, 10); // space around the table's sides
	```

3. Add a new row with three labels, the first and second column set scaling of the width to true to share the remaining space of the container. The third column will auto size to the controls in the column.

	```C#
	layout.Rows.Add(new TableRow(
		new TableCell(new Label { Text = "First Column" }, true),
		new TableCell(new Label { Text = "Second Column" }, true),
		new Label { Text = "Third Column" }
	));
	```

4. Add another row with some data entry controls

	```C#
	layout.Rows.Add(new TableRow(
		new TextBox { Text = "Some Text" },
		new DropDown { Items = { "Item 1", "Item 2", "Item 3" } },
		new CheckBox { Text = "A checkbox" }
	));
	```

5. Add a final empty row that scales to fill the remaining height

	```C#
	layout.Rows.Add(new TableRow { ScaleHeight = true });
		
	// or, this is the same as doing:
		
	layout.Rows.Add(null);
	```

6. Set the content of the form to use the layout

	```C#
	this.Content = layout;
	```		
		
## Declarative Approach

The same table can be defined declaratively using the C# initializer pattern.
This produces the exact same result but is more succinct and can be easier to read.

```C#
Content = new TableLayout
{
	Spacing = new Size(5, 5), // space between each cell
	Padding = new Padding(10, 10, 10, 10), // space around the table's sides
	Rows =
	{
		new TableRow(
			new TableCell(new Label { Text = "First Column" }, true), 
			new TableCell(new Label { Text = "Second Column" }, true),
			new Label { Text = "Third Column" }
		),
		new TableRow(
			new TextBox { Text = "Some text" },
			new DropDown { Items = { "Item 1", "Item 2", "Item 3" } },
			new CheckBox { Text = "A checkbox" }
		),
		// by default, the last row & column will get scaled. This adds a row at the end to take the extra space of the form.
		// otherwise, the above row will get scaled and stretch the TextBox/ComboBox/CheckBox to fill the remaining height.
		new TableRow { ScaleHeight = true }
	}
};
```

- [C# Source](https://github.com/picoe/Eto/blob/develop/samples/Tutorials/CSharp/Tutorial3/Main.cs)
- [F# Source](https://github.com/picoe/Eto/blob/develop/samples/Tutorials/FSharp/Tutorial3/Program.fs)
