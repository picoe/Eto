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
	public static class IosCompatibility
	{
		/// <summary>
		/// Compatibility with ios
		/// </summary>
		public static void SetNeedsDisplay(this NSView view)
		{
			view.NeedsDisplay = true;
		}
	}
}

