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
	public class FontsHandler : IFonts
	{
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }

		public Generator Generator { get; set; }

		public IEnumerable<FontFamily> AvailableFontFamilies
		{
#if OSX
			get { return NSFontManager.SharedFontManager.AvailableFontFamilies.Select (r => new FontFamily(this.Generator, new FontFamilyHandler (r))); }
#elif IOS
			get { return UIFont.FamilyNames.Select (r => new FontFamily(this.Generator, new FontFamilyHandler (r))); }
#endif
		}

		public FontFamily GetFontFamily (string familyName)
		{
			return new FontFamily(Generator.Current, new FontFamilyHandler (familyName));
		}

		public FontFamily GetSystemFontFamily (string systemFamilyName)
		{
			switch (systemFamilyName) {
			case FontFamilies.MonospaceFamilyName:
				systemFamilyName = "Courier New";
				break;
			case FontFamilies.SansFamilyName:
				systemFamilyName = "Helvetica";
				break;
			case FontFamilies.SerifFamilyName:
				systemFamilyName = "Times";
				break;
			default:
				throw new NotSupportedException ();
			}
			return new FontFamily(Generator.Current, new FontFamilyHandler (systemFamilyName));
		}
	}
}

