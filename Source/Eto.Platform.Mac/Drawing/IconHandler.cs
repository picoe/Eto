using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Drawing
{
	public class IconHandler : ImageHandler<NSImage, Icon>, IIcon
	{
		public IconHandler()
		{
		}
		
		public IconHandler(NSImage image)
		{
			Control = image;
		}
		
		public void Create (Stream stream)
		{
			var data = NSData.FromStream(stream);
			Control = new NSImage (data);
		}

		public void Create (string fileName)
		{
			if (!File.Exists (fileName))
				throw new FileNotFoundException ("Icon not found", fileName);
			Control = new NSImage (fileName);
		}
		
		public override Size Size {
			get { return Control.Size.ToEtoSize (); }
		}

		public override NSImage GetImage ()
		{
			return Control;
		}

		public override void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var nsimage = this.Control;
			var sourceRect = graphics.Translate (source.ToSD (), nsimage.Size.Height);
			var destRect = graphics.TranslateView (destination.ToSD (), false);
			nsimage.Draw (destRect, sourceRect, NSCompositingOperation.Copy, 1);
		}
    }
}
