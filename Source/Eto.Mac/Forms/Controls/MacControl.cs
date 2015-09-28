using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
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
		public override NSView ContainerControl { get { return Control; } }

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		internal static readonly object Font_Key = new object();

		public virtual Font Font
		{
			get { return Widget.Properties.Create(Font_Key, () => new Font(new FontHandler(Control.Font))); }
			set
			{
				Widget.Properties.Set(Font_Key, value, () =>
				{
					Control.Font = value.ToNS();
					Control.AttributedStringValue = value.AttributedString(Control.AttributedStringValue);
					LayoutIfNeeded();
				});
			}
		}
	}
}

