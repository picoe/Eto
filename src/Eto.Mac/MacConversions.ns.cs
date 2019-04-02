using System;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac
{
	public static partial class MacConversions
	{
		static readonly DateTime ReferenceDate = new DateTime(2001, 1, 1, 0, 0, 0);

		public static NSUrl ToNS(this Uri uri)
		{
			return uri == null ? null : new NSUrl(uri.AbsoluteUri);
		}

		public static NSDate ToNS(this DateTime date)
		{
			return NSDate.FromTimeIntervalSinceReferenceDate((date.ToUniversalTime() - ReferenceDate).TotalSeconds);
		}

		public static NSDate ToNS(this DateTime? date)
		{
			return date == null ? null : date.Value.ToNS();
		}

		public static DateTime? ToEto(this NSDate date)
		{
			if (date == null)
				return null;
			return new DateTime((long)(date.SecondsSinceReferenceDate * (double)TimeSpan.TicksPerSecond + (double)ReferenceDate.Ticks), DateTimeKind.Utc).ToLocalTime();
		}

		public static NSColor ToNS(this CGColor color)
		{
			if (color == null)
				return null;

			if (MacVersion.IsAtLeast(10, 8))
				return NSColor.FromCGColor(color);

			return NSColor.FromColorSpace(new NSColorSpace(color.ColorSpace), color.Components);
		}

	}
}