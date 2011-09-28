using System;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
{
	public class FontHandler : WidgetHandler<NSFont, Font>, IFont, IDisposable
	{
		public const float FONT_SIZE_FACTOR = 1.3F;
		
		bool bold;
		bool italic;
		
		public void Create (SystemFont systemFont, float? fontSize)
		{
			var size = fontSize;
			if (fontSize != null) size = size.Value * FONT_SIZE_FACTOR;
			bold = false;
			italic = false;
			switch (systemFont) {
			case SystemFont.Default:
				Control = NSFont.SystemFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Bold:
				bold = true;
				Control = NSFont.BoldSystemFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.TitleBar:
				Control = NSFont.TitleBarFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.ToolTip:
				Control = NSFont.ToolTipsFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Label:
				Control = NSFont.LabelFontOfSize(size ?? NSFont.LabelFontSize);
				break;
			case SystemFont.MenuBar:
				Control = NSFont.MenuBarFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Menu:
				Control = NSFont.MenuFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Message:
				Control = NSFont.MessageFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Palette:
				Control = NSFont.PaletteFontOfSize(size ?? NSFont.SmallSystemFontSize);
				break;
			case SystemFont.StatusBar:
				Control = NSFont.SystemFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			default:
				throw new NotSupportedException();
			}
		}
		
		public void Create (FontFamily family, float size, FontStyle style)
		{
			string familyString;
			switch (family)
			{
			case FontFamily.Monospace: familyString = "Courier New"; break; 
			default:
			case FontFamily.Sans: familyString = "Helvetica"; break; 
			case FontFamily.Serif: familyString = "Times"; break; 
			}
			bold = (style & FontStyle.Bold) != 0;
			NSFontTraitMask traits = (bold) ? NSFontTraitMask.Bold : NSFontTraitMask.Unbold;
			italic = (style & FontStyle.Italic) != 0;
			traits |= (italic) ? NSFontTraitMask.Italic : NSFontTraitMask.Unitalic;
			var font = NSFontManager.SharedFontManager.FontWithFamily(familyString, traits, 3, size * FONT_SIZE_FACTOR);
			if (font == null || font.Handle == IntPtr.Zero) throw new Exception(string.Format("Could not allocate font with family {0}, traits {1}, size {2}", familyString, traits, size));
			Control = font;
		}

		public float Size
		{
			get { return Control.PointSize / FONT_SIZE_FACTOR; }
		}

		public bool Bold
		{
			get { return bold; }
		}

		public bool Italic
		{
			get { return italic; }
		}

	}
}
