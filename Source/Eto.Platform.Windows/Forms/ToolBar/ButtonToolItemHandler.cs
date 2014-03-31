using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ButtonToolItemHandler : ToolItemHandler<SWF.ToolStripButton, ButtonToolItem>, IButtonToolItem
	{
		readonly SWF.ToolStripButton control;

		public ButtonToolItemHandler()
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

		public override bool Enabled
		{
			get { return control.Enabled; }
			set { control.Enabled = value; }
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, control);
		}

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
