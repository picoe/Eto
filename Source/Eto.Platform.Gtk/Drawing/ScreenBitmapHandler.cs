using System;
using System.IO;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	
	public class ScreenBitmapHandler : ImageHandler<Gdk.Pixmap, ScreenBitmap>, IScreenBitmap
	{
		Size size;
		
		
		
		#region IIcon Members
		
		public void Create(Bitmap bitmap)
		{
			size = bitmap.Size;
			Gdk.Pixbuf pb = ((BitmapHandler)bitmap.Handler).Control;
			Gdk.Pixmap mask;
			Gdk.Pixmap control;
			pb.RenderPixmapAndMask(out control, out mask, 0);
			Control = control;
			//Console.WriteLine("hello!");
			if (mask != null) mask.Dispose();
			
		}
		
		#endregion
		
		public override Size Size
		{
			get { return size; }
		}
		
		public override void SetImage (Gtk.Image imageView)
		{
			
		}
		
		public override void DrawImage(GraphicsHandler graphics, int x, int y)
		{
			//graphics.Control.DrawDrawable(graphics.GC, Control, 0, 0, x, y, size.Width, size.Height);
		}
		
		public override void DrawImage(GraphicsHandler graphics, int x, int y, int width, int height)
		{
			//graphics.Control.DrawDrawable(graphics.GC, Control, 0, 0, x, y, width, height);
		}
		
		public override void DrawImage(GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			/*
			if (source.Width != destination.Width || source.Height != destination.Height)
			{
				throw new NotSupportedException("Cannot scale portions of screen bitmaps");
			}
			graphics.Control.DrawDrawable(graphics.GC, Control, source.X, source.Y, destination.X, destination.Y, destination.Width, destination.Height);
			*/
		}
		
	}
}
