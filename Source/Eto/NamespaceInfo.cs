using System;
using System.IO;
using System.Reflection;

namespace Eto
{
	public class NamespaceInfo
	{
		string assemblyName;
		Assembly assembly;

		public Assembly Assembly {
			set {
				assembly = value;
				assemblyName = null;
			}
			get {
#if WINRT
			throw new NotImplementedException();
#else
				if (assembly == null && !string.IsNullOrEmpty (assemblyName))
					assembly = Assembly.Load (assemblyName);
				return assembly;
#endif
			}
		}

		public string Namespace { get; private set; }
		
		public string Prefix { get; private set; }
		
		public NamespaceInfo (string ns)
		{
			SetNamespace(ns);
		}
		
		public NamespaceInfo (string ns, Assembly assembly)
		{
			if (ns.Contains (","))
				SetNamespace (ns);
			else {
				this.Namespace = ns;
				this.Assembly = assembly;
			}
		}

		void SetNamespace (string ns)
		{
			var val = ns.Split (',');
			if (val.Length == 2)
			{
				Namespace = val[0].Trim ();
				assemblyName = val[1].Trim ();
			}
			else
				throw new ArgumentException("Namespace must include the assembly name in the form of: My.Namespace, MyAssembly", ns);
		}
		
		public Type FindType (string typeName)
		{
			return Assembly.GetType (Namespace + "." + typeName);
		}

		public Stream FindResource (string resourceName)
		{
			return Assembly.GetManifestResourceStream (Namespace + "." + resourceName);
		}
	
		public Stream FindResource ()
		{
			return Assembly.GetManifestResourceStream (Namespace);
		}
	}
}
