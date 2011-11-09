using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Mac
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
				return Generator.Convert(Control.BackgroundColor);
			}
			set {
				Control.BackgroundColor = Generator.ConvertNS(value);
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

