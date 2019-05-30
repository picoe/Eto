using Eto.Drawing;
using Eto.Forms;
using swi = Windows.UI.Xaml.Input;
using swm = Windows.UI.Xaml.Media;
using sw = Windows.UI.Xaml;
//using Eto.WinRT.Forms.Menu;
using Eto.WinRT.Forms.Controls;
//using Eto.WinRT.Forms.Printing;
using Eto.WinRT.Forms;
using Eto.IO;
using Eto.Forms.ThemedControls;
using Eto.Direct2D.Drawing;

namespace Eto.WinRT
{
	/// <summary>
	/// Xaml platform generator.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class Platform : Eto.Platform
	{
		public override string ID { get { return "winrt"; } }

		//static readonly EmbeddedAssemblyLoader embeddedAssemblies = EmbeddedAssemblyLoader.Register("Eto.WinRT.CustomControls.Assemblies");

		public Platform()
		{
			AddTo(this);
		}

		public static void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<Bitmap.IHandler>(() => new BitmapHandler());
			p.Add<FontFamily.IHandler>(() => new FontFamilyHandler());
			p.Add<Font.IHandler>(() => new FontHandler());
			p.Add<Fonts.IHandler>(() => new FontsHandler());
			p.Add<Graphics.IHandler>(() => new GraphicsHandler());
			p.Add<GraphicsPath.IHandler>(() => new GraphicsPathHandler());
			p.Add<Icon.IHandler>(() => new IconHandler());
			p.Add<IndexedBitmap.IHandler>(() => new IndexedBitmapHandler());
			p.Add<Matrix.IHandler>(() => new MatrixHandler());
			p.Add<Pen.IHandler>(() => new PenHandler());
			p.Add<SolidBrush.IHandler>(() => new SolidBrushHandler());
			p.Add<TextureBrush.IHandler>(() => new TextureBrushHandler());
			p.Add<LinearGradientBrush.IHandler>(() => new LinearGradientBrushHandler());
			p.Add<RadialGradientBrush.IHandler>(() => new RadialGradientBrushHandler());

			// Forms.Cells
			//p.Add<CheckBoxCell.IHandler>(() => new CheckBoxCellHandler());
			//p.Add<ComboBoxCell.IHandler>(() => new ComboBoxCellHandler());
			//p.Add<ImageTextCell.IHandler>(() => new ImageTextCellHandler());
			//p.Add<ImageViewCell.IHandler>(() => new ImageViewCellHandler());
			//p.Add<TextBoxCell.IHandler>(() => new TextBoxCellHandler());
			
			// Forms.Controls
			p.Add<Button.IHandler>(() => new ButtonHandler());
			p.Add<CheckBox.IHandler>(() => new CheckBoxHandler());
			p.Add<DropDown.IHandler>(() => new DropDownHandler());
			p.Add<ComboBox.IHandler>(() => new ComboBoxHandler());
			p.Add<DateTimePicker.IHandler>(() => new DateTimePickerHandler());
			p.Add<Drawable.IHandler>(() => new DrawableHandler());
			//p.Add<GridColumn.IHandler>(() => new GridColumnHandler());
			//p.Add<GridView.IHandler>(() => new GridViewHandler());
			//p.Add<GroupBox.IHandler>(() => new GroupBoxHandler());
			//p.Add<ImageView.IHandler>(() => new ImageViewHandler());
			p.Add<Label.IHandler>(() => new LabelHandler());
			//p.Add<ListBox.IHandler>(() => new ListBoxHandler());
			p.Add<NumericStepper.IHandler>(() => new NumericStepperHandler());
			p.Add<Panel.IHandler>(() => new PanelHandler());
			p.Add<PasswordBox.IHandler>(() => new PasswordBoxHandler());
			p.Add<ProgressBar.IHandler>(() => new ProgressBarHandler());
			p.Add<RadioButton.IHandler>(() => new RadioButtonHandler());
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
			p.Add<Slider.IHandler>(() => new SliderHandler());
			p.Add<Spinner.IHandler>(() => new ThemedSpinnerHandler());
			//p.Add<Splitter.IHandler>(() => new SplitterHandler());
			//p.Add<TabControl.IHandler>(() => new TabControlHandler());
			//p.Add<TabPage.IHandler>(() => new TabPageHandler());
			p.Add<TextArea.IHandler>(() => new TextAreaHandler());
			p.Add<TextBox.IHandler>(() => new TextBoxHandler());
			//p.Add<TreeGridView.IHandler>(() => new TreeGridViewHandler());
			p.Add<TreeView.IHandler>(() => new TreeViewHandler());
			p.Add<WebView.IHandler>(() => new WebViewHandler ());
			//p.Add<Screens.IHandler>(() => new ScreensHandler());

			// Forms.Menu
			//p.Add<CheckMenuItem.IHandler>(() => new CheckMenuItemHandler());
			//p.Add<ContextMenu.IHandler>(() => new ContextMenuHandler());
			//p.Add<ButtonMenuItem.IHandler>(() => new ButtonMenuItemHandler());
			//p.Add<MenuBar.IHandler>(() => new MenuBarHandler());
			//p.Add<RadioMenuItem.IHandler>(() => new RadioMenuItemHandler());
			//p.Add<SeparatorMenuItem.IHandler>(() => new SeparatorMenuItemHandler());
			
			// Forms.Printing
			//p.Add<PrintDialog.IHandler>(() => new PrintDialogHandler());
			//p.Add<PrintDocument.IHandler>(() => new PrintDocumentHandler());
			//p.Add<PrintSettings.IHandler>(() => new PrintSettingsHandler());
			
			// Forms.ToolBar
			p.Add<CheckToolItem.IHandler>(() => new CheckToolItemHandler());
			p.Add<SeparatorToolItem.IHandler>(() => new SeparatorToolItemHandler());
			p.Add<ButtonToolItem.IHandler>(() => new ButtonToolItemHandler());
			//p.Add<ToolBar.IHandler>(() => new ToolBarHandler());
			
			// Forms
			p.Add<Application.IHandler>(() => new ApplicationHandler());
			//p.Add<Clipboard.IHandler>(() => new ClipboardHandler());
			//p.Add<ColorDialog.IHandler>(() => new ColorDialogHandler());
			//p.Add<Cursor.IHandler>(() => new CursorHandler());
			//p.Add<Dialog.IHandler>(() => new DialogHandler());
			//p.Add<FontDialog.IHandler>(() => new FontDialogHandler());
			//p.Add<Form.IHandler>(() => new FormHandler());
			p.Add<MessageBox.IHandler>(() => new MessageBoxHandler());
			//p.Add<OpenFileDialog.IHandler>(() => new OpenFileDialogHandler());
			p.Add<PixelLayout.IHandler>(() => new PixelLayoutHandler());
			//p.Add<SaveFileDialog.IHandler>(() => new SaveFileDialogHandler());
			//p.Add<SelectFolderDialog.IHandler>(() => new SelectFolderDialogHandler());
			p.Add<TableLayout.IHandler>(() => new TableLayoutHandler());
			//p.Add<UITimer.IHandler>(() => new UITimerHandler());
			//p.Add<Mouse.IHandler>(() => new MouseHandler());
			//
			// IO
			//p.Add<SystemIcons.IHandler>(() => new SystemIconsHandler());
			
			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
		}
	}
}
