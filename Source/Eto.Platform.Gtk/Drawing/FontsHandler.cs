using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontsHandler : IFonts
	{
		static Pango.Context context;
		public static Pango.Context Context
		{
			get {
				if (context == null) {
					var window = new Gtk.Window (string.Empty);
					context = window.PangoContext;
				}
				return context;
			}
		}

		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }


		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get { return Context.Families.Select (r => new FontFamily(Generator.Current, new FontFamilyHandler(r))); }
		}

		public FontFamily GetSystemFontFamily (string systemFamilyName)
		{
			switch (systemFamilyName) {
			case FontFamilies.MonospaceFamilyName:
				systemFamilyName = "monospace";
				break;
			case FontFamilies.SansFamilyName:
				systemFamilyName = "sans";
				break;
			case FontFamilies.SerifFamilyName:
				systemFamilyName = "serif";
				break;
			default:
				throw new NotSupportedException ();
			}
			return new FontFamily (Generator.Current, systemFamilyName);
		}
	}
}

