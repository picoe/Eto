using System;
using UIKit;
using Foundation;
using System.Linq;
using Eto.Drawing;
using CoreGraphics;
using Eto.Forms;
using Eto.iOS.Drawing;
using Eto.iOS.Forms.Cells;
using Eto.iOS.Forms.Controls;
using Eto.iOS.Forms;
using Eto.Mac.Forms;
using Eto.Threading;
using Eto.iOS.Threading;
using Eto.iOS.Forms.Toolbar;
using System.Reflection;

namespace Eto.iOS
{
	[Preserve]
	public class Platform : Eto.Platform
	{
		public const string GeneratorID = "ios";

		public override string ID
		{
			get { return GeneratorID; }
		}

		public static bool IsIpad { get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad; } }

		public static bool IsIos7 { get { return UIDevice.CurrentDevice.CheckSystemVersion(7, 0); } }

		public override bool IsIos { get { return true; } }

		public override bool IsMobile { get { return true; } }

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
			p.Add<CheckBoxCell.IHandler>(() => new CheckBoxCellHandler());
			p.Add<ComboBoxCell.IHandler>(() => new ComboBoxCellHandler());
			p.Add<ImageTextCell.IHandler>(() => new ImageTextCellHandler());
			p.Add<ImageViewCell.IHandler>(() => new ImageViewCellHandler());
			p.Add<TextBoxCell.IHandler>(() => new TextBoxCellHandler());
			
			// Forms.Controls
			p.Add<Button.IHandler>(() => new ButtonHandler());
			p.Add<CheckBox.IHandler>(() => new CheckBoxHandler());
			p.Add<DropDown.IHandler>(() => new DropDownHandler());
			p.Add<ComboBox.IHandler>(() => new ComboBoxHandler());
			p.Add<DateTimePicker.IHandler>(() => new DateTimePickerHandler());
			p.Add<Drawable.IHandler>(() => new DrawableHandler());
			p.Add<GridColumn.IHandler>(() => new GridColumnHandler());
			p.Add<GridView.IHandler>(() => new GridViewHandler());
			p.Add<GroupBox.IHandler>(() => new GroupBoxHandler());
			p.Add<ImageView.IHandler>(() => new ImageViewHandler());
			p.Add<Label.IHandler>(() => new LabelHandler());
			p.Add<ListBox.IHandler>(() => new ListBoxHandler());
			p.Add<NumericStepper.IHandler>(() => new NumericStepperHandler());
			p.Add<Panel.IHandler>(() => new PanelHandler());
			p.Add<PasswordBox.IHandler>(() => new PasswordBoxHandler());
			p.Add<ProgressBar.IHandler>(() => new ProgressBarHandler());
			p.Add<RadioButton.IHandler>(() => new RadioButtonHandler());
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			p.Add<Slider.IHandler>(() => new SliderHandler());
			p.Add<Spinner.IHandler>(() => new SpinnerHandler());
			if (IsIpad)
				p.Add<Splitter.IHandler>(() => new SplitterHandler());
			p.Add<TabControl.IHandler>(() => new TabControlHandler());
			p.Add<TabPage.IHandler>(() => new TabPageHandler());
			p.Add<TextArea.IHandler>(() => new TextAreaHandler());
			p.Add<TextBox.IHandler>(() => new TextBoxHandler());
			p.Add<TreeGridView.IHandler>(() => new TreeGridViewHandler());
			//p.Add<TreeView.IHandler>(() => new TreeViewHandler ());
			p.Add<WebView.IHandler>(() => new WebViewHandler());
			p.Add<Navigation.IHandler>(() => new NavigationHandler());

			// Forms.Menu
			//p.Add<CheckMenuItem.IHandler>(() => new CheckMenuItemHandler ());
			//p.Add<ContextMenu.IHandler>(() => new ContextMenuHandler ());
			//p.Add<ImageMenuItem.IHandler>(() => new ImageMenuItemHandler ());
			//p.Add<MenuBar.IHandler>(() => new MenuBarHandler ());
			//p.Add<RadioMenuItem.IHandler>(() => new RadioMenuItemHandler ());
			//p.Add<SeparatorMenuItem.IHandler>(() => new SeparatorMenuItemHandler ());
			
			// Forms.Printing
			//p.Add<PrintDialog.IHandler>(() => new PrintDialogHandler ());
			//p.Add<PrintDocument.IHandler>(() => new PrintDocumentHandler ());
			//p.Add<PrintSettings.IHandler>(() => new PrintSettingsHandler ());
			
			// Forms.ToolBar
			//p.Add<CheckToolItem.IHandler>(() => new CheckToolItemHandler());
			//p.Add<RadioToolItem.IHandler>(() => new RadioToolItemHandler());
			p.Add<SeparatorToolItem.IHandler>(() => new SeparatorToolItemHandler());
			p.Add<ButtonToolItem.IHandler>(() => new ButtonToolItemHandler());
			p.Add<ToolBar.IHandler>(() => new ToolBarHandler());

			// Forms
			p.Add<Application.IHandler>(() => new ApplicationHandler());
			//p.Add<Clipboard.IHandler>(() => new ClipboardHandler ());
			//p.Add<ColorDialog.IHandler>(() => new ColorDialogHandler ());
			//p.Add<Cursor.IHandler>(() => new CursorHandler ());
			p.Add<Dialog.IHandler>(() => new DialogHandler<Dialog, Dialog.ICallback>());
			//p.Add<FontDialog.IHandler>(() => new FontDialogHandler ());
			p.Add<Form.IHandler>(() => UIApplication.SharedApplication.KeyWindow == null ? (Form.IHandler)new FormHandler() : new DialogHandler<Form, Form.ICallback>());
			p.Add<MessageBox.IHandler>(() => new MessageBoxHandler());
			//p.Add<OpenFileDialog.IHandler>(() => new OpenFileDialogHandler ());
			p.Add<PixelLayout.IHandler>(() => new PixelLayoutHandler());
			//p.Add<SaveFileDialog.IHandler>(() => new SaveFileDialogHandler ());
			//p.Add<SelectFolderDialog.IHandler>(() => new SelectFolderDialogHandler ());
			p.Add<TableLayout.IHandler>(() => new TableLayoutHandler());
			p.Add<UITimer.IHandler>(() => new UITimerHandler());
			p.Add<Clipboard.IHandler>(() => new ClipboardHandler());
			
			// IO
			//p.Add<SystemIcons.IHandler>(() => new SystemIconsHandler ());
			
			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
			p.Add<Thread.IHandler>(() => new ThreadHandler());
			p.Add<Screen.IScreensHandler>(() => new ScreensHandler());
		}

		public override IDisposable ThreadStart()
		{
			return new NSAutoreleasePool();
		}

		static void LinkingOverrides()
		{
			// Prevent linking some system code used via reflection in Eto.dll due to pcl restrictions
			Assembly.GetCallingAssembly();
		}
	}
}

