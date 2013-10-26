using System;
using MonoMac.AppKit;
using MonoMac.CoreImage;
using sd = System.Drawing;
using MonoMac.Foundation;
using Eto.Drawing;

namespace Eto.Platform.Mac
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
			CIFilter colorGenerator = CIFilter.FromName("CIConstantColorGenerator");
			CIColor color = CIColor.FromCGColor(tint.ToCG());

			colorGenerator.SetValueForKey(color, CIFilterInputKey.Color);
			CIFilter colorFilter = CIFilter.FromName("CIColorControls");

			colorFilter.SetValueForKey(colorGenerator.ValueForKey(CIFilterOutputKey.Image), CIFilterInputKey.Image);
			colorFilter.SetValueForKey(NSNumber.FromFloat(3f), CIFilterInputKey.Saturation);
			colorFilter.SetValueForKey(NSNumber.FromFloat(0.35f), CIFilterInputKey.Brightness);
			colorFilter.SetValueForKey(NSNumber.FromFloat(1f), CIFilterInputKey.Contrast);

			CIFilter monochromeFilter = CIFilter.FromName("CIColorMonochrome");
			CIImage baseImage = CIImage.FromCGImage(image.CGImage);

			monochromeFilter.SetValueForKey(baseImage, CIFilterInputKey.Image);
			monochromeFilter.SetValueForKey(CIColor.FromRgb(0.75f, 0.75f, 0.75f), CIFilterInputKey.Color);
			monochromeFilter.SetValueForKey(NSNumber.FromFloat(1f), CIFilterInputKey.Intensity);

			CIFilter compositingFilter = CIFilter.FromName("CIMultiplyCompositing");

			compositingFilter.SetValueForKey(colorFilter.ValueForKey(CIFilterOutputKey.Image), CIFilterInputKey.Image);
			compositingFilter.SetValueForKey(monochromeFilter.ValueForKey(CIFilterOutputKey.Image), CIFilterInputKey.BackgroundImage);

			var outputImage = (CIImage)compositingFilter.ValueForKey(CIFilterOutputKey.Image);
			var extent = outputImage.Extent;

			var newsize = sd.Size.Truncate(extent.Size);

			var tintedImage = new NSImage(newsize);
			var newrep = new NSBitmapImageRep(IntPtr.Zero, newsize.Width, newsize.Height, 8, 4, true, false, NSColorSpace.DeviceRGB, 4 * newsize.Width, 32);
			tintedImage.AddRepresentation(newrep);

			var graphics = NSGraphicsContext.FromBitmap(newrep);
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphics;

			var ciContext = CIContext.FromContext(graphics.GraphicsPort, new CIContextOptions { UseSoftwareRenderer = true });
			ciContext.DrawImage(outputImage, extent, extent);

			NSGraphicsContext.GlobalRestoreGraphicsState();

			newrep.Size = image.Size;
			return tintedImage;
		}
	}
}

