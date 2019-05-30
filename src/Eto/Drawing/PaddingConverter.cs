using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter for the <see cref="Padding"/> class
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class PaddingConverterInternal : sc.TypeConverter
	{
		/// <summary>
		/// The character to split up the string which will be converted
		/// </summary>
		static readonly char[] DimensionSplitter = new char[1] { ',' };

		/// <summary>
		/// Determines if the specified <paramref name="sourceType"/> can be converted to a <see cref="Padding"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Converts the specified value to a <see cref="Padding"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion for</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A new instance of the <see cref="Padding"/> object with the value represented by <paramref name="value"/></returns>
		public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null)
			{
				string[] parts = text.Split(DimensionSplitter, StringSplitOptions.RemoveEmptyEntries);

				try
				{
					switch (parts.Length)
					{
						case 1:
							return new Padding(
								int.Parse(parts[0])
							);
						case 2:
							return new Padding(
								int.Parse(parts[0]),
								int.Parse(parts[1])
							);
						case 4:
							return new Padding(
								int.Parse(parts[0]),
								int.Parse(parts[1]),
								int.Parse(parts[2]),
								int.Parse(parts[3])
							);
						default:
							throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as Padding. Should be in the form of 'all', 'horizontal, vertical', or 'left, top, right, bottom'", text));
					}
				}
				catch
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as Padding. Should be in the form of 'all', 'horizontal, vertical', or 'left, top, right, bottom'", text));
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}

	/// <summary>
	/// Converter for the <see cref="Padding"/> class
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Obsolete("Since 2.5. Use TypeDescriptor.GetConverter instead")]
	public class PaddingConverter : TypeConverter
	{
		/// <summary>
		/// The character to split up the string which will be converted
		/// </summary>
		static readonly char[] DimensionSplitter = new char[1] { ',' };

		/// <summary>
		/// Determines if the specified <paramref name="sourceType"/> can be converted to a <see cref="Padding"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Converts the specified value to a <see cref="Padding"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion for</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A new instance of the <see cref="Padding"/> object with the value represented by <paramref name="value"/></returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null)
			{
				string[] parts = text.Split(DimensionSplitter, StringSplitOptions.RemoveEmptyEntries);

				try
				{
					switch (parts.Length)
					{
						case 1:
							return new Padding(
								int.Parse(parts[0])
							);
						case 2:
							return new Padding(
								int.Parse(parts[0]),
								int.Parse(parts[1])
							);
						case 4:
							return new Padding(
								int.Parse(parts[0]),
								int.Parse(parts[1]),
								int.Parse(parts[2]),
								int.Parse(parts[3])
							);
						default:
							throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as Padding. Should be in the form of 'all', 'horizontal, vertical', or 'left, top, right, bottom'", text));
					}
				}
				catch
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as Padding. Should be in the form of 'all', 'horizontal, vertical', or 'left, top, right, bottom'", text));
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}

