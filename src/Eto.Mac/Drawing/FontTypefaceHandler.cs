using System;
using Eto.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Eto.Forms;


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
		NSFont _font;
		string _name;
		static readonly object LocalizedName_Key = new object();

		public NSFont Font => _font ?? (_font = CreateFont(10));

		public string PostScriptName { get; private set; }

		public int Weight { get; private set; }

		public NSFontTraitMask Traits { get; private set; }

		public FontTypefaceHandler(NSArray descriptor)
		{
			PostScriptName = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(0));
			_name = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(1));
			Weight = Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(2))?.Int32Value ?? 1;
			Traits = (NSFontTraitMask)(Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(3))?.Int32Value ?? 0);
		}

		public FontTypefaceHandler(NSFont font, NSFontTraitMask? traits = null)
		{
			_font = font;
			var descriptor = font.FontDescriptor;
			PostScriptName = descriptor.PostscriptName;
			var manager = NSFontManager.SharedFontManager;
			Weight = (int)manager.WeightOfFont(font);
			Traits = traits ?? manager.TraitsOfFont(font);
		}

		public FontTypefaceHandler(string postScriptName, string name, NSFontTraitMask traits, int weight)
		{
			PostScriptName = postScriptName;
			this._name = name;
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
				if (_name == null)
				{
					_name = GetName(Font.Handle);

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
				return _name;
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
			// no (easy) way to get a CTFont from an NSFont
			var localizedNamePtr = CTFontCopyLocalizedName(Font.Handle, CTFontNameKeySubFamily.Handle, out var actualLanguagePtr);
			var actualLanguage = Runtime.GetNSObject<NSString>(actualLanguagePtr);
			var localizedName = Runtime.GetNSObject<NSString>(localizedNamePtr);

			return localizedName;
		}

		public FontStyle FontStyle => Traits.ToEto();

		static string SystemFontName => NSFont.SystemFontOfSize(NSFont.SystemFontSize).FontDescriptor.PostscriptName;
		static string BoldSystemFontName => NSFont.BoldSystemFontOfSize(NSFont.SystemFontSize).FontDescriptor.PostscriptName;

		public bool IsSymbol => Font.FontDescriptor.SymbolicTraits.HasFlag(NSFontSymbolicTraits.SymbolicClass);

		public NSFont CreateFont(float size)
		{

			// we have a postcript name, use that to create the font
			if (!string.IsNullOrEmpty(PostScriptName))
			{
				// if we try to get a system font by name we get errors now..
				if (PostScriptName == SystemFontName)
					return NSFont.SystemFontOfSize(size);
				if (PostScriptName == BoldSystemFontName)
					return NSFont.BoldSystemFontOfSize(size);

				// always return something...
				var font = NSFont.FromFontName(PostScriptName, size) ?? NSFont.UserFontOfSize(size);
				return font;
			}

			var family = (FontFamilyHandler)Widget.Family.Handler;
			return FontHandler.CreateFont(family.MacName, size, Traits, Weight);
		}

		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges)
		{
			var charSet = Font.CoveredCharacterSet;

			foreach (var range in ranges)
			{
				for (int i = range.Start; i <= range.End; i++)
				{
					if (!charSet.Contains((char)i))
						return false;
				}
			}
			return true;
		}
	}
}

