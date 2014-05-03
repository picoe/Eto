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
		///   Linux:   
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
	/// Handler interface for the <see cref="EtoEnvironment"/> class
	/// </summary>
	public interface IEtoEnvironment : IWidget
	{
		/// <summary>
		/// Gets the folder path for the specified special folder
		/// </summary>
		/// <param name="folder">Special folder to retrieve the path for</param>
		/// <returns>Path of the specified folder</returns>
		string GetFolderPath(EtoSpecialFolder folder);
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
			var handler = Eto.Platform.Instance.CreateShared<IEtoEnvironment>();
			return handler.GetFolderPath(folder);
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Gets the folder path for the specified special folder
		/// </summary>
		/// <param name="folder">Special folder to retrieve the path for</param>
		/// <param name="generator">Generator to get the folder path with</param>
		/// <returns>Path of the specified folder</returns>
		[Obsolete("Use variation without generator instead")]
		public static string GetFolderPath(EtoSpecialFolder folder, Generator generator)
		{
			var handler = generator.CreateShared<IEtoEnvironment>();
			return handler.GetFolderPath(folder);
		}

		#pragma warning restore 612,618
	}
}

