using System;
using System.Globalization;
using Eto.Drawing;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	internal class SegmentedItemConverter : sc.TypeConverter
	{
		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || sourceType == typeof(Image);
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string str)
				return new ButtonSegmentedItem { Text = str };
			if (value is Image img)
				return new ButtonSegmentedItem { Image = img };

			return base.ConvertFrom(context, culture, value);
		}
	}
}