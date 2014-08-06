using System;
using MonoMac.AppKit;
using MonoMac.CoreImage;
using sd = System.Drawing;
using MonoMac.Foundation;
using Eto.Drawing;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac
{
	public static class NSImageExtensions
	{
		public static NSImage Resize(this NSImage image, NSSize newsize, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			var newimage = new NSImage(newsize);
			var newrep = new NSBitmapImageRep(IntPtr.Zero, (NSInteger)newsize.Width, (NSInteger)newsize.Height, 8, 4, true, false, NSColorSpace.DeviceRGB, 4 * (NSInteger)newsize.Width, 32);
			newimage.AddRepresentation(newrep);

			var graphics = NSGraphicsContext.FromBitmap(newrep);
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphics;
			graphics.GraphicsPort.InterpolationQuality = interpolation.ToCG();
			image.DrawInRect(new NSRect(new NSPoint(), newimage.Size), new NSRect(new NSPoint(), image.Size), NSCompositingOperation.SourceOver, 1f);
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

			var newsize = sd.Size.Truncate(extent.Size.ToSD());
			if (newsize.IsEmpty)
				return image;

			var tintedImage = new NSImage(newsize.ToNS());
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

