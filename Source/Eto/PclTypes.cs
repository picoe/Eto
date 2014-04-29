#if PCL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

// This file contains type definitions currently needed to compile Eto
// as a Portable Class Library, in the project Eto.Pcl.csproj.
namespace Eto
{
	public interface ISupportInitialize
	{
		void BeginInit();

		void EndInit();
	}

	public abstract class BaseNumberConverter : TypeConverter
	{
		internal virtual bool AllowHex { get { return true; } }

		internal abstract Type NumberType { get; }

		internal abstract object FromString(string value, int fromBase);

		internal abstract object FromString(string value, NumberFormatInfo formatInfo);

		internal abstract string ToString(object value, NumberFormatInfo formatInfo);

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var str = value as string;
			if (str != null)
			{
				string text = str.Trim();
				try
				{
					object result;
					if (AllowHex && text[0] == '#')
					{
						result = FromString(text.Substring(1), 16);
						return result;
					}
					if ((AllowHex && text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) || text.StartsWith("&h", StringComparison.OrdinalIgnoreCase))
					{
						result = FromString(text.Substring(2), 16);
						return result;
					}
					culture = culture ?? CultureInfo.CurrentCulture;
					var formatInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
					result = FromString(text, formatInfo);
					return result;
				}
				catch (Exception innerException)
				{
					throw new Exception(text, innerException);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");

			if (destinationType == typeof(string) && value != null && NumberType.IsInstanceOfType(value))
			{
				culture = culture ?? CultureInfo.CurrentCulture;
				var formatInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
				return ToString(value, formatInfo);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	public class Int32Converter : BaseNumberConverter
	{
		internal override Type NumberType { get { return typeof(int); } }

		internal override object FromString(string value, int fromBase)
		{
			return Convert.ToInt32(value, fromBase);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return int.Parse(value, NumberStyles.Integer, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((int)value).ToString("G", formatInfo);
		}
	}

	public interface ITypeDescriptorContext : IServiceProvider
	{
	}

	public class SingleConverter : BaseNumberConverter
	{
		internal override bool AllowHex { get { return false; } }

		internal override Type NumberType { get { return typeof(float); } }

		internal override object FromString(string value, int fromBase)
		{
			return Convert.ToSingle(value, CultureInfo.CurrentCulture);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return float.Parse(value, NumberStyles.Float, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((float)value).ToString("R", formatInfo);
		}
	}

	public class TypeConverterAttribute : Attribute
	{
		public string TypeName { get; private set; }

		public TypeConverterAttribute(Type type)
		{
			TypeName = type.AssemblyQualifiedName;
		}

		public TypeConverterAttribute(string typeName)
		{
			TypeName = typeName; 
		}
	}

	public class TypeConverter
	{
		public bool CanConvertFrom(Type sourceType)
		{
			return CanConvertFrom(null, sourceType);
		}

		public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return false;
		}

		public bool CanConvertTo(Type destinationType)
		{
			return CanConvertTo(null, destinationType);
		}

		public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return (destinationType == typeof(string));
		}

		public object ConvertFrom(object o)
		{
			return ConvertFrom(null, CultureInfo.CurrentCulture, o);
		}

		public virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return GetConvertFromException(value);
		}

		public object ConvertFromInvariantString(string text)
		{
			return ConvertFromInvariantString(null, text); 
		}

		public object ConvertFromInvariantString(ITypeDescriptorContext context, string text)
		{
			return ConvertFromString(context, CultureInfo.InvariantCulture, text);
		}

		public object ConvertFromString(string text)
		{
			return ConvertFrom(text);
		}

		public object ConvertFromString(ITypeDescriptorContext context, string text)
		{
			return ConvertFromString(context, CultureInfo.CurrentCulture, text);
		}

		public object ConvertFromString(ITypeDescriptorContext context, CultureInfo culture, string text)
		{
			return ConvertFrom(context, culture, text);
		}

		public object ConvertTo(object value, Type destinationType)
		{
			return ConvertTo(null, null, value, destinationType);
		}

		public virtual object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
		                                Type destinationType)
		{
			// context?
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");

			if (destinationType == typeof(string))
			{
				if (value == null)
					return String.Empty;

				if (culture != null)
					return Convert.ToString(value, culture);

				return value.ToString();
			}

			return GetConvertToException(value, destinationType);
		}

		public string ConvertToInvariantString(object value)
		{
			return ConvertToInvariantString(null, value);
		}

		public string ConvertToInvariantString(ITypeDescriptorContext context, object value)
		{
			return (string)ConvertTo(context, CultureInfo.InvariantCulture, value, typeof(string));
		}

		public string ConvertToString(object value)
		{
			return (string)ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));
		}

		public string ConvertToString(ITypeDescriptorContext context, object value)
		{
			return (string)ConvertTo(context, CultureInfo.CurrentCulture, value, typeof(string));
		}

		public string ConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return (string)ConvertTo(context, culture, value, typeof(string));
		}

		protected Exception GetConvertFromException(object value)
		{
			string destinationType;
			if (value == null)
				destinationType = "(null)";
			else
				destinationType = value.GetType().FullName;

			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture,
				"{0} cannot convert from {1}.", GetType().Name,
				destinationType));
		}

		protected Exception GetConvertToException(object value, Type destinationType)
		{
			string sourceType;
			if (value == null)
				sourceType = "(null)";
			else
				sourceType = value.GetType().FullName;

			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture,
				"'{0}' is unable to convert '{1}' to '{2}'.", GetType().Name,
				sourceType, destinationType.FullName));
		}

		public bool IsValid(object value)
		{
			return IsValid(null, value);
		}

		public virtual bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value == null)
				return false;

			if (!CanConvertFrom(context, value.GetType()))
				return false;

			try
			{
				ConvertFrom(context, CultureInfo.InvariantCulture, value);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	internal class SerializableAttribute : Attribute
	{
	}
}
#endif