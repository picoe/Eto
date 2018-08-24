using Eto.Drawing;
using sd = System.Drawing;

namespace Eto.WinForms.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sd.FontStyle, FontTypeface>, FontTypeface.IHandler
	{
		public FontTypefaceHandler (sd.FontStyle style)
		{
			this.Control = style;
			Name = this.FontStyle.ToString ().Replace (',', ' ');
		}

		public string Name { get; set; }

		public string LocalizedName => Name;

		public FontStyle FontStyle
		{
			get { return Control.ToEtoStyle (); }
		}
	}
}
