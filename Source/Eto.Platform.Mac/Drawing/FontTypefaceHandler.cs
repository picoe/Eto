using System;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<object, FontTypeface>, IFontTypeface
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
			PostScriptName = (string)new NSString(descriptor.ValueAt(0));
			name = (string)new NSString(descriptor.ValueAt(1));
			Weight = new NSNumber(descriptor.ValueAt(2)).Int32Value;
			Traits = (NSFontTraitMask)new NSNumber(descriptor.ValueAt(3)).Int32Value;
		}

		public FontTypefaceHandler(NSFont font)
		{
			var descriptor = font.FontDescriptor;
			PostScriptName = descriptor.PostscriptName;
			var manager = NSFontManager.SharedFontManager;
			Weight = manager.WeightOfFont(font);
			Traits = manager.TraitsOfFont(font);
			name = (NSString)descriptor.FontAttributes[NSFontFaceAttribute];
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
			return NSFontManager.SharedFontManager.FontWithFamily(family.MacName, Traits, Weight, size);
		}
	}
}

