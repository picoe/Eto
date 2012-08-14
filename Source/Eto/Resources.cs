using System;
using System.IO;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Obsolete resource helper class
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use Assembly.GetExecutingAssembly().GetManifestResourceStream")]
		public static Stream GetResource(string filename)
		{
			return GetResource(filename, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use Assembly.GetManifestResourceStream")]
		public static Stream GetResource (string resourceName, Assembly asm)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly();
			return asm.GetManifestResourceStream(resourceName);
		}
	}
}
