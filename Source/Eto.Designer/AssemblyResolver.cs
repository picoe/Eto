using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	class AssemblyResolver : IDisposable
	{
		public string AssemblyPath { get; set; }

		public AppDomain AppDomain { get; set; }

		public static IDisposable Register(string assemblyPath, AppDomain domain = null)
		{
			if (domain == null)
				domain = AppDomain.CurrentDomain;
			var resolver = new AssemblyResolver { AssemblyPath = assemblyPath, AppDomain = domain };
			domain.AssemblyResolve += resolver.Resolve;
			return resolver;
		}

		public void Dispose()
		{
			AppDomain.AssemblyResolve -= Resolve;
		}

		Assembly Resolve(object sender, ResolveEventArgs args)
		{
			string assemblyPath = Path.Combine(AssemblyPath, new AssemblyName(args.Name).Name + ".dll");
			if (!File.Exists(assemblyPath))
				return null;
			return Assembly.LoadFrom(assemblyPath);
		}
	}
}
