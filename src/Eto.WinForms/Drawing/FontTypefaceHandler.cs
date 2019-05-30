using Eto.Drawing;
using sd = System.Drawing;

namespace Eto.WinForms.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sd.FontStyle, FontTypeface>, FontTypeface.IHandler
	{
		string name;

		public FontTypefaceHandler(sd.FontStyle style)
		{
			Control = style;
		}

		public string Name => name ?? (name = GetName());

		public string LocalizedName => Name;

		public FontStyle FontStyle => Control.ToEtoStyle();

		string GetName() => FontStyle.ToString().Replace(",", string.Empty);
	}
}
