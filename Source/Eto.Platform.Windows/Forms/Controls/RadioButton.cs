using System;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class RadioButtonHandler : WindowsControl<SWF.RadioButton, RadioButton>, IRadioButton
	{
		ArrayList group;
		
		public RadioButtonHandler()
		{
			Control = new SWF.RadioButton();
			Control.Click += control_Click;
			Control.CheckedChanged += control_CheckedChanged;
		}

		public void Create(RadioButton controller)
		{
			
			if (controller != null)
			{
				RadioButtonHandler controllerInner = (RadioButtonHandler)controller.Handler;
				if (controllerInner.group == null)
				{
					controllerInner.group = new ArrayList();
					controllerInner.group.Add(controller);
					controllerInner.Control.Click += controllerInner.control_RadioSwitch;
				}
				controllerInner.group.Add((RadioButton)this.Widget);
				Control.Click += controllerInner.control_RadioSwitch;
				Control.CheckedChanged += control_CheckedChanged;
			}
		}

		private void control_CheckedChanged(object sender, EventArgs e)
		{
			Widget.OnCheckedChanged(e);
		}
		
		private void control_RadioSwitch(object sender, EventArgs e)
		{
			if (group != null)
			{
				foreach (RadioButton item in group)
				{
					item.Checked = (item.ControlObject == sender);
				}
			}
		}
		
		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}
		
		private void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(e);
		}
	}
}
