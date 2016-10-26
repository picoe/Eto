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
		public List<string> Files { get; set; }

		public AppDomain AppDomain { get; set; }

		public static IDisposable Register(string file, AppDomain domain = null)
		{
			return Register(new string[] { file }, domain);
		}

		public static IDisposable Register(IEnumerable<string> files, AppDomain domain = null)
		{
			if (files == null)
				return null;
			if (domain == null)
				domain = AppDomain.CurrentDomain;
			var resolver = new AssemblyResolver { AppDomain = domain, Files = files.ToList() };
			domain.AssemblyResolve += resolver.Resolve;
			return resolver;
		}

		public void Dispose()
		{
			AppDomain.AssemblyResolve -= Resolve;
		}

		Assembly Resolve(object sender, ResolveEventArgs args)
		{
			var assemblyName = new AssemblyName(args.Name).Name;
			foreach (var path in Files)
			{
				var filePath = Path.GetFileNameWithoutExtension(path);
				if (string.Equals(filePath, assemblyName, StringComparison.OrdinalIgnoreCase) && File.Exists(path))
					return Assembly.LoadFrom(path);
				if (!Directory.Exists(path))
					continue;
				filePath = Path.Combine(path, assemblyName + ".dll");
				if (File.Exists(filePath))
					return Assembly.LoadFrom(filePath);
			}
			return null;
		}
	}
}
