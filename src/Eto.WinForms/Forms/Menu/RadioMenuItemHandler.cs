namespace Eto.WinForms.Forms.Menu
{
	public class RadioMenuItemHandler : MenuItemHandler<swf.ToolStripMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		List<RadioMenuItem> group;
		RadioMenuItem controller;

		public RadioMenuItemHandler()
		{
			Control = new swf.ToolStripMenuItem();
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			UncheckGroup();
			Checked = true;
			Callback.OnClick(Widget, e);
		}

		void UncheckGroup()
		{
			if (group != null)
			{
				var checkedItem = group.FirstOrDefault(r => r.Checked && r != Widget);
				if (checkedItem != null)
					checkedItem.Checked = false;
			}
		}

		public RadioMenuItem Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerInner = value?.Handler as RadioMenuItemHandler;
				if (controllerInner?.group != null && ReferenceEquals(controllerInner.group, group))
					return;

				group?.Remove(Widget);
				group = null;
				if (controllerInner != null)
				{
					controllerInner.group ??= new List<RadioMenuItem> { value };
					group = controllerInner.group;
					group.Add(Widget);
				}

				if (Checked)
					UncheckGroup();
			}
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set
			{
				if (value)
					UncheckGroup();
                Control.Checked = value;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case RadioMenuItem.CheckedChangedEvent:
					Control.CheckedChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
