using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using MonoMac.AppKit;
using Eto.Platform.Mac.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Eto.Platform.Mac.IO;
using System.Threading;

namespace Eto.Platform.Mac
{
	public class Generator : Eto.Generator
	{ 	
		public override string ID {
			get { return Generators.Mac; }
		}
		
		public static Point GetLocation (NSView view, NSEvent theEvent)
		{
			var loc = view.ConvertPointFromView (theEvent.LocationInWindow, null);
			if (!view.IsFlipped)
				loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEtoPoint ();
		}
		
		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}


		public static MouseEventArgs GetMouseEvent (NSView view, NSEvent theEvent)
		{
			var pt = Generator.GetLocation (view, theEvent);
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
		
		static DateTime ReferenceDate = new DateTime (2001, 1, 1, 0, 0, 0);

		public static NSDate Convert (DateTime date)
		{
			return NSDate.FromTimeIntervalSinceReferenceDate ((date.ToUniversalTime () - ReferenceDate).TotalSeconds);
		}
		
		public static NSDate Convert (DateTime? date)
		{
			if (date == null) return null;
			return Convert (date.Value);
		}
		
		public static DateTime? Convert (NSDate date)
		{
			if (date == null) return null;
			return new DateTime ((long)(date.SecondsSinceReferenceDate * TimeSpan.TicksPerSecond + ReferenceDate.Ticks), DateTimeKind.Utc).ToLocalTime ();
		}

		public static NSImageInterpolation Convert (ImageInterpolation value)
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

		public static ImageInterpolation Convert (NSImageInterpolation value)
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

		public static CGInterpolationQuality ConvertCG (ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.Default:
				return CGInterpolationQuality.Default;
			case ImageInterpolation.None:
				return CGInterpolationQuality.None;
			case ImageInterpolation.Low:
				return CGInterpolationQuality.Low;
			case ImageInterpolation.Medium:
				return CGInterpolationQuality.Medium;
			case ImageInterpolation.High:
				return CGInterpolationQuality.High;
			default:
				throw new NotSupportedException();
			}
		}

		public static ImageInterpolation ConvertCG (CGInterpolationQuality value)
		{
			switch (value) {
			case CGInterpolationQuality.Default:
				return ImageInterpolation.Default;
			case CGInterpolationQuality.None:
				return ImageInterpolation.None;
			case CGInterpolationQuality.Low:
				return ImageInterpolation.Low;
			case CGInterpolationQuality.Medium:
				return ImageInterpolation.Medium;
			case CGInterpolationQuality.High:
				return ImageInterpolation.High;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
