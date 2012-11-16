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
				throw new NotSupportedException();
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
				case Eto.Drawing.FontFamily.Monospace: fontFamily = "monospace"; break;
				default: case Eto.Drawing.FontFamily.Sans: fontFamily = "sans"; break;
                case Eto.Drawing.FontFamily.Serif: fontFamily = "serif"; break;
			}
			Control.Family = fontFamily;
		}

		public bool Bold
		{
			get { return Control.Weight == Pango.Weight.Bold; }
        }

		public bool Italic
		{
			get { return Control.Style == Pango.Style.Italic; }
        }

        public bool Underline
        {
            get { throw new NotImplementedException(); }
        }

        public bool Strikeout
        {
            get { throw new NotImplementedException(); }
        }

        public float SizeInPoints
        {
            get { throw new NotImplementedException(); }
        }

        public string FontFamily
        {
            get { throw new NotImplementedException(); }
        }

        public float EmHeightPixels
        {
            get { throw new NotImplementedException(); }
        }

        public float AscentInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public float DescentInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public float HeightInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public float SizeInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public IFont Clone()
        {
            throw new NotImplementedException();
        }

        public void Create(string fontFamily, float sizeInPoints, FontStyle style)
        {
            throw new NotImplementedException();
        }

        public float ExHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public void Create(string fontFamily, float size)
        {
            throw new NotImplementedException();
        }


        public void Create()
        {
            throw new NotImplementedException();
        }
    }
}
