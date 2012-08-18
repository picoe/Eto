using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public enum ImageInterpolation
	{
		Default,
		None,
		Low,
		Medium,
		High
	}

	public interface IGraphics : IInstanceWidget
	{
		void CreateFromImage (Bitmap image);

		void DrawRectangle (Color color, int x, int y, int width, int height);

		void DrawLine (Color color, int startx, int starty, int endx, int endy);

		void FillRectangle (Color color, int x, int y, int width, int height);
		
		void FillPath (Color color, GraphicsPath path);

		void DrawPath (Color color, GraphicsPath path);
		
		void DrawImage (Image image, int x, int y);

		void DrawImage (Image image, int x, int y, int width, int height);

		void DrawImage (Image image, Rectangle source, Rectangle destination);

		void DrawIcon (Icon icon, int x, int y, int width, int height);

		void DrawText (Font font, Color color, int x, int y, string text);

		SizeF MeasureString (Font font, string text);

		Region ClipRegion { get; set; }

		void Flush ();
		
		bool Antialias { get; set; }

		ImageInterpolation ImageInterpolation { get; set; }
	}

	public abstract class Region
	{
		public abstract object ControlObject { get; }

		public abstract void Exclude (Rectangle rect);

		public abstract void Reset ();

		public abstract void Set (Rectangle rect);
	}
	
	public class Graphics : InstanceWidget
	{
		IGraphics handler;

		public Graphics (Generator g, IGraphics handler) : base(g, handler)
		{
			this.handler = handler;
		}

		public Graphics (Bitmap image)
			: this(image.Generator, image)
		{
		}

		public Graphics (Generator g, Bitmap image) : base(g, typeof(IGraphics))
		{
			this.handler = (IGraphics)Handler;
			this.handler.CreateFromImage (image);
		}

		public void DrawLine (Color color, Point start, Point end)
		{
			handler.DrawLine (color, start.X, start.Y, end.X, end.Y);
		}
		
		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			handler.DrawLine (color, startx, starty, endx, endy);
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			handler.DrawRectangle (color, x, y, width, height);
		}

		public void DrawRectangle (Color color, Rectangle rectangle)
		{
			handler.DrawRectangle (color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		
		public void DrawInsetRectangle (Color topLeftColor, Color bottomRightColor, Rectangle rectangle)
		{
			DrawLine (topLeftColor, rectangle.TopLeft, rectangle.TopRight);
			DrawLine (topLeftColor, rectangle.TopLeft, rectangle.BottomLeft);
			DrawLine (bottomRightColor, rectangle.BottomLeft, rectangle.BottomRight);
			DrawLine (bottomRightColor, rectangle.TopRight, rectangle.BottomRight);
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			handler.FillRectangle (color, x, y, width, height);
		}

		public void FillRectangle (Color color, Rectangle rectangle)
		{
			handler.FillRectangle (color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public void FillRectangles (Color color, IEnumerable<Rectangle> rectangles)
		{
			foreach (Rectangle rectangle in rectangles) {
				handler.FillRectangle (color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			}
		}
		
		public void FillPolygon (Color color, IEnumerable<Point> points)
		{
			var path = new GraphicsPath (Generator);
			path.AddLines (points);
			FillPath (color, path);
		}

		public void DrawPolygon (Color color, IEnumerable<Point> points)
		{
			var path = new GraphicsPath (Generator);
			path.AddLines (points);
			DrawPath (color, path);
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
			handler.FillPath (color, path);
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			handler.DrawPath (color, path);
		}
		
		public void DrawImage (Image image, Point point)
		{
			handler.DrawImage (image, point.X, point.Y);
		}

		public void DrawImage (Image image, int x, int y)
		{
			handler.DrawImage (image, x, y);
		}

		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			handler.DrawImage (image, x, y, width, height);
		}

		public void DrawImage (Image image, Rectangle rect)
		{
			handler.DrawImage (image, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImage (Image image, Rectangle source, Point destination)
		{
			handler.DrawImage (image, source, new Rectangle (destination, source.Size));
		}

		public void DrawImage (Image image, Rectangle source, Rectangle destination)
		{
			handler.DrawImage (image, source, destination);
		}

		public void DrawIcon (Icon icon, Rectangle rectangle)
		{
			handler.DrawIcon (icon, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			handler.DrawIcon (icon, x, y, width, height);
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			handler.DrawText (font, color, x, y, text);
		}

		public void DrawText (Font font, Color color, Point location, string text)
		{
			handler.DrawText (font, color, location.X, location.Y, text);
		}
		
		public SizeF MeasureString (Font font, string text)
		{
			return handler.MeasureString (font, text);
		}

		public bool Antialias {
			get { return handler.Antialias; }
			set { handler.Antialias = value; }
		}

		public Region ClipRegion {
			get { return handler.ClipRegion; }
			set { handler.ClipRegion = value; }
		}

		public ImageInterpolation ImageInterpolation {
			get { return handler.ImageInterpolation; }
			set { handler.ImageInterpolation = value; }
		}
		
		public void Flush ()
		{
			handler.Flush ();
		}
	}
}
