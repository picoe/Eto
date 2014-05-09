using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using sd = System.Drawing;

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
				throw new FileNotFoundException("Icon not found", fileName);
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
			var sourceRect = new sd.RectangleF(source.X, Control.Size.Height - source.Y - source.Height, source.Width, source.Height);
			var destRect = graphics.TranslateView(destination.ToSD(), true, true);
			Control.Draw(destRect, sourceRect, NSCompositingOperation.SourceOver, 1, true, null);
		}
	}
}
