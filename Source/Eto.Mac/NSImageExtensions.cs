using System;
using MonoMac.AppKit;
using MonoMac.CoreImage;
using sd = System.Drawing;
using MonoMac.Foundation;
using Eto.Drawing;

namespace Eto.Mac
{
	public static class NSImageExtensions
	{
		public static NSImage Resize(this NSImage image, sd.Size newsize, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			var newimage = new NSImage(newsize);
			var newrep = new NSBitmapImageRep(IntPtr.Zero, newsize.Width, newsize.Height, 8, 4, true, false, NSColorSpace.DeviceRGB, 4 * newsize.Width, 32);
			newimage.AddRepresentation(newrep);

			var graphics = NSGraphicsContext.FromBitmap(newrep);
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphics;
			graphics.GraphicsPort.InterpolationQuality = interpolation.ToCG();
			image.DrawInRect(new sd.RectangleF(sd.PointF.Empty, newimage.Size), new sd.RectangleF(sd.PointF.Empty, image.Size), NSCompositingOperation.SourceOver, 1f);
			NSGraphicsContext.GlobalRestoreGraphicsState();
			return newimage;
		}

		public static NSImage Tint(this NSImage image, NSColor tint)
		{
			var colorGenerator = new CIConstantColorGenerator
			{ 
				Color = CIColor.FromCGColor(tint.CGColor)
			};

			var colorFilter = new CIColorControls
			{
				Image = (CIImage)colorGenerator.ValueForKey(CIFilterOutputKey.Image),
				Saturation = 3f,
				Brightness = 0.35f,
				Contrast = 1f
			};

			var monochromeFilter = new CIColorMonochrome
			{
				Image = CIImage.FromCGImage(image.CGImage),
				Color = CIColor.FromRgb(0.75f, 0.75f, 0.75f),
				Intensity = 1f
			};

			var compositingFilter = new CIMultiplyCompositing
			{
				Image = (CIImage)colorFilter.ValueForKey(CIFilterOutputKey.Image),
				BackgroundImage = (CIImage)monochromeFilter.ValueForKey(CIFilterOutputKey.Image)
			};

			var outputImage = (CIImage)compositingFilter.ValueForKey(CIFilterOutputKey.Image);
			var extent = outputImage.Extent;

			var newsize = sd.Size.Truncate(extent.Size);
			if (newsize.IsEmpty)
				return image;

			var tintedImage = new NSImage(newsize);
			tintedImage.LockFocus();
			try
			{
				var graphics = NSGraphicsContext.CurrentContext.GraphicsPort;
				var ciContext = CIContext.FromContext(graphics, new CIContextOptions { UseSoftwareRenderer = true });
				ciContext.DrawImage(outputImage, extent, extent);
			}
			finally
			{
				tintedImage.UnlockFocus();
			}

			var newrep = tintedImage.Representations()[0];
			newrep.Size = image.Size;
			return tintedImage;
		}
	}
}

