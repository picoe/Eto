using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
#if OSX

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

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

