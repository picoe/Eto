using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Forms
{
	class TableCellConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return typeof(Control).IsAssignableFrom(sourceType) || typeof(TableRow).IsAssignableFrom(sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
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
