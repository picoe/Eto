using System;
using System.Web;
using Eto.Platform.Web.Forms;
using Eto.Platform.Windows.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Web;

namespace Eto.Platform.Web
{
	public class Generator : Eto.Generator
	{
		public Generator()
		{
			base.Add(typeof(IBitmap), typeof(BitmapHandler));
			base.Add(typeof(IGraphics), typeof(GraphicsHandler));
			base.Add(typeof(IIcon), typeof(IconHandler));
			base.Add(typeof(IIndexedBitmap), typeof(IndexedBitmapHandler));

			//Add(typeof(IApplication), typeof(ApplicationHandler));
			Add(typeof(IButton), typeof(ButtonHandler));
			Add(typeof(ICheckBox), typeof(CheckBoxHandler));
			//Add(typeof(IForm), typeof(FormHandler));
			//Add(typeof(IGroupBox), typeof(GroupBoxHandler));
			Add(typeof(ITextBox), typeof(TextBoxHandler));
			Add(typeof(IToolBar), typeof(ToolBarHandler));
			Add(typeof(IToolBarButton), typeof(ToolBarButtonHandler));
			//Add(typeof(IMessageBox), typeof(MessageBoxHandler));
			//Add(typeof(ISplitter), typeof(SplitterHandler));
			//Add(typeof(IPanel), typeof(PanelHandler));
			//Add(typeof(IPixelLayout), typeof(PixelLayoutHandler));
			//Add(typeof(IDockLayout), typeof(DockLayoutHandler));
			//Add(typeof(ITableLayout), typeof(TableLayoutHandler));
			//Add(typeof(ITabControl), typeof(TabControlHandler));
			//Add(typeof(ITabPage), typeof(TabPageHandler));
			//Add(typeof(IMenuBar), typeof(MenuBarHandler));
			//Add(typeof(IMenuItem), typeof(MenuItemHandler));
			
			Configuration config = Configuration.Get(HttpContext.Current);
			config.VirtualPages.Add(new VirtualPage("Resources", typeof(ImageSource)));
		}
	}
}
