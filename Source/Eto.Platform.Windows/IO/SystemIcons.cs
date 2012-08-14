using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.IO;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.IO
{
	public class SystemIconsHandler : WidgetHandler<SystemIcons>, ISystemIcons
	{
		#region ISystemIcons Members

		public Icon GetFileIcon(string fileName, Eto.IO.IconSize size)
		{
			ShellIcon.IconSize iconSize;
			switch (size)
			{
				default:
				case IconSize.Large: iconSize = ShellIcon.IconSize.Large; break;
				case IconSize.Small: iconSize = ShellIcon.IconSize.Small; break;
			}

			SD.Icon icon = ShellIcon.GetFileIcon( fileName, iconSize, false );
			return new Icon(new IconHandler(icon));
		}

		public Icon GetStaticIcon(Eto.IO.StaticIconType type, Eto.IO.IconSize size)
		{
			ShellIcon.IconSize iconSize;
			switch (size)
			{
				default:
				case IconSize.Large: iconSize = ShellIcon.IconSize.Large; break;
				case IconSize.Small: iconSize = ShellIcon.IconSize.Small; break;
			}

			ShellIcon.FolderType folderType;
			switch (type)
			{
				default:
				case StaticIconType.OpenDirectory: folderType = ShellIcon.FolderType.Open;  break;
				case StaticIconType.CloseDirectory: folderType = ShellIcon.FolderType.Closed;  break;
			}
			
			SD.Icon icon = ShellIcon.GetFolderIcon(iconSize, folderType);
			return new Icon(new IconHandler(icon));
		}

		#endregion

	}
}
