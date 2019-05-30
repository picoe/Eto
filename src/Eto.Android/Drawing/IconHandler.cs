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
			throw new NotImplementedException();
		}
		public object Create(IconFrame frame, Func<System.IO.Stream> load)
		{
			throw new NotImplementedException();
		}
		public object Create(IconFrame frame, Bitmap bitmap)
		{
			throw new NotImplementedException();
		}
		public Bitmap GetBitmap(IconFrame frame)
		{
			throw new NotImplementedException();
		}
		public Size GetPixelSize(IconFrame frame)
		{
			throw new NotImplementedException();
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

		public ag.Bitmap GetImageWithSize(int? size)
		{
			throw new NotImplementedException();
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
			this.frames = frames.ToList();
			Control = Widget.GetFrame(1f).Bitmap.ToAndroid();
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
	}
}