using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms
{
	public static class iosCompatibility
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

