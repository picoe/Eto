using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter for the <see cref="PointF"/> class
	/// </summary>
	/// <remarks>
	/// Allows conversion from a string to a <see cref="PointF"/> via json/xaml or other sources.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PointFConverter : TypeConverter
	{
		/// <summary>
		/// The character to split up the string which will be converted
		/// </summary>
		static readonly string[] StringSplitter = new string[1] { "," };

		/// <summary>
		/// Determines if this converter can convert from the specified <paramref name="sourceType"/>
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the specified value to a <see cref="PointF"/>
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A new instance of a <see cref="PointF"/> converted from the specified <paramref name="value"/></returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null)
			{
				string[] parts = text.Split(StringSplitter, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length != 2)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as PointF. Should be in the form of 'x, y'", text));

				try
				{
					return new PointF(
						float.Parse(parts[0]),
						float.Parse(parts[1])
					);
				}
				catch
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as PointF. Should be in the form of 'x, y'", text));
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}

