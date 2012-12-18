using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.Platform.GtkSharp.Drawing;
using System.Threading;
using Eto.Platform.GtkSharp.Forms.Cells;
using Eto.Platform.GtkSharp.Forms.Controls;
using Eto.Platform.GtkSharp.Forms.Printing;
using Eto.Platform.GtkSharp.Forms;
using Eto.Platform.GtkSharp.IO;

namespace Eto.Platform.GtkSharp
{
	public class Generator : Eto.Generator
	{ 	
		public override string ID {
			get { return Generators.Gtk; }
		}
		
		public Generator ()
		{
			// Drawing
			Add <IBitmap> (() => new BitmapHandler ());
			Add <IFontFamily> (() => new FontFamilyHandler ());
			Add <IFont> (() => new FontHandler ());
			Add <IFonts> (() => new FontsHandler ());
			Add <IGraphics> (() => new GraphicsHandler ());
			Add <IGraphicsPath> (() => new GraphicsPathHandler ());
			Add <IIcon> (() => new IconHandler ());
			Add <IIndexedBitmap> (() => new IndexedBitmapHandler ());
			Add <IMatrixHandler> (() => new MatrixHandler ());
			Add <IPenHandler> (() => new PenHandler ());
			Add <ISolidBrushHandler> (() => new SolidBrushHandler ());

			// Forms.Cells
			Add <ICheckBoxCell> (() => new CheckBoxCellHandler ());
			Add <IComboBoxCell> (() => new ComboBoxCellHandler ());
			Add <IImageTextCell> (() => new ImageTextCellHandler ());
			Add <IImageViewCell> (() => new ImageViewCellHandler ());
			Add <ITextBoxCell> (() => new TextBoxCellHandler ());
			
			// Forms.Controls
			Add <IButton> (() => new ButtonHandler ());
			Add <ICheckBox> (() => new CheckBoxHandler ());
			Add <IComboBox> (() => new ComboBoxHandler ());
			Add <IDateTimePicker> (() => new DateTimePickerHandler ());
			Add <IDrawable> (() => new DrawableHandler ());
			Add <IGridColumn> (() => new GridColumnHandler ());
			Add <IGridView> (() => new GridViewHandler ());
			Add <IGroupBox> (() => new GroupBoxHandler ());
			Add <IImageView> (() => new ImageViewHandler ());
			Add <ILabel> (() => new LabelHandler ());
			Add <IListBox> (() => new ListBoxHandler ());
			Add <INumericUpDown> (() => new NumericUpDownHandler ());
			Add <IPanel> (() => new PanelHandler ());
			Add <IPasswordBox> (() => new PasswordBoxHandler ());
			Add <IProgressBar> (() => new ProgressBarHandler ());
			Add <IRadioButton> (() => new RadioButtonHandler ());
			Add <IScrollable> (() => new ScrollableHandler ());
			Add <ISlider> (() => new SliderHandler ());
			Add <ISplitter> (() => new SplitterHandler ());
			Add <ITabControl> (() => new TabControlHandler ());
			Add <ITabPage> (() => new TabPageHandler ());
			Add <ITextArea> (() => new TextAreaHandler ());
			Add <ITextBox> (() => new TextBoxHandler ());
			Add <ITreeGridView> (() => new TreeGridViewHandler ());
			Add <ITreeView> (() => new TreeViewHandler ());
			Add <IWebView> (() => new WebViewHandler ());
			
			// Forms.Menu
			Add <ICheckMenuItem> (() => new CheckMenuItemHandler ());
			Add <IContextMenu> (() => new ContextMenuHandler ());
			Add <IImageMenuItem> (() => new ImageMenuItemHandler ());
			Add <IMenuBar> (() => new MenuBarHandler ());
			Add <IRadioMenuItem> (() => new RadioMenuItemHandler ());
			Add <ISeparatorMenuItem> (() => new SeparatorMenuItemHandler ());
			
			// Forms.Printing
			Add <IPrintDialog> (() => new PrintDialogHandler ());
			Add <IPrintDocument> (() => new PrintDocumentHandler ());
			Add <IPrintSettings> (() => new PrintSettingsHandler ());
			
			// Forms.ToolBar
			Add <ICheckToolBarButton> (() => new CheckToolBarButtonHandler ());
			Add <ISeparatorToolBarItem> (() => new SeparatorToolBarItemHandler ());
			Add <IToolBarButton> (() => new ToolBarButtonHandler ());
			Add <IToolBar> (() => new ToolBarHandler ());

			// Forms
			Add <IApplication> (() => new ApplicationHandler ());
			Add <IClipboard> (() => new ClipboardHandler ());
			Add <IColorDialog> (() => new ColorDialogHandler ());
			Add <ICursor> (() => new CursorHandler ());
			Add <IDialog> (() => new DialogHandler ());
			Add <IDockLayout> (() => new DockLayoutHandler ());
			Add <IFontDialog> (() => new FontDialogHandler ());
			Add <IForm> (() => new FormHandler ());
			Add <IMessageBox> (() => new MessageBoxHandler ());
			Add <IOpenFileDialog> (() => new OpenFileDialogHandler ());
			Add <IPixelLayout> (() => new PixelLayoutHandler ());
			Add <ISaveFileDialog> (() => new SaveFileDialogHandler ());
			Add <ISelectFolderDialog> (() => new SelectFolderDialogHandler ());
			Add <ITableLayout> (() => new TableLayoutHandler ());
			Add <IUITimer> (() => new UITimerHandler ());
			
			// IO
			Add <ISystemIcons> (() => new SystemIconsHandler ());

			// General
			Add <IEtoEnvironment> (() => new EtoEnvironmentHandler ());


			if (EtoEnvironment.Platform.IsWindows && Environment.Is64BitProcess)
				throw new NotSupportedException("Please compile/run GTK in x86 mode (32-bit) on windows");
			Gtk.Application.Init();
			
			Gdk.Threads.Enter ();
		}
	}
}
