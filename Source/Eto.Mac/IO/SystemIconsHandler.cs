using Eto.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using System.IO;
using Eto.Mac.Drawing;
using System;

namespace Eto.Mac.IO
{
	public class SystemIconsHandler : WidgetHandler<SystemIcons>, ISystemIcons
	{

		#region ISystemIcons implementation

		public Icon GetFileIcon(string fileName, IconSize size)
		{
			var ws = new NSWorkspace();
			var image = ws.IconForFileType(Path.GetExtension(fileName));
			return new Icon(Widget.Platform, new IconHandler(image));
		}

		public Icon GetStaticIcon(StaticIconType type, IconSize size)
		{
			var ws = new NSWorkspace();
			string code;
			switch (type)
			{
				case StaticIconType.OpenDirectory:
					code = "ofld";
					break;
				case StaticIconType.CloseDirectory:
					code = "ofld";
					break;
				default:
					throw new NotSupportedException();
			}
			var image = ws.IconForFileType(code);
			return new Icon(Widget.Platform, new IconHandler(image));
		}

		#endregion

	}
}

