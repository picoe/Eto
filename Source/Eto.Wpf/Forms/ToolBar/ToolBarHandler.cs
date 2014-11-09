using swc = System.Windows.Controls;
using swi = System.Windows.Input;
using Eto.Forms;

namespace Eto.Wpf.Forms.ToolBar
{
	public class ToolBarHandler : WidgetHandler<swc.ToolBar, Eto.Forms.ToolBar>, Eto.Forms.ToolBar.IHandler
	{
		public ToolBarHandler()
		{
			Control = new swc.ToolBar { IsTabStop = false, Tag = this };
			swi.KeyboardNavigation.SetTabNavigation(Control, swi.KeyboardNavigationMode.Continue);
		}

		public void AddButton(ToolItem button, int index)
		{
			Control.Items.Insert(index, button.ControlObject);
		}

		public void RemoveButton(ToolItem button)
		{
			Control.Items.Remove(button.ControlObject);
		}

		public void Clear()
		{
			Control.Items.Clear();
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				return ToolBarTextAlign.Underneath;
			}
			set
			{
			}
		}

		public ToolBarDock Dock
		{
			get
			{
				return ToolBarDock.Top;
			}
			set
			{
			}
		}
	}
}
