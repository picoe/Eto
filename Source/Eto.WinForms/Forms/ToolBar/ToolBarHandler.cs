using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.ToolBar
{
	public class ToolBarHandler : WindowsControl<swf.ToolStrip, Eto.Forms.ToolBar, Eto.Forms.ToolBar.ICallback>, Eto.Forms.ToolBar.IHandler
	{
		public ToolBarHandler()
		{
			Control = new swf.ToolStrip();
			Control.LayoutStyle = swf.ToolStripLayoutStyle.StackWithOverflow;
			Control.AutoSize = true;
		}

		public void AddButton(ToolItem item, int index)
		{
			((IToolBarItemHandler)item.Handler).CreateControl(this, index);
		}

		public void Clear()
		{
			Control.Items.Clear();
		}

		public void RemoveButton(ToolItem item)
		{
			Control.Items.Remove((swf.ToolStripItem)item.ControlObject);
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				/*switch (control.TextAlign)
				{
					case swf.ToolBarTextAlign.Right:
						return ToolBarTextAlign.Right;
					default:
					case swf.ToolBarTextAlign.Underneath:
						return ToolBarTextAlign.Underneath;
				}
				 */
				return ToolBarTextAlign.Underneath;
			}
			set
			{
				switch (value)
				{
					case ToolBarTextAlign.Right:
						//control.TextAlign = swf.ToolBarTextAlign.Right;
						break;
					case ToolBarTextAlign.Underneath:
						//control.TextAlign = swf.ToolBarTextAlign.Underneath;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}
	}
}
