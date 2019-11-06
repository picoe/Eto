using Eto.Drawing;
using System;
using System.Runtime.InteropServices;
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
	public class EtoLayoutManager : NSLayoutManager
	{
		public Brush ForegroundBrush { get; set; } = new SolidBrush(SystemColors.ControlText);

		internal GraphicsHandler CurrentGraphics { get; set; }

		static IntPtr selShowCGGlyphs_Positions_Count_Font_Matrix_Attributes_InContext_Handle = Selector.GetHandle("showCGGlyphs:positions:count:font:matrix:attributes:inContext:");

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern void CTFontDrawGlyphs(IntPtr font, IntPtr glyphs, IntPtr positions, nuint count, IntPtr context);


		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern CGAffineTransform CTFontGetMatrix(IntPtr font);

		[Export("showCGGlyphs:positions:count:font:matrix:attributes:inContext:")]
		protected void ShowGlyphs(IntPtr glyphs, IntPtr positions, nuint glyphCount, IntPtr font, CGAffineTransform textMatrix, IntPtr attributes, IntPtr graphicsContext)
		{
			if (ForegroundBrush is SolidBrush)
			{
				// attributes can be null, Xamarin.Mac doesn't allow that when calling base. ugh.
				Messaging.void_objc_msgSendSuper_IntPtr_IntPtr_nuint_IntPtr_CGAffineTransform_IntPtr_IntPtr(SuperHandle, selShowCGGlyphs_Positions_Count_Font_Matrix_Attributes_InContext_Handle, glyphs, positions, glyphCount, font, textMatrix, attributes, graphicsContext);
				//base.ShowGlyphs(glyphs, positions, glyphCount, font, textMatrix, attributes, graphicsContext);
			}
			else if (glyphCount > 0)
			{
				// draw manually so we can use a custom brush/fill.
				var ctx = NSGraphicsContext.CurrentContext.GraphicsPort;
				ctx.SaveState();
				ctx.SetTextDrawingMode(CGTextDrawingMode.Clip);

				// what to do with the attributes?? needed?
				var m = CTFontGetMatrix(font);
				m.Translate(textMatrix.x0, textMatrix.y0);
				m.Scale(1, -1);
				ctx.TextMatrix = m;
				CTFontDrawGlyphs(font, glyphs, positions, glyphCount, ctx.Handle);

				ForegroundBrush.Draw(CurrentGraphics, false, FillMode.Winding, false);
				ctx.RestoreState();
			}
		}
	}

	public class FormattedTextHandler : WidgetHandler<EtoLayoutManager, FormattedText, FormattedText.ICallback>, FormattedText.IHandler
	{
#if OSX
		static readonly NSString ForegroundColorAttribute = NSStringAttributeKey.ForegroundColor;
#elif IOS
		static readonly NSString ForegroundColorAttribute = UIStringAttributeKey.ForegroundColor;
		static readonly Selector selSetSize = new Selector("setSize:");
#endif

		readonly NSTextStorage storage;
		readonly NSTextContainer container;
		bool invalid;
		string _text;
		Font _font = SystemFonts.Default();
		FormattedTextTrimming _trimming;
		FormattedTextWrapMode _wrap;
		FormattedTextAlignment _alignment;

		public FormattedTextAlignment Alignment
		{
			get => _alignment;
			set
			{
				_alignment = value;
				Invalidate();
			}
		}
		public FormattedTextWrapMode Wrap
		{
			get => _wrap;
			set
			{
				_wrap = value;
				Invalidate();
			}
		}
		public FormattedTextTrimming Trimming
		{
			get => _trimming;
			set
			{
				_trimming = value;
				container.LineBreakMode = value.ToNS();
				Invalidate();
			}
		}
		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				Invalidate();
			}
		}

		public SizeF MaximumSize
		{
			get => container.ContainerSize.ToEto();
			set => container.ContainerSize = value.ToNS();
		}

		public Font Font
		{
			get => _font;
			set
			{
				_font = value;
				Invalidate();
			}
		}

		public Brush ForegroundBrush
		{
			get => Control.ForegroundBrush;
			set
			{
				Control.ForegroundBrush = value;
				Invalidate();
			}
		}

		void Invalidate()
		{
			invalid = true;
		}

		private NSAttributedString CreateAttributedString()
		{
			var str = Font.AttributedString(_text ?? string.Empty);
			var range = new NSRange(0, str.Length);
			if (ForegroundBrush is SolidBrush sb)
				str.AddAttribute(ForegroundColorAttribute, sb.Color.ToNSUI(), range);

			var paragraphStyle = CreateParagraphStyle();
			str.AddAttribute(NSStringAttributeKey.ParagraphStyle, paragraphStyle, range);
			return str;
		}

		private NSParagraphStyle CreateParagraphStyle()
		{
			var style = new NSMutableParagraphStyle();
			//style.LineBreakMode = Trimming.ToNS();
			container.MaximumNumberOfLines = 0;
			if (Wrap == FormattedTextWrapMode.None)
			{
				if (Trimming != FormattedTextTrimming.None)
					style.LineBreakMode = Trimming.ToNS();
				else
				{
					// hm, setting style.LineBreakMode to Clipping doesn't appear to clip, so we wrap by character and set max lines to 1
					style.LineBreakMode = NSLineBreakMode.CharWrapping;
					container.MaximumNumberOfLines = 1;
				}
			}
			else
				style.LineBreakMode = Wrap.ToNS();

			style.Alignment = Alignment.ToNS();
			return style;
		}

		void EnsureString()
		{
			if (invalid)
			{
				storage.SetString(CreateAttributedString());
				invalid = false;
			}
		}

		public int MaximumLineCount
		{
			get => (int)container.MaximumNumberOfLines;
			set => container.MaximumNumberOfLines = (nuint)value;
		}

		public SizeF Measure()
		{
			EnsureString();
			var size = Control.BoundingRectForGlyphRange(new NSRange(0, (int)_text.Length), container).Size.ToEto();
			/*if (Wrap == FormattedTextWrapMode.None && Trimming != FormattedTextTrimming.None && Alignment != FormattedTextAlignment.Left)
			{
				size.Width = MaximumSize.Width;
			}*/
			return size;
		}

		public FormattedTextHandler()
		{
			storage = new NSTextStorage();
			Control = new EtoLayoutManager { UsesFontLeading = true };
#if OSX
			Control.BackgroundLayoutEnabled = false;
#endif
			container = new NSTextContainer { LineFragmentPadding = 0f };
			Control.AddTextContainer(container);
			storage.AddLayoutManager(Control);
		}

		public void DrawText(GraphicsHandler graphics, PointF location)
		{
			EnsureString();
			Control.CurrentGraphics = graphics;
			var ctx = graphics.Control;
			Control.DrawGlyphs(new NSRange(0, (int)_text.Length), location.ToNS());
		}
	}
}