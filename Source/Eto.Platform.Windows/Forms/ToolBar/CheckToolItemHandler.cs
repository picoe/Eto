using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class CheckToolItemHandler : ToolItemHandler<SWF.ToolStripButton, CheckToolItem>, ICheckToolItem
	{
		readonly SWF.ToolStripButton control;

		public CheckToolItemHandler()
		{
			control = new SWF.ToolStripButton();
			control.Tag = this;
			control.Click += control_Click;
			Control = control;
		}

		void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public bool Checked
		{
			get { return control.Checked; }
			set { control.Checked = value; }
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, control);
		}


		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public override bool Enabled
		{
			get { return control.Enabled; }
			set { control.Enabled = value; }
		}
	}
}
