using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Eto.Drawing;
using Eto.Mac.Forms;

namespace Eto.iOS.Forms
{
	public abstract class IosLayout<TControl, TWidget, TCallback> : MacContainer<TControl, TWidget, TCallback>, ILayout
		where TControl: UIView
		where TWidget: Layout
		where TCallback: Layout.ICallback
	{
		public override UIView ContainerControl { get { return Control; } }

	}
}
