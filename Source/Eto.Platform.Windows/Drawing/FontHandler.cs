using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public static class FontExtensions
	{
		public static SD.Font ToSD (this Font font)
		{
			if (font == null)
				return null;
			var handler = font.Handler as FontHandler;
			return handler.Control;
		}

		public static Font ToEto (this SD.Font font, Eto.Generator generator)
		{
			if (font == null)
				return null;
			return new Font (generator, new FontHandler (font));
		}
	}

	public class FontHandler : WidgetHandler<System.Drawing.Font, Font>, IFont
	{
		FontTypeface typeface;
		FontFamily family;

		public FontHandler ()
		{
		}

		public FontHandler (SD.Font font)
		{
			Control = font;
		}

		public void Create (FontFamily family, float size, FontStyle style)
		{
			this.family = family;
			var familyHandler = (FontFamilyHandler)family.Handler;
			Control = new SD.Font (familyHandler.Control, size, style.ToSD ());
		}

		public void Create (FontTypeface typeface, float size)
		{
			this.typeface = typeface;

			Control = new SD.Font (typeface.Family.Name, size, typeface.FontStyle.ToSD ());
		}
		
		public void Create (SystemFont systemFont, float? size)
		{
			switch (systemFont) {
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
				throw new NotImplementedException();
			}
			if (size != null) {
				Control = new SD.Font(Control.FontFamily, size.Value, Control.Style, SD.GraphicsUnit.Point);
			}
		}
		
		public string FamilyName
		{
			get { return this.Control.FontFamily.Name; }
		}


		public FontStyle FontStyle
		{
			get { return Control.Style.ToEto (); }
		}

		public FontFamily Family
		{
			get
			{
				if (family == null) {
					family = new FontFamily (Widget.Generator, new FontFamilyHandler (Control.FontFamily));
				}
				return family;
			}
		}

		public FontTypeface Typeface
		{
			get
			{
				if (typeface == null) {
					typeface = new FontTypeface (Family, new FontTypefaceHandler (Control.Style));
				}
				return typeface;
			}
		}

		public SD.FontFamily WindowsFamily
		{
			get { return Control.FontFamily; }
		}


		public float XHeight
		{
			get
			{
				return Size * 0.5f;
			}
		}

		public float Baseline
		{
			get { return Ascent; }
		}

		public float Leading
		{
			get { return LineHeight - (Ascent + Descent); }
		}

		float? ascentInPixels;
		public float Ascent
		{
			get
			{
				if (ascentInPixels == null)
					ascentInPixels = Size * Control.FontFamily.GetCellAscent (Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);
				return ascentInPixels ?? 0f;
			}
		}

		float? descentInPixels;
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
				if (descentInPixels == null)
					descentInPixels = Size * Control.FontFamily.GetCellDescent(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);

				return descentInPixels ?? 0f;
			}
		}

		public float LineHeight { get { return Size * Control.FontFamily.GetLineSpacing (Control.Style) / Control.FontFamily.GetEmHeight(Control.Style); } }

		public float Size { get { return Control.SizeInPoints; } }
	}
}
