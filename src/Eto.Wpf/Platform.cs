using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Forms.Cells;
using Eto.Wpf.Forms.ToolBar;
using swi = System.Windows.Input;
using swm = System.Windows.Media;
using sw = System.Windows;
using Eto.Wpf.Drawing;
using Eto.Wpf.Forms.Menu;
using Eto.Wpf.Forms.Controls;
using Eto.Wpf.Forms.Printing;
using Eto.Wpf.Forms;
using Eto.IO;
using Eto.Wpf.IO;
using Eto.Forms.ThemedControls;
using Eto.Shared.Forms;
using System.Linq;

namespace Eto.Wpf
{
	public class Platform : Eto.Platform
	{
		public override string ID => "Wpf";

		public override bool IsDesktop { get { return true; } }

		public override bool IsWpf { get { return true; } }

		public override PlatformFeatures SupportedFeatures =>
			PlatformFeatures.DrawableWithTransparentContent
            | PlatformFeatures.CustomCellSupportsControlView
			| PlatformFeatures.TabIndexWithCustomContainers;

		static Platform()
		{
			EmbeddedAssemblyLoader.Register("Eto.Wpf.CustomControls.Assemblies");

			Style.Add<ThemedSegmentedButtonHandler>(null, h =>
			{
				h.Control.Styles.Add<ToggleButtonHandler>(null, tb =>
				{
					if (tb.Widget.Parent is TableLayout tl && tl.Rows.Count > 0 && tl.Spacing.Width == 0)
					{
						var isFirst = ReferenceEquals(tl.Rows[0].Cells[0].Control, tb.Widget);
						var thickness = tb.Control.BorderThickness;
						thickness.Left = isFirst ? thickness.Right : 0;
						tb.Control.BorderThickness = thickness;
					}
				});
			});
		}

		public Platform()
		{
			AddTo(this);

			// by default, use WinForms web view (it has more features we can control)
			UseSwfWebView();
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
			p.Add<DateTimePicker.IHandler>(() => new DateTimePickerHandler());
			p.Add<Drawable.IHandler>(() => new DrawableHandler());
			p.Add<Expander.IHandler>(() => new ExpanderHandler());
			p.Add<FontPicker.IHandler>(() => new ThemedFontPickerHandler());
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
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
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
			//p.Add<WebView.IHandler>(()  => new WebViewHandler ());
			p.Add<RichTextArea.IHandler>(() => new RichTextAreaHandler());
			p.Add<Stepper.IHandler>(() => new StepperHandler());
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
			p.Add<SeparatorToolItem.IHandler>(() => new SeparatorToolItemHandler());
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

			// IO
			p.Add<SystemIcons.IHandler>(() => new SystemIconsHandler());
			
			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
		}

		public void UseWpfWebView()
		{
			Add<WebView.IHandler>(() => new WpfWebViewHandler());
		}

		public void UseSwfWebView()
		{
			Add<WebView.IHandler>(() => new SwfWebViewHandler());
		}
	}
}
