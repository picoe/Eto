using Eto.Drawing;
using System.Globalization;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;

namespace Eto.Wpf.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<swm.Typeface, FontTypeface>, FontTypeface.IHandler
	{
		string name;

		public FontTypefaceHandler (swm.Typeface type)
		{
			this.Control = type;
		}

		public string Name
		{
			get
			{
				if (name == null) {
					var lang = sw.Markup.XmlLanguage.GetLanguage (CultureInfo.CurrentUICulture.IetfLanguageTag);
					if (!Control.FaceNames.TryGetValue (lang, out name)) {
						name = Control.FaceNames.First ().Value;
					}
				}
				return name;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return WpfConversions.Convert (Control.Style, Control.Weight);
			}
		}
	}
}
