using System;
using System.Collections;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class RadioButtonHandler : MacButton<NSButton, RadioButton>, IRadioButton
	{
		List<RadioButton> group;

		public event EventHandler<EventArgs> Activated;

		public class EtoRadioButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return (object)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public RadioButtonHandler()
		{
			Control = new EtoRadioButton { Handler = this };
			Control.SetButtonType(NSButtonType.Radio);
			Control.Activated += HandleActivated;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as RadioButtonHandler;
			if (handler != null)
			{
				if (handler.Activated != null)
					handler.Activated(handler, e);
				handler.Widget.OnClick(EventArgs.Empty);
				handler.Widget.OnCheckedChanged(EventArgs.Empty);

				if (handler.Control.AcceptsFirstResponder() && handler.Control.Window != null) 
					handler.Control.Window.MakeFirstResponder(handler.Control);
			}
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
					controllerInner.Activated += controllerInner.control_RadioSwitch;
				}
				controllerInner.group.Add((RadioButton)this.Widget);
				this.Activated += controllerInner.control_RadioSwitch;
			}
		}

		private void control_RadioSwitch(object sender, EventArgs e)
		{
			if (group != null)
			{
				foreach (RadioButton item in group)
				{
					var c = item.ControlObject as NSButton;
					var ischecked = (item.Handler == sender);
					c.State = ischecked ? NSCellStateValue.On : NSCellStateValue.Off;
				}
			}
		}

		public bool Checked
		{
			get { return Control.State == NSCellStateValue.On; }
			set
			{ 
				if (value != Checked)
				{
					Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
					Widget.OnCheckedChanged(EventArgs.Empty);
					if (Activated != null)
						Activated(this, EventArgs.Empty);
					
				}
			}
		}
	}
}
