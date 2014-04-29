using System;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	// TODO: use UISwitch instead with custom state images, better accessibility
	public class RadioButtonHandler : IosButton<UIButton, RadioButton>, IRadioButton
	{
		RadioButtonHandler controller;
		List<RadioButtonHandler> children;

		public void Create (RadioButton controller)
		{
			this.controller = (RadioButtonHandler)(controller != null ? controller.Handler : this);
			if (this.controller != this)
				this.controller.children.Add (this);
			else
				children = new List<RadioButtonHandler>();
		}

		public override UIButton CreateControl ()
		{
			return new UIButton(UIButtonType.RoundedRect);
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.ShowsTouchWhenHighlighted = false;
			Control.TouchUpInside += (sender, e) => {
				this.Checked = true;
				Widget.OnCheckedChanged(EventArgs.Empty);
			};
		}

		public bool Checked {
			get { return Control.Selected; }
			set { 
				Control.Highlighted = value;
				if (value && controller != null) {
					foreach (var b in controller.children.Where(r => r != this)) {
						b.Control.Highlighted = false;
					}
				}
			}
		}
	}
}

