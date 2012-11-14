using System;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public static partial class Conversions
	{
		static DateTime ReferenceDate = new DateTime (2001, 1, 1, 0, 0, 0);
		
		public static NSUrl ToNS (this Uri uri)
		{
			if (uri == null) return null;
			return new NSUrl(uri.AbsoluteUri);
		}

		public static NSDate ToNS (this DateTime date)
		{
			return NSDate.FromTimeIntervalSinceReferenceDate ((date.ToUniversalTime () - ReferenceDate).TotalSeconds);
		}
		
		public static NSDate ToNS (this DateTime? date)
		{
			if (date == null) return null;
			return date.Value.ToNS ();
		}
		
		public static DateTime? ToEto (this NSDate date)
		{
			if (date == null) return null;
			return new DateTime ((long)(date.SecondsSinceReferenceDate * TimeSpan.TicksPerSecond + ReferenceDate.Ticks), DateTimeKind.Utc).ToLocalTime ();
		}
		
	}
}

