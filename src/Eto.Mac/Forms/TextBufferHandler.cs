using System;
using System.Collections.Generic;
using System.IO;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using CoreText;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
using MonoMac.CoreText;
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

#if XAMMAC
using nnint = System.Int32;
#elif Mac64
using nnint = System.UInt64;
#else
using nnint = System.UInt32;
#endif

namespace Eto.Mac.Forms
{
	public class TextBufferHandler : WidgetHandler<NSMutableAttributedString, TextBuffer>, TextBuffer.IHandler
	{
		public TextBufferHandler()
		{
			Control = new NSMutableAttributedString();
		}

		public NSDictionary TypingAttributes { get; set; }

		public IEnumerable<RichTextAreaFormat> SupportedFormats => throw new NotImplementedException();

		public int TextLength => (int)Control.Length;

		public void Append(string text) => Control.Append(TypingAttributes != null ? new NSAttributedString(text, TypingAttributes) : new NSAttributedString(text));

		public void Append(ITextBuffer buffer) => Control.Append(buffer.ToNS());

		public void BeginEdit() => Control.BeginEditing();

		public void Clear() => Control.SetString(new NSAttributedString());

		public void Delete(Range<int> range) => Control.DeleteRange(range.ToNS());

		public void EndEdit() => Control.EndEditing();

		public void Insert(int position, string text) => Control.Insert(new NSAttributedString(text), (nnint)position);

		public void Insert(int position, ITextBuffer buffer) => Control.Insert(buffer.ToNS(), (nnint)position);

		public void Load(Stream stream, RichTextAreaFormat format)
		{
			throw new NotImplementedException();
		}

		public void Save(Stream stream, RichTextAreaFormat format)
		{
			throw new NotImplementedException();
		}

		public void SetBackground(Range<int> range, Color color)
		{
			Control.AddAttribute(NSStringAttributeKey.BackgroundColor, color.ToNSUI(), range.ToNS());
		}

		public void SetBold(Range<int> range, bool bold)
		{
			SetFontAttribute(range.ToNS(), NSFontTraitMask.Bold, NSFontTraitMask.Unbold, bold);
		}

		public void SetFamily(Range<int> range, FontFamily family)
		{
			var familyName = ((FontFamilyHandler)family.Handler).MacName;
			SetFontAttribute(range.ToNS(), true, font => NSFontManager.SharedFontManager.ConvertFontToFamily(font, familyName));
		}

		public void SetFont(Range<int> range, Font font)
		{
			var nsrange = range.ToNS();
			var attr = font.Attributes();
			if (attr != null && attr.Count > 0)
				Control.AddAttributes(attr, nsrange);
		}

		public void SetForeground(Range<int> range, Color color)
		{
			Control.AddAttribute(NSStringAttributeKey.ForegroundColor, color.ToNSUI(), range.ToNS());
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			SetFontAttribute(range.ToNS(), NSFontTraitMask.Italic, NSFontTraitMask.Unitalic, italic);
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			Control.AddAttribute(NSStringAttributeKey.StrikethroughStyle, new NSNumber((int)(strikethrough ? NSUnderlineStyle.Single : NSUnderlineStyle.None)), range.ToNS());
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			Control.AddAttribute(NSStringAttributeKey.UnderlineStyle, new NSNumber((int)(underline ? NSUnderlineStyle.Single : NSUnderlineStyle.None)), range.ToNS());
		}

		void SetFontAttribute(NSRange range, NSFontTraitMask traitMask, NSFontTraitMask traitUnmask, bool enabled)
		{
			SetFontAttribute(range, enabled, f => UpdateFontTrait(f, traitMask, traitUnmask, enabled));
		}

		void SetFontAttribute(NSRange range, bool enabled, Func<NSFont, NSFont> updateFont)
		{
			NSRange effectiveRange;
			Control.BeginEditing();
			var current = range;
			var left = current.Length;
			do
			{
				var attribs = Control.GetAttributes(current.Location, out effectiveRange, current);
				attribs = UpdateFontAttributes(attribs, enabled, updateFont);
				var span = effectiveRange.Location + effectiveRange.Length - current.Location;
				var newRange = new NSRange(current.Location, (nint)Math.Min(span, current.Length));
				Control.AddAttributes(attribs, newRange);
				current.Location += span;
				current.Length -= span;
				left -= span;
			} while (left > 0);
			Control.EndEditing();
		}

		NSFont UpdateFontTrait(NSFont font, NSFontTraitMask traitMask, NSFontTraitMask traitUnmask, bool enabled)
		{
			var traits = enabled ? traitMask : traitUnmask;
			return NSFontManager.SharedFontManager.ConvertFont(font, traits);
		}

		NSDictionary UpdateFontAttributes(NSDictionary attribs, bool enabled, Func<NSFont, NSFont> updateFont)
		{
			NSObject fontValue = null;
			if ((enabled && attribs == null)
				|| (attribs != null && attribs.TryGetValue(NSStringAttributeKey.Font, out fontValue)))
			{
				var font = fontValue as NSFont ?? NSFont.SystemFontOfSize(NSFont.SystemFontSize);

				font = updateFont(font);

				var mutableAttribs = new NSMutableDictionary(attribs);
				mutableAttribs.SetValueForKey(font, NSStringAttributeKey.Font);
				return mutableAttribs;
			}
			return attribs;
		}

	}
}
