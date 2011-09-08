using System;
using System.IO;
using System.Reflection;

namespace Eto
{
	public static class Resources
	{
		public static Stream GetResource(string filename)
		{
			return GetResource(filename, Assembly.GetCallingAssembly());
		}

		public static Stream GetResource(string resourceName, Assembly asm)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly();
			return asm.GetManifestResourceStream(resourceName);
		}

	}
}
