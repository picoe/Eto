using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
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

namespace Eto.Mac.Drawing
{
	public class FontFamilyHandler : WidgetHandler<object, FontFamily>, FontFamily.IHandler
	{
		public string MacName { get; set; }

		public string Name { get; set; }

		static readonly IntPtr sel_LocalizedNameForFamilyFace = Selector.GetHandle("localizedNameForFamily:face:");

		public string LocalizedName
		{
			get
			{
				// faceName cannot be null.  Use this when it is fixed in xammac/monomac:
				// return NSFontManager.SharedFontManager.LocalizedNameForFamily(MacName, null);

				var familyPtr = NSString.CreateNative(MacName);
				var facePtr = IntPtr.Zero;
				var result = NSString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(NSFontManager.SharedFontManager.Handle, sel_LocalizedNameForFamilyFace, familyPtr, facePtr));
				NSString.ReleaseNative(familyPtr);
				return result;
			}
		}

		public NSFontTraitMask TraitMask { get; set; }

		public IEnumerable<FontTypeface> Typefaces
		{
			get
			{ 
				var descriptors = NSFontManager.SharedFontManager.AvailableMembersOfFontFamily(MacName);
				if (descriptors == null)
					return Enumerable.Empty<FontTypeface>();
				return descriptors.Select(r => new FontTypeface(Widget, new FontTypefaceHandler(r)));
			}
		}

		public FontFamilyHandler()
		{
			TraitMask = (NSFontTraitMask)int.MaxValue;
		}

		public FontFamilyHandler(string familyName)
		{
			Create(familyName);
		}

		public void Create(string familyName)
		{
			Name = MacName = familyName;
			TraitMask = (NSFontTraitMask)int.MaxValue;

			switch (familyName.ToUpperInvariant())
			{
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

		public FontTypeface GetFace(NSFont font, NSFontTraitMask? traits)
		{
			var postScriptName = font.FontDescriptor.PostscriptName;
			var faceHandler = Typefaces.Select(r => r.Handler).OfType<FontTypefaceHandler>().FirstOrDefault(r => r.PostScriptName == postScriptName && (traits == null || r.Traits == traits));
			if (faceHandler == null)
				faceHandler = new FontTypefaceHandler(font, traits);
			return new FontTypeface(Widget, faceHandler);
		}
	}
}

