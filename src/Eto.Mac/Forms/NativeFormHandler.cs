using System;
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

namespace Eto.Mac.Forms
{
	public class NativeFormHandler : FormHandler
	{
		public NativeFormHandler(NSWindow window)
			: base(window)
		{
		}
		public NativeFormHandler(NSWindowController windowController)
			: base(windowController)
		{
		}

		protected override void ConfigureWindow()
		{
		}
	}
}

