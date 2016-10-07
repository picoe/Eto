using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using System.IO;
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
		List<IconFrame> iconFrames;
		IEnumerable<IconFrame> Icon.IHandler.Frames
		{
			get
			{
				return iconFrames;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			if (iconFrames == null)
			{
				// create frames
				iconFrames = Frames.Select(r => IconFrame.FromControlObject(1f, new Bitmap(new BitmapHandler(r)))).ToList();
			}
		}

		public void Create(IEnumerable<IconFrame> frames)
		{
			iconFrames = frames.ToList();
			Frames = iconFrames.Select(r => r.Bitmap.ToWic()).ToArray();
			var sortedFrames = iconFrames.OrderByDescending(r => r.PixelSize.Width * r.PixelSize.Height);
			var frame = sortedFrames.FirstOrDefault(r => r.Scale == 1) ?? sortedFrames.First();
			Control = frame.Bitmap.ToWic();
		}

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
