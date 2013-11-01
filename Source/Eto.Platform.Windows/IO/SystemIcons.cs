using SD = System.Drawing;
using Eto.Drawing;
using Eto.IO;
using Eto.Platform.Windows.Drawing;
using System;

namespace Eto.Platform.Windows.IO
{
	public class SystemIconsHandler : WidgetHandler<SystemIcons>, ISystemIcons
	{

		#region ISystemIcons Members

		public Icon GetFileIcon(string fileName, IconSize size)
		{
			ShellIcon.IconSize iconSize;
			switch (size)
			{
				case IconSize.Large:
					iconSize = ShellIcon.IconSize.Large;
					break;
				case IconSize.Small:
					iconSize = ShellIcon.IconSize.Small;
					break;
				default:
					throw new NotSupportedException();
			}

			SD.Icon icon = ShellIcon.GetFileIcon(fileName, iconSize, false);
			return new Icon(Widget.Generator, new IconHandler(icon));
		}

		public Icon GetStaticIcon(StaticIconType type, IconSize size)
		{
			ShellIcon.IconSize iconSize;
			switch (size)
			{
				case IconSize.Large:
					iconSize = ShellIcon.IconSize.Large;
					break;
				case IconSize.Small:
					iconSize = ShellIcon.IconSize.Small;
					break;
				default:
					throw new NotSupportedException();
			}

			ShellIcon.FolderType folderType;
			switch (type)
			{
				case StaticIconType.OpenDirectory:
					folderType = ShellIcon.FolderType.Open;
					break;
				case StaticIconType.CloseDirectory:
					folderType = ShellIcon.FolderType.Closed;
					break;
				default:
					throw new NotSupportedException();
			}
			
			SD.Icon icon = ShellIcon.GetFolderIcon(iconSize, folderType);
			return new Icon(Widget.Generator, new IconHandler(icon));
		}

		#endregion

	}
}
