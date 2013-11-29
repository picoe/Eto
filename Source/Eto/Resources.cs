using System;
using System.IO;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Obsolete resource helper class
	/// </summary>
	[Obsolete("Use standard resource retrieval mechanisms")]
	public static class Resources
	{
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use Assembly.GetExecutingAssembly().GetManifestResourceStream")]
		public static Stream GetResource(string filename)
		{
#if WINRT
			throw new NotImplementedException();
#else
			return GetResource(filename, Assembly.GetCallingAssembly());
#endif
		}

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use Assembly.GetManifestResourceStream")]
		public static Stream GetResource (string resourceName, Assembly asm)
		{
#if WINRT
			throw new NotImplementedException();
#else
			if (asm == null) asm = Assembly.GetCallingAssembly();
			return asm.GetManifestResourceStream(resourceName);
#endif
		}
	}
}
