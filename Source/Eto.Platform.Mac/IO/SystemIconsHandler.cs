using System;
using Eto.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using System.IO;
using Eto.Platform.Mac.Drawing;
namespace Eto.Platform.Mac.IO
{
	public class SystemIconsHandler : WidgetHandler<SystemIcons>, ISystemIcons
	{
		#region ISystemIcons implementation
		public Icon GetFileIcon (string fileName, IconSize size)
		{
			var ws = new NSWorkspace ();
			var image = ws.IconForFileType (Path.GetExtension (fileName));
			return new Icon (Widget.Generator, new IconHandler (image));
		}

		public Icon GetStaticIcon (StaticIconType type, IconSize size)
		{
			var ws = new NSWorkspace ();
			string code;
			switch (type) {
			case StaticIconType.OpenDirectory: code = "ofld"; break;
			default:
			case StaticIconType.CloseDirectory: code = "ofld"; break;
			}
			var image = ws.IconForFileType (code);
			return new Icon (Widget.Generator, new IconHandler (image));
		}
		#endregion
	}
}

