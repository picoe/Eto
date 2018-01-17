using System;
using Eto.Drawing;
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
	public class FontTypefaceHandler : WidgetHandler<FontTypeface>, FontTypeface.IHandler
	{
		readonly string name;

		public string PostScriptName { get; private set; }

		public int Weight { get; private set; }

		public NSFontTraitMask Traits { get; private set; }
		// remove when implemented in monomac
		static readonly IntPtr AppKit_libraryHandler;
		static NSString _NSFontFamilyAttribute;
		static NSString _NSFontFaceAttribute;

		static FontTypefaceHandler()
		{
			AppKit_libraryHandler = Dlfcn.dlopen("/System/Library/Frameworks/AppKit.framework/AppKit", 0);
		}

		public static NSString NSFontFamilyAttribute
		{
			get
			{
				if (_NSFontFamilyAttribute == null)
					_NSFontFamilyAttribute = Dlfcn.GetStringConstant(AppKit_libraryHandler, "NSFontFamilyAttribute");
				return _NSFontFamilyAttribute;
			}
		}

		public static NSString NSFontFaceAttribute
		{
			get
			{
				if (_NSFontFaceAttribute == null)
					_NSFontFaceAttribute = Dlfcn.GetStringConstant(AppKit_libraryHandler, "NSFontFaceAttribute");
				return _NSFontFaceAttribute;
			}
		}

		public FontTypefaceHandler(NSArray descriptor)
		{
			PostScriptName = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(0));
			name = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(1));
			Weight = Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(2)).Int32Value;
			Traits = (NSFontTraitMask)Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(3)).Int32Value;
		}

		public FontTypefaceHandler(NSFont font, NSFontTraitMask? traits = null)
		{
			var descriptor = font.FontDescriptor;
			PostScriptName = descriptor.PostscriptName;
			var manager = NSFontManager.SharedFontManager;
			Weight = (int)manager.WeightOfFont(font);
			Traits = traits ?? manager.TraitsOfFont(font);
			name = (NSString)descriptor.FontAttributes[NSFontFaceAttribute];
			if (name == null)
			{
				// no attribute, find font face based on postscript name
				var members = manager.AvailableMembersOfFontFamily(font.FamilyName);
				var member = members.FirstOrDefault(r => (string)Runtime.GetNSObject<NSString>(r.ValueAt(0)) == PostScriptName);
				if (member != null)
				{
					name = (string)Runtime.GetNSObject<NSString>(member.ValueAt(1));
				}
			}
		}

		public FontTypefaceHandler(string postScriptName, string name, NSFontTraitMask traits, int weight)
		{
			PostScriptName = postScriptName;
			this.name = name;
			Weight = weight;
			Traits = traits;
		}

		public string Name
		{
			get { return name; }
		}

		public FontStyle FontStyle
		{
			get { return Traits.ToEto(); }
		}

		public NSFont CreateFont(float size)
		{
			var family = (FontFamilyHandler)Widget.Family.Handler;
			return FontHandler.CreateFont(family, size, Traits, Weight);
		}
	}
}

