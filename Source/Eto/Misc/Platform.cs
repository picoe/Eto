using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Eto.Misc
{
	/// <summary>
	/// Obsolete. Use <see cref="EtoEnvironment.Platform"/> instead.
	/// </summary>
	[Obsolete ("Use EtoEnvironment.Platform instead")]
	public static class Platform
	{
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		public static bool IsMono { get { return EtoEnvironment.Platform.IsMono; } }

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		public static bool IsWindows { get { return EtoEnvironment.Platform.IsWindows; } }

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		public static bool IsUnix { get { return EtoEnvironment.Platform.IsUnix; } }

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		public static bool IsMac { get { return EtoEnvironment.Platform.IsMac; } }

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		public static bool IsLinux { get { return EtoEnvironment.Platform.IsLinux; } }
	}
}
