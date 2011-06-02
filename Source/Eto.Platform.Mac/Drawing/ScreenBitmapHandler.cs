using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Drawing
{
	
	public class ScreenBitmapHandler : ImageHandler<NSImage, ScreenBitmap>, IScreenBitmap
	{
		Size size;
		
		public ScreenBitmapHandler()
		{
		}
		
		
		#region IIcon Members
		
		public void Create(Bitmap bitmap)
		{
			size = bitmap.Size;
			//Gdk.Pixbuf pb = ((BitmapHandler)bitmap.InnerControl).Pixbuf;
			//Gdk.Pixmap mask;
			//pb.RenderPixmapAndMask(out control, out mask, 0);
			//Console.WriteLine("hello!");
			//if (mask != null) mask.Dispose();
			
		}
		
		#endregion
		
		public override Size Size
		{
			get { return size; }
		}
		
		public override void DrawImage(GraphicsHandler graphics, int x, int y)
		{
			//graphics.Drawable.DrawDrawable(graphics.GC, control, 0, 0, x, y, size.Width, size.Height);
		}
		
		public override void DrawImage(GraphicsHandler graphics, int x, int y, int width, int height)
		{
			//graphics.Drawable.DrawDrawable(graphics.GC, control, 0, 0, x, y, width, height);
		}
		
		public override void DrawImage(GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			if (source.Width != destination.Width || source.Height != destination.Height)
			{
				throw new NotSupportedException("Cannot scale portions of screen bitmaps");
			}
			//graphics.Drawable.DrawDrawable(graphics.GC, control, source.X, source.Y, destination.X, destination.Y, destination.Width, destination.Height);
		}
		
		
	}
}
