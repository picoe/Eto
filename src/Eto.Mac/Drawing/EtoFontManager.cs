using System;
#if XAMMAC2
using AppKit;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Drawing
{
	public class EtoFontManager : NSFontManager
	{
		public EtoFontManager()
		{
		}

		public EtoFontManager(IntPtr handle)
			: base(handle)
		{
		}

		public static void Install() => SetFontManagerFactory(new Class(typeof(EtoFontManager)));

		public override NSFont ConvertFont(NSFont fontObj, NSFontTraitMask trait)
		{
			// be a little less conservative when converting fonts to use the name when translating to italics.
			// e.g. 'Klavika Medium' translates to 'Klavika Italic' if adding italic trait, instead of
			// 'Klavika Medium Italic'.
			if (trait == NSFontTraitMask.Italic || trait == NSFontTraitMask.Unitalic)
			{
				var oldName = (string)FontTypefaceHandler.GetName(fontObj.Handle);
				const string italicSuffix = " Italic";
				string newName = null;
				if (trait == NSFontTraitMask.Italic)
					newName = oldName + italicSuffix;
				else if (oldName.EndsWith(italicSuffix, StringComparison.OrdinalIgnoreCase))
					newName = oldName.Substring(0, oldName.Length - italicSuffix.Length);

				if (newName != null)
				{
					foreach (var descriptor in AvailableMembersOfFontFamily(fontObj.FamilyName))
					{
						var fontName = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(1));
						if (string.Equals(fontName, newName, StringComparison.OrdinalIgnoreCase))
						{
							var postScriptName = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(0));
							return NSFont.FromFontName(postScriptName, fontObj.PointSize);
						}
					}
				}
			}

			return base.ConvertFont(fontObj, trait);
		}
	}
}
