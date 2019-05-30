using System.IO;
using System.Collections;
using Eto.Drawing;
using System.Collections.Generic;
using System;

namespace Eto.IO
{
	/// <summary>
	/// Type of static icon to get
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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
	[Handler(typeof(SystemIcons.IHandler))]
	public static class SystemIcons
	{
		static IHandler Handler { get { return Platform.Instance.CreateShared<IHandler>(); } }

		static readonly object cacheKey = new object();

		static Dictionary<object, Icon> GetLookupTable(IconSize size)
		{
			var htSizes = Platform.Instance.Cache<IconSize, Dictionary<object, Icon>>(cacheKey);
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
		public static Icon GetFileIcon(string fileName, IconSize size)
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
		public static Icon GetStaticIcon(StaticIconType type, IconSize size)
		{
			var htIcons = GetLookupTable(size);
			Icon icon;
			if (!htIcons.TryGetValue(type, out icon))
			{
				icon = Handler.GetStaticIcon(type, size);
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
			Icon GetFileIcon(string fileName, IconSize size);

			/// <summary>
			/// Gets a static system-defined icon for the specified type.
			/// </summary>
			/// <returns>The static icon for the specified type.</returns>
			/// <param name="type">Type of the static icon to retrieve.</param>
			/// <param name="size">Size of the icon.</param>
			Icon GetStaticIcon(StaticIconType type, IconSize size);
		}
	}
}