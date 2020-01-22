using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

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

