namespace Eto.IO;

/// <summary>
/// Type of static icon to get
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
[Obsolete("Use Eto.Drawing.SystemIconType instead")]
public enum StaticIconType
{
	/// <summary>
	/// Icon for an open directory/folder
	/// </summary>
	OpenDirectory,
	/// <summary>
	/// Icon for a closed directory/folder
	/// </summary>
	CloseDirectory
}

/// <summary>
/// Size of icon to get
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
[Obsolete("Use Eto.Drawing.SystemIconSize instead")]
public enum IconSize
{
	/// <summary>
	/// Large icon (32x32 or 64x64 for retina)
	/// </summary>
	Large,
	/// <summary>
	/// Small icon (16x16 or 32x32 for retina)
	/// </summary>
	Small
}

/// <summary>
/// Methods to get system icons for file types and static icons
/// </summary>
/// <copyright>(c) 2014 by Curtis Wensley</copyright>
/// <license type="BSD-3">See LICENSE for full terms</license>
[Obsolete("Use Eto.Drawing.SystemImages instead")]
public static class SystemIcons
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
	public static Icon GetFileIcon(string fileName, IconSize size)
	{
		return Drawing.SystemIcons.GetFileIcon(fileName, size == IconSize.Large ? Drawing.SystemIconSize.Large : Drawing.SystemIconSize.Small);
	}

	/// <summary>
	/// Gets a static system-defined icon for the specified type.
	/// </summary>
	/// <returns>The static icon for the specified type.</returns>
	/// <param name="type">Type of the static icon to retrieve.</param>
	/// <param name="size">Size of the icon.</param>
	public static Icon GetStaticIcon(StaticIconType type, IconSize size)
	{
		var dtype = type == StaticIconType.OpenDirectory ? Drawing.SystemIconType.OpenDirectory : Drawing.SystemIconType.CloseDirectory;
		return Drawing.SystemIcons.Get(dtype, size == IconSize.Large ? Drawing.SystemIconSize.Large : Drawing.SystemIconSize.Small);
	}
}