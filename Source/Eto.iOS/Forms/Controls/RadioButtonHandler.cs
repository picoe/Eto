using System;
using UIKit;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	// TODO: use UISwitch instead with custom state images, better accessibility
	public class RadioButtonHandler : IosButton<UIButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		RadioButtonHandler controller;
		List<RadioButtonHandler> children;

		public RadioButtonHandler()
		{
			Control = new UIButton(UIButtonType.RoundedRect);
		}

		public void Create(RadioButton controller)
		{
			this.controller = (RadioButtonHandler)(controller != null ? controller.Handler : this);
			if (this.controller != this)
				this.controller.children.Add(this);
			else
				children = new List<RadioButtonHandler> { this };
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.ShowsTouchWhenHighlighted = false;
			Control.TouchUpInside += (sender, e) => ApplicationHandler.Instance.AsyncInvoke(() => Checked = true);
		}

		public bool Checked
		{
			get { return Control.Selected; }
			set
			{ 
				if (Control.Selected != value)
				{
					Control.Selected = value;
					if (!Platform.IsIos7)
						Control.Highlighted = value;
					if (value && controller != null)
					{
						foreach (var b in controller.children.Where(r => r != this))
						{
							b.Checked = false;
						}
					}
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}
	}
}

