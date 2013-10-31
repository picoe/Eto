using System;
using Eto.Drawing;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;

namespace Eto.Platform.Mac.Drawing
{
	public class FontFamilyHandler : WidgetHandler<object, FontFamily>, IFontFamily
	{
		public string MacName { get; set; }

		public string Name { get; set; }

		public NSFontTraitMask TraitMask { get; set; }
		
		public IEnumerable<FontTypeface> Typefaces
		{
			get
			{ 
				var descriptors = NSFontManager.SharedFontManager.AvailableMembersOfFontFamily (MacName);
				return descriptors.Select (r => new FontTypeface (Widget, new FontTypefaceHandler (r)));
			}
		}

		public FontFamilyHandler ()
		{
			TraitMask = (NSFontTraitMask)int.MaxValue;
		}

		public FontFamilyHandler (string familyName)
		{
			Create (familyName);
		}
		
		public void Create (string familyName)
		{
			Name = MacName = familyName;
			TraitMask = (NSFontTraitMask)int.MaxValue;

			switch (familyName.ToUpperInvariant()) {
			case FontFamilies.MonospaceFamilyName:
				MacName = "Courier New";
				break;
			case FontFamilies.SansFamilyName:
				MacName = "Helvetica";
				break;
			case FontFamilies.SerifFamilyName:
#if OSX
				MacName = "Times";
#elif IOS
				MacName = "Times New Roman";
#endif
				break;
			case FontFamilies.CursiveFamilyName:
				MacName = "Papyrus";
				TraitMask = NSFontTraitMask.Condensed | NSFontTraitMask.Unbold | NSFontTraitMask.Unitalic;
				break;
			case FontFamilies.FantasyFamilyName:
				MacName = "Impact";
				break;
			}
		}

		public FontTypeface GetFace (NSFont font)
		{
			var postScriptName = font.FontDescriptor.PostscriptName;
			var faceHandler = Typefaces.Select (r => r.Handler).OfType<FontTypefaceHandler> ().FirstOrDefault (r => r.PostScriptName == postScriptName);
			if (faceHandler == null)
				faceHandler = new FontTypefaceHandler (font);
			return new FontTypeface (Widget, faceHandler);
		}
	}
}

