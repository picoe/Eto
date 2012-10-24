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

		public void Create (FontFamily family, float size, FontStyle style)
		{
			Control = new SD.Font (family.ToSD (), size, style.ToSD ());
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

		public bool Bold
		{
			get { return this.Control.Bold; }
		}

		public bool Italic
		{
			get { return this.Control.Italic; }
		}
	}
}
