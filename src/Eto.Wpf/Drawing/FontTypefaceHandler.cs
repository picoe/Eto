using Eto.Drawing;
using System.Globalization;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;
using swd = System.Windows.Documents;

namespace Eto.Wpf.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<swm.Typeface, FontTypeface>, FontTypeface.IHandler
	{
		string name;

		public FontTypefaceHandler (swm.Typeface type)
		{
			this.Control = type;
		}

		public FontTypefaceHandler(swd.TextSelection range, sw.Controls.RichTextBox control)
		{
			var family = range.GetPropertyValue(swd.TextElement.FontFamilyProperty) as swm.FontFamily ?? swd.TextElement.GetFontFamily(control);
			var style = range.GetPropertyValue(swd.TextElement.FontStyleProperty) as sw.FontStyle? ?? swd.TextElement.GetFontStyle(control);
			var weight = range.GetPropertyValue(swd.TextElement.FontWeightProperty) as sw.FontWeight? ?? swd.TextElement.GetFontWeight(control);
			var stretch = range.GetPropertyValue(swd.TextElement.FontStretchProperty) as sw.FontStretch? ?? swd.TextElement.GetFontStretch(control);
			Control = new swm.Typeface(family, style, weight, stretch);
		}

		public void Apply(swd.TextRange range)
		{
			range.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, Control.FontFamily);
			range.ApplyPropertyValue(swd.TextElement.FontStyleProperty, Control.Style);
			range.ApplyPropertyValue(swd.TextElement.FontStretchProperty, Control.Stretch);
			range.ApplyPropertyValue(swd.TextElement.FontWeightProperty, Control.Weight);
		}

		public string Name
		{
			get
			{
				if (name == null) {
					name = CustomControls.FontDialog.NameDictionaryHelper.GetDisplayName(Control.FaceNames);
				}
				return name;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return WpfConversions.Convert (Control.Style, Control.Weight);
			}
		}
	}
}
