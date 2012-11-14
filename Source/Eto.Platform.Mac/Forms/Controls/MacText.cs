using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public abstract class MacText<T, W> : MacControl<T, W>, ITextControl
		where T: NSTextField
		where W: TextControl
	{
		public MacText ()
		{
		}
		
		public override Color BackgroundColor {
			get {
				return Control.BackgroundColor.ToEto ();
			}
			set {
				Control.BackgroundColor = value.ToNS ();
			}
		}
		
		public virtual string Text {
			get {
				return Control.StringValue;
			}
			set {
				Control.StringValue = value ?? string.Empty;
			}
		}
		
	}
}

