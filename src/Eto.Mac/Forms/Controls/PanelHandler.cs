using System;
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
	public class EtoPaddedPanel : MacEventView
	{
		new Panel.IHandler Handler => base.Handler as Panel.IHandler;

		public EtoPaddedPanel(IntPtr handle) : base(handle)
		{
		}

		public EtoPaddedPanel()
		{
			AutoresizesSubviews = false;
		}

		public override void Layout()
		{
			base.Layout();
			var subviews = Subviews;
			if (subviews.Length == 1)
			{
				var view = subviews[0];
				view.Frame = Bounds.WithPadding(Handler?.Padding ?? Padding.Empty);
			}
		}
	}
	public class PanelHandler : MacPanel<NSView, Panel, Panel.ICallback>, Panel.IHandler
	{
		protected override NSView CreateControl() => new EtoPaddedPanel();
		
		public override NSView ContainerControl => Control;

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			Control.NeedsLayout = true;
		}
	}
}
