using System;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public static class FontExtensions
	{
		public static SD.Font ToSD(this Font font)
		{
			if (font == null)
				return null;
			var handler = (FontHandler)font.Handler;
			return handler.Control;
		}

		public static Font ToEto(this SD.Font font, Eto.Generator generator)
		{
			return font == null ? null : new Font(generator, new FontHandler(font));
		}
	}

	public class FontHandler : WidgetHandler<System.Drawing.Font, Font>, IFont
	{
		FontTypeface typeface;
		FontFamily family;

		public FontHandler()
		{
		}

		public FontHandler(SD.Font font)
		{
			Control = font;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.family = family;
			var familyHandler = (FontFamilyHandler)family.Handler;
			Control = new SD.Font(familyHandler.Control, size, style.ToSD() | decoration.ToSD());
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			this.typeface = typeface;

			var familyHandler = (FontFamilyHandler)typeface.Family.Handler;
			Control = new SD.Font(familyHandler.Control, size, typeface.FontStyle.ToSD() | decoration.ToSD());
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			switch (systemFont)
			{
				case SystemFont.Default:
					Control = SD.SystemFonts.DefaultFont;
					break;
				case SystemFont.Bold:
					Control = new SD.Font(SD.SystemFonts.DefaultFont, SD.FontStyle.Bold);
					break;
				case SystemFont.TitleBar:
					Control = SD.SystemFonts.CaptionFont;
					break;
				case SystemFont.ToolTip:
					Control = SD.SystemFonts.DefaultFont;
					break;
				case SystemFont.Label:
					Control = SD.SystemFonts.DialogFont;
					break;
				case SystemFont.MenuBar:
					Control = SD.SystemFonts.MenuFont;
					break;
				case SystemFont.Menu:
					Control = SD.SystemFonts.MenuFont;
					break;
				case SystemFont.Message:
					Control = SD.SystemFonts.MessageBoxFont;
					break;
				case SystemFont.Palette:
					Control = SD.SystemFonts.DialogFont;
					break;
				case SystemFont.StatusBar:
					Control = SD.SystemFonts.StatusFont;
					break;
				default:
					throw new NotSupportedException();
			}
			if (size != null || decoration != FontDecoration.None)
			{
				var newsize = size ?? Control.SizeInPoints;
				Control = new SD.Font(Control.FontFamily, newsize, Control.Style | decoration.ToSD(), SD.GraphicsUnit.Point);
			}
		}

		public string FamilyName
		{
			get { return Control.FontFamily.Name; }
		}

		public FontStyle FontStyle
		{
			get { return Control.Style.ToEtoStyle(); }
		}

		public FontDecoration FontDecoration
		{
			get { return Control.Style.ToEtoDecoration(); }
		}

		public FontFamily Family
		{
			get { return family = family ?? new FontFamily(Widget.Generator, new FontFamilyHandler(Control.FontFamily)); }
		}

		public FontTypeface Typeface
		{
			get { return typeface = typeface ?? new FontTypeface(Family, new FontTypefaceHandler(Control.Style)); }
		}

		public SD.FontFamily WindowsFamily
		{
			get { return Control.FontFamily; }
		}

		public float XHeight
		{
			get { return Size * 0.5f; }
		}

		public float Baseline
		{
			get { return Ascent; }
		}

		public float Leading
		{
			get { return LineHeight - (Ascent + Descent); }
		}

		float? ascent;
		public float Ascent
		{
			get
			{
				ascent = ascent ?? Size * Control.FontFamily.GetCellAscent(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);
				return ascent ?? 0f;
			}
		}

		float? descent;
		/// <summary>
		/// Gets the descent of the font
		/// </summary>
		/// <remarks>
		/// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
		/// </remarks>
		public float Descent
		{
			get
			{
				descent = descent ?? Size * Control.FontFamily.GetCellDescent(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);
				return descent ?? 0f;
			}
		}

		public float LineHeight { get { return Size * Control.FontFamily.GetLineSpacing(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style); } }

		public float Size { get { return Control.SizeInPoints; } }
	}
}
