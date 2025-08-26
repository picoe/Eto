Eto.Forms comes with a standard set of controls.  Controls can be subclassed to override or implement behaviour.  Each control has an implementation for each supported platform, which can be customized or replaced entirely if a particular implementation is not suitable for your use.

Custom controls can either be composed entirely of Eto.Forms controls, which would work across all platforms.  If you wish to create a more complex control, or use functionality of a platform that isn't exposed in Eto.Forms, then you can create your own custom controls with implementations for each platform (see [[Custom Platform Controls]]).

The standard controls are:

* [Button](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Button.htm) - Standard button with text
* [Calendar](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Calendar.htm) - Calendar control to pick a date or range of dates
* [CheckBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_CheckBox.htm) - Check box with a label
* [ColorPicker](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ColorPicker.htm) - Pick a single color value
* [ComboBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ComboBox.htm) - Text entry with a drop down list of items
* [DateTimePicker](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_DateTimePicker.htm) - Control to enter a date and/or time
* [Drawable](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Drawable.htm) - Owner-drawn control using [Graphics](https://pages.picoe.ca/docs/api/html/T_Eto_Drawing_Graphics.htm) object
* [DropDown](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_DropDown.htm) - Drop down with a list of items
* [EnumDropDown](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_EnumDropDown_1.htm) - A simple way to create a drop down with values from an Enumeration
* [EnumRadioButtonList](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_EnumRadioButtonList_1.htm) - Manages a list of radio buttons from an Enumeration
* [[GridView]] - A virtualized grid of data with editable cells
* [GroupBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_GroupBox.htm) - A panel with a border and optional title
* [ImageView](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ImageView.htm) - A view to display a single image
* [Label](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Label.htm) - Displays text
* [LinkButton](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_LinkButton.htm) - A simple label that acts like a button, similar to a hyperlink
* [ListBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ListBox.htm) - A scrollable list of items
* [ListControl](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ListControl.htm) - Base for ListBox, DropDown, ComboBox, and other list-type controls
* [MaskedTextBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_MaskedTextBox.htm) - TextBox for variable or fixed length masks
* [Navigation](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Navigation.htm) - (mobile) - A pane that can present multiple pages
* [NumericUpDown](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_NumericUpDown.htm) - Numeric control that allows the user to adjust the value with the mouse
* [Panel](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Panel.htm) - A blank panel container to add other controls
* [PasswordBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_PasswordBox.htm) - Enter passwords or sensitive data
* [ProgressBar](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ProgressBar.htm) - Show progress of long running tasks
* [RadioButton](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_RadioButton.htm) - Used in a group of radio buttons to allow user to select from values
* [RadioButtonList](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_RadioButtonList.htm) - Manages a list of radio buttons
* [RichTextArea](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_RichTextArea.htm) - Multi-line text area with rich text formatting
* [Scrollable](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Scrollable.htm) - A scrollable container
* [SearchBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_SearchBox.htm) - A text box with search-box functionality
* [Slider](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Slider.htm) - A horizontal or vertical slider to select a value from a range
* [Spinner](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Spinner.htm) - A spinner to show indeterminate progress in compact space
* [Splitter](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Splitter.htm) - Splits two panes horizontally or vertically
* [TabControl](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TabControl.htm) - Presents multiple TabPage containers which the user can select
* [TabPage](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TabPage.htm) - A single page of a TabControl
* [TextArea](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TextArea.htm) - Multi-line text control with scrollbars
* [TextBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TextBox.htm) - Single line text input
* [TextControl](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TextControl.htm) - Base for any control that contains text
* [TreeView](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TreeView.htm) - A control to present nodes in a tree
* [TreeGridView](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_TreeGridView.htm) - A TreeView with columns
* [WebView](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_WebView.htm) - Control to present a web page through a url or static HTML

### Windows

* [Form](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Form.htm) - A modeless window
* [Dialog](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Dialog.htm) - A modal dialog with no result value
* [Dialog&lt;T&gt;](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_Dialog_1.htm) - A modal dialog with a custom result value

### Dialogs

* [ColorDialog](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_ColorDialog.htm) - Select a color
* [FontDialog](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_FontDialog.htm) - Choose a font and style
* [OpenFileDialog](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_OpenFileDialog.htm) - Open an existing file or files matching a set of patterns
* [SaveFileDialog](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_SaveFileDialog.htm) - Choose a file and format to save to
* [SelectFolderDialog](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_SelectFolderDialog.htm) - Select a folder
* [MessageBox](https://pages.picoe.ca/docs/api/html/T_Eto_Forms_MessageBox.htm) - Show a standard message box with specific buttons

