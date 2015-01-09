using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xaml;
using System.Windows.Markup;
using System.Linq;
using System.ComponentModel;
using System.Xaml.Schema;

namespace Eto.Serialization.Xaml
{
#if PCL
	class TypeConverterConverter : System.ComponentModel.TypeConverter
	{
		Eto.TypeConverter etoConverter;
		public TypeConverterConverter(Eto.TypeConverter etoConverter)
		{
			this.etoConverter = etoConverter;
		}

		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			return etoConverter.CanConvertFrom(sourceType);
		}

		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			return etoConverter.CanConvertTo(destinationType);
		}

		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			return etoConverter.ConvertFrom(null, culture, value);
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			return etoConverter.ConvertTo(null, culture, value, destinationType);
		}
	}

	class EtoValueConverter : XamlValueConverter<System.ComponentModel.TypeConverter>
	{
		public EtoValueConverter(Type converterType, XamlType targetType)
			: base(converterType, targetType)
		{
		}

		protected override System.ComponentModel.TypeConverter CreateInstance()
		{
			var etoConverter = Activator.CreateInstance(ConverterType) as Eto.TypeConverter;
			return new TypeConverterConverter(etoConverter);
		}
	}
#endif

	public class EtoXamlType : XamlType
	{
		public EtoXamlType(Type underlyingType, XamlSchemaContext schemaContext)
			: base(underlyingType, schemaContext)
		{
		}

		T GetCustomAttribute<T>(bool inherit = true)
		{
			return UnderlyingType.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
		}

#if PCL
		protected override XamlValueConverter<System.ComponentModel.TypeConverter> LookupTypeConverter()
		{
			var typeConverterAttrib = this.UnderlyingType.GetCustomAttribute<Eto.TypeConverterAttribute>();
			if (typeConverterAttrib != null)
			{
				var converterType = Type.GetType(typeConverterAttrib.ConverterTypeName);
				if (converterType != null)
					return new EtoValueConverter(converterType, this);
			}
			// convert from Eto.TypeConverter to System.ComponentModel.TypeConverter
			return base.LookupTypeConverter();
		}
#endif

		protected override XamlMember LookupContentProperty()
		{
			var contentAttribute = GetCustomAttribute<ContentPropertyAttribute>();
			if (contentAttribute == null || contentAttribute.Name == null)
				return base.LookupContentProperty();
			return GetMember(contentAttribute.Name);
		}

		protected override XamlMember LookupAliasedProperty(XamlDirective directive)
		{
			if (directive == XamlLanguage.Name)
			{
				// mono doesn't support the name attribute yet (throws null exception)
				if (!EtoEnvironment.Platform.IsMono)
				{
					var nameAttribute = GetCustomAttribute<RuntimeNamePropertyAttribute>();
					if (nameAttribute != null && nameAttribute.Name != null)
						return GetMember(nameAttribute.Name);
				}
			}
			return base.LookupAliasedProperty(directive);
		}
	}
}