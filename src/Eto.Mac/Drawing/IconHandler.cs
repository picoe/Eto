using System;
using System.Globalization;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;

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

namespace Eto.Mac.Drawing
{
	public class IconHandler : ImageHandler<NSImage, Icon>, Icon.IHandler
	{
		List<IconFrame> _frames;
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

		IEnumerable<IconFrame> GetFrames()
		{
			foreach (var rep in Control.Representations())
			{
				var img = new NSImage();
				img.AddRepresentation(rep);
				yield return IconFrame.FromControlObject(1, new Bitmap(new BitmapHandler(img)));
			}
		}

		public void Create(IEnumerable<IconFrame> frames)
		{
			_frames = frames.ToList();
			var curScale = Screen.PrimaryScreen.LogicalPixelSize;
			var item = _frames.FirstOrDefault(r => Math.Abs(r.Scale - curScale) < 0.0001) ?? _frames.First();

			var size = Size.Ceiling((SizeF)item.PixelSize / (float)item.Scale).ToNS();

			Control = new NSImage { Size = size };
			foreach (var frame in _frames)
			{
				var rep = (NSImageRep)frame.Bitmap.ToNS().Representations().First().Copy();

				rep.Size = (new SizeF(rep.PixelsWide, rep.PixelsHigh) / (float)frame.Scale).ToNS();
				if (rep is IconFrameHandler.LazyImageRep mns)
				{
					mns.Size = size;//(size.ToEto() * (float)r.Scale).ToNS();
					var pixelSize = Size.Ceiling(size.ToEto() * (float)frame.Scale);
					mns.PixelsHigh = pixelSize.Height;
					mns.PixelsWide = pixelSize.Width;
				}

				Control.AddRepresentation(rep);
			}
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

		public IEnumerable<IconFrame> Frames => _frames ?? (_frames = GetFrames().ToList());
	}
}
