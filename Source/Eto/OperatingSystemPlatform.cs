using System;
using System.Runtime.InteropServices;

namespace Eto
{
	/// <summary>
	/// Operating system platform information
	/// </summary>
	/// <remarks>
	/// Access this information from <see cref="EtoEnvironment.Platform"/>
	/// </remarks>
	public sealed class OperatingSystemPlatform
	{
		/// <summary>
		/// Gets a value indicating that the current .NET runtime is mono
		/// </summary>
		public bool IsMono { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is windows system
		/// </summary>
		public bool IsWindows { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a Windows Runtime (WinRT) system.
		/// </summary>
		public bool IsWinRT { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a unix-based system
		/// </summary>
		/// <remarks>
		/// This will be true for both Unix (e.g. OS X) and all Linux variants.
		/// </remarks>
		public bool IsUnix { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a Mac OS X system
		/// </summary>
		public bool IsMac { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is a Linux system
		/// </summary>
		public bool IsLinux { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is iOS.
		/// </summary>
		public bool IsIos { get; set; }

		/// <summary>
		/// Gets a value indicating that the current OS is Android.
		/// </summary>
		public bool IsAndroid { get; set; }
	}
}
