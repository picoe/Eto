using Eto.Forms;
using Eto.Drawing;


#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreText;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreText;
#endif

namespace Eto.Mac.Forms.Controls
{
	public abstract class MacButton<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler
		where TControl: NSButton
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		static readonly object textKey = new object();

		public virtual string Text
		{
			get { return Widget.Properties.Get<string>(textKey); }
			set
			{
				Widget.Properties[textKey] = value;
				SetText(value);
				InvalidateMeasure();
			}
		}

		static readonly object textColorKey = new object();

		public virtual Color TextColor
		{
			get { return Widget.Properties.Get<Color?>(textColorKey) ?? NSColor.ControlText.ToEto(); }
			set {
				if (value != TextColor)
				{
					Widget.Properties[textColorKey] = value;
					SetText(Text);
				}
			}
		}

		void SetText(string text)
		{
			Control.Title = MacConversions.StripAmpersands(text ?? string.Empty);
			var color = Widget.Properties.Get<Color?>(textColorKey);
			if (color != null)
			{
				var attr = NSDictionary.FromObjectAndKey(color.Value.ToNSUI(), NSStringAttributeKey.ForegroundColor);
				var str = new NSMutableAttributedString(Control.AttributedTitle);
				str.AddAttributes(attr, new NSRange(0, str.Length));
				Control.AttributedTitle = str;
			}
		}
	}
}

