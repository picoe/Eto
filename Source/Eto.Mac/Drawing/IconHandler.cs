using System;
using System.Globalization;
using System.IO;
using Eto.Drawing;

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
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Drawing
{
	public class IconHandler : ImageHandler<NSImage, Icon>, Icon.IHandler
	{
		public IconHandler()
		{
		}

		public IconHandler(NSImage image)
		{
			Control = image;
		}

		public void Create(Stream stream)
		{
			var data = NSData.FromStream(stream);
			Control = new NSImage(data);
		}

		public void Create(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, "Icon not found"), fileName);
			Control = new NSImage(fileName);
		}

		public override Size Size
		{
			get { return Control.Size.ToEtoSize(); }
		}

		public override NSImage GetImage()
		{
			return Control;
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var sourceRect = new CGRect(source.X, (float)Control.Size.Height - source.Y - source.Height, source.Width, source.Height);
			var destRect = destination.ToNS();
			Control.Draw(destRect, sourceRect, NSCompositingOperation.SourceOver, 1, true, null);
		}
	}
}
