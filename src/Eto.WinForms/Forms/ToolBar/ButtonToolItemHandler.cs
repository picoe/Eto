namespace Eto.WinForms.Forms.ToolBar
{
	public class ButtonToolItemHandler : ToolItemHandler<swf.ToolStripButton, ButtonToolItem>, ButtonToolItem.IHandler
	{
		public ButtonToolItemHandler()
		{
			Control = new swf.ToolStripButton();
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
