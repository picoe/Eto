using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using swi = System.Windows.Input;

namespace Eto.Wpf.Forms.ToolBar
{
	public class ToolBarHandler : WpfControl<swc.ToolBar, Eto.Forms.ToolBar, Eto.Forms.ToolBar.ICallback>, Eto.Forms.ToolBar.IHandler
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

		public void Clear()
		{
			Control.Items.Clear();
		}

		public void RemoveButton(ToolItem button)
		{
			Control.Items.Remove(button.ControlObject);
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
	}
}
