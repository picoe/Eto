The ```TableLayout``` is useful to align controls in a grid-like fashion and works similarly to an HTML table.  Rows and columns can be *scaled* to equally share the flexible space of the container. When a row or column is not scaled, it will only take up space necessary for the contained controls in that row and/or column.

To define space around the border of the table, use the ```Padding``` property to specify the top/left/bottom/right padding.  The ```Spacing``` property is used to specify the vertical/horizontal space between each cell.

## Declarative Approach
The ```TableLayout``` allows you to declaratively define a table layout, either via one of the constructors that take a set of ```TableRow``` objects, or via the ```Rows``` property. 
Each ```TableRow``` can take an array of ```TableCell``` objects via its constructor or ```Cells``` property.  Each ```TableCell``` can only contain a single control.

Instead of having to create TableRow and TableCell objects directly, you can specify any ```Control``` class which will convert implicitly.

For example, the following three variations are identical:

```c#
var layout = new TableLayout(new Label(), new TextBox());
	
var layout = new TableLayout { Rows = { new Label(), new TextBox() } };
	
var layout = new TableLayout();
layout.Rows.Add(new TableRow(new TableCell(new Label())));
layout.Rows.Add(new TableRow(new TableCell(new TextBox())));
```
	
## Scaling

Each row or column in a ```TableLayout``` can be scaled so that the controls expand to fit the height or width of the remaining space for the table.  If none are specified, the **last** row or column will automatically be scaled, as all controls must fill the space of its container.

To specify that a _row_ should scale (expand/contract with the height of the form) other than the last row, set `TableRow.ScaleHeight` to `true`:

```c#
new TableRow { ScaleHeight = true, Cells = { myCell, myControl, etc } }
```

To specify that a _column_ should scale (expand/contract with the width of the form) other than the last column, set `TableCell.ScaleWidth` to `true`:

```c#
new TableCell { ScaleWidth = true, Control = myControl }
// or 
new TableCell(myControl, true)
```

To easily define an empty space that scales, you can specify `null` for either a `TableRow` or `TableCell` when defining your table:

```c#
new TableLayout { Rows = { topRow, null, bottomRow } } 
// or
new TableRow { Cells = { controlOnLeft, null, controlOnRight }
```


## Helpers
There are a few helper methods that provide a way to organize your layout:

### ```TableLayout.AutoSized()``` 

This method creates a new table that auto-sizes your control in its container.  This creates extra space to the right and bottom to fill the container.  You can also center the control by setting the centered parameter to true.

Example:

```c#
var layout = new TableLayout(
	new TableRow(
		new Label { Text = "Auto-Sized drop down" },
		TableLayout.AutoSized(new DropDown { Items = { "Item 1", "Item 2" })
	)
);
```

### ```TableLayout.Horizontal()```

Creates a new table with the specified cells in a new row.  This is equivalent to:

```c#
new TableLayout(new TableRow(cells));
```

## Example 1

This example shows how you can create a table with 2 columns and 3 rows

```c#
// This creates a layout like this:
//
//  (auto sized)  (expands to container's width)
//  ----------------------------------------------
// | First Row  | [Text Box                     ] |
// |------------|---------------------------------|
// | Second Row | [                             ] |
// |            | [List Box                     ] |
// |            | [                             ] |
// |------------|---------------------------------|
// | Third Row  | [Drop Down]                     |
// |------------|---------------------------------|
// | <empty>    | <empty>                         | expands to
// |            |                                 | container's
// |            |                                 | height
//  ----------------------------------------------

var layout = new TableLayout
{
	Padding = new Padding(10), // padding around cells
	Spacing = new Size(5, 5), // spacing between each cell
	Rows =
	{
		new TableRow(new Label {Text = "First Row"}, new TextBox()),
		new TableRow(new Label {Text = "Second Row"}, new ListBox()),
		new TableRow(
			new Label {Text = "Third Row"},
			TableLayout.AutoSized(new DropDown {Items = {"Item 1", "Item 2"}})
		),
		null
	}
};
```

Notes:

1. If you do not include the ```null``` as the end, the third row with the ```ListBox``` will expand instead of having blank space at the bottom.
2. This takes advantage of the fact that the last column is automatically scaled. If you want the first column to expand, then you would have to do that explicitly.

## Example 2

This example shows how to scale different rows/columns

```c#
// This creates a layout like this:
//
//  (auto sized)  (expands to container's width)  (auto sized)
//  ---------------------------------------------------------
// | First Row  | [Text Box                     ] | [Button] |
// |------------|---------------------------------|----------|
// | Second Row | [                             ] |          | expands to
// |            | [List Box                     ] |          | container's
// |            | [                             ] |          | height
// |------------|---------------------------------|----------|
// | Third Row  | [Drop Down]                     |          |
//  ---------------------------------------------------------

var layout = new TableLayout
{
	Padding = new Padding(10), // padding around cells
	Spacing = new Size(5, 5), // spacing between each cell
	Rows =
	{
		new TableRow(
			new Label { Text = "First Row" }, 
			new TableCell(new TextBox(), true), 
			new Button { Text = "Button" }
		),
		new TableRow {
			ScaleHeight = true,
			Cells = {
				new Label { Text = "Second Row" }, 
				new ListBox()
			}
		},
		new TableRow(
			new Label { Text = "Third Row" }, 
			TableLayout.AutoSized(new DropDown())
		),
	}
}
```

Here's a breakdown of what the above is doing:

1. `new TableCell(new TextBox(), true)` makes the middle column expand/contract to the available width of the form, by passing `true` to the constructor (which sets `TableCell.ScaleWidth` to `true`).
2. `ScaleHeight = true` makes the 2nd row expand/contract to the available height of the form.
3. `TableLayout.AutoSized(new DropDown())` makes it so the drop down sizes itself to its contents and will not expand to the width of the column it is contained in.