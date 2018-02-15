using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.ToolBar
{
	public class CheckToolItemHandler : ToolItemHandler<SWF.ToolStripButton, CheckToolItem>, CheckToolItem.IHandler
	{
		public CheckToolItemHandler()
		{
			Control = new SWF.ToolStripButton();
			Control.Tag = this;
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			Control.Checked = !Control.Checked;
			Widget.OnClick(EventArgs.Empty);
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
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
	}
}
