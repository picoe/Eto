using System;
using Eto.Drawing;
using System.Text;
using System.Linq;
using MonoMac.Foundation;
using MonoTouch.UIKit;
using MonoMac.AppKit;
using System.Runtime.InteropServices;
using MonoMac.ObjCRuntime;
using MonoMac.CoreGraphics;

#if OSX
namespace Eto.Platform.Mac.Drawing
#elif IOS

using NSFont = MonoTouch.UIKit.UIFont;

namespace Eto.Platform.iOS.Drawing
#endif
{
	public static class FontExtensions
	{
		#if OSX
		public static NSFont ToNSFont(this Font font)
		{
			return font == null ? null : ((FontHandler)font.Handler).Control;
		}

		public static void Apply(this Font font, NSMutableAttributedString str)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				str.AddAttributes(handler.Attributes, new NSRange(0, str.Length));
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

		public static float LineHeight(this NSFont font)
		{
			return layout.DefaultLineHeightForFont(font);
			/**
			var leading = Math.Floor (Math.Max (0, font.Leading) + 0.5f);
			var lineHeight = (float)(Math.Floor(font.Ascender + 0.5f) - Math.Floor (font.Descender + 0.5f) + leading);

			if (leading > 0)
				return lineHeight;
			else
				return (float)(lineHeight + Math.Floor(0.2 * lineHeight + 0.5));
			/**/
		}

		static readonly NSTextStorage storage;
		static readonly NSLayoutManager layout;
		static readonly NSTextContainer container;

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern void void_objc_msgSend_NSRange_NSPoint(IntPtr receiver, IntPtr selector, NSRange arg1, PointF arg2);

		static readonly IntPtr selDrawGlyphs = Selector.GetHandle("drawGlyphsForGlyphRange:atPoint:");

		public static SizeF MeasureString(string text, Font font = null, SizeF? availableSize = null)
		{
			container.ContainerSize = (availableSize ?? SizeF.MaxValue).ToSD();
			storage.SetString(font.AttributedString(text));
			//layout.EnsureLayoutForBoundingRect(new System.Drawing.RectangleF(System.Drawing.PointF.Empty, container.ContainerSize), container);
			//return layout.GetUsedRectForTextContainer(container).Size.ToEto();
			return layout.BoundingRectForGlyphRange(new NSRange(0, text.Length), container).Size.ToEto();
		}

		public static SizeF MeasureString(NSAttributedString str, SizeF? availableSize = null)
		{
			container.ContainerSize = (availableSize ?? SizeF.MaxValue).ToSD();
			storage.SetString(str);
			//layout.EnsureLayoutForBoundingRect(new System.Drawing.RectangleF(System.Drawing.PointF.Empty, container.ContainerSize), container);
			//return layout.GetUsedRectForTextContainer(container).Size.ToEto();
			return layout.BoundingRectForGlyphRange(new NSRange(0, str.Length), container).Size.ToEto();
		}

		public static void DrawString(NSAttributedString str, PointF point, SizeF? availableSize = null)
		{
			container.ContainerSize = (availableSize ?? SizeF.MaxValue).ToSD();
			storage.SetString(str);
			void_objc_msgSend_NSRange_NSPoint(layout.Handle, selDrawGlyphs, new NSRange(0, str.Length), point);
		}

		public static void DrawString(string text, PointF point, Color color, Font font = null, SizeF? availableSize = null)
		{
			container.ContainerSize = (availableSize ?? SizeF.MaxValue).ToSD();
			var str = font.AttributedString(text);
			str.AddAttribute(NSAttributedString.ForegroundColorAttributeName, color.ToNS(), new NSRange(0, text.Length));
			storage.SetString(str);
			void_objc_msgSend_NSRange_NSPoint(layout.Handle, selDrawGlyphs, new NSRange(0, text.Length), point);
		}

		static FontExtensions()
		{
			storage = new NSTextStorage();
			layout = new NSLayoutManager();
			container = new NSTextContainer();
			container.LineFragmentPadding = 0f;
			//layout.AllowsNonContiguousLayout = true;
			layout.UsesFontLeading = true;
			//layout.UsesScreenFonts = true;
			layout.AddTextContainer(container);
			storage.AddLayoutManager(layout);
		}
		#elif IOS
		public static float LineHeight(this NSFont font)
		{
			return font.LineHeight;
		}
		#endif
	}
}