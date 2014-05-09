using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public abstract class MacText<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler
		where TControl: NSTextField
		where TWidget: TextControl
		where TCallback: TextControl.ICallback
	{
		public override Color BackgroundColor
		{
			get { return Control.BackgroundColor.ToEto(); }
			set { Control.BackgroundColor = value.ToNSUI(); }
		}

		public virtual string Text
		{
			get { return Control.AttributedStringValue.Value; }
			set { Control.AttributedStringValue = Font.AttributedString(value ?? string.Empty, Control.AttributedStringValue); }
		}
	}
}

