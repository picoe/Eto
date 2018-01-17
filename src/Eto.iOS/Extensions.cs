using System;
using UIKit;
using CoreGraphics;
using Eto.Forms;
using Eto.iOS.Forms;
using Foundation;
using ObjCRuntime;

namespace Eto.iOS
{
	public static class Extensions
	{
		public static void SetFrameSize(this UIView view, CGSize size)
		{
			var frame = view.Frame;
			frame.Size = size;
			view.Frame = frame;
		}

		public static void SetFrameOrigin(this UIView view, CGPoint location)
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

		static readonly Selector selAutomaticallyAdjustsScrollViewInsets = new Selector("automaticallyAdjustsScrollViewInsets");

		public static bool AutomaticallyAdjustsScrollViewInsetsIsSupported(this UIViewController controller)
		{
			return controller.RespondsToSelector(selAutomaticallyAdjustsScrollViewInsets);
		}

		static readonly Selector selExtendLayoutIncludesOpaqueBars = new Selector("extendedLayoutIncludesOpaqueBars");

		public static bool ExtendLayoutIncludesOpaqueBarsIsSupported(this UIViewController controller)
		{
			return controller.RespondsToSelector(selExtendLayoutIncludesOpaqueBars);
		}

		static readonly Selector selEdgesForExtendedLayout = new Selector("edgesForExtendedLayout");

		public static bool EdgesForExtendedLayoutIsSupported(this UIViewController controller)
		{
			return controller.RespondsToSelector(selEdgesForExtendedLayout);
		}
	}
}

