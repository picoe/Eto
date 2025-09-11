# Controls

Eto.Forms comes with a standard set of controls.  Controls can be subclassed to override or implement behaviour.  Each control has an implementation for each supported platform, which can be customized or replaced entirely if a particular implementation is not suitable for your use.

Custom controls can either be composed entirely of Eto.Forms controls, which would work across all platforms.  If you wish to create a more complex control, or use functionality of a platform that isn't exposed in Eto.Forms, then you can create your own custom controls with implementations for each platform (see [[Custom Platform Controls]]).

The standard controls are:

* [Button](../../api/Eto.Forms.Button.yml) - Standard button with text
* [Calendar](../../api/Eto.Forms.Calendar.yml) - Calendar control to pick a date or range of dates
* [CheckBox](../../api/Eto.Forms.CheckBox.yml) - Check box with a label
* [ColorPicker](../../api/Eto.Forms.ColorPicker.yml) - Pick a single color value
* [ComboBox](../../api/Eto.Forms.ComboBox.yml) - Text entry with a drop down list of items
* [DateTimePicker](../../api/Eto.Forms.DateTimePicker.yml) - Control to enter a date and/or time
* [Drawable](../../api/Eto.Forms.Drawable.yml) - Owner-drawn control using [Graphics](../../api/Eto.Drawing.Graphics.yml) object
* [DropDown](../../api/Eto.Forms.DropDown.yml) - Drop down with a list of items
* [EnumDropDown](../../api/Eto.Forms.EnumDropDown-1.yml) - A simple way to create a drop down with values from an Enumeration
* [EnumRadioButtonList](../../api/Eto.Forms.EnumRadioButtonList-1.yml) - Manages a list of radio buttons from an Enumeration
* [GridView](./GridView.md) - A virtualized grid of data with editable cells
* [GroupBox](../../api/Eto.Forms.GroupBox.yml) - A panel with a border and optional title
* [ImageView](../../api/Eto.Forms.ImageView.yml) - A view to display a single image
* [Label](../../api/Eto.Forms.Label.yml) - Displays text
* [LinkButton](../../api/Eto.Forms.LinkButton.yml) - A simple label that acts like a button, similar to a hyperlink
* [ListBox](../../api/Eto.Forms.ListBox.yml) - A scrollable list of items
* [ListControl](../../api/Eto.Forms.ListControl.yml) - Base for ListBox, DropDown, ComboBox, and other list-type controls
* [MaskedTextBox](../../api/Eto.Forms.MaskedTextBox.yml) - TextBox for variable or fixed length masks
* [Navigation](../../api/Eto.Forms.Navigation.yml) - (mobile) - A pane that can present multiple pages
* [NumericUpDown](../../api/Eto.Forms.NumericUpDown.yml) - Numeric control that allows the user to adjust the value with the mouse
* [Panel](../../api/Eto.Forms.Panel.yml) - A blank panel container to add other controls
* [PasswordBox](../../api/Eto.Forms.PasswordBox.yml) - Enter passwords or sensitive data
* [ProgressBar](../../api/Eto.Forms.ProgressBar.yml) - Show progress of long running tasks
* [RadioButton](../../api/Eto.Forms.RadioButton.yml) - Used in a group of radio buttons to allow user to select from values
* [RadioButtonList](../../api/Eto.Forms.RadioButtonList.yml) - Manages a list of radio buttons
* [RichTextArea](../../api/Eto.Forms.RichTextArea.yml) - Multi-line text area with rich text formatting
* [Scrollable](../../api/Eto.Forms.Scrollable.yml) - A scrollable container
* [SearchBox](../../api/Eto.Forms.SearchBox.yml) - A text box with search-box functionality
* [Slider](../../api/Eto.Forms.Slider.yml) - A horizontal or vertical slider to select a value from a range
* [Spinner](../../api/Eto.Forms.Spinner.yml) - A spinner to show indeterminate progress in compact space
* [Splitter](../../api/Eto.Forms.Splitter.yml) - Splits two panes horizontally or vertically
* [TabControl](../../api/Eto.Forms.TabControl.yml) - Presents multiple TabPage containers which the user can select
* [TabPage](../../api/Eto.Forms.TabPage.yml) - A single page of a TabControl
* [TextArea](../../api/Eto.Forms.TextArea.yml) - Multi-line text control with scrollbars
* [TextBox](../../api/Eto.Forms.TextBox.yml) - Single line text input
* [TextControl](../../api/Eto.Forms.TextControl.yml) - Base for any control that contains text
* [TreeView](../../api/Eto.Forms.TreeView.yml) - A control to present nodes in a tree
* [TreeGridView](../../api/Eto.Forms.TreeGridView.yml) - A TreeView with columns
* [WebView](../../api/Eto.Forms.WebView.yml) - Control to present a web page through a url or static HTML

## Windows

* [Form](../../api/Eto.Forms.Form.yml) - A modeless window
* [Dialog](../../api/Eto.Forms.Dialog.yml) - A modal dialog with no result value
* [Dialog&lt;T&gt;](../../api/Eto.Forms.Dialog-1.yml) - A modal dialog with a custom result value

## Dialogs

* [ColorDialog](../../api/Eto.Forms.ColorDialog.yml) - Select a color
* [FontDialog](../../api/Eto.Forms.FontDialog.yml) - Choose a font and style
* [OpenFileDialog](../../api/Eto.Forms.OpenFileDialog.yml) - Open an existing file or files matching a set of patterns
* [SaveFileDialog](../../api/Eto.Forms.SaveFileDialog.yml) - Choose a file and format to save to
* [SelectFolderDialog](../../api/Eto.Forms.SelectFolderDialog.yml) - Select a folder
* [MessageBox](../../api/Eto.Forms.MessageBox.yml) - Show a standard message box with specific buttons
