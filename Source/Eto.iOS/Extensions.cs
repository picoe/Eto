using System;
using MonoTouch.UIKit;
using System.Drawing;
using Eto.Forms;
using Eto.iOS.Forms;
using MonoTouch.Foundation;

namespace Eto.iOS
{
	public static class Extensions
	{
		public static void SetFrameSize(this UIView view, SizeF size)
		{
			var frame = view.Frame;
			frame.Size = size;
			view.Frame = frame;
		}

		public static void SetFrameOrigin(this UIView view, PointF location)
		{
			var frame = view.Frame;
			frame.Location = location;
			view.Frame = frame;
		}

		public static NSUrl ToNSUrl(this Uri uri)
		{
			if (uri == null)
				return null;
			else
				return new NSUrl(uri.AbsoluteUri);
		}

		public static Uri ToUri(this NSUrl url)
		{
			if (url == null)
				return null;
			else
				return new Uri(url.AbsoluteString);
		}
	}
}

