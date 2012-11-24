using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontFamilyHandler : WidgetHandler<Pango.FontFamily, FontFamily>, IFontFamily
	{
		public string Name { get { return Control.Name; } }

		public IEnumerable<FontTypeface> Typefaces
		{
			get { return Control.Faces.Select (r => new FontTypeface(Widget, new FontTypefaceHandler(this, r))); }
		}

		public FontFamilyHandler ()
		{
		}
		
		public FontFamilyHandler (Pango.FontFamily pangoFamily)
		{
			this.Control = pangoFamily;
		}

		public void Create (string familyName)
		{
            Control = GetFontFamily(familyName);
		}

        public static Pango.FontFamily GetFontFamily(string familyName)
        {
            return FontsHandler.Context.Families.First(r => string.Equals(r.Name, familyName, StringComparison.InvariantCultureIgnoreCase));
        }
	}
}

