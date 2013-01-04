using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using System.Text.RegularExpressions;
using Eto.Forms;
using swi = System.Windows.Input;
using swm = System.Windows.Media;
using sw = System.Windows;
using System.Reflection;
using System.IO;
using Eto.Platform.Wpf.Drawing;
using Eto.Platform.Wpf.Forms.Menu;
using Eto.Platform.Wpf.Forms.Controls;
using Eto.Platform.Wpf.Forms.Printing;
using Eto.Platform.Wpf.Forms;
using Eto.IO;

namespace Eto.Platform.Wpf
{
	public class Generator : Eto.Generator
	{
		public override string ID
		{
			get { return Generators.Wpf; }
		}

		static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly> ();

		static Generator ()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
				var assemblyName = new AssemblyName (args.Name);
				if (assemblyName.Name.EndsWith (".resources")) return null;

				string resourceName = "Eto.Platform.Wpf.CustomControls.Assemblies." + assemblyName.Name + ".dll";
				Assembly assembly = null;
				lock (loadedAssemblies) {
					if (!loadedAssemblies.TryGetValue (resourceName, out assembly)) {
						using (var stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourceName)) {
							if (stream != null) {
								using (var binaryReader = new BinaryReader (stream)) {
									assembly = Assembly.Load (binaryReader.ReadBytes ((int)stream.Length));
									loadedAssemblies.Add (resourceName, assembly);
								}
							}
						}
					}
				}
				return assembly;
			};
		}

		public Generator ()
		{
			AddTo(this);

			// by default, use WinForms web view (it has more features we can control)
			UseSwfWebView(this);
		}

		public static void AddTo(Eto.Generator g)
		{
			// Drawing
			g.Add <IBitmap> (() => new BitmapHandler ());
			g.Add <IFontFamily> (() => new FontFamilyHandler ());
			g.Add <IFont> (() => new FontHandler ());
			g.Add <IFonts> (() => new FontsHandler ());
			g.Add <IGraphics> (() => new GraphicsHandler ());
			g.Add <IGraphicsPath> (() => new GraphicsPathHandler ());
			g.Add <IIcon> (() => new IconHandler ());
			g.Add <IIndexedBitmap> (() => new IndexedBitmapHandler ());
			g.Add <IMatrixHandler> (() => new MatrixHandler ());
			g.Add <IPen>(() => new PenHandler());
			g.Add <IBrush>(() => new BrushHandler());
			
			// Forms.Cells
			g.Add <ICheckBoxCell> (() => new CheckBoxCellHandler ());
			g.Add <IComboBoxCell> (() => new ComboBoxCellHandler ());
			g.Add <IImageTextCell> (() => new ImageTextCellHandler ());
			g.Add <IImageViewCell> (() => new ImageViewCellHandler ());
			g.Add <ITextBoxCell> (() => new TextBoxCellHandler ());
			
			// Forms.Controls
			g.Add <IButton> (() => new ButtonHandler ());
			g.Add <ICheckBox> (() => new CheckBoxHandler ());
			g.Add <IComboBox> (() => new ComboBoxHandler ());
			g.Add <IDateTimePicker> (() => new DateTimePickerHandler ());
			g.Add <IDrawable> (() => new DrawableHandler ());
			g.Add <IGridColumn> (() => new GridColumnHandler ());
			g.Add <IGridView> (() => new GridViewHandler ());
			g.Add <IGroupBox> (() => new GroupBoxHandler ());
			g.Add <IImageView> (() => new ImageViewHandler ());
			g.Add <ILabel> (() => new LabelHandler ());
			g.Add <IListBox> (() => new ListBoxHandler ());
			g.Add <INumericUpDown> (() => new NumericUpDownHandler ());
			g.Add <IPanel> (() => new PanelHandler ());
			g.Add <IPasswordBox> (() => new PasswordBoxHandler ());
			g.Add <IProgressBar> (() => new ProgressBarHandler ());
			g.Add <IRadioButton> (() => new RadioButtonHandler ());
			g.Add <IScrollable> (() => new ScrollableHandler ());
			g.Add <ISlider> (() => new SliderHandler ());
			g.Add <ISplitter> (() => new SplitterHandler ());
			g.Add <ITabControl> (() => new TabControlHandler ());
			g.Add <ITabPage> (() => new TabPageHandler ());
			g.Add <ITextArea> (() => new TextAreaHandler ());
			g.Add <ITextBox> (() => new TextBoxHandler ());
			g.Add <ITreeGridView> (() => new TreeGridViewHandler ());
			g.Add <ITreeView> (() => new TreeViewHandler ());
			//g.Add <IWebView> (() => new WebViewHandler ());
			
			// Forms.Menu
			g.Add <ICheckMenuItem> (() => new CheckMenuItemHandler ());
			g.Add <IContextMenu> (() => new ContextMenuHandler ());
			g.Add <IImageMenuItem> (() => new ImageMenuItemHandler ());
			g.Add <IMenuBar> (() => new MenuBarHandler ());
			g.Add <IRadioMenuItem> (() => new RadioMenuItemHandler ());
			g.Add <ISeparatorMenuItem> (() => new SeparatorMenuItemHandler ());
			
			// Forms.Printing
			g.Add <IPrintDialog> (() => new PrintDialogHandler ());
			g.Add <IPrintDocument> (() => new PrintDocumentHandler ());
			g.Add <IPrintSettings> (() => new PrintSettingsHandler ());
			
			// Forms.ToolBar
			g.Add <ICheckToolBarButton> (() => new CheckToolBarButtonHandler ());
			g.Add <ISeparatorToolBarItem> (() => new SeparatorToolBarItemHandler ());
			g.Add <IToolBarButton> (() => new ToolBarButtonHandler ());
			g.Add <IToolBar> (() => new ToolBarHandler ());
			
			// Forms
			g.Add <IApplication> (() => new ApplicationHandler ());
			//g.Add <IClipboard> (() => new ClipboardHandler ());
			g.Add <IColorDialog> (() => new ColorDialogHandler ());
			g.Add <ICursor> (() => new CursorHandler ());
			g.Add <IDialog> (() => new DialogHandler ());
			g.Add <IDockLayout> (() => new DockLayoutHandler ());
			g.Add <IFontDialog> (() => new FontDialogHandler ());
			g.Add <IForm> (() => new FormHandler ());
			g.Add <IMessageBox> (() => new MessageBoxHandler ());
			g.Add <IOpenFileDialog> (() => new OpenFileDialogHandler ());
			g.Add <IPixelLayout> (() => new PixelLayoutHandler ());
			g.Add <ISaveFileDialog> (() => new SaveFileDialogHandler ());
			g.Add <ISelectFolderDialog> (() => new SelectFolderDialogHandler ());
			g.Add <ITableLayout> (() => new TableLayoutHandler ());
			g.Add <IUITimer> (() => new UITimerHandler ());
			
			// IO
			//g.Add <ISystemIcons> (() => new SystemIconsHandler ());
			
			// General
			g.Add <IEtoEnvironment> (() => new EtoEnvironmentHandler ());
		}

		public static void UseWpfWebView (Eto.Generator g)
		{
			g.Add<IWebView>(() => new WpfWebViewHandler());
		}

		public static void UseSwfWebView(Eto.Generator g)
		{
			g.Add<IWebView> (() => new SwfWebViewHandler ());
		}
	}
}
