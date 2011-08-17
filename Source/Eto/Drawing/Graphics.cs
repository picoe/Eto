using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public interface IGraphics : IWidget
	{
		void CreateFromImage(Bitmap image);
		void DrawRectangle(Color color, int x, int y, int width, int height);
		void DrawLine(Color color, int startx, int starty, int endx, int endy);
		void FillRectangle(Color color, int x, int y, int width, int height);
		
		void FillPath(Color color, GraphicsPath path);
		void DrawPath(Color color, GraphicsPath path);
		
		void DrawImage(IImage image, int x, int y);
		void DrawImage(IImage image, int x, int y, int width, int height);
		void DrawImage(IImage image, Rectangle source, Rectangle destination);
		void DrawIcon(Icon icon, int x, int y, int width, int height);
		void DrawText(Font font, Color color, int x, int y, string text);
		SizeF MeasureString(Font font, string text);
		Region ClipRegion { get; set; }
		void Flush();
		
		bool Antialias { get; set; }
	}

	public abstract class Region
	{
		
		public abstract object ControlObject { get; }
		public abstract void Exclude(Rectangle rect);
		public abstract void Reset();
		public abstract void Set(Rectangle rect);
	}
	
	public class Graphics : Widget
	{
		IGraphics inner;

		public Graphics(Generator g, IGraphics inner) : base(g, inner)
		{
			this.inner = inner;
		}

		public Graphics(Generator g, Bitmap image) : base(g, typeof(IGraphics))
		{
			inner = (IGraphics)Handler;
			inner.CreateFromImage(image);
		}

		public void DrawLine(Color color, Point start, Point end)
		{
			inner.DrawLine(color, start.X, start.Y, end.X, end.Y);
		}
		
		public void DrawLine(Color color, int startx, int starty, int endx, int endy)
		{
			inner.DrawLine(color, startx, starty, endx, endy);
		}

		public void DrawRectangle(Color color, int x, int y, int width, int height)
		{
			inner.DrawRectangle(color, x, y, width, height);
		}

		public void DrawRectangle(Color color, Rectangle rectangle)
		{
			inner.DrawRectangle(color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		
		public void DrawInsetRectangle(Color topLeftColor, Color bottomRightColor, Rectangle rectangle)
		{
			DrawLine(topLeftColor, rectangle.TopLeft, rectangle.TopRight);
			DrawLine(topLeftColor, rectangle.TopLeft, rectangle.BottomLeft);
			DrawLine(bottomRightColor, rectangle.BottomLeft, rectangle.BottomRight);
			DrawLine(bottomRightColor, rectangle.TopRight, rectangle.BottomRight);
		}

		public void FillRectangle(Color color, int x, int y, int width, int height)
		{
			inner.FillRectangle(color, x, y, width, height);
		}

		public void FillRectangle(Color color, Rectangle rectangle)
		{
			inner.FillRectangle(color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public void FillRectangles(Color color, IEnumerable<Rectangle> rectangles)
		{
			foreach (Rectangle rectangle in rectangles)
			{
				inner.FillRectangle(color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			}
		}
		
		public void FillPolygon(Color color, IEnumerable<Point> points)
		{
			var path = new GraphicsPath(Generator);
			path.AddLines (points);
			FillPath (color, path);
		}

		public void DrawPolygon(Color color, IEnumerable<Point> points)
		{
			var path = new GraphicsPath(Generator);
			path.AddLines (points);
			DrawPath (color, path);
		}
		
		public void FillPath(Color color, GraphicsPath path)
		{
			inner.FillPath(color, path);
		}

		public void DrawPath(Color color, GraphicsPath path)
		{
			inner.DrawPath(color, path);
		}
		
		public void DrawImage(IImage image, Point point)
		{
			inner.DrawImage(image, point.X, point.Y);
		}

		public void DrawImage(IImage image, int x, int y)
		{
			inner.DrawImage(image, x, y);
		}

		public void DrawImage(IImage image, int x, int y, int width, int height)
		{
			inner.DrawImage(image, x, y, width, height);
		}

		public void DrawImage(IImage image, Rectangle rect)
		{
			inner.DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImage(IImage image, Rectangle source, Point destination)
		{
			inner.DrawImage(image, source, new Rectangle(destination, source.Size));
		}

		public void DrawImage(IImage image, Rectangle source, Rectangle destination)
		{
			inner.DrawImage(image, source, destination);
		}

		public void DrawIcon(Icon icon, Rectangle rectangle)
		{
			inner.DrawIcon(icon, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public void DrawIcon(Icon icon, int x, int y, int width, int height)
		{
			inner.DrawIcon(icon, x, y, width, height);
		}

		public void DrawText(Font font, Color color, int x, int y, string text)
		{
			inner.DrawText(font, color, x, y, text);
		}
		
		public SizeF MeasureString(Font font, string text)
		{
			return inner.MeasureString(font, text);
		}

		public bool Antialias
		{
			get { return inner.Antialias; }
			set { inner.Antialias = value; }
		}

		public Region ClipRegion
		{
			get { return inner.ClipRegion; }
			set { inner.ClipRegion = value; }
		}
		
		public void Flush()
		{
			inner.Flush();
		}
	}
}
