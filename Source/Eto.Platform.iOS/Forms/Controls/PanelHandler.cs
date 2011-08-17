using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class PanelHandler : iosContainer<UIView, Panel>, IPanel
	{
		public PanelHandler ()
		{
			Control = new UIView();
		}
	}
}

