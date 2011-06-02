using System;
using SD = System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class GraphicsHandler : IGraphics
	{
		wx.DC dc;

		public GraphicsHandler(wx.DC dc)
		{
			this.dc = dc;
		}

		#region IGraphics Members

		public void FillRectangle(SD.Color color, int x, int y, int width, int height)
		{
			dc.Brush = new wx.Brush(new wx.Colour(color.R, color.G, color.B));
			dc.DrawRectangle(x, y, x+width, y+height);
//			graphics.FillRectangle(new SD.SolidBrush(color), x, y, width, height);
		}

		public void DrawImage(Image image, int x, int y)
		{
			dc.DrawBitmap((wx.Bitmap)image.ControlObject, x, y);
//			graphics.DrawImageUnscaled((SD.Image)image.ControlObject, x, y);
		}

		public void DrawImage(Image image, int x, int y, int width, int height)
		{
			wx.Bitmap bmp = (wx.Bitmap)image.ControlObject;
			bmp = bmp.GetSubBitmap(new SD.Rectangle(0, 0, width, height));
			dc.DrawBitmap(bmp, x, y);
			bmp.Dispose();
		}

		public void DrawImage(Image image, SD.Rectangle source, SD.Rectangle destination)
		{
//			graphics.DrawImage((SD.Image)image.ControlObject, destination, source, SD.GraphicsUnit.Pixel);
		}

		#endregion

		#region IWidget Members

		public object ControlObject
		{
			get	{ return dc; }
		}

		public void Initialize()
		{
		}

		#endregion
	}
}
