#if WPF
namespace Eto.Wpf.Drawing;
#elif WINFORMS
namespace Eto.WinForms.Drawing;
#endif

public class SystemIconsHandler : SystemIcons.IHandler
{
	public Icon GetFileIcon(string fileName, SystemIconSize size)
	{
		var pixels = GetDesiredPixels(size);
		using (var icon = ShellIcon.GetFileIcon(fileName, pixels, false))
		{
			if (icon != null)
				return WithSize(new Icon(new IconHandler(icon)), size);
		}

		// fall back to old method
		var iconSize = size switch
		{
			SystemIconSize.Large => ShellIcon.IconSize.Large,
			SystemIconSize.Small => ShellIcon.IconSize.Small,
			_ => throw new NotSupportedException(),
		};
		using (var icon = ShellIcon.GetFileIcon(fileName, iconSize, false))
		{
			return WithSize(new Icon(new IconHandler(icon)), size);
		}
	}

	public Icon Get(SystemIconType type, SystemIconSize size)
	{
		return type switch
		{
			SystemIconType.OpenDirectory => GetSystemIcon(Win32.SHSTOCKICONID.SIID_FOLDEROPEN, size),
			SystemIconType.CloseDirectory => GetSystemIcon(Win32.SHSTOCKICONID.SIID_FOLDER, size),
			SystemIconType.Question => GetSystemIcon(Win32.SHSTOCKICONID.SIID_HELP, size),
			SystemIconType.Error => GetSystemIcon(Win32.SHSTOCKICONID.SIID_ERROR, size),
			SystemIconType.Information => GetSystemIcon(Win32.SHSTOCKICONID.SIID_INFO, size),
			SystemIconType.Warning => GetSystemIcon(Win32.SHSTOCKICONID.SIID_WARNING, size),
			_ => throw new NotSupportedException(),
		};
	}

	Icon GetSystemIcon(Win32.SHSTOCKICONID icon_id, SystemIconSize size)
	{
		// Try to get a high-DPI icon via SHGetImageList
		var pixels = GetDesiredPixels(size);
		var sii = new Win32.SHSTOCKICONINFO();
		sii.cbSize = (UInt32)Marshal.SizeOf(typeof(Win32.SHSTOCKICONINFO));
		if (Win32.SHGetStockIconInfo(icon_id, Win32.SHGSI.SHGSI_SYSICONINDEX, ref sii) == 0)
		{
			var imageListType = Win32.GetBestImageListType(pixels);
			var sdicon = Win32.GetIconFromImageList(imageListType, sii.iSysIconIndex);
			if (sdicon != null)
				return WithSize(new Icon(new IconHandler(sdicon)), size);
		}

		// Fall back to old method
		Win32.SHGSI flags = Win32.SHGSI.SHGSI_ICON;
		flags |= size switch
		{
			SystemIconSize.Large => Win32.SHGSI.SHGSI_LARGEICON,
			SystemIconSize.Small => Win32.SHGSI.SHGSI_SMALLICON,
			_ => throw new NotSupportedException(),
		};
		sii = new Win32.SHSTOCKICONINFO();
		sii.cbSize = (UInt32)Marshal.SizeOf(typeof(Win32.SHSTOCKICONINFO));
		Win32.SHGetStockIconInfo(icon_id, flags, ref sii);
		var fallbackIcon = (sd.Icon)sd.Icon.FromHandle(sii.hIcon).Clone();
		Win32.DestroyIcon(sii.hIcon);
		return WithSize(new Icon(new IconHandler(fallbackIcon)), size);
	}

	private static int GetDesiredPixels(SystemIconSize size)
	{
		var basePixels = size switch
		{
			SystemIconSize.Large => 32,
			SystemIconSize.Small => 16,
			_ => throw new NotSupportedException()
		};
		var scale = Win32.SystemDpi;
		return (int)Math.Ceiling(basePixels * scale);
	}

	private static Icon WithSize(Icon icon, SystemIconSize size)
	{
		var pixels = size switch
		{
			SystemIconSize.Large => 32,
			SystemIconSize.Small => 16,
			_ => throw new NotSupportedException()
		};
		icon = icon.WithSize(pixels, pixels);
		return icon;
	}

}
