using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using sd = System.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public abstract class MacControl<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl: NSControl
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		internal Font font;

		public override NSView ContainerControl { get { return Control; } }

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public virtual Font Font
		{
			get
			{
				if (font == null)
					font = new Font(new FontHandler(Control.Font));
				return font;
			}
			set
			{
				font = value;
				Control.Font = font.ToNSFont();
				Control.AttributedStringValue = font.AttributedString(Control.AttributedStringValue);
				LayoutIfNeeded();
			}
		}
	}
}

