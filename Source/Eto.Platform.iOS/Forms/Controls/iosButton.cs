using System;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;
using Eto.Platform.iOS.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public abstract class iosButton<T, W> : iosControl<T, W>, ITextControl
		where T: UIButton
		where W: Control
	{	
		Font font;

		public virtual string Text {
			get {
				return Control.Title(UIControlState.Normal);
			}
			set {
				Control.SetTitle(value, UIControlState.Normal);
			}
		}

		public override Eto.Drawing.Font Font {
			get { return font; }
			set {
				font = value;
				Control.Font = font.ToUIFont ();
			}
		}
	}
}

