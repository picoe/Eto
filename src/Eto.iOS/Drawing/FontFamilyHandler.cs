using System;
using Eto.Drawing;
using System.Collections.Generic;
using UIKit;
using System.Linq;

namespace Eto.iOS.Drawing
{
	public class FontFamilyHandler : WidgetHandler<object, FontFamily>, FontFamily.IHandler
	{
		public string MacName { get; set; }

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(string familyName)
		{
			Create(familyName);
		}

		public void Create(string familyName)
		{
			Name = MacName = familyName;
			switch (familyName.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					MacName = "Courier New";
					break;
				case FontFamilies.SansFamilyName:
					MacName = "Helvetica";
					break;
				case FontFamilies.SerifFamilyName:
					MacName = "Times New Roman";
					break;
				case FontFamilies.CursiveFamilyName:
					MacName = "Papyrus";
					break;
				case FontFamilies.FantasyFamilyName:
					MacName = "Futura";
					break;
			}

		}

		public string Name { get; private set; }

		public string LocalizedName
		{
			get { return Name; }
		}

		public IEnumerable<FontTypeface> Typefaces
		{
			get { return UIFont.FontNamesForFamilyName(MacName).Select(r => new FontTypeface(Widget, new FontTypefaceHandler(r))); }
		}

		public UIFont CreateFont(float size, FontStyle style)
		{
			var matched = Typefaces.FirstOrDefault(r => r.FontStyle == style);
			if (matched == null)
				matched = Typefaces.FirstOrDefault();
			if (matched == null)
				return UIFont.SystemFontOfSize(size);
			var handler = (FontTypefaceHandler)matched.Handler;
			return handler.CreateFont(size);
		}

		public FontTypeface GetFace(UIFont font)
		{
			return Typefaces.FirstOrDefault(r => string.Equals(((FontTypefaceHandler)r.Handler).FontName, font.Name, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}

