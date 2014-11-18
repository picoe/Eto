using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.ToolBar
{
	public class SeparatorToolBarItemHandler : WidgetHandler<SWF.ToolStripSeparator, SeparatorToolItem>, SeparatorToolItem.IHandler, IToolBarItemHandler
	{
		public SeparatorToolBarItemHandler()
		{
			Control = new SWF.ToolStripSeparator();
		}

		public SeparatorToolItemType Type
		{
			get
			{
				return Control.AutoSize ? SeparatorToolItemType.Divider : SeparatorToolItemType.FlexibleSpace;
			}
			set
			{
				switch (value)
				{
					case SeparatorToolItemType.Divider:
						Control.AutoSize = true;
						break;
					default:
						Control.AutoSize = false;
						break;
				}
			}
		}

		public void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, Control);
		}

		public string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public Eto.Drawing.Image Image
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
