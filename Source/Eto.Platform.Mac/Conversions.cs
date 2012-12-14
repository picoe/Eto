using System;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using Eto.Drawing;
using MonoMac.Foundation;
using Eto.Forms;
using Eto.Platform.Mac.Drawing;

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
			return new CGColor (cs.ColorSpace, components);
		}
		
		public static NSRange ToNS (this Range range)
		{
			return new NSRange (range.Start, range.Length);
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
				throw new NotSupportedException ();
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
				throw new NotSupportedException ();
			}
		}

		public static NSFontTraitMask ToNS (this FontStyle style)
		{
			var traits = (NSFontTraitMask)0;
			traits |= style.HasFlag (FontStyle.Bold) ? NSFontTraitMask.Bold : NSFontTraitMask.Unbold;
			traits |= style.HasFlag (FontStyle.Italic) ? NSFontTraitMask.Italic : NSFontTraitMask.Unitalic;
			//if (style.HasFlag (FontStyle.Condensed)) traits |= NSFontTraitMask.Condensed;
			//if (style.HasFlag (FontStyle.Light)) traits |= NSFontTraitMask.Narrow;
			return traits;
		}
		
		public static FontStyle ToEto (this NSFontTraitMask traits)
		{
			var style = FontStyle.Normal;
			if (traits.HasFlag (NSFontTraitMask.Bold))
				style |= FontStyle.Bold;
			if (traits.HasFlag (NSFontTraitMask.Italic))
				style |= FontStyle.Italic;
			//if (traits.HasFlag (NSFontTraitMask.Condensed))
			//	style |= FontStyle.Condensed;
			//if (traits.HasFlag (NSFontTraitMask.Narrow))
			//	style |= FontStyle.Light;
			return style;
		}

		public static NSPrintingOrientation ToNS (this PageOrientation value)
		{
			switch (value) {
			case PageOrientation.Landscape:
				return NSPrintingOrientation.Landscape;
			case PageOrientation.Portrait:
				return NSPrintingOrientation.Portrait;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PageOrientation ToEto (this NSPrintingOrientation value)
		{
			switch (value) {
			case NSPrintingOrientation.Landscape:
				return PageOrientation.Landscape;
			case NSPrintingOrientation.Portrait:
				return PageOrientation.Portrait;
			default:
				throw new NotSupportedException ();
			}
		}

		public static Point GetLocation (NSView view, NSEvent theEvent)
		{
			var loc = view.ConvertPointFromView (theEvent.LocationInWindow, null);
			if (!view.IsFlipped)
				loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEtoPoint ();
		}

		public static MouseEventArgs GetMouseEvent (NSView view, NSEvent theEvent)
		{
			var pt = Conversions.GetLocation (view, theEvent);
			Key modifiers = KeyMap.GetModifiers (theEvent);
			MouseButtons buttons = KeyMap.GetMouseButtons (theEvent);
			return new MouseEventArgs (buttons, modifiers, pt);
		}

		public static void SetSizeWithAuto (NSView view, Size size)
		{
			var newSize = view.Frame.Size;
			if (size.Width >= 0)
				newSize.Width = size.Width;
			if (size.Height >= 0)
				newSize.Height = size.Height;
			view.SetFrameSize (newSize);
		}

		public static CGAffineTransform ToCG (this IMatrix matrix)
		{
			return (CGAffineTransform)matrix.ControlObject;
		}

		public static float DegreesToRadians (float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

	}
}

