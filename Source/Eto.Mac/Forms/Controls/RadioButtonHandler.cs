using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;


#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class RadioButtonHandler : MacButton<NSButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		static readonly object ControllerHelperKey = new object();

		RadioGroup Group
		{
			get { return Widget.Properties.Get<RadioGroup>(ControllerHelperKey); }
			set { Widget.Properties[ControllerHelperKey] = value; }
		}

		class RadioGroup
		{
			object lastChecked;
			readonly RadioButtonHandler controller;
			readonly List<RadioButtonHandler> buttons = new List<RadioButtonHandler>();

			public RadioGroup(RadioButtonHandler controller)
			{
				this.controller = controller;
				this.controller.Activated += RadioActivated;
			}

			public void AddButton(RadioButtonHandler button)
			{
				buttons.Add(button);
				button.Activated += RadioActivated;
			}

			void RadioActivated(object sender, EventArgs e)
			{
				var item = buttons.FirstOrDefault(r => r == lastChecked);
				if (item != null)
				{
					item.Callback.OnCheckedChanged(item.Widget, EventArgs.Empty);
				}
				lastChecked = sender;
			}
		}

		public event EventHandler<EventArgs> Activated;

		public class EtoRadioButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public RadioButtonHandler()
		{
			Control = new EtoRadioButton { Handler = this, Title = string.Empty };
			Control.SetButtonType(NSButtonType.Radio);
			Control.Activated += HandleActivated;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as RadioButtonHandler;
			if (handler != null)
			{
				handler.TriggerMouseCallback();

				if (handler.Activated != null)
					handler.Activated(handler, e);
				handler.Callback.OnClick(handler.Widget, EventArgs.Empty);
				handler.Callback.OnCheckedChanged(handler.Widget, EventArgs.Empty);

				if (handler.Control.AcceptsFirstResponder() && handler.Control.Window != null)
					handler.Control.Window.MakeFirstResponder(handler.Control);
			}
		}

		public void Create(RadioButton controller)
		{
			if (controller != null)
			{
				var controllerHandler = controller.Handler as RadioButtonHandler;
				if (controllerHandler != null)
				{
					var group = controllerHandler.Group;
					if (group == null)
					{
						group = controllerHandler.Group = new RadioGroup(controllerHandler);
					}
					group.AddButton(this);
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
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
					if (Activated != null)
						Activated(this, EventArgs.Empty);
				}
			}
		}
	}
}
