using System;
using Eto.Drawing;
using Eto.IO;

namespace Eto.Platform.GtkSharp.IO
{
	/// <summary>
	/// Summary description for SystemIcons.
	/// </summary>
	public class SystemIcons : WidgetHandler, ISystemIcons
	{
		#region ISystemIcons Members

		public Icon GetFileIcon(string fileName, Eto.IO.IconSize size)
		{
			return null;
		}

		public Icon GetStaticIcon(Eto.IO.StaticIconType type, Eto.IO.IconSize size)
		{
			return null;
		}

		#endregion

	}
}
