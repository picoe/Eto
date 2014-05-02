using System;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;
using Eto.Platform.iOS.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public abstract class IosButton<TControl, TWidget> : IosControl<TControl, TWidget>, ITextControl
		where TControl: UIButton
		where TWidget: Control
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
				Control.Font = font.ToUI ();
			}
		}
	}
}

