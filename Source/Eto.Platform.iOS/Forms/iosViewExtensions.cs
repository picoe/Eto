using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public static class MacViewExtensions
	{
		public static UIView GetContainerView (this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IiosContainer;
			if (containerHandler != null)
				return containerHandler.ContentControl;
			return control.ControlObject as UIView;
		}

		public static Size GetPreferredSize(this Control control)
		{
			if (control == null)
				return Size.Empty;
			var mh = control.Handler as IiosView;
			if (mh != null) {
				var size = mh.PreferredSize;
				if (size != null)
					return size.Value;
			}
			
			var c = control.ControlObject as UIView;
			if (c != null) {
				return Generator.ConvertF (c.SizeThatFits(UIView.UILayoutFittingCompressedSize));
			}
			return Size.Empty;
		}

	}
}

