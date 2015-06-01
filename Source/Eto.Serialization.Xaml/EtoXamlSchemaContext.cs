using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xaml;
using System.Windows.Markup;
using System.Linq;

namespace Eto.Serialization.Xaml
{
	class EtoXamlSchemaContext : XamlSchemaContext
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		readonly Dictionary<string, XamlType> cache = new Dictionary<string, XamlType>();
		readonly Dictionary<Type, XamlType> typeCache = new Dictionary<Type, XamlType>();
		readonly object cache_sync = new object();

		public EtoXamlSchemaContext(IEnumerable<Assembly> assemblies)
			: base(assemblies)
		{
		}

		const string clr_namespace = "clr-namespace:";
		const string clr_assembly = "assembly=";
		const string eto_namespace = "clr-namespace:Eto.Forms;assembly=Eto";

		protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			var type = base.GetXamlType(xamlNamespace, name, typeArguments);
			if (type == null && xamlNamespace == EtoFormsNamespace)
				xamlNamespace = eto_namespace;
			if (type == null && xamlNamespace.StartsWith(clr_namespace, StringComparison.OrdinalIgnoreCase))
			{
				lock (cache_sync)
				{
					if (!cache.TryGetValue(xamlNamespace + name, out type))
					{
						var nsComponents = xamlNamespace.Split(';');
						if (nsComponents.Length == 2 && nsComponents[1].StartsWith(clr_assembly, StringComparison.Ordinal))
						{
							var assemblyName = nsComponents[1].Substring(clr_assembly.Length);
							var ns = nsComponents[0].Substring(clr_namespace.Length);
							var assembly = Assembly.Load(assemblyName);
							if (assembly != null)
							{
								var realType = assembly.GetType(ns + "." + name);
								if (realType != null)
								{
									type = xamlNamespace == eto_namespace ? new EtoXamlType(realType, this) : GetXamlType(realType);
									cache.Add(xamlNamespace + name, type);
								}
							}
						}
					}
				}
			}

			return type;
		}

		public override XamlType GetXamlType(Type type)
		{
			// Use EtoXamlType for all types on mono so we can override incorrect behavior when getting collection 
			// item types in EtoItemType.LookupItemType
			if (EtoEnvironment.Platform.IsMono || type.Assembly == typeof(Platform).Assembly)
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