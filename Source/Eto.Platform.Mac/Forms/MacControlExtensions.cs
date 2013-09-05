using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms
{
	public static class MacControlExtensions
	{
		public static Size GetPreferredSize (this Control view, Size availableSize)
		{
			if (view == null)
				return Size.Empty;
			var mh = view.Handler as IMacAutoSizing;
			if (mh != null) {
				return mh.GetPreferredSize (availableSize);
			}
			
			var c = view.ControlObject as NSControl;
			if (c != null) {
				c.SizeToFit ();
				return c.Frame.Size.ToEtoSize ();
			}
			return Size.Empty;
		}
	}
}

