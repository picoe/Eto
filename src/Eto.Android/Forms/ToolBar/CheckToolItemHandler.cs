using aw = Android.Widget;

namespace Eto.Android.Forms.ToolBar
{
	public class CheckToolItemHandler : ToolItemHandler<aw.ToggleButton, CheckToolItem>, CheckToolItem.IHandler
	{
		protected override aw.ToggleButton GetInnerControl(ToolBarHandler handler)
		{
			if (!HasControl)
			{
				Control = new aw.ToggleButton(Platform.AppContextThemed);
				Control.Click += control_Click;
				Control.CheckedChange += Control_CheckedChange;
			}

			return Control;
		}

		protected override void SetEverything()
		{
			base.SetEverything();
			SetChecked(Checked);
		}

		private void Control_CheckedChange(object sender, aw.CompoundButton.CheckedChangeEventArgs e)
		{
			@checked = Control.Checked;
			Widget.OnCheckedChanged(EventArgs.Empty);
		}

		private void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public bool Checked
		{
			get => @checked;
			set
			{
				if (@checked == value)
					return;
				
				@checked = value;

				if (HasControl)
					SetChecked(value);
			}
		}

		private void SetChecked(bool value)
		{
			Control.Checked = value;
		}

		private bool @checked;
	}
}
