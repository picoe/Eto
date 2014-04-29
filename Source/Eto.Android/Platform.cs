using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Android.Drawing;
using Eto.Android.Forms.Controls;
using Eto.Android.Forms;
using Eto.Android.Forms.Cells;

namespace Eto.Android
{
	public class Platform : Eto.Platform
	{
		public override string ID { get { return "android"; } }

		public override bool IsMobile { get { return true; } }

		public override bool IsAndroid { get { return true; } }

		public Platform()
		{
			AddTo(this);
		}

		public static void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<IBitmap>(() => new BitmapHandler());
			p.Add<IFontFamily>(() => new FontFamilyHandler());
			p.Add<IFont>(() => new FontHandler());
			//g.Add<IFonts>(() => new FontsHandler());
			p.Add<IGraphics>(() => new GraphicsHandler());
			p.Add<IGraphicsPathHandler>(() => new GraphicsPathHandler());
			p.Add<IIcon>(() => new IconHandler());
			//g.Add<IIndexedBitmap>(() => new IndexedBitmapHandler());
			p.Add<IMatrixHandler>(() => new MatrixHandler());
			p.Add<IPen>(() => new PenHandler());
			p.Add<ISolidBrush>(() => new SolidBrushHandler());
			//g.Add<ITextureBrush>(() => new TextureBrushHandler());
			p.Add<ILinearGradientBrush>(() => new LinearGradientBrushHandler());

			// Forms.Cells
			//g.Add <ICheckBoxCell> (() => new CheckBoxCellHandler ());
			//g.Add <IComboBoxCell> (() => new ComboBoxCellHandler ());
			//g.Add<IImageTextCell>(() => new ImageTextCellHandler());
			//g.Add <IImageViewCell> (() => new ImageViewCellHandler ());
			p.Add<ITextBoxCell>(() => new TextBoxCellHandler());

			// Forms.Controls
			p.Add<IButton>(() => new ButtonHandler());
			//g.Add<ICheckBox>(() => new CheckBoxHandler());
			//g.Add<IComboBox>(() => new ComboBoxHandler());
			//g.Add <IDateTimePicker> (() => new DateTimePickerHandler ());
			//g.Add<IDrawable>(() => new DrawableHandler());
			p.Add<IGridColumn>(() => new GridColumnHandler());
			p.Add<IGridView>(() => new GridViewHandler());
			//g.Add <IGroupBox> (() => new GroupBoxHandler ());
			//g.Add<IImageView>(() => new ImageViewHandler());
			p.Add<ILabel>(() => new LabelHandler());
			//g.Add<IListBox>(() => new ListBoxHandler());
			//g.Add<INumericUpDown>(() => new NumericUpDownHandler());
			p.Add<IPanel>(() => new PanelHandler());
			//g.Add<IPasswordBox>(() => new PasswordBoxHandler());
			//g.Add<IProgressBar>(() => new ProgressBarHandler());
			//g.Add<IRadioButton>(() => new RadioButtonHandler());
			p.Add<IScrollable>(() => new ScrollableHandler());
			p.Add<ISearchBox>(() => new SearchBoxHandler());
			//g.Add<ISlider>(() => new SliderHandler());
			p.Add<ISpinner>(() => new SpinnerHandler());
			//g.Add<ISplitter>(() => new SplitterHandler());
			//g.Add <ITabControl> (() => new TabControlHandler ());
			//g.Add <ITabPage> (() => new TabPageHandler ());
			//g.Add<ITextArea>(() => new TextAreaHandler());
			p.Add<ITextBox>(() => new TextBoxHandler());
			//g.Add<ITreeGridView>(() => new TreeGridViewHandler());
			//g.Add <ITreeView> (() => new TreeViewHandler ());
			//g.Add<IWebView>(() => new WebViewHandler());
			//g.Add<INavigation>(() => new NavigationHandler());

			// Forms.Menu
			//g.Add <ICheckMenuItem> (() => new CheckMenuItemHandler ());
			//g.Add <IContextMenu> (() => new ContextMenuHandler ());
			//g.Add <IImageMenuItem> (() => new ImageMenuItemHandler ());
			//g.Add <IMenuBar> (() => new MenuBarHandler ());
			//g.Add <IRadioMenuItem> (() => new RadioMenuItemHandler ());
			//g.Add <ISeparatorMenuItem> (() => new SeparatorMenuItemHandler ());

			// Forms.Printing
			//g.Add <IPrintDialog> (() => new PrintDialogHandler ());
			//g.Add <IPrintDocument> (() => new PrintDocumentHandler ());
			//g.Add <IPrintSettings> (() => new PrintSettingsHandler ());

			// Forms.ToolBar
			//g.Add <ICheckToolBarButton> (() => new CheckToolBarButtonHandler ());
			//g.Add <ISeparatorToolBarItem> (() => new SeparatorToolBarItemHandler ());
			//g.Add <IToolBarButton> (() => new ToolBarButtonHandler ());
			//g.Add <IToolBar> (() => new ToolBarHandler ());

			// Forms
			p.Add<IApplication>(() => new ApplicationHandler());
			//g.Add <IClipboard> (() => new ClipboardHandler ());
			//g.Add <IColorDialog> (() => new ColorDialogHandler ());
			//g.Add <ICursor> (() => new CursorHandler ());
			//g.Add<IDialog>(() => new DialogHandler());
			//g.Add <IFontDialog> (() => new FontDialogHandler ());
			p.Add<IForm>(() => new FormHandler());
			//g.Add<IMessageBox>(() => new MessageBoxHandler());
			//g.Add <IOpenFileDialog> (() => new OpenFileDialogHandler ());
			//g.Add<IPixelLayout>(() => new PixelLayoutHandler());
			//g.Add <ISaveFileDialog> (() => new SaveFileDialogHandler ());
			//g.Add <ISelectFolderDialog> (() => new SelectFolderDialogHandler ());
			p.Add<ITableLayout>(() => new TableLayoutHandler());
			//g.Add<IUITimer>(() => new UITimerHandler());

			// IO
			//g.Add <ISystemIcons> (() => new SystemIconsHandler ());

			// General
			//g.Add<IEtoEnvironment>(() => new EtoEnvironmentHandler());
		}
	}
}
