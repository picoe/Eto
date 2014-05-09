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

namespace Eto.Android.Drawing
{
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
	}
}