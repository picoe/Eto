using System;
using Eto.Drawing;
using UIKit;

namespace Eto.iOS.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<object, FontTypeface>, FontTypeface.IHandler
	{
		FontStyle? style;
		string name;

		public string FontName { get; private set; }

		public FontTypefaceHandler (string fontName)
		{
			this.FontName = fontName;
		}

		public string Name {
			get {
				if (name == null) {
					// parse this out!
					name = FontName;
					// find '-' to split the family name with the typeface
					var splitIndex = name.IndexOf ('-');
					if (splitIndex > 0)
						name = name.Substring (splitIndex + 1);
					// trim the family name if no split is found
					var familyPrefix = Widget.Family.Name.Replace (" ", string.Empty);
					if (name.StartsWith (familyPrefix, StringComparison.InvariantCultureIgnoreCase))
						name = name.Substring (familyPrefix.Length);
					// remove PS (postscript?) name prefix
					if (name.StartsWith ("PS"))
						name = name.Substring (2);
					if (name.EndsWith ("MT"))
						name = name.Substring (0, name.Length - 2);

					name = name.TrimStart ('-');
					if (string.IsNullOrEmpty (name))
						name = "Normal";
				}
				return name;
			}
		}

		public FontStyle FontStyle {
			get {
				if (style == null) {
					style = FontStyle.None;
					var name = this.Name;
					if (name.Contains ("Bold"))
						style |= FontStyle.Bold;
					if (name.Contains ("Italic"))
						style |= FontStyle.Italic;
					if (name.Contains ("Oblique"))
						style |= FontStyle.Italic;
				}
				return style.Value;
			}
		}

		public UIFont CreateFont (float size)
		{
			return UIFont.FromName (FontName, size);
		}
	}
}

