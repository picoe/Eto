namespace Eto.Drawing;

/// <summary>
/// Type of system icon to get
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
public enum SystemIconType
{
	/// <summary>
	/// Icon for an open directory/folder
	/// </summary>
	OpenDirectory,
	/// <summary>
	/// Icon for a closed directory/folder
	/// </summary>
	CloseDirectory,
	/// <summary>
	/// Icon for a question mark
	/// </summary>
	Question,
	/// <summary>
	/// Icon for errors
	/// </summary>
	Error,
	/// <summary>
	/// Icon to use for informational messages
	/// </summary>
	Information,
	/// <summary>
	/// Icon to use for warnings
	/// </summary>
	Warning

}

/// <summary>
/// Size of icon to get
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
public enum SystemIconSize
{
	/// <summary>
	/// Large icon (usually suitable to display at 32x32 logical pixels)
	/// </summary>
	Large,
	/// <summary>
	/// Small icon (usually suitable to display at 16x16 logical pixels)
	/// </summary>
	Small
}

/// <summary>
/// Methods to get system icons for file types and static icons
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
[Handler(typeof(IHandler))]
public static class SystemIcons
{
	static IHandler Handler => Platform.Instance.CreateShared<IHandler>();

	static readonly object cacheKey = new object();

	static Dictionary<object, Icon> GetLookupTable(SystemIconSize size)
	{
		var htSizes = Platform.Instance.Cache<SystemIconSize, Dictionary<object, Icon>>(cacheKey);
		Dictionary<object, Icon> htIcons;
		if (!htSizes.TryGetValue(size, out htIcons))
		{
			htIcons = new Dictionary<object, Icon>();
			htSizes.Add(size, htIcons);
		}
		return htIcons;
	}

	/// <summary>
	/// Gets a file icon for the specified file
	/// </summary>
	/// <remarks>
	/// The file does not necessarily have to exist for the icon to be retrieved, though if it is a specific file
	/// then the platform may be able to return a file-specific icon if one is available.
	/// </remarks>
	/// <returns>The icon for the specified file name.</returns>
	/// <param name="fileName">Name of the file to get the icon for.</param>
	/// <param name="size">Size of the icon.</param>
	public static Icon GetFileIcon(string fileName, SystemIconSize size)
	{
		var htIcons = GetLookupTable(size);
		string ext = Path.GetExtension(fileName).ToUpperInvariant();
		Icon icon;
		if (!htIcons.TryGetValue(ext, out icon))
		{
			icon = Handler.GetFileIcon(fileName, size);
			htIcons.Add(ext, icon);
		}
		return icon;
	}

	/// <summary>
	/// Gets a static system-defined icon for the specified type.
	/// </summary>
	/// <returns>The static icon for the specified type.</returns>
	/// <param name="type">Type of the static icon to retrieve.</param>
	/// <param name="size">Size of the icon.</param>
	public static Icon Get(SystemIconType type, SystemIconSize size)
	{
		var htIcons = GetLookupTable(size);
		Icon icon;
		if (!htIcons.TryGetValue(type, out icon))
		{
			icon = Handler.Get(type, size);
			htIcons.Add(type, icon);
		}
		return icon;
	}

	/// <summary>
	/// Handler interface for the <see cref="SystemIcons"/> methods
	/// </summary>
	public interface IHandler
	{
		/// <summary>
		/// Gets a file icon for the specified file
		/// </summary>
		/// <remarks>
		/// The file does not necessarily have to exist for the icon to be retrieved, though if it is a specific file
		/// then the platform may be able to return a file-specific icon if one is available.
		/// </remarks>
		/// <returns>The icon for the specified file name.</returns>
		/// <param name="fileName">Name of the file to get the icon for.</param>
		/// <param name="size">Size of the icon.</param>
		Icon GetFileIcon(string fileName, SystemIconSize size);

		/// <summary>
		/// Gets a static system-defined icon for the specified type.
		/// </summary>
		/// <returns>The static icon for the specified type.</returns>
		/// <param name="type">Type of the static icon to retrieve.</param>
		/// <param name="size">Size of the icon.</param>
		Icon Get(SystemIconType type, SystemIconSize size);
	}
}