using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Platform.Mac.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class PanelHandler : MacPanel<UIView, Panel>, IPanel
	{
		public override UIView ContainerControl { get { return Control; } }

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

