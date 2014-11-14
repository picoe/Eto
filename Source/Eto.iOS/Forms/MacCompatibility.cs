using System;
using MonoTouch.UIKit;
using sd = System.Drawing;

namespace Eto.Mac.Forms
{
	public static class MacCompatibility
	{
		public static void SetFrameSize(this UIView view, sd.SizeF size)
		{
			var frame = view.Frame;
			frame.Size = size;
			view.Frame = frame;
		}

		public static void SetFrameOrigin(this UIView view, sd.PointF location)
		{
			var frame = view.Frame;
			frame.Location = location;
			view.Frame = frame;
		}
	}
}

