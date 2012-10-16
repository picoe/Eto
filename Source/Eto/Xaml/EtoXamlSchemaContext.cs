#if XAML

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace Eto.Xaml
{
	class EtoXamlSchemaContext : XamlSchemaContext
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";

		Dictionary<string, XamlType> cache = new Dictionary<string, XamlType> ();
		object cache_sync = new object ();

		public EtoXamlSchemaContext (IEnumerable<Assembly> assemblies)
			: base (assemblies)
		{
		}

		const string clr_namespace = "clr-namespace:";
		const string clr_assembly = "assembly=";

		public override bool TryGetCompatibleXamlNamespace (string xamlNamespace, out string compatibleNamespace)
		{
			var val = base.TryGetCompatibleXamlNamespace (xamlNamespace, out compatibleNamespace);
			return val;
		}

		public override IEnumerable<string> GetAllXamlNamespaces ()
		{
			foreach (var ns in base.GetAllXamlNamespaces ())
				yield return ns;
			yield return "clr-namespace:Eto.Forms;assembly=Eto";
		}

		public override ICollection<XamlType> GetAllXamlTypes (string xamlNamespace)
		{
			return base.GetAllXamlTypes (xamlNamespace);
		}

		protected override Assembly OnAssemblyResolve (string assemblyName)
		{
			return base.OnAssemblyResolve (assemblyName);
		}

		protected override XamlType GetXamlType (string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			var type = base.GetXamlType (xamlNamespace, name, typeArguments);
			if (type == null && xamlNamespace == EtoFormsNamespace)
				xamlNamespace = "clr-namespace:Eto.Forms;assembly=Eto";
			if (type == null && xamlNamespace.StartsWith (clr_namespace))
			{
				lock (this.cache_sync)
				{
					if (!this.cache.TryGetValue (xamlNamespace + name, out type))
					{
						var nsComponents = xamlNamespace.Split (';');
						if (nsComponents.Length == 2 && nsComponents[1].StartsWith (clr_assembly))
						{
							var assemblyName = nsComponents[1].Substring (clr_assembly.Length);
							var ns = nsComponents[0].Substring (clr_namespace.Length);
							var assembly = Assembly.Load (assemblyName);
							if (assembly != null)
							{
								var realType = assembly.GetType (ns + "." + name);
								type = this.GetXamlType (realType);
								this.cache.Add (xamlNamespace + name, type);
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