
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

