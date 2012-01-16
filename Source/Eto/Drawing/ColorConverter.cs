using System;
using System.ComponentModel;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Drawing
{
	public class ColorConverter : TypeConverter
	{
		
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string) {
				return ConvertFromString(context, (string)value, culture);
			}
			return base.ConvertFrom (context, culture, value);
			
			
		}
		
		static Color ConvertFromString (ITypeDescriptorContext context, string s, CultureInfo culture)
		{
			culture = culture ?? CultureInfo.InvariantCulture;
			s = s.Trim ();
			if (s.Length == 0)
				return Color.Empty;
			
			string listSeparator = culture.TextInfo.ListSeparator;
			if (s.IndexOf (listSeparator) == -1)
			{
				bool isArgb = s [0] == '#';
				int num = (!isArgb) ? 0 : 1;
				bool ixHex = false;
				if (s.Length > num + 1 && s [num] == '0')
				{
					ixHex = (s [num + 1] == 'x' || s [num + 1] == 'X');
					if (ixHex)
					{
						num += 2;
					}
				}
				if (isArgb || ixHex)
				{
					s = s.Substring (num);
					uint num2;
					try
					{
						num2 = uint.Parse (s, NumberStyles.HexNumber);
					}
					catch (Exception innerException2)
					{
						string text2 = string.Format ("Invalid value '{0}'.", s);
						throw new Exception (text2, innerException2);
					}
					if (s.Length < 6 || (s.Length == 6 && isArgb && ixHex))
					{
						num2 &= 0xFFFFFF;
					}
					else
					{
						if (num2 >> 24 == 0) num2 |= 0xFF000000;
					}
					return Color.FromArgb (num2);
				}
			}
			var int32Converter = new UInt32Converter ();
			string[] array = s.Split (listSeparator.ToCharArray ());
			uint[] array2 = new uint[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2 [i] = (uint)int32Converter.ConvertFrom (context, culture, array [i]);
			}
			switch (array.Length)
			{
			case 1:
		
				{
					return Color.FromArgb (array2 [0]);
				}
			case 3:
		
					{
					return new Color (array2 [0], array2 [1], array2 [2]);
				}
			case 4:
		
				{
					return new Color (array2 [0], array2 [1], array2 [2], array2 [3]);
				}
			}
			throw new ArgumentException (s + " is not a valid color value.");
		}
		
	}
}

