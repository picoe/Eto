#if OSX

namespace Eto.Mac.Drawing
#elif IOS
using UIKit;
using Foundation;

namespace Eto.iOS.Drawing
#endif
{
	public class FontsHandler : WidgetHandler<Widget>, Fonts.IHandler
	{
		readonly string [] availableFontFamilies;

		public FontsHandler ()
		{
#if OSX
			availableFontFamilies = NSFontManager.SharedFontManager.AvailableFontFamilies;
#elif IOS
			availableFontFamilies = UIFont.FamilyNames;
#endif
		}

		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get { return availableFontFamilies.Select (r => new FontFamily(new FontFamilyHandler (r))); }
		}

		public bool FontFamilyAvailable (string fontFamily)
		{
			return availableFontFamilies.Contains (fontFamily, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}

