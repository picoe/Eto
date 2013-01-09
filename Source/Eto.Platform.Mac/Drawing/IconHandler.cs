using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;

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
			var sourceRect = graphics.Translate (source.ToSD (), Control.Size.Height);
			var destRect = graphics.TranslateView (destination.ToSD (), true, true);
			graphics.Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, graphics.ViewHeight));
			destRect.Y = graphics.ViewHeight - destRect.Y - destRect.Height;
			Control.Draw (destRect, sourceRect, NSCompositingOperation.SourceOver, 1);
		}

        #region IImage Members

        public int Width
        {
            get { return 0;/* TODO */ }
        }

        public int Height
        {
            get { return 0;/* TODO */ }
        }

        #endregion
	}
}
