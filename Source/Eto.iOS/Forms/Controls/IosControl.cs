using System;
using Eto.Forms;
using MonoTouch.UIKit;
using Eto.iOS.Drawing;
using Eto.Drawing;
using MonoTouch.Foundation;

namespace Eto.iOS.Forms.Controls
{
	public class IosControl<T, W> : IosView<T, W>, IControl
		where T: UIView
		where W: Control
	{
		public override UIView ContainerControl { get { return Control; } }

	}
}

