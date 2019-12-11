using System;
using System.Globalization;
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

#if IOS

using UIKit;
using Foundation;
using NSFont = UIKit.UIFont;

namespace Eto.iOS.Drawing

#elif OSX

namespace Eto.Mac.Drawing
#endif
{
	public class FontHandler : WidgetHandler<NSFont, Font>, Font.IHandler
	{
		FontFamily family;
		FontTypeface typeface;
		FontStyle? style;
		FontDecoration decoration;
		NSDictionary _attributes;
		FormattedText _formattedText;

		FormattedText FormattedText => _formattedText ?? (_formattedText = new FormattedText { Font = Widget });

		static NSLayoutManager s_layoutManager;
		static NSLayoutManager SharedLayoutManager => s_layoutManager ?? (s_layoutManager = new NSLayoutManager { UsesFontLeading = true });

		public FontHandler()
		{
		}

		public FontHandler(NSFont font)
		{
			Control = font;
		}

		public FontHandler(NSFont font, FontDecoration decoration)
		{
			Control = font;
			this.decoration = decoration;
		}

		public FontHandler(NSFont font, FontStyle style)
		{
			Control = font;
			this.style = style;
		}

		public void Create(FontTypeface face, float size, FontDecoration decoration)
		{
			typeface = face;
			family = face.Family;
			Control = ((FontTypefaceHandler)face.Handler).CreateFont(size);
			this.decoration = decoration;
		}

		public void Create(SystemFont systemFont, float? fontSize, FontDecoration decoration)
		{
			switch (systemFont)
			{
				case SystemFont.Default:
					Control = NSFont.SystemFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.Bold:
					Control = NSFont.BoldSystemFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.Label:
#if IOS
					Control = NSFont.SystemFontOfSize(fontSize ?? NSFont.LabelFontSize);
#elif OSX
					Control = NSFont.LabelFontOfSize((nfloat)(fontSize ?? (float)(NSFont.LabelFontSize + 2))); // labels get a size of 12 
#endif
					break;
#if OSX
				case SystemFont.TitleBar:
					Control = NSFont.TitleBarFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.ToolTip:
					Control = NSFont.ToolTipsFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.MenuBar:
					Control = NSFont.MenuBarFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.Menu:
					Control = NSFont.MenuFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.Message:
					Control = NSFont.MessageFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.Palette:
					Control = NSFont.PaletteFontOfSize((nfloat)(fontSize ?? (float)NSFont.SmallSystemFontSize));
					break;
				case SystemFont.StatusBar:
					Control = NSFont.SystemFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
				case SystemFont.User:
					Control = NSFont.UserFontOfSize((nfloat)(fontSize ?? (float)NSFont.SystemFontSize));
					break;
#endif
				default:
					throw new NotSupportedException();
			}
			this.decoration = decoration;
		}

		#if OSX
		NSFontTraitMask? traits;
		[Obsolete]
		public static NSFont CreateFont(FontFamilyHandler familyHandler, float size, NSFontTraitMask traits, int weight = 5)
		{
			return CreateFont(familyHandler.MacName, size, traits, weight);
		}

		public static NSFont CreateFont(string familyName, nfloat size, NSFontTraitMask traits, int weight = 5)
		{
			var font = NSFontManager.SharedFontManager.FontWithFamily(familyName, traits, weight, size);
			if (font == null)
			{
				if (traits.HasFlag(NSFontTraitMask.Italic))
				{
					// fake italics by transforming the font
					const float kRotationForItalicText = 14.0f;
					var fontTransform = new NSAffineTransform();
					fontTransform.Scale(size);
					var italicTransform = new NSAffineTransform();
					italicTransform.TransformStruct = Matrix.FromSkew(0, kRotationForItalicText).ToCG();
					fontTransform.AppendTransform(italicTransform);
					traits &= ~NSFontTraitMask.Italic;
					font = NSFontManager.SharedFontManager.FontWithFamily(familyName, traits, 5, size);
					if (font != null)
					{
						font = NSFont.FromDescription(font.FontDescriptor, fontTransform);
					}
				}
			}
			return font;
		}
		#endif

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.style = style;
			this.family = family;
			this.decoration = decoration;
#if OSX
			var familyHandler = (FontFamilyHandler)family.Handler;
			traits = style.ToNS() & familyHandler.TraitMask;
			var font = CreateFont(familyHandler.MacName, size, traits.Value);

			if (font == null || font.Handle == IntPtr.Zero)
				throw new ArgumentOutOfRangeException(string.Empty, string.Format(CultureInfo.CurrentCulture, "Could not allocate font with family {0}, traits {1}, size {2}", family.Name, traits, size));
#elif IOS
			var familyHandler = (FontFamilyHandler)family.Handler;
			var font = familyHandler.CreateFont (size, style);
			/*
			var familyString = new StringBuilder();
			switch (family)
			{
			case FontFamily.Monospace: familyString.Append ("CourierNewPS"); suffix = "MT"; break; 
			default:
			case FontFamily.Sans: familyString.Append ("Helvetica"); italicString = "Oblique"; break; 
			case FontFamily.Serif: familyString.Append ("TimesNewRomanPS"); suffix = "MT"; break; 
			}
			bold = (style & FontStyle.Bold) != 0;
			italic = (style & FontStyle.Italic) != 0;
			
			if (bold || italic) familyString.Append ("-");
			if (bold) familyString.Append ("Bold");
			if (italic) familyString.Append (italicString);
			
			familyString.Append (suffix);
			var font = UIFont.FromName (familyString.ToString (), size);
			*/
			#endif
			Control = font;
		}

		public float Size => (float)Control.PointSize;

		public string FamilyName => Control.FamilyName;

		public FontFamily Family
		{
			get
			{
				if (family == null)
					family = new FontFamily(new FontFamilyHandler(Control.FamilyName));
				return family;
			}
		}

		public FontTypeface Typeface
		{
			get
			{
				if (typeface == null)
					#if IOS
					typeface = ((FontFamilyHandler)Family.Handler).GetFace(Control);
					#else
					typeface = ((FontFamilyHandler)Family.Handler).GetFace(Control, traits);
					#endif
				return typeface;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				if (style == null)
#if OSX
					style = NSFontManager.SharedFontManager.TraitsOfFont(Control).ToEto();
#elif IOS
					style = Typeface.FontStyle;
#endif
				return style.Value;
			}
		}

		public FontDecoration FontDecoration => decoration;

		public float Ascent => (float)Control.Ascender;

		public float Descent => (float)-Control.Descender;

		public float XHeight
		{
#if OSX
			get { return (float)Control.XHeight; }
#elif IOS
			get { return (float)Control.xHeight; }
#endif
		}

		public float Leading => (float)Control.Leading;

		public float Baseline => LineHeight - Leading - Descent;

		public float LineHeight => (float)SharedLayoutManager.DefaultLineHeightForFont(Control);

		public NSDictionary Attributes
		{
			get
			{ 
				if (_attributes == null)
					CreateAttributes();
				return _attributes ?? (_attributes = CreateAttributes());
			}
		}

		static readonly NSObject[] attributeKeys =
		{
#if OSX
			#if UNIFIED
			NSStringAttributeKey.Font,
			NSStringAttributeKey.UnderlineStyle,
			NSStringAttributeKey.StrikethroughStyle
			#else
			NSAttributedString.FontAttributeName,
			NSAttributedString.UnderlineStyleAttributeName,
			NSAttributedString.StrikethroughStyleAttributeName
			#endif
#elif IOS
			UIStringAttributeKey.Font,
			UIStringAttributeKey.UnderlineStyle,
			UIStringAttributeKey.StrikethroughStyle
#endif
		};

		NSDictionary CreateAttributes()
		{
			return NSDictionary.FromObjectsAndKeys(
				new NSObject[]
				{
					Control ?? NSFont.UserFontOfSize(Size),
					new NSNumber((int)(decoration.HasFlag(FontDecoration.Underline) ? NSUnderlineStyle.Single : NSUnderlineStyle.None)),
					NSNumber.FromBoolean(decoration.HasFlag(FontDecoration.Strikethrough))
				},
				attributeKeys
			);
		}

		public SizeF MeasureString(string text)
		{
			FormattedText.Text = text;
			return FormattedText.Measure();
		}
	}
}