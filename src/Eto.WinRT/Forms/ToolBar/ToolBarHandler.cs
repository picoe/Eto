#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using swi = Windows.UI.Xaml.Input;
using Eto.Forms;

namespace Eto.WinRT.Forms
{
	public class ToolBarHandler : WidgetHandler<swc.ToolBar, ToolBar>, IToolBar
	{
		public override swc.ToolBar CreateControl()
		{
			var control = new swc.ToolBar { IsTabStop = false };
			swi.KeyboardNavigation.SetTabNavigation(control, swi.KeyboardNavigationMode.Continue);
			return control;
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
#endif