using Eto.Android.Drawing;
using Eto.Android.Forms.Controls;
using Eto.Android.Forms;
using Eto.Android.Forms.Cells;
using Eto.Android.Forms.ToolBar;

using a = Android;

namespace Eto.Android
{
	public class Platform : Eto.Platform
	{
		public override string ID { get { return "android"; } }

		public override bool IsMobile { get { return true; } }

		public override bool IsAndroid { get { return true; } }

		private static a.Util.DisplayMetrics displayMetrics;
		private static Double dpiRatio;
		private static Double fontScale;

		public static a.Content.Context AppContext { get; }
		public static a.Content.Context AppContextThemed { get; private set; }

		static Platform()
		{
			AppContext = a.App.Application.Context;
			AppContextThemed = new a.Views.ContextThemeWrapper(AppContext, a.Resource.Style.ThemeDeviceDefault);
		}

		public Platform()
		{
			displayMetrics = AppContext.Resources.DisplayMetrics;
			dpiRatio = displayMetrics.Density;
			fontScale = (160d / 72d) * (displayMetrics.ScaledDensity / displayMetrics.Density);

			AddTo(this);
		}

		public static void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<Bitmap.IHandler>(() => new BitmapHandler());
			p.Add<FontFamily.IHandler>(() => new FontFamilyHandler());
			p.Add<Font.IHandler>(() => new FontHandler());
			p.Add<Fonts.IHandler>(() => new FontsHandler());
			p.Add<FormattedText.IHandler>(() => new FormattedTextHandler());
			p.Add<Graphics.IHandler>(() => new GraphicsHandler());
			p.Add<GraphicsPath.IHandler>(() => new GraphicsPathHandler());
			p.Add<Icon.IHandler>(() => new IconHandler());
			p.Add<IconFrame.IHandler>(() => new IconFrameHandler());
			//p.Add<IndexedBitmap.IHandler>(() => new IndexedBitmapHandler());
			p.Add<Matrix.IHandler>(() => new MatrixHandler());
			p.Add<Pen.IHandler>(() => new PenHandler());
			p.Add<SolidBrush.IHandler>(() => new SolidBrushHandler());
			//p.Add<TextureBrush.IHandler>(() => new TextureBrushHandler());
			p.Add<LinearGradientBrush.IHandler>(() => new LinearGradientBrushHandler());
			p.Add<Screen.IScreensHandler>(() => new ScreensHandler());
			p.Add<SystemColors.IHandler>(() => new SystemColorsHandler());

			// Forms.Cells
			p.Add<CheckBoxCell.IHandler>(() => new CheckBoxCellHandler());
			p.Add<ComboBoxCell.IHandler>(() => new ComboBoxCellHandler());
			p.Add<ImageTextCell.IHandler>(() => new ImageTextCellHandler());
			p.Add<ImageViewCell.IHandler>(() => new ImageViewCellHandler());
			p.Add<ProgressCell.IHandler>(() => new ProgressCellHandler());
			p.Add<TextBoxCell.IHandler>(() => new TextBoxCellHandler());

			// Forms.Controls
			p.Add<Button.IHandler>(() => new ButtonHandler());
			p.Add<CheckBox.IHandler>(() => new CheckBoxHandler());
			p.Add<DropDown.IHandler>(() => new DropDownHandler());
			p.Add<ComboBox.IHandler>(() => new ComboBoxHandler());
			//p.Add<IDateTimePicker.IHandler>(() => new DateTimePickerHandler ());
			p.Add<Drawable.IHandler>(() => new DrawableHandler());
			p.Add<FontPicker.IHandler>(() => new FontPickerHandler());
			p.Add<GridColumn.IHandler>(() => new GridColumnHandler());
			p.Add<GridView.IHandler>(() => new GridViewHandler());
			//p.Add<IGroupBox.IHandler>(() => new GroupBoxHandler ());
			p.Add<ImageView.IHandler>(() => new ImageViewHandler());
			p.Add<Label.IHandler>(() => new LabelHandler());
			p.Add<LinkButton.IHandler>(() => new LinkButtonHandler());
			p.Add<ListBox.IHandler>(() => new ListBoxHandler());
			p.Add<NumericUpDown.IHandler>(() => new NumericUpDownHandler());
			p.Add<NumericStepper.IHandler>(() => new NumericStepperHandler());
			p.Add<Panel.IHandler>(() => new PanelHandler());
			p.Add<PasswordBox.IHandler>(() => new PasswordBoxHandler());
			p.Add<ProgressBar.IHandler>(() => new ProgressBarHandler());
			p.Add<RadioButton.IHandler>(() => new RadioButtonHandler());
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			//p.Add<Slider.IHandler>(() => new SliderHandler());
			p.Add<Spinner.IHandler>(() => new SpinnerHandler());
			p.Add<Splitter.IHandler>(() => new SplitterHandler());
			//p.Add<ITabControl.IHandler>(() => new TabControlHandler ());
			//p.Add<ITabPage.IHandler>(() => new TabPageHandler ());
			p.Add<TextArea.IHandler>(() => new TextAreaHandler());
			p.Add<TextBox.IHandler>(() => new TextBoxHandler());
			//p.Add<TreeGridView.IHandler>(() => new TreeGridViewHandler());
			//p.Add<ITreeView.IHandler>(() => new TreeViewHandler ());
			//p.Add<WebView.IHandler>(() => new WebViewHandler());
			p.Add<Navigation.IHandler>(() => new NavigationHandler());

