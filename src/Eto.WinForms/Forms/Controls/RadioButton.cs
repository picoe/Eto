using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.WinForms.Forms.Controls
{
	public class RadioButtonHandler : WindowsControl<swf.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		List<RadioButtonHandler> group;

		public class EtoRadioButton : swf.RadioButton
		{
			public EtoRadioButton()
			{
				this.SetStyle(swf.ControlStyles.StandardClick | swf.ControlStyles.StandardDoubleClick, true);
			}
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
		}

		public void Create(RadioButton controller)
		{
			var controllerHandler = controller?.Handler as RadioButtonHandler;
			if (controllerHandler != null)
			{
				if (controllerHandler.group == null)
				{
					controllerHandler.group = new List<RadioButtonHandler>();
					controllerHandler.group.Add(controllerHandler);
				}
				group = controllerHandler.group;
				group.Add(this);
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

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
