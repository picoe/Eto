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
		WeakReference WeakHandler { get; set; }
		
		public FormattedTextHandler Handler
		{ 
			get => WeakHandler.Target as FormattedTextHandler;
			set => WeakHandler = new WeakReference(value);
		}

		static IntPtr selShowCGGlyphs_Positions_Count_Font_Matrix_Attributes_InContext_Handle = Selector.GetHandle("showCGGlyphs:positions:count:font:matrix:attributes:inContext:");

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern void CTFontDrawGlyphs(IntPtr font, IntPtr glyphs, IntPtr positions, nuint count, IntPtr context);


		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern CGAffineTransform CTFontGetMatrix(IntPtr font);

		[Export("showCGGlyphs:positions:count:font:matrix:attributes:inContext:")]
		protected void ShowGlyphs(IntPtr glyphs, IntPtr positions, nuint glyphCount, IntPtr font, CGAffineTransform textMatrix, IntPtr attributes, IntPtr graphicsContext)
		{
			var h = Handler;
			if (h == null)
				return;
			var foregroundBrush = h.ForegroundBrush;
			if (foregroundBrush is SolidBrush)
			{
				// attributes can be null, Xamarin.Mac doesn't allow that when calling base. ugh.
				Messaging.void_objc_msgSendSuper_IntPtr_IntPtr_nuint_IntPtr_CGAffineTransform_IntPtr_IntPtr(SuperHandle, selShowCGGlyphs_Positions_Count_Font_Matrix_Attributes_InContext_Handle, glyphs, positions, glyphCount, font, textMatrix, attributes, graphicsContext);
				//base.ShowGlyphs(glyphs, positions, glyphCount, font, textMatrix, attributes, graphicsContext);
			}
			else if (glyphCount > 0 && h.CurrentGraphics != null)
			{
				// draw manually so we can use a custom brush/fill.
				var ctx = NSGraphicsContext.CurrentContext.GraphicsPort;
				ctx.SaveState();
				ctx.SetTextDrawingMode(CGTextDrawingMode.Clip);

				// what to do with the attributes?? needed?
				var m = CTFontGetMatrix(font);
#if !VSMAC && (MONOMAC)
				m.Translate(textMatrix.x0, textMatrix.y0);
#else
				m.Translate(textMatrix.Tx, textMatrix.Ty);
#endif
				m.Scale(1, -1);
				ctx.TextMatrix = m;
				CTFontDrawGlyphs(font, glyphs, positions, glyphCount, ctx.Handle);

				foregroundBrush.Draw(h.CurrentGraphics, false, FillMode.Winding, false);
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
		SizeF _maximumSize = SizeF.MaxValue;
		Brush _foregroundBrush = new SolidBrush(SystemColors.ControlText);

		internal GraphicsHandler CurrentGraphics { get; set; }

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
			get => _maximumSize;
			set
			{
				_maximumSize = value;
				SetMaxSize();
			}
		}

		private void SetMaxSize()
		{
			var size = _maximumSize;
			if (size.Width >= float.MaxValue && Alignment != FormattedTextAlignment.Left)
			{
				// need a width to support aligning
				size.Width = GetMaxTextWidth();
			}
			size.Width = Math.Min(int.MaxValue, size.Width);
			size.Height = Math.Min(int.MaxValue, size.Height);
			container.Size = size.ToNS();
		}

		private float GetMaxTextWidth()
		{
			float maxWidth = 0;
			char newline = '\n';
			int newlineIndex = _text.IndexOf(newline);
			if (newlineIndex == -1)
			{
				return _maximumSize.Width;
			}
			int startIndex = 0;
			container.Size = new CGSize(0, 0);
			while (newlineIndex >= 0)
			{
				var glyphRange = new NSRange(startIndex, newlineIndex - startIndex);
#if MACOS_NET
				var rect = Control.GetBoundingRect(glyphRange, container).Size.ToEto();
#else
				var rect = Control.BoundingRectForGlyphRange(glyphRange, container).Size.ToEto();
#endif
				maxWidth = Math.Max(maxWidth, rect.Width);
				startIndex = newlineIndex + 1;
				newlineIndex = _text.IndexOf(newline, startIndex);
			}
			return maxWidth;
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
			get => _foregroundBrush;
			set
			{
				_foregroundBrush = value;
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
			if (Wrap == FormattedTextWrapMode.None)
				style.LineBreakMode = Trimming.ToNS();
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
				SetMaxSize();
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
			// var size = Control.BoundingRectForGlyphRange(new NSRange(0, (int)_text.Length), container).Size.ToEto();
			var size = storage.BoundingRectWithSize(container.Size, NSStringDrawingOptions.UsesLineFragmentOrigin).Size.ToEto();
			return size;
		}

		public FormattedTextHandler()
		{
			storage = new NSTextStorage();
			Control = new EtoLayoutManager { UsesFontLeading = true };
#if OSX
			Control.BackgroundLayoutEnabled = false;
#endif
			container = new NSTextContainer { LineFragmentPadding = 0f, Size = new CGSize(int.MaxValue, int.MaxValue) };
			Control.AddTextContainer(container);
			storage.AddLayoutManager(Control);
		}

		public void DrawText(GraphicsHandler graphics, PointF location)
		{
			EnsureString();
			CurrentGraphics = graphics;
			storage.DrawString(new CGRect(location.ToNS(), container.Size), NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.TruncatesLastVisibleLine);
			CurrentGraphics = null;
			// Control.DrawGlyphs(new NSRange(0, (int)_text.Length), location.ToNS());
		}
	}
}