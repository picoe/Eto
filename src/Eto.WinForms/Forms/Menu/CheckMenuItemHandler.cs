namespace Eto.WinForms.Forms.Menu
{
	public class CheckMenuItemHandler : MenuItemHandler<swf.ToolStripMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
		public CheckMenuItemHandler()
		{
			Control = new swf.ToolStripMenuItem();
			Control.Click += (sender, e) =>
			{
				Control.Checked = !Control.Checked;
				Callback.OnClick(Widget, e);
			};
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case CheckMenuItem.CheckedChangedEvent:
					Control.CheckedChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
