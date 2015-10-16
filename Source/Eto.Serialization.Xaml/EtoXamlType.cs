using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Schema;
using cm = Portable.Xaml.ComponentModel;
#if NET40
using EtoTypeConverter = System.ComponentModel.TypeConverter;
using EtoTypeConverterAttribute = System.ComponentModel.TypeConverterAttribute;
#else
using EtoTypeConverter = Eto.TypeConverter;
using EtoTypeConverterAttribute = Eto.TypeConverterAttribute;
#endif
#else
using System.Xaml;
using System.Xaml.Schema;
using cm = System.ComponentModel;
using EtoTypeConverter = Eto.TypeConverter;
using EtoTypeConverterAttribute = Eto.TypeConverterAttribute;
#endif

namespace Eto.Serialization.Xaml
{
	#if NET40
	static class TypeExtensions
	{
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}

		public static T GetCustomAttribute<T>(this Type type, bool inherit = true)
		{
			return type.GetCustomAttributes(typeof(T), inherit).OfType<T>().FirstOrDefault();
		}
	}
	#endif 

	#if PORTABLE || NET45
	class TypeConverterConverter : cm.TypeConverter
	{
		readonly EtoTypeConverter etoConverter;
		public TypeConverterConverter(EtoTypeConverter etoConverter)
		{
			this.etoConverter = etoConverter;
		}

		public override bool CanConvertFrom(cm.ITypeDescriptorContext context, Type sourceType)
		{
			return etoConverter.CanConvertFrom(sourceType);
		}

		public override bool CanConvertTo(cm.ITypeDescriptorContext context, Type destinationType)
		{
			return etoConverter.CanConvertTo(destinationType);
		}

		public override object ConvertFrom(cm.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			return etoConverter.ConvertFrom(null, culture, value);
		}

		public override object ConvertTo(cm.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			return etoConverter.ConvertTo(null, culture, value, destinationType);
		}
	}

	class EtoValueConverter : XamlValueConverter<cm.TypeConverter>
	{
		public EtoValueConverter(Type converterType, XamlType targetType)
			: base(converterType, targetType)
		{
		}

		protected override cm.TypeConverter CreateInstance()
		{
			var etoConverter = Activator.CreateInstance(ConverterType) as EtoTypeConverter;
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
			where T: Attribute
		{
			return UnderlyingType.GetTypeInfo().GetCustomAttribute<T>(inherit);
		}

		#if PORTABLE || NET45
		XamlValueConverter<cm.TypeConverter> typeConverter;

		protected override XamlValueConverter<cm.TypeConverter> LookupTypeConverter()
		{
			if (typeConverter != null)
				return typeConverter;
			
			// convert from Eto.TypeConverter to Portable.Xaml.ComponentModel.TypeConverter
			var typeConverterAttrib = GetCustomAttribute<EtoTypeConverterAttribute>();
			if (typeConverterAttrib != null)
			{
				var converterType = Type.GetType(typeConverterAttrib.ConverterTypeName);
				if (converterType != null)
					typeConverter = new EtoValueConverter(converterType, this);
			}
			if (typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(UnderlyingType.GetTypeInfo()))
			{
				var context = SchemaContext as EtoXamlSchemaContext;
				if (context.DesignMode)
				{
					return null;
				}
			}

			if (typeConverter == null)
				typeConverter = base.LookupTypeConverter();
			return typeConverter;
		}

		#endif

		class EmptyXamlMember : XamlMember
		{
			public EmptyXamlMember(EventInfo eventInfo, XamlSchemaContext context)
				: base(eventInfo, context)
			{

			}

			class EmptyConverter : cm.TypeConverter
			{
				public override bool CanConvertFrom(cm.ITypeDescriptorContext context, Type sourceType)
				{
					return true;
				}

				public override object ConvertFrom(cm.ITypeDescriptorContext context, CultureInfo culture, object value)
				{
					return null;
				}
			}

			protected override XamlValueConverter<cm.TypeConverter> LookupTypeConverter()
			{
				return new XamlValueConverter<cm.TypeConverter>(typeof(EmptyConverter), Type);
			}
		}

		protected override XamlMember LookupMember(string name, bool skipReadOnlyCheck)
		{
			var member = base.LookupMember(name, skipReadOnlyCheck);
			if (member != null && member.IsEvent)
			{
				var context = SchemaContext as EtoXamlSchemaContext;
				if (context != null && context.DesignMode)
				{
					// in design mode, ignore wiring up events
					return new EmptyXamlMember(member.UnderlyingMember as EventInfo, context);
				}
			}
			return member;
		}

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
				var nameAttribute = GetCustomAttribute<RuntimeNamePropertyAttribute>();
				if (nameAttribute != null && nameAttribute.Name != null)
					return GetMember(nameAttribute.Name);
			}
			return base.LookupAliasedProperty(directive);
		}
	}
}