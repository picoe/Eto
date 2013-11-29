using System.IO;
using System.Collections;
using Eto.Drawing;
using System.Collections.Generic;

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

	public interface ISystemIcons : IWidget
	{
		Icon GetFileIcon(string fileName, IconSize size);
		Icon GetStaticIcon(StaticIconType type, IconSize size);
	}
	
	public class SystemIcons : Widget
	{
		new ISystemIcons Handler { get { return (ISystemIcons)base.Handler; } }
		readonly Dictionary<IconSize, Dictionary<object, Icon>> htSizes = new Dictionary<IconSize, Dictionary<object, Icon>>();

		public SystemIcons(Generator g) : base(g, typeof(ISystemIcons))
		{
		}

		Dictionary<object, Icon> GetLookupTable(IconSize size)
		{
			Dictionary<object, Icon> htIcons;
			if (!htSizes.TryGetValue(size, out htIcons))
			{
				htIcons = new Dictionary<object, Icon>();
				htSizes.Add(size, htIcons);
			}
			return htIcons;
		}

		public Icon GetFileIcon(string fileName, IconSize size)
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

		public Icon GetStaticIcon(StaticIconType type, IconSize size)
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
