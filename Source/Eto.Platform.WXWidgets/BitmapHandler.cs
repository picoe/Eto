using System;
using System.IO;
using SD = System.Drawing;

namespace Eto.Forms.WXWidgets
{


	internal class BitmapHandler : WidgetHandler, IBitmap
	{
		wx.Bitmap control;

		public BitmapHandler(Widget widget) : base(widget)
		{
			control = null;
		}

		public override object ControlObject
		{
			get	{ return control; }
		}

		#region IBitmap Members

		public void Create(string fileName)
		{
			control = new wx.Bitmap(fileName);
		}

		public void Create(Stream stream)
		{
			throw new NotImplementedException("Cannot load bitmaps from a stream in wx");
		}

		public void Create(int width, int height)
		{
			control = new wx.Bitmap(width, height);
		}

		public SD.Size Size
		{
			get { return new SD.Size(control.Width, control.Height); }
		}

		public void Resize(int width, int height)
		{
			//wx.Image img = control.ConvertToImage();
			//img = img.Scale(width, height);
			//control = new wx.Bitmap(img);
		}

		#endregion
	}
}
