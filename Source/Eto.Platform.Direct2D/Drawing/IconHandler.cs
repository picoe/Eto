using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Direct2D.Drawing
{
    public class IconHandler : ImageHandler<Icon>, IIcon, IWindowsIconSource
    {
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
	}
}
