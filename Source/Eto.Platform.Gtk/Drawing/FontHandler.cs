using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontHandler : WidgetHandler<Pango.FontDescription, Font>, IFont
	{


		public void Create(FontFamily family)
		{
			Control = new Pango.FontDescription();
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
			set { 
				Control.Size = (int)(value * Pango.Scale.PangoScale); 
			}
		}

		public bool Bold
		{
			get { return Control.Weight == Pango.Weight.Bold; }
			set { Control.Weight = (value) ? Pango.Weight.Bold : Pango.Weight.Normal; }
		}

		public bool Italic
		{
			get { return Control.Style == Pango.Style.Italic; }
			set { Control.Style = (value) ? Pango.Style.Italic : Pango.Style.Normal; }
		}


	}
}
