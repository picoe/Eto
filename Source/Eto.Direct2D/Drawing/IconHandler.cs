using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
#if WINFORMS
using Eto.WinForms.Drawing;
#endif

namespace Eto.Direct2D.Drawing
{
	public class IconHandler : ImageHandler<Icon>, Icon.IHandler
#if WINFORMS
		, IWindowsIconSource
#endif
    {
#if WINFORMS
		System.Drawing.Icon sdicon;
		public System.Drawing.Icon GetIcon()
		{
			if (sdicon == null && Frames != null)
			{
				// TODO: Convert each bitmap in Frames to a single icon
				sdicon = System.Drawing.Icon.FromHandle(Control.ToBitmap().ToSD().GetHicon());
			}
			return sdicon;
		}

		public override void Reset()
		{
			base.Reset();
			if (sdicon != null)
			{
				sdicon.Dispose();
				sdicon = null;
			}
		}
#endif
	}
}
