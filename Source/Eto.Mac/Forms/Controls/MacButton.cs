using Eto.Forms;
using Eto.Drawing;
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
	public abstract class MacButton<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler
		where TControl: NSButton
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public virtual string Text
		{
			get
			{
				return Control.Title;
			}
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				Control.SetTitleWithMnemonic(value ?? string.Empty);
				LayoutIfNeeded(oldSize);
			}
		}
	}
}

