#if XAML

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xaml;

namespace Eto.Xaml
{
	class EtoXamlSchemaContext : XamlSchemaContext
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";

		readonly Dictionary<string, XamlType> cache = new Dictionary<string, XamlType> ();
		readonly object cache_sync = new object ();

		public EtoXamlSchemaContext (IEnumerable<Assembly> assemblies)
			: base (assemblies)
		{
		}

		const string clr_namespace = "clr-namespace:";
		const string clr_assembly = "assembly=";

		protected override XamlType GetXamlType (string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			var type = base.GetXamlType (xamlNamespace, name, typeArguments);
			if (type == null && xamlNamespace == EtoFormsNamespace)
				xamlNamespace = "clr-namespace:Eto.Forms;assembly=Eto";
			if (type == null && xamlNamespace.StartsWith (clr_namespace, StringComparison.OrdinalIgnoreCase))
			{
				lock (cache_sync)
				{
					if (!cache.TryGetValue (xamlNamespace + name, out type))
					{
						var nsComponents = xamlNamespace.Split (';');
						if (nsComponents.Length == 2 && nsComponents[1].StartsWith(clr_assembly, StringComparison.Ordinal))
						{
							var assemblyName = nsComponents[1].Substring (clr_assembly.Length);
							var ns = nsComponents[0].Substring (clr_namespace.Length);
							var assembly = Assembly.Load (assemblyName);
							if (assembly != null)
							{
								var realType = assembly.GetType (ns + "." + name);
								type = GetXamlType (realType);
								cache.Add (xamlNamespace + name, type);
							}
						}
					}
				}
			}

			return type;
		}
	}
}

#endif