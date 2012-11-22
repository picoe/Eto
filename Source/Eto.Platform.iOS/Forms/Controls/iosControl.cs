using System;
using Eto.Forms;
using MonoTouch.UIKit;
using Eto.Platform.iOS.Drawing;
using Eto.Drawing;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class iosControl<T, W> : iosView<T, W>, IControl
		where T: UIView
		where W: Control
	{

	}
}

