namespace Eto.WinForms.Forms.Controls
{
	public class RadioButtonHandler : WindowsControl<swf.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		List<RadioButtonHandler> group;
		RadioButton controller;

		public class EtoRadioButton : swf.RadioButton
		{
		}

		public RadioButtonHandler()
		{
			Control = new EtoRadioButton
			{
				TabStop = true,
				AutoSize = true,
				AutoCheck = false,
				Checked = false
			};

			Control.Click += Control_Click;
			Control.CheckedChanged += Control_CheckedChanged;
		}

		void Control_CheckedChanged(object sender, EventArgs e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);

		void Control_Click(object sender, EventArgs e)
		{
			if (Enabled)
				SetChecked(true);
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		public RadioButton Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerHandler = value?.Handler as RadioButtonHandler;
				if (controllerHandler?.group != null && ReferenceEquals(controllerHandler.group, group))
					return;

				group?.Remove(this);
				group = null;
				if (controllerHandler != null)
				{
					controllerHandler.group ??= new List<RadioButtonHandler> { controllerHandler };
					group = controllerHandler.group;
					group.Add(this);
				}

				if (Checked)
					SetChecked(true);
			}
		}

		void SetChecked(bool value)
		{
			if (group != null && value)
			{
				foreach (RadioButtonHandler item in group)
				{
					if (ReferenceEquals(item, this))
						continue;
					item.Checked = false;
				}
			}
			Control.Checked = value;
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { SetChecked(value); }
		}

		public bool UseMnemonic
		{
			get => Control.UseMnemonic;
			set => Control.UseMnemonic = value;
		}

		public bool AlwaysShowMnemonic
		{
			get => false;
			set { /* not supported */ }
		}

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
