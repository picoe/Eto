using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;


#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml
{
	class EtoXamlSchemaContext : XamlSchemaContext
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		readonly Dictionary<Type, XamlType> typeCache = new Dictionary<Type, XamlType>();

		public bool DesignMode { get; set; }

		static readonly Assembly EtoAssembly = typeof(Platform).GetTypeInfo().Assembly;

		protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			XamlType type = null;
			try
			{
				type = base.GetXamlType(xamlNamespace, name, typeArguments);
			}
			catch
			{
				if (!DesignMode || type != null)
					throw;
				// in designer mode, fail gracefully
				type = new EtoDesignerType(typeof(DesignerMarkupExtension), this) { TypeName = name, Namespace = xamlNamespace };
			}
			return type;
		}

		public override XamlType GetXamlType(Type type)
		{
			XamlType xamlType;
			if (typeCache.TryGetValue(type, out xamlType))
				return xamlType;

			var info = type.GetTypeInfo();

			if (
				info.IsSubclassOf(typeof(Widget))
				|| info.Assembly == EtoAssembly // struct
				|| (
					// nullable struct
				    info.IsGenericType
				    && info.GetGenericTypeDefinition() == typeof(Nullable<>)
					&& Nullable.GetUnderlyingType(type).GetTypeInfo().Assembly == EtoAssembly
				))
			{
				xamlType = new EtoXamlType(type, this);
				typeCache.Add(type, xamlType);
				return xamlType;
			}
			return base.GetXamlType(type);
		}
		bool isInResourceMember;
		PropertyInfo resourceMember;

		internal bool IsResourceMember(PropertyInfo member)
		{
			if (member == null)
				return false;
			if (resourceMember == null)
			{
				if (isInResourceMember)
					return false;
				isInResourceMember = true;
				try
				{
					resourceMember = typeof(Widget).GetRuntimeProperty("Properties");
				}
				finally
				{
					isInResourceMember = false;
				}
			}

			return member.DeclaringType == resourceMember.DeclaringType
				   && member.Name == resourceMember.Name;
		}

		class PropertiesXamlMember : XamlMember
		{
			public PropertiesXamlMember(PropertyInfo propertyInfo, XamlSchemaContext context)
				: base(propertyInfo, context)
			{
			}

			protected override bool LookupIsAmbient() => true;
		}

		protected override XamlMember GetProperty(PropertyInfo propertyInfo)
		{
			if (IsResourceMember(propertyInfo))
			{
				return new PropertiesXamlMember(propertyInfo, this);
			}

			return base.GetProperty(propertyInfo);
		}

		protected override XamlMember GetEvent(EventInfo eventInfo)
		{
			if (DesignMode)
			{
				// in design mode, ignore wiring up events
				return new EmptyXamlMember(eventInfo, this);
			}

			return base.GetEvent(eventInfo);
		}
	}
}