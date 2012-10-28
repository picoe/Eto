using System;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using Eto.Drawing;
using MonoMac.Foundation;
using Eto.Forms;

namespace Eto.Platform.Mac
{
	public static partial class Conversions
	{
		public static NSColor ToNS (this Color color)
		{
			return NSColor.FromDeviceRgba (color.R, color.G, color.B, color.A);
		}

		public static Color ToEto (this NSColor color)
		{
			if (color == null)
				return Colors.Black;
			float red, green, blue, alpha;
			color.GetRgba (out red, out green, out blue, out alpha);
			return new Color (red, green, blue, alpha);
		}
		
		public static CGColor ToCG (this NSColor color)
		{
			var cs = NSColorSpace.DeviceRGBColorSpace;
			
			var devColor = color.UsingColorSpace (cs);
			float[] components;
			devColor.GetComponents (out components);
			return new CGColor(cs.ColorSpace, components);
		}
		
		public static NSRange ToNS (this Range range)
		{
			return new NSRange(range.Location, range.Length);
		}
		
		public static Range ToEto (this NSRange range)
		{
			return new Range (range.Location, range.Length);
		}
		
		public static NSImageInterpolation ToNS (this ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.None:
				return NSImageInterpolation.None;
			case ImageInterpolation.Low:
				return NSImageInterpolation.Low;
			case ImageInterpolation.Medium:
				return NSImageInterpolation.Medium;
			case ImageInterpolation.High:
				return NSImageInterpolation.High;
			default:
				throw new NotSupportedException();
			}
		}
		
		public static ImageInterpolation ToEto (this NSImageInterpolation value)
		{
			switch (value) {
			case NSImageInterpolation.None:
				return ImageInterpolation.None;
			case NSImageInterpolation.Low:
				return ImageInterpolation.Low;
			case NSImageInterpolation.Medium:
				return ImageInterpolation.Medium;
			case NSImageInterpolation.Default:
			case NSImageInterpolation.High:
				return ImageInterpolation.High;
			default:
				throw new NotSupportedException();
			}
		}
	}
}

