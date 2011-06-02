using System;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Controls
{
	public abstract class iosButton<T, W> : iosControl<T, W>, ITextControl
		where T: UIButton
		where W: Control
	{	

		public virtual string Text {
			get {
				return Control.Title(UIControlState.Normal);
			}
			set {
				Control.SetTitle(value, UIControlState.Normal);
			}
		}

	}
}

