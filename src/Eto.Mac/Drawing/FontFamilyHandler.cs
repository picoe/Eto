using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreText;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreText;
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
				if (MacName == null)
					return Name;
					
				var facePtr = IntPtr.Zero;
#if XAMMAC && NET6_0_OR_GREATER
				var familyPtr = CFString.CreateNative(MacName);
				var result = CFString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(NSFontManager.SharedFontManager.Handle, sel_LocalizedNameForFamilyFace, familyPtr, facePtr));
				CFString.ReleaseNative(familyPtr);
#else
				var familyPtr = NSString.CreateNative(MacName);
				var result = NSString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(NSFontManager.SharedFontManager.Handle, sel_LocalizedNameForFamilyFace, familyPtr, facePtr));
				NSString.ReleaseNative(familyPtr);
#endif
				return result;
			}
		}

		public NSFontTraitMask TraitMask { get; set; }

		IList<FontTypeface> _typefaces;

		public IEnumerable<FontTypeface> Typefaces => _typefaces ?? (_typefaces = GetTypefaces().ToArray());

		IEnumerable<FontTypeface> GetTypefaces()
		{
			var descriptors = NSFontManager.SharedFontManager.AvailableMembersOfFontFamily(MacName);
			if (descriptors == null)
				return Enumerable.Empty<FontTypeface>();
			return descriptors.Select(r => new FontTypeface(Widget, new FontTypefaceHandler(r)));
		}

		public FontFamilyHandler()
		{
			TraitMask = (NSFontTraitMask)int.MaxValue;
		}

		public FontFamilyHandler(CGFont cgfont, FontTypefaceHandler typeface)
		{
			_typefaces = new[] { typeface.Widget };
			Name = cgfont.FullName;
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

		public void CreateFromFiles(IEnumerable<string> fileNames)
		{
			var typefaces = new List<FontTypeface>();
			foreach (var fileName in fileNames)
			{
				using (var dataProvider = new CGDataProvider(fileName))
				{
					typefaces.Add(GetTypeface(dataProvider));
				}
			}
			_typefaces = typefaces.ToArray();
		}

		public void CreateFromStreams(IEnumerable<Stream> streams)
		{
			var typefaces = new List<FontTypeface>();
			foreach (var stream in streams)
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					var bytes = ms.ToArray();
					using (var dataProvider = new CGDataProvider(bytes, 0, bytes.Length))
					{
						typefaces.Add(GetTypeface(dataProvider));
					}
				}
			}
			_typefaces = typefaces.ToArray();
		}

		private FontTypeface GetTypeface(CGDataProvider dataProvider)
		{
			var cgfont = CGFont.CreateFromProvider(dataProvider);
			var ctfont = new CTFont(cgfont, 10, null);
			var currentName = ctfont.GetName(CTFontNameKey.Family);
			var faceName = ctfont.GetName(CTFontNameKey.SubFamily);

			if (Name == null)
				Name = currentName;
			else if (Name != currentName)
				throw new InvalidOperationException($"Family name of the supplied font files do not match. '{Name}' and '{currentName}'");

			var typefaceHandler = new FontTypefaceHandler(cgfont, faceName);
			var typeface = new FontTypeface(Widget, typefaceHandler);
			return typeface;
		}
	}
}

