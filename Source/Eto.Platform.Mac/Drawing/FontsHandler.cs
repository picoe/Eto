using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
#if OSX
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Drawing
#endif
{
	public class FontsHandler : WidgetHandler<Widget>, IFonts
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
			get { return availableFontFamilies.Select (r => new FontFamily(this.Generator, new FontFamilyHandler (r))); }
		}

		public bool FontFamilyAvailable (string fontFamily)
		{
			return availableFontFamilies.Contains (fontFamily, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}

