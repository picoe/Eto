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
		readonly Dictionary<string, XamlType> cache = new Dictionary<string, XamlType>();
		readonly Dictionary<Type, XamlType> typeCache = new Dictionary<Type, XamlType>();
		readonly object cache_sync = new object();

		public bool DesignMode { get; set; }

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

							try
							{
								var assembly = Assembly.Load(new AssemblyName(assemblyName));
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
							catch
							{
								if (DesignMode && xamlNamespace != eto_namespace)
								{
									// in design mode, just show a placeholder when we can't load the assembly
									type = GetXamlType(typeof(DesignerUserControl));
									cache.Add(xamlNamespace + name, type);
								}
								else
									throw;
							}
						}
					}
				}
			}

			return type;
		}

		public override XamlType GetXamlType(Type type)
		{
			if (type.GetTypeInfo().Assembly == typeof(Platform).GetTypeInfo().Assembly)
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