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

	public class ButtonToolItemHandler : ToolItemHandler<UIBarButtonItem, ButtonToolItem>, ButtonToolItem.IHandler
	{
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
	
}
