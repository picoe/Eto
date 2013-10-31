using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Eto
{
	class Int32ArrayConverter : TypeConverter
	{
		readonly Int32Converter int32Converter = new Int32Converter ();

		public override bool CanConvertTo (ITypeDescriptorContext ctx, Type destinationType)
		{
			return destinationType == typeof (string) || base.CanConvertTo (ctx, destinationType);
		}

		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string) || base.CanConvertFrom (context, sourceType);
		}

		public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			var items = value as IEnumerable<int>;
			if (items != null && destinationType == typeof (string))
			{
				var sb = new StringBuilder ();
				foreach (var item in items)
				{
					if (sb.Length > 0)
						sb.Append (',');
					sb.Append(Convert.ToString(int32Converter.ConvertTo (context, culture, item, destinationType), CultureInfo.InvariantCulture));
				}
				return sb.ToString ();
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}

		public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var val = value as string;
			if (val != null)
			{
				var items = val.Split (',');
				var arr = new int[items.Length];
				for (int i = 0; i < items.Length; i++)
				{
					arr[i] = (int)int32Converter.ConvertFrom (context, culture, items[i]);
				}
				return arr;
			}
			return base.ConvertFrom (context, culture, value);
		}
	}
}
