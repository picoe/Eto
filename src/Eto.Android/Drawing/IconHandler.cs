using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Eto.Forms;

namespace Eto.Android.Drawing
{
	class IconFrameHandler : IconFrame.IHandler
	{
		public object Create(IconFrame frame, System.IO.Stream stream)
		{
			return ag.BitmapFactory.DecodeStream(stream);
			throw new NotImplementedException();
		}
		public object Create(IconFrame frame, Func<System.IO.Stream> load)
		{
			return ag.BitmapFactory.DecodeStream(load());
			throw new NotImplementedException();
		}
		public object Create(IconFrame frame, Bitmap bitmap)
		{
			return bitmap.ToAndroid();
			throw new NotImplementedException();
		}
		public Bitmap GetBitmap(IconFrame frame)
		{
			return new Bitmap(new BitmapHandler(((ag.Bitmap)frame.ControlObject)));
			}
		public Size GetPixelSize(IconFrame frame)
		{
			var agbitmap = (ag.Bitmap)frame.ControlObject;
			return new Size(agbitmap.Width, agbitmap.Height);
		}
	}

	class IconHandler : WidgetHandler<ag.Bitmap, Icon>, Icon.IHandler, IAndroidImage
	{
		public void Create(System.IO.Stream stream)
		{
			Control = ag.BitmapFactory.DecodeStream(stream);
		}

		public void Create(string fileName)
		{
			Control = ag.BitmapFactory.DecodeFile(fileName);
		}
		public Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
		}

		public ag.Bitmap GetImageWithDensity(int? density)
		{
			if (density.HasValue)
				Control.Density = density.Value;

			return Control;
		}

		public void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			graphics.Control.DrawBitmap(Control, new Rectangle(source).ToAndroid(), destination.ToAndroid(), paint: null);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			graphics.Control.DrawBitmap(Control, x, y, paint: null);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			graphics.Control.DrawBitmap(Control, null, new RectangleF(x, y, width, height).ToAndroid(), paint: null);
		}

		public void Create(IEnumerable<IconFrame> frames)
		{
			//Control = Widget.GetFrame(1f).Bitmap.ToAndroid();
			this.frames = frames.ToList();
			var frame = GetIdealIcon();
//			size = frame.Size;
			Control = (ag.Bitmap)frame.ControlObject;
		}

		List<IconFrame> frames;

		public IEnumerable<IconFrame> Frames
		{
			get
			{
				if (frames == null)
					frames = new List<IconFrame> { new IconFrame(1f, new Bitmap(new BitmapHandler(Control))) };
				return frames;
			}
		}

		public IconFrame GetIdealIcon()
		{
			IconFrame idealFrame = null;

			if (idealFrame != null)
				return idealFrame;

			var orderedFrames = frames.OrderByDescending(r => r.PixelSize.Width * r.PixelSize.Height);
			idealFrame = orderedFrames.FirstOrDefault(r => r.Scale == 1) ?? orderedFrames.First();
			return idealFrame;
		}
	}
}
