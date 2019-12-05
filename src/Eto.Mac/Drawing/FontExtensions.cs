using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
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

#if OSX
namespace Eto.Mac.Drawing
#elif IOS

using UIKit;
using Foundation;
using ObjCRuntime;
using Eto.Mac;
using NSFont = UIKit.UIFont;

namespace Eto.iOS.Drawing
#endif
{

	class FontUtility
	{
#if OSX
		static readonly NSString ForegroundColorAttribute = NSStringAttributeKey.ForegroundColor;
#elif IOS
		static readonly NSString ForegroundColorAttribute = UIStringAttributeKey.ForegroundColor;
		static readonly Selector selSetSize = new Selector("setSize:");
#endif
		readonly NSTextStorage storage;
		readonly NSLayoutManager layout;
		readonly NSTextContainer container;

		public FontUtility()
		{
			storage = new NSTextStorage();
			layout = new NSLayoutManager { UsesFontLeading = true };
#if OSX
			layout.BackgroundLayoutEnabled = false;
#endif
			container = new NSTextContainer { LineFragmentPadding = 0f };
			layout.UsesFontLeading = true;
			layout.AddTextContainer(container);
			storage.AddLayoutManager(layout);
		}

		public SizeF MeasureString(NSAttributedString str, SizeF? availableSize = null)
		{
			SetContainerSize(availableSize);
			storage.SetString(str);
			return layout.BoundingRectForGlyphRange(new NSRange(0, (int)str.Length), container).Size.ToEto();
		}

		void SetContainerSize(SizeF? availableSize)
		{
#if OSX
			container.ContainerSize = (availableSize ?? SizeF.MaxValue).ToNS();
#elif IOS
			if (container.RespondsToSelector(selSetSize))
				container.Size = (availableSize ?? SizeF.MaxValue).ToNS();
#endif
		}
	}

	public static class FontExtensions
	{

		public static NSDictionary Attributes(this Font font)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				return handler.Attributes;
			}
			return null;
		}

		public static void Apply(this Font font, NSMutableAttributedString str)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				str.AddAttributes(handler.Attributes, new NSRange(0, (int)str.Length));
			}
		}

		public static void Apply(this Font font, NSMutableDictionary dic)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				foreach (var item in handler.Attributes)
					dic.Add(item.Key, item.Value);
			}
		}

		public static NSAttributedString AttributedString(this Font font, NSAttributedString attributedString)
		{
			if (font != null)
			{
				var str = attributedString as NSMutableAttributedString ?? new NSMutableAttributedString(attributedString);
				font.Apply(str);
				return str;
			}
			return attributedString;
		}

		public static NSMutableAttributedString ToMutable(this NSAttributedString attributedString, string text)
		{
			if (attributedString != null && attributedString.Length > 0)
			{
				NSRange range;
				return new NSMutableAttributedString(text, attributedString.GetAttributes(0, out range));
			}

			return new NSMutableAttributedString(text);
		}

		public static NSMutableAttributedString AttributedString(this Font font, string text, NSAttributedString attributedString = null)
		{
			var mutable = attributedString.ToMutable(text);
			font.Apply(mutable);

			return mutable;
		}
	}
}