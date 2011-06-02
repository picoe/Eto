using System;
using SWF = System.Windows.Forms;

namespace Eto.Forms.WXWidgets
{
	public class WXGenerator : Generator
	{
		int buttonID = 1;

		public WXGenerator()
		{
			AddControlGenerator(typeof(IApplication), typeof(ApplicationHandler));
			AddControlGenerator(typeof(IButton), typeof(ButtonHandler));
			AddControlGenerator(typeof(ICheckBox), typeof(CheckBoxHandler));
			AddControlGenerator(typeof(IForm), typeof(FormHandler));
			AddControlGenerator(typeof(IGroupBox), typeof(GroupBoxHandler));
			AddControlGenerator(typeof(ITextBox), typeof(TextBoxHandler));
			AddControlGenerator(typeof(IMessageBox), typeof(MessageBoxHandler));
			AddControlGenerator(typeof(ISplitter), typeof(SplitterHandler));
			AddControlGenerator(typeof(IPanel), typeof(PanelHandler));
			AddControlGenerator(typeof(IPixelLayout), typeof(PixelLayoutHandler));
			AddControlGenerator(typeof(IDockLayout), typeof(DockLayoutHandler));
			AddControlGenerator(typeof(ITableLayout), typeof(TableLayoutHandler), new Type[] { typeof(Widget), typeof(int), typeof(int) });
			AddControlGenerator(typeof(ITabControl), typeof(TabControlHandler));
			AddControlGenerator(typeof(ITabPage), typeof(TabPageHandler));
			AddControlGenerator(typeof(IMenuBar), typeof(MenuBarHandler));
			AddControlGenerator(typeof(IMenuItem), typeof(MenuItemHandler));

			AddControlGenerator(typeof(IBitmap), typeof(BitmapHandler));
			AddControlGenerator(typeof(IDrawable), typeof(DrawableHandler));
		}

		internal int GetNextButtonID()
		{
			return buttonID++;
		}
	}
}