			// Forms.Menu
			p.Add<CheckMenuItem.IHandler>(() => new CheckMenuItemHandler());
			p.Add<ContextMenu.IHandler>(() => new ContextMenuHandler());
			p.Add<ButtonMenuItem.IHandler>(() => new ButtonMenuItemHandler());
			//p.Add<IImageMenuItem.IHandler>(() => new ImageMenuItemHandler ());
			//p.Add<IMenuBar.IHandler>(() => new MenuBarHandler ());
			//p.Add<IRadioMenuItem.IHandler>(() => new RadioMenuItemHandler ());
			p.Add<SeparatorMenuItem.IHandler>(() => new SeparatorMenuItemHandler());

			// Forms.Printing
			//p.Add<IPrintDialog.IHandler>(() => new PrintDialogHandler ());
			//p.Add<IPrintDocument.IHandler>(() => new PrintDocumentHandler ());
			//p.Add<IPrintSettings.IHandler>(() => new PrintSettingsHandler ());

			// Forms.ToolBar
			p.Add<CheckToolItem.IHandler>(() => new CheckToolItemHandler ());
			p.Add<SeparatorToolItem.IHandler>(() => new SeparatorToolItemHandler());
			p.Add<ButtonToolItem.IHandler>(() => new ButtonToolItemHandler());
			p.Add<ToolBar.IHandler>(() => new ToolBarHandler());

			// Forms
			p.Add<Application.IHandler>(() => new ApplicationHandler());
			//p.Add<IClipboard.IHandler>(() => new ClipboardHandler ());
			//p.Add<IColorDialog.IHandler>(() => new ColorDialogHandler ());
			//p.Add<ICursor.IHandler>(() => new CursorHandler ());
			p.Add<Dialog.IHandler>(() => new DialogHandler());
			//p.Add<IFontDialog.IHandler>(() => new FontDialogHandler ());
			p.Add<Form.IHandler>(() => new FormHandler());
			p.Add<MessageBox.IHandler>(() => new MessageBoxHandler());
			//p.Add<IOpenFileDialog.IHandler>(() => new OpenFileDialogHandler ());
			p.Add<PixelLayout.IHandler>(() => new PixelLayoutHandler());
			//p.Add<ISaveFileDialog.IHandler>(() => new SaveFileDialogHandler ());
			//p.Add<ISelectFolderDialog.IHandler>(() => new SelectFolderDialogHandler ());
			p.Add<TableLayout.IHandler>(() => new TableLayoutHandler());
			p.Add<UITimer.IHandler>(() => new UITimerHandler());

			// IO
			//p.Add<ISystemIcons.IHandler>(() => new SystemIconsHandler ());

			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
		}

		public static Size DpToPx(Size value) => new Size((Int32)(value.Width * dpiRatio), (Int32)(value.Height * dpiRatio));

		public static SizeF DpToPx(SizeF value) => new SizeF((Single)(value.Width * dpiRatio), (Single)(value.Height * dpiRatio));

		public static Int32 DpToPx(Int32 value) => (Int32)(value * dpiRatio);

		public static Single DpToPx(Single value) => (Single)(value * dpiRatio);

		private static Single DpToPt(Single value) => (Single)(value / fontScale);

		public static Size PxToDp(Size value) => new Size((Int32)(value.Width / dpiRatio), (Int32)(value.Height / dpiRatio));

		public static SizeF PxToDp(SizeF value) => new SizeF((Single)(value.Width / dpiRatio), (Single)(value.Height / dpiRatio));

		public static Int32 PxToDp(Int32 value) => (Int32)(value / dpiRatio);

		public static Single PxToDp(Single value) => (Single)(value / dpiRatio);

		public static Single PxToPt(Single value) => DpToPt(PxToDp(value));

		public static Single PtToDp(Single value) => (Single)(value * fontScale);

		public static Single PtToPx(Single value) => DpToPx(PtToDp(value));
	}
}
