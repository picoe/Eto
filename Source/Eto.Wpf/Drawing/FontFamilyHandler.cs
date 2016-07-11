using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;
using swd = System.Windows.Documents;
using System.Diagnostics;
using System.Globalization;

namespace Eto.Wpf.Drawing
{

	public class FontFamilyHandler : WidgetHandler<swm.FontFamily, FontFamily>, FontFamily.IHandler
	{
		public FontFamilyHandler ()
		{
		}

		public FontFamilyHandler(swm.FontFamily wpfFamily)
		{
			Control = wpfFamily;
			var familyMapName = Control.FamilyNames.Select(r => r.Value).FirstOrDefault();
			Name = familyMapName ?? Control.Source;
		}

		public FontFamilyHandler(swd.TextSelection range, sw.Controls.RichTextBox control)
		{
			Control = range.GetPropertyValue(swd.TextElement.FontFamilyProperty) as swm.FontFamily ?? swd.TextElement.GetFontFamily(control);
			var familyMapName = Control.FamilyNames.Select(r => r.Value).FirstOrDefault();
			Name = familyMapName ?? Control.Source;
		}

		public void Create (string familyName)
		{
			Name = familyName;
			switch (familyName.ToUpperInvariant ()) {
			case FontFamilies.MonospaceFamilyName:
				familyName = "Courier New";
				break;
			case FontFamilies.SansFamilyName:
				familyName = "Tahoma, Arial, Verdana, Trebuchet, MS Sans Serif, Helvetica";
				break;
			case FontFamilies.SerifFamilyName:
				familyName = "Times New Roman";
				break;
			case FontFamilies.CursiveFamilyName:
				familyName = "Comic Sans MS, Monotype Corsiva, Papryus";
				break;
			case FontFamilies.FantasyFamilyName:
				familyName = "Impact, Juice ITC";
				break;
			}
			Control = new swm.FontFamily (familyName);
		}

		public string Name { get; set; }

		public string LocalizedName
		{
			get
			{
				var lang = sw.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
				string name;
				if (!Control.FamilyNames.TryGetValue(lang, out name))
					name = Name;
				return name;
			}
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

		public void Apply(sw.Documents.TextRange control)
		{
			control.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, Control);
		}
	}
}
