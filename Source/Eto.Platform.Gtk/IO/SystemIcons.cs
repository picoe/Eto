using Eto.Drawing;
using Eto.IO;

namespace Eto.Platform.GtkSharp.IO
{
	public class SystemIconsHandler : WidgetHandler<SystemIcons>, ISystemIcons
	{
		#region ISystemIcons Members

		public Icon GetFileIcon(string fileName, IconSize size)
		{
			return null;
		}

		public Icon GetStaticIcon(StaticIconType type, IconSize size)
		{
			return null;
		}

		#endregion

	}
}
