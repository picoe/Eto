using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.GtkSharp.Drawing;
using Eto.GtkSharp.Forms.Cells;
using Eto.GtkSharp.Forms.Controls;
using Eto.GtkSharp.Forms.Printing;
using Eto.GtkSharp.Forms;
using Eto.GtkSharp.IO;
using Eto.Forms.ThemedControls;

namespace Eto.GtkSharp
{
	static class Helper
	{
		public static void Init()
		{
			var args = new string[0];
			if (Gtk.Application.InitCheck(string.Empty, ref args))
			{
				Gdk.Threads.Enter();
			}
		}
	}

	public class Platform : Eto.Platform
	{
		public override bool IsDesktop { get { return true; } }

		public override bool IsGtk { get { return true; } }

		#if GTK2
		public override string ID { get { return "gtk2"; } }
		#else
		public override string ID { get { return "gtk3"; } }
		#endif
		public Platform()
		{
			if (EtoEnvironment.Platform.IsWindows && Environment.Is64BitProcess)
				throw new NotSupportedException("Please compile/run GTK in x86 mode (32-bit) on windows");

			AddTo(this);
		}

		public static void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<IBitmap>(() => new BitmapHandler());
			p.Add<IFontFamily>(() => new FontFamilyHandler());
			p.Add<IFont>(() => new FontHandler());
			p.Add<IFonts>(() => new FontsHandler());
			p.Add<IGraphics>(() => new GraphicsHandler());
			p.Add<IGraphicsPathHandler>(() => new GraphicsPathHandler());
			p.Add<IIcon>(() => new IconHandler());
			p.Add<IIndexedBitmap>(() => new IndexedBitmapHandler());
			p.Add<IMatrixHandler>(() => new MatrixHandler());
			p.Add<IPen>(() => new PenHandler());
			p.Add<ISolidBrush>(() => new SolidBrushHandler());
			p.Add<ITextureBrush>(() => new TextureBrushHandler());
			p.Add<ILinearGradientBrush>(() => new LinearGradientBrushHandler());

			// Forms.Cells
			p.Add<ICheckBoxCell>(() => new CheckBoxCellHandler());
			p.Add<IComboBoxCell>(() => new ComboBoxCellHandler());
			p.Add<IImageTextCell>(() => new ImageTextCellHandler());
			p.Add<IImageViewCell>(() => new ImageViewCellHandler());
			p.Add<ITextBoxCell>(() => new TextBoxCellHandler());
			
			// Forms.Controls
			p.Add<IButton>(() => new ButtonHandler());
			p.Add<ICheckBox>(() => new CheckBoxHandler());
			p.Add<IComboBox>(() => new ComboBoxHandler());
			p.Add<IDateTimePicker>(() => new DateTimePickerHandler());
			p.Add<IDrawable>(() => new DrawableHandler());
			p.Add<IGridColumn>(() => new GridColumnHandler());
			p.Add<IGridView>(() => new GridViewHandler());
			p.Add<IGroupBox>(() => new GroupBoxHandler());
			p.Add<IImageView>(() => new ImageViewHandler());
			p.Add<ILabel>(() => new LabelHandler());
			p.Add<IListBox>(() => new ListBoxHandler());
			p.Add<INumericUpDown>(() => new NumericUpDownHandler());
			p.Add<IPanel>(() => new PanelHandler());
			p.Add<IPasswordBox>(() => new PasswordBoxHandler());
			p.Add<IProgressBar>(() => new ProgressBarHandler());
			p.Add<IRadioButton>(() => new RadioButtonHandler());
			p.Add<IScrollable>(() => new ScrollableHandler());
			p.Add<ISearchBox>(() => new SearchBoxHandler());
			p.Add<ISlider>(() => new SliderHandler());
			#if GTK3
			p.Add<ISpinner>(() => new SpinnerHandler());
			#else
			p.Add<ISpinner>(() => new ThemedSpinnerHandler());
			#endif
			p.Add<ISplitter>(() => new SplitterHandler());
			p.Add<ITabControl>(() => new TabControlHandler());
			p.Add<ITabPage>(() => new TabPageHandler());
			p.Add<ITextArea>(() => new TextAreaHandler());
			p.Add<ITextBox>(() => new TextBoxHandler());
			p.Add<ITreeGridView>(() => new TreeGridViewHandler());
			p.Add<ITreeView>(() => new TreeViewHandler());
			p.Add<IWebView>(() => new WebViewHandler());
			p.Add<IScreens>(() => new ScreensHandler());
			
			// Forms.Menu
			p.Add<ICheckMenuItem>(() => new CheckMenuItemHandler());
			p.Add<IContextMenu>(() => new ContextMenuHandler());
			p.Add<IButtonMenuItem>(() => new ButtonMenuItemHandler());
			p.Add<IMenuBar>(() => new MenuBarHandler());
			p.Add<IRadioMenuItem>(() => new RadioMenuItemHandler());
			p.Add<ISeparatorMenuItem>(() => new SeparatorMenuItemHandler());
			
			// Forms.Printing
			p.Add<IPrintDialog>(() => new PrintDialogHandler());
			p.Add<IPrintDocument>(() => new PrintDocumentHandler());
			p.Add<IPrintSettings>(() => new PrintSettingsHandler());
			
			// Forms.ToolBar
			p.Add<ICheckToolItem>(() => new CheckToolItemHandler());
			p.Add<ISeparatorToolItem>(() => new SeparatorToolItemHandler());
			p.Add<IButtonToolItem>(() => new ButtonToolItemHandler());
			p.Add<IToolBar>(() => new ToolBarHandler());

			// Forms
			p.Add<IApplication>(() => new ApplicationHandler());
			p.Add<IClipboard>(() => new ClipboardHandler());
			p.Add<IColorDialog>(() => new ColorDialogHandler());
			p.Add<ICursor>(() => new CursorHandler());
			p.Add<IDialog>(() => new DialogHandler());
			p.Add<IFontDialog>(() => new FontDialogHandler());
			p.Add<IForm>(() => new FormHandler());
			p.Add<IMessageBox>(() => new MessageBoxHandler());
			p.Add<IOpenFileDialog>(() => new OpenFileDialogHandler());
			p.Add<IPixelLayout>(() => new PixelLayoutHandler());
			p.Add<ISaveFileDialog>(() => new SaveFileDialogHandler());
			p.Add<ISelectFolderDialog>(() => new SelectFolderDialogHandler());
			p.Add<ITableLayout>(() => new TableLayoutHandler());
			p.Add<IUITimer>(() => new UITimerHandler());
			p.Add<IMouse>(() => new MouseHandler());

			// IO
			p.Add<ISystemIcons>(() => new SystemIconsHandler());

			// General
			p.Add<IEtoEnvironment>(() => new EtoEnvironmentHandler());
		}
	}
}
