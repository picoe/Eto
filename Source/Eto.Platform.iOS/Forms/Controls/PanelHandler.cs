using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class PanelHandler : iosContainer<UIView, Panel>, IPanel
	{
		public override UIView CreateControl ()
		{
			return new UIView();
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.BackgroundColor = UIColor.White;
		}
	}
}

