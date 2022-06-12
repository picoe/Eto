using System;

namespace Eto
{

	/// <summary>
	/// Enumeration of the special folders that can be retrieved
	/// </summary>
	public enum EtoSpecialFolder
	{
		/// <summary>
		/// Application settings folder to store settings or data
		/// </summary>
		/// <remarks>
		/// This will return a different folder, depending on the platform: <br/>
		///   OS X:    ~/Library/Application Support/ <br/>
		///   Windows: %APPDATA% [User's Home]/Appdata/Roaming <br/>
		///   Linux:   ~/.config <br/>
		/// </remarks>
		ApplicationSettings,

		/// <summary>
		/// The application resources.path
		/// </summary>
		/// <remarks>
		/// In OS X, this will be the .app bundle's resource path.  Other platforms
		/// will typically return the same path as the current executable file.
		/// </remarks>
		ApplicationResources,

		/// <summary>
		/// The user's documents folder
		/// </summary>
		Documents,

		/// <summary>
		/// Gets the path of the entry executable (.exe or native executable)
		/// </summary>
		/// <remarks>
		/// This is used as in some cases when the application is bundled (e.g. using mkbundle),
		/// the location of the assembly can no longer be found as it is loaded from memory.
		/// </remarks>
		EntryExecutable,

		/// <summary>
		/// Gets the user's downloads folder
		/// </summary>
		/// <remarks>
		/// Note that for GTK on Windows, this will *always* return "[User's Home]/Downloads",
		/// regardless of what the user's actual download path is.
		/// </remarks>
		Downloads
	}

	/// <summary>
	/// Environment methods
	/// </summary>
	public static class EtoEnvironment
	{
		static OperatingSystemPlatform platform;

		/// <summary>
		/// Gets the platform information for the currently running operating system
		/// </summary>
		public static OperatingSystemPlatform Platform
		{
			get
			{
				if (platform == null)
					platform = new OperatingSystemPlatform();
				return platform;
			}
		}

		/// <summary>
		/// Gets the folder path for the specified special folder
		/// </summary>
		/// <param name="folder">Special folder to retrieve the path for</param>
		/// <returns>Path of the specified folder</returns>
		public static string GetFolderPath(EtoSpecialFolder folder)
		{
			var handler = Eto.Platform.Instance.CreateShared<IHandler>();
			return handler.GetFolderPath(folder);
		}

		/// <summary>
		/// Gets a value indicating the runtime is a 64 bit process.
		/// </summary>
		/// <value><c>true</c> if running under 64 bit; otherwise, <c>false</c>.</value>
		public static bool Is64BitProcess
		{
			get
			{
				#if NETSTANDARD
				return IntPtr.Size == 8; // test based on size of IntPtr, which is 4 bytes in 32 bit, 8 in 64 bit.
				#else
				return Environment.Is64BitProcess;
				#endif
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="EtoEnvironment"/> class
		/// </summary>
		public interface IHandler
		{
			/// <summary>
			/// Gets the folder path for the specified special folder
			/// </summary>
			/// <param name="folder">Special folder to retrieve the path for</param>
			/// <returns>Path of the specified folder</returns>
			string GetFolderPath(EtoSpecialFolder folder);
		}
	}
}

