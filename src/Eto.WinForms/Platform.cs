using Eto.Forms;
using Eto.Drawing;
using Eto.IO;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.WinForms.Drawing;
using Eto.WinForms.Forms;
using Eto.WinForms.Forms.Printing;
using Eto.WinForms.Forms.Controls;
using Eto.WinForms.IO;
using Eto.Forms.ThemedControls;
using Eto.WinForms.Forms.Cells;
using Eto.WinForms.Forms.Menu;
using Eto.WinForms.Forms.ToolBar;
using Eto.Shared.Forms;

namespace Eto.WinForms
{
	public class Platform : Eto.Platform
	{
		public override bool IsDesktop { get { return true; } }

		public override bool IsWinForms { get { return true; } }

		public override string ID => "WinForms";

		public override PlatformFeatures SupportedFeatures =>
			PlatformFeatures.DrawableWithTransparentContent;

		static Platform()
		{
			EmbeddedAssemblyLoader.Register("Eto.WinForms.CustomControls.Assemblies");
		}

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
			p.Add<IconFrame.IHandler>(() => new IconFrameHandler());
			p.Add<IndexedBitmap.IHandler>(() => new IndexedBitmapHandler());
			p.Add<Matrix.IHandler>(() => new MatrixHandler());
			p.Add<Pen.IHandler>(() => new PenHandler());
			p.Add<SolidBrush.IHandler>(() => new SolidBrushHandler());
			p.Add<TextureBrush.IHandler>(() => new TextureBrushHandler());
			p.Add<LinearGradientBrush.IHandler>(() => new LinearGradientBrushHandler());
			p.Add<RadialGradientBrush.IHandler>(() => new RadialGradientBrushHandler());
			p.Add<SystemColors.IHandler>(() => new SystemColorsHandler());
			p.Add<FormattedText.IHandler>(() => new FormattedTextHandler());

			// Forms.Cells
			p.Add<CheckBoxCell.IHandler>(() => new CheckBoxCellHandler());
			p.Add<ComboBoxCell.IHandler>(() => new ComboBoxCellHandler());
			p.Add<ImageTextCell.IHandler>(() => new ImageTextCellHandler());
			p.Add<ImageViewCell.IHandler>(() => new ImageViewCellHandler());
			p.Add<TextBoxCell.IHandler>(() => new TextBoxCellHandler());
			p.Add<DrawableCell.IHandler>(() => new DrawableCellHandler());
			p.Add<ProgressCell.IHandler>(() => new ProgressCellHandler());
			p.Add<CustomCell.IHandler>(() => new CustomCellHandler());

			// Forms.Controls
			p.Add<Button.IHandler>(() => new ButtonHandler());
			p.Add<Calendar.IHandler>(() => new CalendarHandler());
			p.Add<CheckBox.IHandler>(() => new CheckBoxHandler());
			p.Add<DropDown.IHandler>(() => new DropDownHandler());
			p.Add<ComboBox.IHandler>(() => new ComboBoxHandler());
			p.Add<ColorPicker.IHandler>(() => new ColorPickerHandler());
			p.Add<FontPicker.IHandler>(() => new ThemedFontPickerHandler());
			p.Add<DateTimePicker.IHandler>(() => new DateTimePickerHandler());
			p.Add<Drawable.IHandler>(() => new DrawableHandler());
			p.Add<Expander.IHandler>(() => new ThemedExpanderHandler());
			p.Add<GridColumn.IHandler>(() => new GridColumnHandler());
			p.Add<GridView.IHandler>(() => new GridViewHandler());
			p.Add<GroupBox.IHandler>(() => new GroupBoxHandler());
			p.Add<ImageView.IHandler>(() => new ImageViewHandler());
			p.Add<Label.IHandler>(() => new LabelHandler());
			p.Add<LinkButton.IHandler>(() => new LinkButtonHandler());
			p.Add<ListBox.IHandler>(() => new ListBoxHandler());
			p.Add<NumericStepper.IHandler>(() => new NumericStepperHandler());
			p.Add<Panel.IHandler>(() => new PanelHandler());
			p.Add<PasswordBox.IHandler>(() => new PasswordBoxHandler());
			p.Add<ProgressBar.IHandler>(() => new ProgressBarHandler());
			p.Add<RadioButton.IHandler>(() => new RadioButtonHandler());
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			p.Add<Slider.IHandler>(() => new SliderHandler());
			p.Add<Spinner.IHandler>(() => new ThemedSpinnerHandler());
			p.Add<Splitter.IHandler>(() => new SplitterHandler());
			p.Add<TabControl.IHandler>(() => new TabControlHandler());
			p.Add<TabPage.IHandler>(() => new TabPageHandler());
			p.Add<TextArea.IHandler>(() => new TextAreaHandler());
			p.Add<TextBox.IHandler>(() => new TextBoxHandler());
			p.Add<TreeGridView.IHandler>(() => new TreeGridViewHandler());
#pragma warning disable CS0618 // Type or member is obsolete
			p.Add<TreeView.IHandler>(() => new TreeViewHandler());
#pragma warning restore CS0618 // Type or member is obsolete
			p.Add<WebView.IHandler>(() => new WebViewHandler());
			p.Add<RichTextArea.IHandler>(() => new RichTextAreaHandler());
			p.Add<Stepper.IHandler>(() => new ThemedStepperHandler());
			p.Add<TextStepper.IHandler>(() => new TextStepperHandler());
			p.Add<FilePicker.IHandler>(() => new ThemedFilePickerHandler());
			p.Add<DocumentControl.IHandler>(() => new ThemedDocumentControlHandler());
			p.Add<DocumentPage.IHandler>(() => new ThemedDocumentPageHandler());
			p.Add<SegmentedButton.IHandler>(() => new ThemedSegmentedButtonHandler());
			p.Add<ButtonSegmentedItem.IHandler>(() => new ThemedButtonSegmentedItemHandler());
			p.Add<MenuSegmentedItem.IHandler>(() => new ThemedMenuSegmentedItemHandler());
			p.Add<ToggleButton.IHandler>(() => new ToggleButtonHandler());
			p.Add<PropertyGrid.IHandler>(() => new ThemedPropertyGridHandler());
			p.Add<CollectionEditor.IHandler>(() => new ThemedCollectionEditorHandler());

