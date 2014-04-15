using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This file contains type definitions currently needed to compile Eto
/// as a Portable Class Library, in the project Eto.Pcl.csproj.
/// </summary>
namespace System.ComponentModel
{
	public sealed class DesignerCategoryAttribute : Attribute
	{
		public DesignerCategoryAttribute(object o)
		{
		}
	}

	public class DesignTimeVisibleAttribute : Attribute
	{
		public DesignTimeVisibleAttribute(object o)
		{
		}
	}

	public class ToolboxItemAttribute : Attribute
	{
		public ToolboxItemAttribute(object o)
		{
		}
	}

	public interface ISupportInitialize
	{
		void BeginInit();
		void EndInit();
	}

	public class Int32Converter
	{
		internal int ConvertFromString(ITypeDescriptorContext context, Globalization.CultureInfo culture, string p)
		{
			throw new NotImplementedException();
		}

		internal ulong ConvertTo(ITypeDescriptorContext context, Globalization.CultureInfo culture, int item, Type destinationType)
		{
			throw new NotImplementedException();
		}

		internal int ConvertFrom(ITypeDescriptorContext context, Globalization.CultureInfo culture, string p)
		{
			throw new NotImplementedException();
		}
	}

	public class ITypeDescriptorContext : IServiceProvider
	{
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}

	public class PropertyDescriptor
	{
		public Type ComponentType { get { throw new NotImplementedException(); } }
		public Type PropertyType { get { throw new NotImplementedException(); } }

		internal object GetValue(object dataItem)
		{
			throw new NotImplementedException();
		}

		public void SetValue(object component, object value)
		{
			throw new NotImplementedException();
		}
	}

	public class SingleConverter
	{
		internal int ConvertFromString(ITypeDescriptorContext context, Globalization.CultureInfo culture, string p)
		{
			throw new NotImplementedException();
		}
	}

	public class TypeConverter
	{
		public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			throw new Exception();
		}

		public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			throw new Exception();
		}

		public virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			throw new Exception();
		}

		public virtual object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new Exception();
		}
	}

	public class TypeConverterAttribute : Attribute
	{
		public TypeConverterAttribute(object o)
		{
		}
	}
}

namespace System
{
	public interface ICloneable
	{
		object Clone();
	}

	public class SerializableAttribute : Attribute
	{
	}
}
