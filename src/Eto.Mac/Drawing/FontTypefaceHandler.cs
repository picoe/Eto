using System;
using Eto.Drawing;
using System.Linq;
using System.Runtime.InteropServices;


#if XAMMAC2
using AppKit;
using Foundation;
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
	public class FontTypefaceHandler : WidgetHandler<FontTypeface>, FontTypeface.IHandler
	{
		NSFont font;
		string name;
		static readonly object LocalizedName_Key = new object();

		public string PostScriptName { get; private set; }

		public int Weight { get; private set; }

		public NSFontTraitMask Traits { get; private set; }

		public FontTypefaceHandler(NSArray descriptor)
		{
			PostScriptName = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(0));
			name = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(1));
			Weight = Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(2))?.Int32Value ?? 1;
			Traits = (NSFontTraitMask)(Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(3))?.Int32Value ?? 0);
		}

		public FontTypefaceHandler(NSFont font, NSFontTraitMask? traits = null)
		{
			this.font = font;
			var descriptor = font.FontDescriptor;
			PostScriptName = descriptor.PostscriptName;
			var manager = NSFontManager.SharedFontManager;
			Weight = (int)manager.WeightOfFont(font);
			Traits = traits ?? manager.TraitsOfFont(font);
		}

		public FontTypefaceHandler(string postScriptName, string name, NSFontTraitMask traits, int weight)
		{
			PostScriptName = postScriptName;
			this.name = name;
			Weight = weight;
			Traits = traits;
		}

		internal static NSString GetName(IntPtr fontHandle)
		{
			var namePtr = CTFontCopyName(fontHandle, CTFontNameKeySubFamily.Handle);
			return Runtime.GetNSObject<NSString>(namePtr);
		}

		public string Name
		{
			get
			{
				if (name == null)
				{
					if (font == null)
						font = CreateFont(10);
					name = GetName(font.Handle);

					/*
					var manager = NSFontManager.SharedFontManager;
					// no attribute, find font face based on postscript name
					var members = manager.AvailableMembersOfFontFamily(PostScriptName);
					var member = members.FirstOrDefault(r => (string)Runtime.GetNSObject<NSString>(r.ValueAt(0)) == PostScriptName);
					if (member != null)
					{
						name = (string)Runtime.GetNSObject<NSString>(member.ValueAt(1));
					}*/
				}
				return name;
			}
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		static extern IntPtr CTFontCopyName(IntPtr font, IntPtr nameKey);

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		static extern IntPtr CTFontCopyLocalizedName(IntPtr font, IntPtr nameKey, out IntPtr actualLanguage);

		static NSString CTFontNameKeySubFamily = Dlfcn.GetStringConstant(Messaging.CoreTextHandle, "kCTFontSubFamilyNameKey");


		public string LocalizedName => Widget.Properties.Create<string>(LocalizedName_Key, GetLocalizedName);

		string GetLocalizedName()
		{
			if (font == null)
				font = CreateFont(10);

			// no (easy) way to get a CTFont from an NSFont
			var localizedNamePtr = CTFontCopyLocalizedName(font.Handle, CTFontNameKeySubFamily.Handle, out var actualLanguagePtr);
			var actualLanguage = Runtime.GetNSObject<NSString>(actualLanguagePtr);
			var localizedName = Runtime.GetNSObject<NSString>(localizedNamePtr);

			return localizedName;
		}

		public FontStyle FontStyle => Traits.ToEto();

		public NSFont CreateFont(float size)
		{
			// we have a postcript name, use that to create the font
			if (!string.IsNullOrEmpty(PostScriptName))
				return NSFont.FromFontName(PostScriptName, size);

			var family = (FontFamilyHandler)Widget.Family.Handler;
			return FontHandler.CreateFont(family.MacName, size, Traits, Weight);
		}
	}
}

