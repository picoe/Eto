Styling allows you to have platform-specific or common code applied to controls.  The framework allows you to easily separate the non-platform specific Eto.Forms code from the platform-specific code, usually in different assemblies.

This also allows you to add custom platform-specific controls to your UI, but it is recommended to create Eto.Forms [[Custom Controls|Custom Platform Controls]] to allow interaction with the platform-specific code.

The style is applied using the Style property for each control.  In the platform-specific assembly/launcher, you then add styles using the static Style.Add() methods.

To make your style apply to all created controls, pass a ``null`` as the style alias.

Note that spaces delimit multiple styles, so your style identifiers should not include a space.

## Example
This example shows how to change the highlight style of a listview for MonoMac/Cocoa apps.  Each time a list box is created with the style of "main-list", it will execute the platform-specific code.

In your Eto.Forms UI code:

```C#
var mylist = new ListBox { Style = "main-list" };
```
In your OS X app (usually called before the application starts):

```C#
Eto.Style.Add<ListBoxHandler>("main-list", handler => {
	handler.Control.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList;
});
```	
	
You can also apply styles for any control without any native code:

```C#
Eto.Style.Add<TableLayout>("padded-table", table => {
	table.Padding = new Padding(10);
	table.Spacing = new Size(5, 5);
});
	
// will automatically get the padding & spacing applied
var myTable = new TableLayout { Style = "padded-table" };
```