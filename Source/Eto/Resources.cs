using System;
using System.IO;
using System.Reflection;

namespace Eto
{
	public static class Resources
	{
		[Obsolete ("Use Assembly.GetExecutingAssembly().GetManifestResourceStream")]
		public static Stream GetResource(string filename)
		{
			return GetResource(filename, Assembly.GetCallingAssembly());
		}

		[Obsolete ("Use Assembly.GetManifestResourceStream")]
		public static Stream GetResource (string resourceName, Assembly asm)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly();
			return asm.GetManifestResourceStream(resourceName);
		}
	}
}