			// Forms.Menu
			p.Add<CheckMenuItem.IHandler>(() => new CheckMenuItemHandler());
			p.Add<ContextMenu.IHandler>(() => new ContextMenuHandler());
			p.Add<ButtonMenuItem.IHandler>(() => new ButtonMenuItemHandler());
			p.Add<MenuBar.IHandler>(() => new MenuBarHandler());
			p.Add<RadioMenuItem.IHandler>(() => new RadioMenuItemHandler());
			p.Add<SeparatorMenuItem.IHandler>(() => new SeparatorMenuItemHandler());
			
			// Forms.Printing
			p.Add<PrintDialog.IHandler>(() => new PrintDialogHandler());
			p.Add<PrintDocument.IHandler>(() => new PrintDocumentHandler());
			p.Add<PrintSettings.IHandler>(() => new PrintSettingsHandler());
			
			// Forms.ToolBar
			p.Add<CheckToolItem.IHandler>(() => new CheckToolItemHandler());
			p.Add<RadioToolItem.IHandler>(() => new RadioToolItemHandler());
			p.Add<SeparatorToolItem.IHandler>(() => new SeparatorToolBarItemHandler());
			p.Add<ButtonToolItem.IHandler>(() => new ButtonToolItemHandler());
			p.Add<ToolBar.IHandler>(() => new ToolBarHandler());
			
			// Forms
			p.Add<AboutDialog.IHandler>(() => new ThemedAboutDialogHandler());
			p.Add<Application.IHandler>(() => new ApplicationHandler());
			p.Add<Clipboard.IHandler>(() => new ClipboardHandler());
			p.Add<ColorDialog.IHandler>(() => new ColorDialogHandler());
			p.Add<Cursor.IHandler>(() => new CursorHandler());
			p.Add<Dialog.IHandler>(() => new DialogHandler());
			p.Add<FontDialog.IHandler>(() => new FontDialogHandler());
			p.Add<Form.IHandler>(() => new FormHandler());
			p.Add<MessageBox.IHandler>(() => new MessageBoxHandler());
			p.Add<OpenFileDialog.IHandler>(() => new OpenFileDialogHandler());
			p.Add<OpenWithDialog.IHandler>(() => new OpenWithDialogHandler());
			p.Add<PixelLayout.IHandler>(() => new PixelLayoutHandler());
			p.Add<SaveFileDialog.IHandler>(() => new SaveFileDialogHandler());
			if (Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialog.IsPlatformSupported)
				p.Add<SelectFolderDialog.IHandler>(() => new VistaSelectFolderDialogHandler());
			else
				p.Add<SelectFolderDialog.IHandler>(() => new SelectFolderDialogHandler());
			p.Add<TableLayout.IHandler>(() => new TableLayoutHandler());
			p.Add<UITimer.IHandler>(() => new UITimerHandler());
			p.Add<Mouse.IHandler>(() => new MouseHandler());
			p.Add<Screen.IScreensHandler>(() => new ScreensHandler());
			p.Add<Keyboard.IHandler>(() => new KeyboardHandler());
			p.Add<FixedMaskedTextProvider.IHandler>(() => new FixedMaskedTextProviderHandler());
			p.Add<TrayIndicator.IHandler>(() => new TrayIndicatorHandler());
			p.Add<Notification.IHandler>(() => new NotificationHandler());
			p.Add<DataObject.IHandler>(() => new DataObjectHandler());
			p.Add<Taskbar.IHandler>(() => new TaskbarHandler());

			// IO
			p.Add<SystemIcons.IHandler>(() => new SystemIconsHandler());
			
			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
		}
	}
}
