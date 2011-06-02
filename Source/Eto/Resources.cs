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

		public static Stream GetResource(string filename, Assembly asm)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly();
			string resourceName = asm.GetName().Name + "." + filename;
			return asm.GetManifestResourceStream(resourceName);
		}

	}
}
