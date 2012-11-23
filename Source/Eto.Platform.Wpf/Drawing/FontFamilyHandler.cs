using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using sw = System.Windows;
using System.Globalization;

namespace Eto.Platform.Wpf.Drawing
{

	public class FontFamilyHandler : WidgetHandler<swm.FontFamily, FontFamily>, IFontFamily
	{
		public FontFamilyHandler ()
		{
		}

		public FontFamilyHandler (swm.FontFamily wpfFamily)
		{
			this.Control = wpfFamily;
		}

		public void Create (string familyName)
		{
			Control = new swm.FontFamily (familyName);
		}

		public string Name
		{
			get { return Control.Source; }
		}

		public IEnumerable<FontTypeface> Typefaces
		{
			get {
				foreach (var type in Control.GetTypefaces ()) {
					yield return new FontTypeface(Widget, new FontTypefaceHandler (type));
				}
			}
		}

		public FontTypeface GetFamilyTypeface (sw.FontStyle fontStyle, sw.FontWeight fontWeight)
		{
			var typefaces = Control.GetTypefaces ();
			foreach (var type in typefaces) {
				if (type.Style == fontStyle && type.Weight == fontWeight)
					return new FontTypeface(Widget, new FontTypefaceHandler (type));
			}
			return new FontTypeface(Widget, new FontTypefaceHandler (typefaces.First ()));
		}
	}
}
