using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontHandler : WidgetHandler<Pango.FontDescription, Font>, IFont
	{
		
		public void Create (SystemFont systemFont, float? size)
		{
			switch (systemFont) {
			case SystemFont.Default:
				Control = Gtk.Window.DefaultStyle.FontDescription;
				break;
			case SystemFont.Bold:
				Control = new Pango.FontDescription();
				Control.Merge (Gtk.Window.DefaultStyle.FontDescription, true);
				Control.Weight = Pango.Weight.Bold;
				break;
			case SystemFont.TitleBar:
				Control = Gtk.Window.DefaultStyle.FontDescription;
				break;
			case SystemFont.ToolTip:
				Control = Gtk.Label.DefaultStyle.FontDescription;
				break;
			case SystemFont.Label:
				Control = Gtk.Label.DefaultStyle.FontDescription;
				break;
			case SystemFont.MenuBar:
				Control = Gtk.MenuBar.DefaultStyle.FontDescription;
				break;
			case SystemFont.Menu:
				Control = Gtk.Menu.DefaultStyle.FontDescription;
				break;
			case SystemFont.Message:
				Control = Gtk.MessageDialog.DefaultStyle.FontDescription;
				break;
			case SystemFont.Palette:
				Control = Gtk.Dialog.DefaultStyle.FontDescription;
				break;
			case SystemFont.StatusBar:
				Control = Gtk.Statusbar.DefaultStyle.FontDescription;
				break;
			default:
				throw new NotImplementedException();
			}
			if (size != null) {
				var old = Control;
				Control = new Pango.FontDescription();
				Control.Merge (old, true);
				Control.Size = (int)(size * Pango.Scale.PangoScale);
			}
		}
		
		public void Create (FontFamily family, float size, FontStyle style)
		{
			Control = new Pango.FontDescription();
			Control.Size = (int)(size * Pango.Scale.PangoScale);
			if ((style & FontStyle.Bold) != 0) 
				Control.Weight = Pango.Weight.Bold;
			if ((style & FontStyle.Italic) != 0) 
				Control.Style = Pango.Style.Italic;
			
			string fontFamily = string.Empty;
			switch (family)
			{
				case FontFamily.Monospace: fontFamily = "monospace"; break;
				default: case FontFamily.Sans: fontFamily = "sans"; break;
				case FontFamily.Serif: fontFamily = "serif"; break;
			}
			Control.Family = fontFamily;
		}

		public float Size
		{
			get { return (float)(Control.Size / Pango.Scale.PangoScale); }
		}

		public bool Bold
		{
			get { return Control.Weight == Pango.Weight.Bold; }
		}

		public bool Italic
		{
			get { return Control.Style == Pango.Style.Italic; }
		}


	}
}
