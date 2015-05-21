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
using Eto.GtkSharp.Forms.Menu;
using Eto.GtkSharp.Forms.ToolBar;
using Eto.Shared.Forms;

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

		public override bool IsValid
		{
			get
			{
				try
				{
					return typeof(Gtk.Application) != null;
				}
				catch
				{
					return false;
				}
			}
		}

		#if GTK2
		public override string ID { get { return "gtk2"; } }
		#else
		public override string ID { get { return "gtk3"; } }
		#endif
		public Platform()
		{
			if (EtoEnvironment.Platform.IsWindows && Environment.Is64BitProcess)
				throw new NotSupportedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Please compile/run GTK in x86 mode (32-bit) on windows"));

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
			p.Add<CheckBoxCell.IHandler>(() => new CheckBoxCellHandler());
			p.Add<ComboBoxCell.IHandler>(() => new ComboBoxCellHandler());
			p.Add<ImageTextCell.IHandler>(() => new ImageTextCellHandler());
			p.Add<ImageViewCell.IHandler>(() => new ImageViewCellHandler());
			p.Add<TextBoxCell.IHandler>(() => new TextBoxCellHandler());
			p.Add<DrawableCell.IHandler>(() => new DrawableCellHandler());
			p.Add<ProgressCell.IHandler>(() => new ProgressCellHandler());

			// Forms.Controls
			p.Add<Button.IHandler>(() => new ButtonHandler());
			p.Add<Calendar.IHandler>(() => new CalendarHandler());
			p.Add<CheckBox.IHandler>(() => new CheckBoxHandler());
			p.Add<DropDown.IHandler>(() => new DropDownHandler());
			p.Add<ComboBox.IHandler>(() => new ComboBoxHandler());
			p.Add<ColorPicker.IHandler>(() => new ColorPickerHandler());
			p.Add<DateTimePicker.IHandler>(() => new DateTimePickerHandler());
			p.Add<Drawable.IHandler>(() => new DrawableHandler());
			p.Add<GridColumn.IHandler>(() => new GridColumnHandler());
			p.Add<GridView.IHandler>(() => new GridViewHandler());
			p.Add<GroupBox.IHandler>(() => new GroupBoxHandler());
			p.Add<ImageView.IHandler>(() => new ImageViewHandler());
			p.Add<Label.IHandler>(() => new LabelHandler());
			p.Add<LinkButton.IHandler>(() => new LinkButtonHandler());
			p.Add<ListBox.IHandler>(() => new ListBoxHandler());
			p.Add<NumericUpDown.IHandler>(() => new NumericUpDownHandler());
			p.Add<Panel.IHandler>(() => new PanelHandler());
			p.Add<PasswordBox.IHandler>(() => new PasswordBoxHandler());
			p.Add<ProgressBar.IHandler>(() => new ProgressBarHandler());
			p.Add<RadioButton.IHandler>(() => new RadioButtonHandler());
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			p.Add<Slider.IHandler>(() => new SliderHandler());
			#if GTK3
			p.Add<Spinner.IHandler>(() => new SpinnerHandler());
			#else
			p.Add<Spinner.IHandler>(() => new ThemedSpinnerHandler());
			#endif
			p.Add<Splitter.IHandler>(() => new SplitterHandler());
			p.Add<TabControl.IHandler>(() => new TabControlHandler());
			p.Add<TabPage.IHandler>(() => new TabPageHandler());
			p.Add<TextArea.IHandler>(() => new TextAreaHandler());
			p.Add<TextBox.IHandler>(() => new TextBoxHandler());
			p.Add<TreeGridView.IHandler>(() => new TreeGridViewHandler());
			p.Add<TreeView.IHandler>(() => new TreeViewHandler());
			p.Add<WebView.IHandler>(() => new WebViewHandler());
			p.Add<RichTextArea.IHandler>(() => new RichTextAreaHandler());

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
			p.Add<ButtonToolItem.IHandler>(() => new ButtonToolItemHandler());
			p.Add<CheckToolItem.IHandler>(() => new CheckToolItemHandler());
			p.Add<RadioToolItem.IHandler>(() => new RadioToolItemHandler());
			p.Add<SeparatorToolItem.IHandler>(() => new SeparatorToolItemHandler());
			p.Add<ToolBar.IHandler>(() => new ToolBarHandler());
			p.Add<ToolBarView.IHandler>(() => new ToolBarViewHandler());

			// Forms
			p.Add<Application.IHandler>(() => new ApplicationHandler());
			p.Add<Clipboard.IHandler>(() => new ClipboardHandler());
			p.Add<ColorDialog.IHandler>(() => new ColorDialogHandler());
			p.Add<Cursor.IHandler>(() => new CursorHandler());
			p.Add<Dialog.IHandler>(() => new DialogHandler());
			p.Add<FontDialog.IHandler>(() => new FontDialogHandler());
			p.Add<Form.IHandler>(() => new FormHandler());
			p.Add<MessageBox.IHandler>(() => new MessageBoxHandler());
			p.Add<OpenFileDialog.IHandler>(() => new OpenFileDialogHandler());
			p.Add<PixelLayout.IHandler>(() => new PixelLayoutHandler());
			p.Add<SaveFileDialog.IHandler>(() => new SaveFileDialogHandler());
			p.Add<SelectFolderDialog.IHandler>(() => new SelectFolderDialogHandler());
			p.Add<TableLayout.IHandler>(() => new TableLayoutHandler());
			p.Add<UITimer.IHandler>(() => new UITimerHandler());
			p.Add<Mouse.IHandler>(() => new MouseHandler());
			p.Add<Screen.IScreensHandler>(() => new ScreensHandler());
			p.Add<Keyboard.IHandler>(() => new KeyboardHandler());
			p.Add<FixedMaskedTextProvider.IHandler>(() => new FixedMaskedTextProviderHandler());

			// IO
			p.Add<SystemIcons.IHandler>(() => new SystemIconsHandler());

			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
		}
	}
}
