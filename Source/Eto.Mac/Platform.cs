using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.Mac.Drawing;
using Eto.Mac.IO;
using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms.Printing;
using Eto.Mac.Forms;
using Eto.Mac.Forms.Menu;
using Eto.Mac.Threading;
using Eto.Threading;
using Eto.Mac.Forms.Cells;
using Eto.Mac.Forms.ToolBar;
using Eto.Shared.Forms;
using Eto.Forms.ThemedControls;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac
{

	[Preserve(AllMembers = true)]
	public class Platform : Eto.Platform
	{
		static bool initialized;

		public override bool IsDesktop { get { return true; } }

		public override bool IsMac { get { return true; } }

		#if XAMMAC
		public override string ID { get { return "xammac"; } }
		#else
		public override string ID { get { return "mac"; } }
		#endif

		public override PlatformFeatureFlags SupportedFeatures =>
			PlatformFeatureFlags.DrawableWithTransparentContent;

		public Platform()
		{
			#if Mac64
			unsafe
			{
				if (sizeof(IntPtr) != 8)
					throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "You can only run this platform in 64-bit mode. Use the 32-bit Eto.Mac platform instead."));
			}
			#endif
			if (!initialized)
			{
				var appType = typeof(NSApplication);
				var initField = appType.GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic);
				if (initField == null || Equals(initField.GetValue(null), false))
					NSApplication.Init();
				// until everything is marked as thread safe correctly in monomac
				// e.g. overriding NSButtonCell.DrawBezelWithFrame will throw an exception
				NSApplication.CheckForIllegalCrossThreadCalls = false;

				initialized = true;
			}
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

			// Forms.Cells
			p.Add<CheckBoxCell.IHandler>(() => new CheckBoxCellHandler());
			p.Add<ComboBoxCell.IHandler>(() => new ComboBoxCellHandler());
			p.Add<CustomCell.IHandler>(() => new CustomCellHandler());
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
			p.Add<Scrollable.IHandler>(() => new ScrollableHandler());
			p.Add<SearchBox.IHandler>(() => new SearchBoxHandler());
			p.Add<Slider.IHandler>(() => new SliderHandler());
			p.Add<Spinner.IHandler>(() => new SpinnerHandler());
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
			p.Add<Stepper.IHandler>(() => new StepperHandler());
			p.Add<TextStepper.IHandler>(() => new ThemedTextStepperHandler());
			p.Add<FilePicker.IHandler>(() => new ThemedFilePickerHandler());

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
			p.Add<PixelLayout.IHandler>(() => new PixelLayoutHandler());
			p.Add<SaveFileDialog.IHandler>(() => new SaveFileDialogHandler());
			p.Add<SelectFolderDialog.IHandler>(() => new SelectFolderDialogHandler());
			p.Add<TableLayout.IHandler>(() => new TableLayoutHandler());
			p.Add<UITimer.IHandler>(() => new UITimerHandler());
			p.Add<Mouse.IHandler>(() => new MouseHandler());
			p.Add<Screen.IScreensHandler>(() => new ScreensHandler());
			p.Add<Keyboard.IHandler>(() => new KeyboardHandler());
			p.Add<FixedMaskedTextProvider.IHandler>(() => new FixedMaskedTextProviderHandler());
			p.Add<OpenWithDialog.IHandler>(() => new OpenWithDialogHandler());

			// IO
			p.Add<SystemIcons.IHandler>(() => new SystemIconsHandler());

			// General
			p.Add<EtoEnvironment.IHandler>(() => new EtoEnvironmentHandler());
			p.Add<Thread.IHandler>(() => new ThreadHandler());
		}

		public override IDisposable ThreadStart()
		{
			return new NSAutoreleasePool();
		}

		public override bool IsValid
		{
			get
			{
				var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
				return assembly.Location.StartsWith(NSBundle.MainBundle.BundlePath, StringComparison.Ordinal);
			}
		}

		static void LinkingOverrides()
		{
			// Prevent linking system code used via reflection in Eto.dll due to pcl restrictions
			Assembly.GetCallingAssembly();
		}
	}
}
