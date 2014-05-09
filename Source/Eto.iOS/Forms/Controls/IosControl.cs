using System;
using Eto.Forms;
using MonoTouch.UIKit;
using Eto.iOS.Drawing;
using Eto.Drawing;
using MonoTouch.Foundation;

namespace Eto.iOS.Forms.Controls
{
	public class IosControl<TControl, TWidget, TCallback> : IosView<TControl, TWidget, TCallback>, IControl
		where TControl: UIView
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public override UIView ContainerControl { get { return Control; } }

	}
}

