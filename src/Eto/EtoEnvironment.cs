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
		/// This will return a different folder, depending on the platform:
		///   OS X:    ~/Library/Application Settings/[Name Of Application]
		///   Windows: [User's Home]/AppSettings
		///   Linux:   ~/.config
		/// </remarks>
		ApplicationSettings,

		/// <summary>
		/// The application resources.path
		/// </summary>
		/// <remarks>
		/// In OS X, this will be the .app bunldle's resource path.  Other platforms
		/// will typically return the same path as the current executable file
		/// </remarks>
		ApplicationResources,

		/// <summary>
		/// The user's documents folder
		/// </summary>
		Documents
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

