using System.IO;
using System.Collections;
using Eto.Drawing;
using System.Collections.Generic;
using System;

namespace Eto.IO
{
	public enum StaticIconType
	{
		OpenDirectory,
		CloseDirectory
	}
	
	public enum IconSize
	{
		Large,
		Small
	}

	public interface ISystemIcons
	{
		Icon GetFileIcon(string fileName, IconSize size);
		Icon GetStaticIcon(StaticIconType type, IconSize size);
	}

	[Handler(typeof(ISystemIcons))]
	public static class SystemIcons
	{
		static ISystemIcons Handler { get { return Platform.Instance.CreateShared<ISystemIcons>(); } }

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
	}
}