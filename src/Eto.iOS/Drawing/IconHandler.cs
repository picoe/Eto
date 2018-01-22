using System;
using System.IO;
using Eto.Drawing;
using UIKit;
using Foundation;
using SD = System.Drawing;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Drawing
{
	public class IconFrameHandler : IconFrame.IHandler
	{
		public object Create(IconFrame frame, Stream stream)
		{
			return new Bitmap(stream);
		}
		public object Create(IconFrame frame, Func<Stream> load)
		{
			return new Bitmap(load());
		}
		public object Create(IconFrame frame, Bitmap bitmap)
		{
			return bitmap;
		}
		public Bitmap GetBitmap(IconFrame frame)
		{
			return (Bitmap)frame.ControlObject;
		}
		public Size GetPixelSize(IconFrame frame)
		{
			return GetBitmap(frame).Size;
		}
	}

	public class IconHandler : ImageHandler<UIImage, Icon>, Icon.IHandler
	{
		public IconHandler()
		{
		}

		public IconHandler(UIImage image)
		{
			Control = image;
		}

		public void Create(Stream stream)
		{
			var data = NSData.FromStream(stream);
			Control = new UIImage(data);
		}

		public void Create(string fileName)
		{
			Control = new UIImage(fileName);
		}

		public override Size Size
		{
			get { return Control.Size.ToEtoSize(); }
		}

		public override UIImage GetUIImage(int? maxSize = null)
		{
			var size = Size;
			var imgSize = Math.Max(size.Width, size.Height);
			if (maxSize != null && imgSize > maxSize.Value)
			{
				size = (Size)(size * ((float)maxSize.Value / (float)imgSize));
				var img = new Bitmap(Widget, size.Width, size.Height);
				return img.ToUI();
			}
			return Control;
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var sourceRect = source.ToNS();
			var imgsize = Control.Size;
			CGRect destRect = destination.ToNS();
			if (source.TopLeft != Point.Empty || sourceRect.Size != imgsize)
			{
				graphics.Control.TranslateCTM(destRect.X - sourceRect.X, imgsize.Height - (destRect.Y - sourceRect.Y));
				graphics.Control.ScaleCTM(imgsize.Width / sourceRect.Width, -(imgsize.Height / sourceRect.Height));
				graphics.Control.DrawImage(new CGRect(CGPoint.Empty, destRect.Size), Control.CGImage);
			}
			else
			{
				Control.Draw(destRect, CGBlendMode.Normal, 1);
			}
		}

		List<IconFrame> frames;

		public void Create(IEnumerable<IconFrame> frames)
		{
			this.frames = frames.ToList();
			Control = Widget.GetFrame((float)UIScreen.MainScreen.NativeScale).Bitmap.ToUI();
		}

		public System.Collections.Generic.IEnumerable<IconFrame> Frames
		{
			get
			{
				if (frames == null)
					frames = new List<IconFrame> { new IconFrame((float)UIScreen.MainScreen.NativeScale, new Bitmap(new BitmapHandler(Control))) };
				return frames;
			}
		}
	}
}
