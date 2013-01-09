using System;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.CoreImage;
using sd = System.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public static class NSImageExtensions
	{
		public static NSImage Tint (this NSImage image, NSColor tint)
		{
			CIFilter colorGenerator = CIFilter.FromName ("CIConstantColorGenerator");
			CIColor color = CIColor.FromCGColor (tint.ToCG ());

			colorGenerator.SetValueForKey (color, CIFilterInputKey.Color);
			CIFilter colorFilter = CIFilter.FromName ("CIColorControls");

			colorFilter.SetValueForKey (colorGenerator.ValueForKey(CIFilterOutputKey.Image), CIFilterInputKey.Image);
			colorFilter.SetValueForKey (NSNumber.FromFloat (3f), CIFilterInputKey.Saturation);
			colorFilter.SetValueForKey (NSNumber.FromFloat (0.35f), CIFilterInputKey.Brightness);
			colorFilter.SetValueForKey (NSNumber.FromFloat (1f), CIFilterInputKey.Contrast);

			CIFilter monochromeFilter = CIFilter.FromName("CIColorMonochrome");
			CIImage baseImage = new CIImage (image.AsTiff ());

			monochromeFilter.SetValueForKey (baseImage, CIFilterInputKey.Image);
			monochromeFilter.SetValueForKey (CIColor.FromRgb (0.75f, 0.75f, 0.75f), CIFilterInputKey.Color);
			monochromeFilter.SetValueForKey (NSNumber.FromFloat(1f), CIFilterInputKey.Intensity);

			CIFilter compositingFilter = CIFilter.FromName("CIMultiplyCompositing");

			compositingFilter.SetValueForKey (colorFilter.ValueForKey(CIFilterOutputKey.Image), CIFilterInputKey.Image);
			compositingFilter.SetValueForKey (monochromeFilter.ValueForKey(CIFilterOutputKey.Image), CIFilterInputKey.BackgroundImage);

			CIImage outputImage = (CIImage)compositingFilter.ValueForKey (CIFilterOutputKey.Image);
			var extent = outputImage.Extent;
			NSImage tintedImage = new NSImage (extent.Size);

			tintedImage.LockFocus();
			{
				CGContext contextRef = NSGraphicsContext.CurrentContext.GraphicsPort;
				CIContext ciContext = CIContext.FromContext (contextRef,  new CIContextOptions { UseSoftwareRenderer = true });
				ciContext.DrawImage (outputImage, extent, extent);
			}
			tintedImage.UnlockFocus ();
			var rep = tintedImage.Representations ()[0];
			rep.Size = image.Size;

			tintedImage = new NSImage();
			tintedImage.AddRepresentation (rep);

			return tintedImage;
		}
	}
}

