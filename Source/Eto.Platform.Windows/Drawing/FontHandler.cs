using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class FontHandler : WidgetHandler<System.Drawing.Font, Font>, IFont
	{
        public FontHandler()
        {
        }

        public FontHandler(SD.Font font)
        {
            this.Control = font;
        }

        public void Create()
        {
            var f = this.Control; // this creates the font, bizarrely
        }

        public void Create(string fontFamily, float sizeInPoints)
        {
            Control = new SD.Font(fontFamily, sizeInPoints);
        }

        public void Create(string fontFamily, float sizeInPoints, FontStyle style)
        {
            Control = new SD.Font(fontFamily, sizeInPoints, (SD.FontStyle)style);
        }

        public void Create(FontFamily family, float size, FontStyle style)
		{
			Control = new SD.Font(Generator.Convert(family), size, Convert(style));
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
		
		System.Drawing.FontStyle Convert(FontStyle style)
		{
			SD.FontStyle ret = SD.FontStyle.Regular;
			if ((style & FontStyle.Bold) != 0) ret |= SD.FontStyle.Bold;
			if ((style & FontStyle.Italic) != 0) ret |= SD.FontStyle.Italic;
			return ret;
		}

        private System.Drawing.Font font = null;
        public override System.Drawing.Font Control
        {
            get
            {
                if (this.font == null)
                {
                    var fontFamily = FontFamily;

                    var messageBoxFont =
                        SD.SystemFonts.MessageBoxFont;

                    // font family
                    if (string.IsNullOrEmpty(
                            fontFamily))
                        fontFamily =
                            messageBoxFont.FontFamily.Name;

                    // font size
                    var fontSize =
                        sizeInPixels != null
                        ? sizeInPixels.Value
                        : SD.SystemFonts.MessageBoxFont.SizeInPoints *
                            Constants.PointsToPixels;

                    var fontStyle = System.Drawing.FontStyle.Regular;

                    if (Bold)
                        fontStyle |= System.Drawing.FontStyle.Bold;

                    if (Italic)
                        fontStyle |= System.Drawing.FontStyle.Italic;

                    if (Underline)
                        fontStyle |= System.Drawing.FontStyle.Underline;

                    if (Strikeout)
                        fontStyle |= System.Drawing.FontStyle.Strikeout;

                    // call the setter
                    this.font =
                        new System.Drawing.Font(
                            fontFamily,
                            fontSize,
                            fontStyle,
                            // we always save fonts in pixel sizes.
                            SD.GraphicsUnit.Pixel);
                }

                return this.font;
            }
            protected set
            {
                if (value != null)
                {
                    // Font family - resets this.font.
                    this.fontFamily =
                        value.FontFamily.Name;

                    // FontSizePixels - resets this.font.
                    this.sizeInPixels =
                        value.SizeInPoints
                        * Constants.PointsToPixels;

                    // Bold - resets this.font.
                    this.bold =
                        (value.Style &
                         System.Drawing.FontStyle.Bold) ==
                         System.Drawing.FontStyle.Bold;

                    // Italic - resets this.font.
                    this.italic =
                        (value.Style &
                        System.Drawing.FontStyle.Italic) ==
                        System.Drawing.FontStyle.Italic;

                    // Underline - resets this.font.
                    this.underline =
                        (value.Style &
                        System.Drawing.FontStyle.Underline) ==
                        System.Drawing.FontStyle.Underline;

                    // Strikeout - resets this.font.
                    this.strikeout =
                        (value.Style &
                        System.Drawing.FontStyle.Strikeout) ==
                        System.Drawing.FontStyle.Strikeout;
                }

                this.font =
                    value;
            }
        }


        private string fontFamily;

        public string FontFamily { get { return fontFamily; } }

        private bool bold;
        public bool Bold { get { return this.bold; } }

        private bool italic;
        public bool Italic { get { return this.italic; } }

        private bool underline;
        public bool Underline { get { return this.underline; } }

        private bool strikeout;
        public bool Strikeout { get { return this.strikeout; } }

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
            FontHandler font = new FontHandler();
            font.fontFamily = FontFamily;
            font.sizeInPixels = SizeInPixels;
            font.bold = Bold;
            font.italic = Italic;
            font.underline = Underline;
            font.strikeout = Strikeout;

            // Assigned last since
            // the previous setters reset it.
            font.Control = Control;

            return font;
        }
    }
}
