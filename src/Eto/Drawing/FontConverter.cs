using System;
using sc = System.ComponentModel;
using System.IO;
using System.Linq;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter to convert a string to a <see cref="Font"/>
	/// </summary>
	/// <copyright>(c) 2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class FontConverter : sc.TypeConverter
	{
		const string SystemFontPrefix = "SystemFont.";

		/// <summary>
		/// Gets a value indicating that this converter can convert from the source type to a Font
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can handle converting from the specified <paramref name="sourceType"/> to an font</returns>
		public override bool CanConvertFrom (sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Gets a value indicating that this converter can convert to the specified type.
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter can convert to the specified <paramref name="destinationType"/>, otherwise false.</returns>
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		/// <summary>
		/// Performs the conversion from the given <paramref name="value"/> to an <see cref="Image"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion</param>
		/// <param name="value">Value to convert to an image</param>
		/// <returns>A new instance of an image, or null if it cannot be converted</returns>
		public override object ConvertFrom (sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var text = value as string;
			if (!string.IsNullOrEmpty(text))
			{
				var values = text.Split(new [] { '+' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToList();
				if (values.Count > 0)
				{
					string familyName = null;
					float? size = null;
					var decoration = FontDecoration.None;
					var style = FontStyle.None;
					string typefaceName = null;

					for (int i = 0; i < values.Count; i++)
					{
						var cur = values[i];

						var sizeStr = cur;
						if (sizeStr.EndsWith("pt", StringComparison.OrdinalIgnoreCase))
							sizeStr = sizeStr.Substring(0, sizeStr.Length - 2);
						float tsize;
						if (float.TryParse(sizeStr, out tsize))
						{
							size = tsize;
							continue;
						}

						FontDecoration tdecoration;
						if (Enum.TryParse(cur, true, out tdecoration))
						{
							decoration |= tdecoration;
							continue;
						}

						FontStyle tstyle;
						if (Enum.TryParse(cur, true, out tstyle))
						{
							style |= tstyle;
							continue;
						}

						if (familyName == null)
						{
							familyName = cur;
							continue;
						}

						if (typefaceName != null)
							familyName += "," + typefaceName;

						typefaceName = cur;
					}
					if (familyName == null)
						familyName = SystemFonts.Default().FamilyName;

					if (familyName.StartsWith(SystemFontPrefix, StringComparison.OrdinalIgnoreCase))
					{
						SystemFont systemFont;
						if (Enum.TryParse(familyName.Substring(SystemFontPrefix.Length), true, out systemFont))
							return new Font(systemFont, size, decoration);
					}

					var fontSize = size ?? SystemFonts.Default().Size;
					var family = new FontFamily(familyName);
					if (!string.IsNullOrEmpty(typefaceName))
					{
						var typeface = family.Typefaces.FirstOrDefault(r => string.Equals(r.Name, typefaceName, StringComparison.OrdinalIgnoreCase));
						if (typeface != null)
							return new Font(typeface, fontSize, decoration);
					}
					return new Font(family, fontSize, style, decoration);
				}
			}
			return null;
		}
	}
}
