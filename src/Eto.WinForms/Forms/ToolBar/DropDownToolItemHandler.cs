using Eto.Forms;
using System;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<swf.ToolStripDropDownButton, DropDownToolItem>, DropDownToolItem.IHandler
	{
		ContextMenu contextMenu;

		public DropDownToolItemHandler()
		{
			Control = new swf.ToolStripDropDownButton();
			Control.Tag = this;
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, Control);
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set {
				contextMenu = value;
				Control.DropDown = (contextMenu == null) ? null : contextMenu.ControlObject as swf.ContextMenuStrip;
			}
		}
	}
}
