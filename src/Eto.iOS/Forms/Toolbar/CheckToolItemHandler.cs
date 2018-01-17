using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using ObjCRuntime;
using UIKit;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Toolbar
{

	public class CheckToolItemHandler : ToolItemHandler<UIBarButtonItem, CheckToolItem>, CheckToolItem.IHandler
	{
		public bool Checked
		{
			get { return Button.Selected; }
			set
			{
				if (value != Button.Selected)
				{
					Button.Selected = value;
					Widget.OnCheckedChanged(EventArgs.Empty);
				}
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Button.ShowsTouchWhenHighlighted = false;
			Selectable = true;
		}

		public override void InvokeButton()
		{
			Checked = !Checked;
			Widget.OnClick(EventArgs.Empty);
		}
	}
	
}
