using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Eto.Drawing;
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

		public EtoXamlSchemaContext(IEnumerable<Assembly> assemblies)
			: base(assemblies)
		{
		}

		static readonly Assembly EtoAssembly = typeof(Platform).GetTypeInfo().Assembly;

		public override XamlType GetXamlType(Type type)
		{
			var info = type.GetTypeInfo();

			if (info.Assembly == EtoAssembly
				|| (
				    info.IsGenericType
				    && info.GetGenericTypeDefinition() == typeof(Nullable<>)
				    && Nullable.GetUnderlyingType(type).GetTypeInfo().Assembly == EtoAssembly
				))
			{
				XamlType xamlType;
				if (typeCache.TryGetValue(type, out xamlType))
					return xamlType;
				xamlType = new EtoXamlType(type, this);
				typeCache.Add(type, xamlType);
				return xamlType;
			}
			return base.GetXamlType(type);
		}
	}
}