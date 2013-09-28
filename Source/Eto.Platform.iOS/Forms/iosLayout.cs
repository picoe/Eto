using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;

namespace Eto.Platform.iOS.Forms
{
	public abstract class iosLayout<T, W> : MacContainer<T, W>, ILayout
		where T: UIView
		where W: Layout
	{
		public override UIView ContainerControl { get { return Control; } }

	}
}
