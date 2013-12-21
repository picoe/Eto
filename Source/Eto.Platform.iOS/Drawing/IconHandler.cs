using System;
using System.IO;
using Eto.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SD = System.Drawing;
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
{
	public class IconHandler : ImageHandler<UIImage, Icon>, IIcon
	{
		public IconHandler()
		{
		}
		
		public IconHandler(UIImage image)
		{
			Control = image;
		}
		
		public void Create (Stream stream)
		{
			var data = NSData.FromStream(stream);
			Control = new UIImage (data);
		}

		public void Create (string fileName)
		{
			Control = new UIImage (fileName);
		}

		public override Size Size {
			get {
				return Control.Size.ToEtoSize ();
			}
		}

		public override UIImage GetUIImage ()
		{
			return this.Control;
		}

		public override void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var sourceRect = source.ToSD();
			var imgsize = Control.Size;
			SD.RectangleF destRect = graphics.TranslateView(destination.ToSD(), false);
			if (source.TopLeft != Point.Empty || sourceRect.Size != imgsize)
			{
				graphics.Control.TranslateCTM(destRect.X - sourceRect.X, imgsize.Height - (destRect.Y + sourceRect.Y));
				graphics.Control.ScaleCTM(imgsize.Width / sourceRect.Width, -(imgsize.Height / sourceRect.Height));
				graphics.Control.DrawImage(new SD.RectangleF(SD.PointF.Empty, destRect.Size), Control.CGImage);
			}
			else
			{
				Control.Draw(destRect, CGBlendMode.Normal, 1);
			}
		}
	}
}
