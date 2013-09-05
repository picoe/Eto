using System;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows
{
	public class RadioButtonHandler : WindowsControl<SWF.RadioButton, RadioButton>, IRadioButton
	{
		List<RadioButton> group;
		
		public RadioButtonHandler()
		{
			Control = new SWF.RadioButton();
			Control.AutoSize = true;
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);	
			};
			Control.CheckedChanged += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
		}

		public void Create(RadioButton controller)
		{
			if (controller != null)
			{
				RadioButtonHandler controllerInner = (RadioButtonHandler)controller.Handler;
				if (controllerInner.group == null)
				{
					controllerInner.group = new List<RadioButton>();
					controllerInner.group.Add(controller);
					controllerInner.Control.Click += controllerInner.control_RadioSwitch;
				}
				controllerInner.group.Add((RadioButton)this.Widget);
				Control.Click += controllerInner.control_RadioSwitch;
			}
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
	}
}
