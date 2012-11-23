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

        public void Create()
        {
            var f = this.Control; // this creates the font, bizarrely
        }

        public void Create(string fontFamily, float sizeInPoints)
        {
            Control = new SD.Font(fontFamily, sizeInPoints);
        }

        public void Create(string fontName, float size, FontStyle style)
		{
			Control = new SD.Font (fontName, size, style.ToSD ());
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
		
		public float Size
		{
			get { return this.Control.Size; }
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


        public float ExHeightInPixels
        {
            get
            {
#if DEBUG
                // Hard code Ahem font characteristics
                // for testability.
                if (Control != null &&
                    Control.FontFamily != null &&
                    Control.FontFamily.Name == "Ahem")
                    return
                        SizeInPixels * 0.8f;
#endif
                return
                    SizeInPixels * 0.5f;
            }
        }

        /// <summary>
        /// Gets the ascent of the font
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        private float? ascentInPixels;
        public float AscentInPixels
        {
            get
            {
                if (ascentInPixels == null)
                    ascentInPixels =
                        Control != null
                        ? SizeInPixels
                        * Control.FontFamily.GetCellAscent(
                            Control.Style)
                        / Control.FontFamily.GetEmHeight(
                            Control.Style)
                        : 0f;

                return
                    ascentInPixels == null
                    ? 0f
                    : ascentInPixels.Value;
            }
        }

        /// <summary>
        /// Gets the descent of the font
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        private float? descentInPixels;
        public float DescentInPixels
        {
            get
            {
                if (descentInPixels == null)
                    descentInPixels =
                        Control != null
                        ? SizeInPixels
                        * Control.FontFamily.GetCellDescent(
                            Control.Style)
                        / Control.FontFamily.GetEmHeight(
                            Control.Style)
                        : 0f;

                return
                    descentInPixels == null
                    ? 0f
                    : descentInPixels.Value;
            }
        }

        private float? heightInPixels;
        public float HeightInPixels
        {
            get
            {
                if (heightInPixels == null &&
                    Control != null)
                    heightInPixels = Control.Height;

                return
                    heightInPixels == null
                    ? 0f
                    : heightInPixels.Value;
            }
        }

        private float? sizeInPoints;
        public float SizeInPoints
        {
            get
            {
                if (sizeInPoints == null &&
                    Control != null)
                    sizeInPoints =
                        Control.SizeInPoints;

                return sizeInPoints == null
                    ? 0f
                    : sizeInPoints.Value;
            }
        }

        private float? sizeInPixels;
        public float SizeInPixels
        {
            get
            {
                if (sizeInPixels == null &&
                    Control != null)
                    sizeInPixels =
                        Control.SizeInPoints
                        * Constants.PointsToPixels;

                return sizeInPixels == null
                    ? 0f
                    : sizeInPixels.Value;
            }
        }

        public IFont Clone()
        {
            throw new NotImplementedException();
        }
    }
}
