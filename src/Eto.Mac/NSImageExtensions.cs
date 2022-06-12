using System;
using Eto.Drawing;


namespace Eto.Mac
{
	public static class NSImageExtensions
	{
		public static NSImage Resize(this NSImage image, CGSize newsize, ImageInterpolation interpolation = ImageInterpolation.Default, CGSize? imageSize = null)
		{
			var newimage = new NSImage(imageSize ?? newsize);
			var newrep = new NSBitmapImageRep(IntPtr.Zero, (nint)newsize.Width, (nint)newsize.Height, 8, 4, true, false, NSColorSpace.DeviceRGB, 4 * (nint)newsize.Width, 32);
			newrep.Size = imageSize ?? newsize;
			newimage.AddRepresentation(newrep);

			var graphics = NSGraphicsContext.FromBitmap(newrep);
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphics;
			graphics.GraphicsPort.InterpolationQuality = interpolation.ToCG();
			image.Draw(new CGRect(CGPoint.Empty, newimage.Size), CGRect.Empty, NSCompositingOperation.SourceOver, 1f);
			NSGraphicsContext.GlobalRestoreGraphicsState();
			return newimage;
		}

#if XAMMAC || MACOS_NET
		static IntPtr selDrawInRect_FromRect_Operation_Fraction_RespectFlipped_Hints_Handle = Selector.GetHandle("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");
#endif

		public static NSImageRep Resize(this NSImageRep image, CGSize newsize, ImageInterpolation interpolation = ImageInterpolation.Default, CGSize? imageSize = null)
		{
			var newrep = new NSBitmapImageRep(IntPtr.Zero, (nint)newsize.Width, (nint)newsize.Height, 8, 4, true, false, NSColorSpace.DeviceRGB, 4 * (nint)newsize.Width, 32);
			newrep.Size = imageSize ?? newsize;

			var graphics = NSGraphicsContext.FromBitmap(newrep);
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphics;
			graphics.GraphicsPort.InterpolationQuality = interpolation.ToCG();
#if XAMMAC || MACOS_NET
			// Xamarin.Mac doesn't allow null for hints, remove this when it does.
			Messaging.bool_objc_msgSend_CGRect_CGRect_UIntPtr_nfloat_bool_IntPtr(image.Handle, selDrawInRect_FromRect_Operation_Fraction_RespectFlipped_Hints_Handle, new CGRect(CGPoint.Empty, newrep.Size), CGRect.Empty, (UIntPtr)(ulong)NSCompositingOperation.SourceOver, 1f, true, IntPtr.Zero);
#else
			image.DrawInRect(new CGRect(CGPoint.Empty, newrep.Size), CGRect.Empty, NSCompositingOperation.SourceOver, 1f, true, null);
#endif
			NSGraphicsContext.GlobalRestoreGraphicsState();
			return newrep;
		}

		public static NSImage Tint(this NSImage image, NSColor tint)
		{
			var colorGenerator = new CIConstantColorGenerator
			{ 
				Color = CIColor.FromCGColor(tint.ToCG())
			};

			var colorFilter = new CIColorControls
			{
				InputImage = (CIImage)colorGenerator.ValueForKey(CIFilterOutputKey.Image),
				Saturation = 3f,
				Brightness = 0.35f,
				Contrast = 1f
			};

			var monochromeFilter = new CIColorMonochrome
			{
				InputImage = CIImage.FromCGImage(image.CGImage),
				Color = CIColor.FromRgb(0.75f, 0.75f, 0.75f),
				Intensity = 1f
			};

			var compositingFilter = new CIMultiplyCompositing
			{
				InputImage = (CIImage)colorFilter.ValueForKey(CIFilterOutputKey.Image),
				BackgroundImage = (CIImage)monochromeFilter.ValueForKey(CIFilterOutputKey.Image)
			};

			var outputImage = (CIImage)compositingFilter.ValueForKey(CIFilterOutputKey.Image);
			var extent = outputImage.Extent;

			var newsize = Size.Truncate(extent.Size.ToEto());
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
			tintedImage.Size = image.Size;
			return tintedImage;
		}
	}
}

