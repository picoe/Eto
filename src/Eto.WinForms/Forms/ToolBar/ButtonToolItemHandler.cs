using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.ToolBar
{
	public class ButtonToolItemHandler : ToolItemHandler<SWF.ToolStripButton, ButtonToolItem>, ButtonToolItem.IHandler
	{
		public ButtonToolItemHandler()
		{
			Control = new SWF.ToolStripButton();
			Control.Tag = this;
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, Control);
		}
	}
}
