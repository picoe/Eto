using System;
using Eto.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;

namespace Eto.Platform.iOS.Drawing
{
	public class FontFamilyHandler : WidgetHandler<object, FontFamily>, IFontFamily
	{
		public string MacName { get; set; }

		public FontFamilyHandler ()
		{
		}

		public FontFamilyHandler (string familyName)
		{
			Create (familyName);
		}

		public void Create (string familyName)
		{
			this.Name = this.MacName = familyName;
			switch (familyName.ToLowerInvariant ()) {
			case FontFamilies.MonospaceFamilyName:
				MacName = "Courier New";
				break;
			case FontFamilies.SansFamilyName:
				MacName = "Helvetica";
				break;
			case FontFamilies.SerifFamilyName:
				MacName = "Times";
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

		public IEnumerable<FontTypeface> Typefaces {
			get { return UIFont.FontNamesForFamilyName(this.MacName).Select(r => new FontTypeface(Widget, new FontTypefaceHandler(r))); }
		}

		public UIFont CreateFont (float size, FontStyle style)
		{
			var matched = Typefaces.FirstOrDefault (r => r.FontStyle == style);
			if (matched == null) matched = Typefaces.First ();
			var handler = matched.Handler as FontTypefaceHandler;
			return handler.CreateFont(size);
		}

		public FontTypeface GetFace(UIFont font)
		{
			return Typefaces.FirstOrDefault (r => string.Equals (((FontTypefaceHandler)r.Handler).FontName, font.Name, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}

