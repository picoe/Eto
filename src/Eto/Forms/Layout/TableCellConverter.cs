using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Forms
{
	class TableCellConverter : sc.TypeConverter
	{
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return typeof(Control).IsAssignableFrom(sourceType) || typeof(TableRow).IsAssignableFrom(sourceType);
		}

		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var control = value as Control;
			if (control != null)
				return new TableCell { Control = control };

			var row = value as TableRow;
			if (row != null)
				return new TableCell(row);

			return null;
		}
	}
}
