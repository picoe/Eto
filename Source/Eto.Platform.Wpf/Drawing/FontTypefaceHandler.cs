using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<swm.Typeface, FontTypeface>, IFontTypeface
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
				return Conversions.Convert (Control.Style, Control.Weight);
			}
		}
	}
}
