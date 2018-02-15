using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<Pango.FontFace, FontTypeface>, FontTypeface.IHandler
	{
		public FontTypefaceHandler (Pango.FontFace pangoFace)
		{
			this.Control = pangoFace;
		}

		public string Name
		{
			get { return Control.FaceName; }
		}

		public FontStyle FontStyle
		{
			get {
				var style = FontStyle.None;
				var description = Control.Describe ();
				if (description.Style == Pango.Style.Italic || description.Style == Pango.Style.Oblique)
					style |= FontStyle.Italic;
				if ((int)description.Weight >= (int)Pango.Weight.Semibold)
					style |= FontStyle.Bold;
				return style;
			}
		}
	}
}

