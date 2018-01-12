using Eto.Forms;
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
	public class PanelHandler : MacPanel<NSView, Panel, Panel.ICallback>, Panel.IHandler
	{
		class EtoPanel : MacEventView
		{
		}

		protected override NSView CreateControl()
		{
			return new EtoPanel { Handler = this };
		}
		
		public override NSView ContainerControl { get { return Control; } }
	}
}
