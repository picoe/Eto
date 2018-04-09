using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Drawing
{
	public class FontsHandler : WidgetHandler<Widget>, Fonts.IHandler
	{
		HashSet<string> familyNames;
		static Pango.Context context;

		public static Pango.Context Context
		{
			get
			{
				if (context == null) {
					var label = new Gtk.Label();
					context = label.PangoContext;
				}
				return context;
			}
		}

		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get { return Context.Families.Select (r => new FontFamily (new FontFamilyHandler (r))); }
		}

		public bool FontFamilyAvailable (string fontFamily)
		{
			if (familyNames == null) {
				familyNames = new HashSet<string> (StringComparer.InvariantCultureIgnoreCase);
				foreach (var family in Context.Families) {
					familyNames.Add (family.Name);
				}
			}
			return familyNames.Contains (fontFamily);
		}
	}
}

