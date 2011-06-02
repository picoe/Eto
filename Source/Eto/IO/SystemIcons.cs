using System;
using System.IO;
using System.Collections;
using Eto.Drawing;

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
		ISystemIcons inner;
		Hashtable htSizes = new Hashtable();

		public SystemIcons(Generator g) : base(g, typeof(ISystemIcons))
		{
			inner = (ISystemIcons)Handler;
		}

		private Hashtable GetLookupTable(IconSize size)
		{
			Hashtable htIcons = (Hashtable)htSizes[size];
			if (htIcons == null)
			{
				htIcons = new Hashtable();
				htSizes[size] = htIcons;
			}
			return htIcons;
		}

		public Icon GetFileIcon(string fileName, IconSize size)
		{
			Hashtable htIcons = GetLookupTable(size);
			string ext = Path.GetExtension(fileName).ToUpperInvariant();
			Icon icon = (Icon)htIcons[ext];
			if (icon == null) 
			{
				icon = inner.GetFileIcon(fileName, size);
				htIcons.Add(ext, icon);
			}
			return icon;
		}

		public Icon GetStaticIcon(StaticIconType type, IconSize size)
		{
			Hashtable htIcons = GetLookupTable(size);
			Icon icon = (Icon)htIcons[type];
			if (icon == null) 
			{
				icon = inner.GetStaticIcon(type, size);
				htIcons.Add(type, icon);
			}
			return icon;
		}
	}
}
