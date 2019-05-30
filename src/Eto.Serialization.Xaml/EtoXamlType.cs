using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Eto.Forms;


#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Schema;
using cm = System.ComponentModel;

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
	[Obsolete("Since 2.5")]
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

		public override object ConvertFrom(cm.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return etoConverter.ConvertFrom(null, culture, value);
		}

		public override object ConvertTo(cm.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return etoConverter.ConvertTo(null, culture, value, destinationType);
		}
	}

	[Obsolete("Since 2.5")]
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

	class EtoDesignerType : EtoXamlType
	{
		public string TypeName { get; set; }

		public string Namespace { get; set; }

		public EtoDesignerType(Type underlyingType, XamlSchemaContext schemaContext)
			: base(underlyingType, schemaContext)
		{
		}

		class DesignerInvoker : XamlTypeInvoker
		{
			public EtoDesignerType DesignerType { get; private set; }

			public DesignerInvoker(EtoDesignerType type)
				: base(type)
			{
				DesignerType = type;
			}

			public override object CreateInstance(object[] arguments)
			{
				var instance = base.CreateInstance(arguments);
				var ctl = instance as DesignerMarkupExtension;
				if (ctl != null)
				{
					ctl.Text = "[" + DesignerType.TypeName + "]";
					ctl.ToolTip = DesignerType.Namespace;
				}
				
				return instance;
			}
		}

		protected override XamlTypeInvoker LookupInvoker()
		{
			return new DesignerInvoker(this);
		}
	}

	class EmptyXamlMember : XamlMember
	{
		public EmptyXamlMember(EventInfo eventInfo, XamlSchemaContext context)
			: base(eventInfo, context)
		{

		}

		class EmptyConverter : cm.TypeConverter
		{
			public override bool CanConvertFrom(cm.ITypeDescriptorContext context, Type sourceType) => true;

			public override object ConvertFrom(cm.ITypeDescriptorContext context, CultureInfo culture, object value) => null;
		}

		protected override XamlValueConverter<cm.TypeConverter> LookupTypeConverter()
		{
			return new XamlValueConverter<cm.TypeConverter>(typeof(EmptyConverter), Type);
		}
	}

	class EtoXamlType : XamlType
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
		bool gotTypeConverter;

		protected override XamlValueConverter<cm.TypeConverter> LookupTypeConverter()
		{
			if (gotTypeConverter)
				return typeConverter;

			gotTypeConverter = true;

#pragma warning disable 618
			// convert from Eto.TypeConverter to Portable.Xaml.ComponentModel.TypeConverter
			var typeConverterAttrib = GetCustomAttribute<EtoTypeConverterAttribute>();

			if (typeConverterAttrib == null
			    && UnderlyingType.GetTypeInfo().IsGenericType
			    && UnderlyingType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				typeConverterAttrib = Nullable.GetUnderlyingType(UnderlyingType).GetTypeInfo().GetCustomAttribute<EtoTypeConverterAttribute>();
			}

			if (typeConverterAttrib != null)
			{
				var converterType = Type.GetType(typeConverterAttrib.ConverterTypeName);
				if (converterType != null)
					typeConverter = new EtoValueConverter(converterType, this);
			}
#pragma warning restore 618

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

		protected override bool LookupIsAmbient()
		{
			if (this.UnderlyingType != null && UnderlyingType == typeof(PropertyStore))
				return true;
			return base.LookupIsAmbient();
		}

		bool gotContentProperty;
		XamlMember contentProperty;

		protected override XamlMember LookupContentProperty()
		{
			if (gotContentProperty)
				return contentProperty;
			gotContentProperty = true;
			var contentAttribute = GetCustomAttribute<ContentPropertyAttribute>();
			if (contentAttribute == null || contentAttribute.Name == null)
				contentProperty = base.LookupContentProperty();
			else
				contentProperty = GetMember(contentAttribute.Name);
			return contentProperty;
		}

		XamlMember nameAliasedProperty;

		protected override XamlMember LookupAliasedProperty(XamlDirective directive)
		{
			if (directive == XamlLanguage.Name)
			{
				if (nameAliasedProperty != null)
					return nameAliasedProperty;

				var nameAttribute = GetCustomAttribute<RuntimeNamePropertyAttribute>();
				if (nameAttribute != null && nameAttribute.Name != null)
				{
					nameAliasedProperty = GetMember(nameAttribute.Name);
					return nameAliasedProperty;
				}

			}
			return base.LookupAliasedProperty(directive);
		}
	}
}