using Eto.Drawing;
using Eto.IO;

namespace Eto.GtkSharp.IO
{
	public class SystemIconsHandler : SystemIcons.IHandler
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
