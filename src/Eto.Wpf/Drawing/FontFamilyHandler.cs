using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;
using swd = System.Windows.Documents;
using System.Diagnostics;
using System.Globalization;
using Eto.Forms;

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


		static readonly object LocalizedName_Key = new object();

		public string LocalizedName
		{
			get
			{
				if (Widget.Properties.ContainsKey(LocalizedName_Key))
					return Widget.Properties.Get<string>(LocalizedName_Key);

				var localizedName = CustomControls.FontDialog.NameDictionaryExtensions.GetDisplayName(Control.FamilyNames);
				Widget.Properties.Set(LocalizedName_Key, localizedName);
				return localizedName;
			}
		}

		public IEnumerable<FontTypeface> Typefaces
		{
			get {
				foreach (var type in Control.GetTypefaces ()) {
					if (!FontHandler.ShowSimulatedFonts && (type.IsBoldSimulated || type.IsObliqueSimulated))
						continue;
					yield return new FontTypeface(Widget, new FontTypefaceHandler (type));
				}
			}
		}

		public void Apply(sw.Documents.TextRange control)
		{
			control.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, Control);
		}
	}
}
