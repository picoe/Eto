using System;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontHandler : WidgetHandler<Pango.FontDescription, Font>, IFont
	{
		FontFamily family;
		FontTypeface face;
		FontStyle? style;

		public FontHandler ()
		{
		}

		public FontHandler (Gtk.Widget widget)
			: this (widget.Style.FontDescription)
		{
		}

		public FontHandler (Pango.FontDescription fontDescription)
		{
			Control = fontDescription;
		}

		public FontHandler (string fontName)
		{
			Control = Pango.FontDescription.FromString (fontName);
		}

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
			this.family = family;
			Control = new Pango.FontDescription();
			SetStyle (style);
			Size = size;

			Control.Family = family.Name;
		}

		public void Create (FontTypeface face, float size)
		{
			this.face = face;
			Control = ((FontTypefaceHandler)face.Handler).Control.Describe();
			Size = size;
		}

		void SetStyle (FontStyle style)
		{
			if ((style & FontStyle.Bold) != 0) 
				Control.Weight = Pango.Weight.Bold;
			if ((style & FontStyle.Italic) != 0) 
				Control.Style = Pango.Style.Italic;
		}

        public float Size
        {
            get { return (float)(Control.Size / Pango.Scale.PangoScale); }
            private set { Control.Size = (int)(value * Pango.Scale.PangoScale); }
        }

        public FontStyle FontStyle
		{
			get {
				if (style == null) {
					style = FontStyle.Normal;
					if (Control.Weight == Pango.Weight.Bold)
						style |= FontStyle.Bold;
					if (Control.Style == Pango.Style.Italic)
						style |= FontStyle.Italic;
				}
				return style.Value;
			}
        }

		public string FamilyName
		{
			get { return Control.Family; }
        }

		public FontFamily Family
		{
			get {
				if (family == null) {
					family = new FontFamily (Widget.Generator, Control.Family);
				}
				return family;
			}
		}

		public FontTypeface Typeface
		{
			get {
				if (face == null) {
					face = Family.Typefaces.FirstOrDefault(r => r.FontStyle == this.FontStyle);
				}
				return face;
			}
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

        public float ExHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public void Create()
        {
            throw new NotImplementedException();
        }
    }
}
