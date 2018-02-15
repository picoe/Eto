using Eto.IO;
using Eto.Drawing;
using System.IO;
using Eto.Mac.Drawing;
using System;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.IO
{
	public class SystemIconsHandler : SystemIcons.IHandler
	{

		public Icon GetFileIcon(string fileName, IconSize size)
		{
			var ws = new NSWorkspace();
			var image = ws.IconForFileType(Path.GetExtension(fileName));
			return new Icon(new IconHandler(image));
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
			return new Icon(new IconHandler(image));
		}
	}
}

