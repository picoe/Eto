using System;
using System.Reflection;
using System.Xaml;
using System.Linq;
using System.Xaml.Schema;

namespace Eto.Serialization.Xaml
{
#if NET45
	class TypeConverterConverter : System.ComponentModel.TypeConverter
	{
		readonly Eto.TypeConverter etoConverter;
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
			return UnderlyingType.GetCustomAttributes(typeof(T), inherit).OfType<T>().FirstOrDefault();
		}

#if NET45
		XamlValueConverter<System.ComponentModel.TypeConverter> typeConverter;

		protected override XamlType LookupItemType()
		{
			if (EtoEnvironment.Platform.IsMono)
			{
				// mono doesn't use SchemaContext.GetXamlType here, which we need to override the type converter.
				var underlyingType = UnderlyingType;
				Type type;
				if (IsArray)
					type = underlyingType.GetElementType();
				else if (IsDictionary)
				{
					type = !IsGeneric ? typeof(object) : underlyingType.GetGenericArguments()[1];
				}
				else if (!IsCollection)
					type = null;
				else if (!IsGeneric)
					type = typeof(object);
				else
					type = underlyingType.GetGenericArguments()[0];

				return type != null ? SchemaContext.GetXamlType(type) : null;
			}
			return base.LookupItemType();
		}

		protected override XamlValueConverter<System.ComponentModel.TypeConverter> LookupTypeConverter()
		{
			if (typeConverter != null)
				return typeConverter;
			var typeConverterAttrib = UnderlyingType.GetCustomAttribute<Eto.TypeConverterAttribute>();
			if (typeConverterAttrib != null)
			{
				var converterType = Type.GetType(typeConverterAttrib.ConverterTypeName);
				if (converterType != null)
					typeConverter = new EtoValueConverter(converterType, this);
			}
			if (typeConverter == null)
			// convert from Eto.TypeConverter to System.ComponentModel.TypeConverter
				typeConverter = base.LookupTypeConverter();
			return typeConverter;
		}
#endif

		XamlMember contentProperty;
		protected override XamlMember LookupContentProperty()
		{
			if (contentProperty != null)
				return contentProperty;
			var contentAttribute = GetCustomAttribute<ContentPropertyAttribute>();
			if (contentAttribute == null || contentAttribute.Name == null)
				contentProperty = base.LookupContentProperty();
			else
				contentProperty = GetMember(contentAttribute.Name);
			return contentProperty;
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