using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;

namespace Eto.Platform.iOS.Forms
{
	public abstract class IosLayout<TControl, TWidget> : MacContainer<TControl, TWidget>, ILayout
		where TControl: UIView
		where TWidget: Layout
	{
		public override UIView ContainerControl { get { return Control; } }

	}
}
