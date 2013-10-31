using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<Pango.FontFace, FontTypeface>, IFontTypeface
	{
		public FontTypefaceHandler (FontFamilyHandler family, Pango.FontFace pangoFace)
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
				if (description.Style.HasFlag (Pango.Style.Italic))
					style |= FontStyle.Italic;
				if (description.Weight.HasFlag (Pango.Weight.Bold))
					style |= FontStyle.Bold;
				return style;
			}
		}
	}
}

