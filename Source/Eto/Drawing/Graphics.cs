using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	/// <summary>
	/// Interpolation modes when drawing images using the <see cref="Graphics"/> object
	/// </summary>
	/// <seealso cref="Graphics.ImageInterpolation"/>
	public enum ImageInterpolation
	{
		/// <summary>
		/// Default interplation mode - usually a balance between quality vs. performance
		/// </summary>
		Default,

		/// <summary>
		/// No interpolation (also known as nearest neighbour)
		/// </summary>
		None,

		/// <summary>
		/// Low interpolation quality (usually fastest)
		/// </summary>
		Low,

		/// <summary>
		/// Medium interpolation quality slower than <see cref="Low"/>, but better quality
		/// </summary>
		Medium,

		/// <summary>
		/// Highest interpolation quality - slowest but best quality
		/// </summary>
		High
	}

	/// <summary>
	/// Platform handler interface for the <see cref="Graphics"/> class
	/// </summary>
	public interface IGraphics : IInstanceWidget
	{
		/// <summary>
		/// Creates the graphics object for drawing on the specified <paramref name="image"/>
		/// </summary>
		/// <param name="image">Image to perform drawing operations on</param>
		void CreateFromImage (Bitmap image);

		/// <summary>
		/// Draws a rectangle outline
		/// </summary>
		/// <param name="color">Color for the outline</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		void DrawRectangle (Color color, int x, int y, int width, int height);

		/// <summary>
		/// Draws a line with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color for the outline</param>
		/// <param name="startx">X co-ordinate of the starting point</param>
		/// <param name="starty">Y co-ordinate of the starting point</param>
		/// <param name="endx">X co-ordinate of the ending point</param>
		/// <param name="endy">Y co-ordinate of the ending point</param>
		void DrawLine (Color color, int startx, int starty, int endx, int endy);

		/// <summary>
		/// Fills a rectangle with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Fill color</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		void FillRectangle (Color color, int x, int y, int width, int height);
		
		/// <summary>
		/// Fills the specified <paramref name="path"/>
		/// </summary>
		/// <param name="color">Fill color</param>
		/// <param name="path">Path to fill</param>
		void FillPath (Color color, GraphicsPath path);

		/// <summary>
		/// Draws the specified <paramref name="path"/>
		/// </summary>
		/// <param name="color">Draw color</param>
		/// <param name="path">Path to draw</param>
		void DrawPath (Color color, GraphicsPath path);
		
		/// <summary>
		/// Draws the specified <paramref name="image"/> at a location with no scaling
		/// </summary>
		/// <param name="image">Image to draw</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		void DrawImage (Image image, int x, int y);

		/// <summary>
		/// Draws the specified <paramref name="image"/> in a rectangle
		/// </summary>
		/// <remarks>
		/// This will scale the image to the specified width and height using the <see cref="ImageInterpolation"/> mode
		/// </remarks>
		/// <param name="image">Image to draw</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		/// <param name="width">Destination width of the image to draw</param>
		/// <param name="height">Destination height of the image to draw</param>
		void DrawImage (Image image, int x, int y, int width, int height);

		/// <summary>
		/// Draws the <paramref name="source"/> portion of an <paramref name="image"/>, scaling to the specified <paramref name="destination"/>
		/// </summary>
		/// <param name="image">Image to draw</param>
		/// <param name="source">Source rectangle of the image portion to draw</param>
		/// <param name="destination">Destination rectangle of where to draw the portion</param>
		void DrawImage (Image image, Rectangle source, Rectangle destination);

		/// <summary>
		/// Draws the <paramref name="icon"/> at the specified location and size
		/// </summary>
		/// <param name="icon">Icon to draw</param>
		/// <param name="x">X co-ordinate of the location to draw the icon</param>
		/// <param name="y">Y co-ordinate of the location to draw the icon</param>
		/// <param name="width">Destination width of the icon</param>
		/// <param name="height">Destination height of the icon</param>
		void DrawIcon (Icon icon, int x, int y, int width, int height);

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="color"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="color">Color of the text</param>
		/// <param name="x">X co-ordinate of where to start drawing the text</param>
		/// <param name="y">Y co-ordinate of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		void DrawText (Font font, Color color, int x, int y, string text);

		/// <summary>
		/// Measures the string with the given <paramref name="font"/>
		/// </summary>
		/// <param name="font">Font to measure with</param>
		/// <param name="text">Text string to measure</param>
		/// <returns>Size representing the dimensions of the entire text would take to draw given the specified <paramref name="font"/></returns>
		SizeF MeasureString (Font font, string text);

		/// <summary>
		/// Not yet implemented
		/// </summary>
		Region ClipRegion { get; set; }

		/// <summary>
		/// Flushes the drawing (for some platforms)
		/// </summary>
		/// <remarks>
		/// Flushing the drawing will force any undrawn changes to be shown to the user.  Typically when you are doing
		/// a lot of drawing, you may want to flush the changed periodically so that the user does not think the UI is unresponsive.
		/// </remarks>
		void Flush ();
		
		/// <summary>
		/// Gets or sets a value indicating that drawing operations will use antialiasing
		/// </summary>
		bool Antialias { get; set; }

		/// <summary>
		/// Gets or sets the interpolation mode for drawing images
		/// </summary>
		ImageInterpolation ImageInterpolation { get; set; }
	}

	public abstract class Region
	{
		public abstract object ControlObject { get; }

		public abstract void Exclude (Rectangle rect);

		public abstract void Reset ();

		public abstract void Set (Rectangle rect);
	}
	
	/// <summary>
	/// Graphics context object for drawing operations
	/// </summary>
	/// <remarks>
	/// This class allows you to draw on either a <see cref="Bitmap"/> or a <see cref="T:Eto.Forms.Drawable"/> control.
	/// </remarks>
	public class Graphics : InstanceWidget
	{
		IGraphics handler;

		/// <summary>
		/// Initializes a new instance of the Graphics class with the specified platform <paramref name="handler"/>
		/// </summary>
		/// <param name="generator">Generator for this instance</param>
		/// <param name="handler">Platform handler to use for this instance</param>
		public Graphics (Generator generator, IGraphics handler) : base(generator, handler)
		{
			this.handler = handler;
		}

		/// <summary>
		/// Initializes a new instance of the Generator class to draw on the given <paramref name="image"/>
		/// </summary>
		/// <param name="image">Image to draw on using this graphics context</param>
		public Graphics (Bitmap image)
			: this(image.Generator, image)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Generator class to draw on the given <paramref name="image"/>
		/// </summary>
		/// <param name="generator">Generator to create this graphics context for</param>
		/// <param name="image">Image to draw on using this graphics context</param>
		public Graphics (Generator generator, Bitmap image)
			: base (generator, typeof (IGraphics))
		{
			this.handler = (IGraphics)Handler;
			this.handler.CreateFromImage (image);
		}

		/// <summary>
		/// Draws a line with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color for the outline</param>
		/// <param name="start">Starting location</param>
		/// <param name="end">Ending location</param>
		public void DrawLine (Color color, Point start, Point end)
		{
			handler.DrawLine (color, start.X, start.Y, end.X, end.Y);
		}

		/// <summary>
		/// Draws a line with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color for the outline</param>
		/// <param name="startx">X co-ordinate of the starting point</param>
		/// <param name="starty">Y co-ordinate of the starting point</param>
		/// <param name="endx">X co-ordinate of the ending point</param>
		/// <param name="endy">Y co-ordinate of the ending point</param>
		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			handler.DrawLine (color, startx, starty, endx, endy);
		}

		/// <summary>
		/// Draws a rectangle
		/// </summary>
		/// <param name="color">Color for the outline</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			handler.DrawRectangle (color, x, y, width, height);
		}

		/// <summary>
		/// Draws a rectangle
		/// </summary>
		/// <param name="color">Color for the outline</param>
		/// <param name="rectangle">Where to draw the rectangle</param>
		public void DrawRectangle (Color color, Rectangle rectangle)
		{
			handler.DrawRectangle (color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		
		/// <summary>
		/// Draws an rectangle with colors on the top/left and bottom/right with the given <paramref name="width"/>
		/// </summary>
		/// <param name="topLeftColor">Color for top/left edges</param>
		/// <param name="bottomRightColor">Color for bottom/right edges</param>
		/// <param name="rectangle">Outside of inset rectangle to draw</param>
		/// <param name="width">Width of the rectangle, in pixels</param>
		public void DrawInsetRectangle (Color topLeftColor, Color bottomRightColor, Rectangle rectangle, int width = 1)
		{
			for (int i = 0; i < width; i++) {
				DrawLine (topLeftColor, rectangle.TopLeft, rectangle.InnerTopRight);
				DrawLine (topLeftColor, rectangle.TopLeft, rectangle.InnerBottomLeft);
				DrawLine (bottomRightColor, rectangle.InnerBottomLeft, rectangle.InnerBottomRight);
				DrawLine (bottomRightColor, rectangle.InnerTopRight, rectangle.InnerBottomRight);
				rectangle.Inflate (-1, -1);
			}
		}

		/// <summary>
		/// Fills a rectangle with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Fill color</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			handler.FillRectangle (color, x, y, width, height);
		}

		/// <summary>
		/// Fills a rectangle with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Fill color</param>
		/// <param name="rectangle">Location for the rectangle</param>
		public void FillRectangle (Color color, Rectangle rectangle)
		{
			handler.FillRectangle (color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		/// <summary>
		/// Fills the specified <paramref name="rectangles"/>
		/// </summary>
		/// <param name="color">Color to fill the rectangles</param>
		/// <param name="rectangles">Enumeration of rectangles to fill</param>
		public void FillRectangles (Color color, IEnumerable<Rectangle> rectangles)
		{
			foreach (Rectangle rectangle in rectangles) {
				handler.FillRectangle (color, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			}
		}
		
		/// <summary>
		/// Fills a polygon defined by <paramref name="points"/> with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Fill color</param>
		/// <param name="points">Points of the polygon</param>
		public void FillPolygon (Color color, IEnumerable<Point> points)
		{
			var path = new GraphicsPath (Generator);
			path.AddLines (points);
			FillPath (color, path);
		}

		/// <summary>
		/// Draws a polygon with the specified <paramref name="points"/>
		/// </summary>
		/// <param name="color">Color to draw the polygon lines</param>
		/// <param name="points">Points of the polygon</param>
		public void DrawPolygon (Color color, IEnumerable<Point> points)
		{
			var path = new GraphicsPath (Generator);
			path.AddLines (points);
			DrawPath (color, path);
		}

		/// <summary>
		/// Fills the specified <paramref name="path"/>
		/// </summary>
		/// <param name="color">Fill color</param>
		/// <param name="path">Path to fill</param>
		public void FillPath (Color color, GraphicsPath path)
		{
			handler.FillPath (color, path);
		}

		/// <summary>
		/// Draws the specified <paramref name="path"/>
		/// </summary>
		/// <param name="color">Draw color</param>
		/// <param name="path">Path to draw</param>
		public void DrawPath (Color color, GraphicsPath path)
		{
			handler.DrawPath (color, path);
		}

		/// <summary>
		/// Draws the specified <paramref name="image"/> at a location with no scaling
		/// </summary>
		/// <param name="image">Image to draw</param>
		/// <param name="location">Location to draw the image</param>
		public void DrawImage (Image image, Point location)
		{
			handler.DrawImage (image, location.X, location.Y);
		}

		/// <summary>
		/// Draws the specified <paramref name="image"/> at a location with no scaling
		/// </summary>
		/// <param name="image">Image to draw</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		public void DrawImage (Image image, int x, int y)
		{
			handler.DrawImage (image, x, y);
		}

		/// <summary>
		/// Draws the specified <paramref name="image"/> in a rectangle
		/// </summary>
		/// <remarks>
		/// This will scale the image to the specified width and height using the <see cref="ImageInterpolation"/> mode
		/// </remarks>
		/// <param name="image">Image to draw</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		/// <param name="width">Destination width of the image to draw</param>
		/// <param name="height">Destination height of the image to draw</param>
		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			handler.DrawImage (image, x, y, width, height);
		}

		/// <summary>
		/// Draws the specified <paramref name="image"/> in a rectangle
		/// </summary>
		/// <remarks>
		/// This will scale the image to the specified width and height using the <see cref="ImageInterpolation"/> mode
		/// </remarks>
		/// <param name="image">Image to draw</param>
		/// <param name="rectangle">Where to draw the image</param>
		public void DrawImage (Image image, Rectangle rectangle)
		{
			handler.DrawImage (image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		/// <summary>
		/// Draws the <paramref name="source"/> portion of an <paramref name="image"/>, scaling to the specified <paramref name="destination"/>
		/// </summary>
		/// <param name="image">Image to draw</param>
		/// <param name="source">Source rectangle of the image portion to draw</param>
		/// <param name="destination">Destination rectangle of where to draw the portion</param>
		public void DrawImage (Image image, Rectangle source, Point destination)
		{
			handler.DrawImage (image, source, new Rectangle (destination, source.Size));
		}

		/// <summary>
		/// Draws the <paramref name="source"/> portion of an <paramref name="image"/>, scaling to the specified <paramref name="destination"/>
		/// </summary>
		/// <param name="image">Image to draw</param>
		/// <param name="source">Source rectangle of the image portion to draw</param>
		/// <param name="destination">Destination rectangle of where to draw the portion</param>
		public void DrawImage (Image image, Rectangle source, Rectangle destination)
		{
			handler.DrawImage (image, source, destination);
		}

		/// <summary>
		/// Draws the <paramref name="icon"/> at the specified location and size
		/// </summary>
		/// <param name="icon">Icon to draw</param>
		/// <param name="rectangle">Where to draw the icon</param>
		public void DrawIcon (Icon icon, Rectangle rectangle)
		{
			handler.DrawIcon (icon, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		/// <summary>
		/// Draws the <paramref name="icon"/> at the specified location and size
		/// </summary>
		/// <param name="icon">Icon to draw</param>
		/// <param name="x">X co-ordinate of the location to draw the icon</param>
		/// <param name="y">Y co-ordinate of the location to draw the icon</param>
		/// <param name="width">Destination width of the icon</param>
		/// <param name="height">Destination height of the icon</param>
		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			handler.DrawIcon (icon, x, y, width, height);
		}

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="color"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="color">Color of the text</param>
		/// <param name="x">X co-ordinate of where to start drawing the text</param>
		/// <param name="y">Y co-ordinate of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			handler.DrawText (font, color, x, y, text);
		}

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="color"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="color">Color of the text</param>
		/// <param name="location">Location of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		public void DrawText (Font font, Color color, Point location, string text)
		{
			handler.DrawText (font, color, location.X, location.Y, text);
		}

		/// <summary>
		/// Measures the string with the given <paramref name="font"/>
		/// </summary>
		/// <param name="font">Font to measure with</param>
		/// <param name="text">Text string to measure</param>
		/// <returns>Size representing the dimensions of the entire text would take to draw given the specified <paramref name="font"/></returns>
		public SizeF MeasureString (Font font, string text)
		{
			return handler.MeasureString (font, text);
		}

		/// <summary>
		/// Gets or sets a value indicating that drawing operations will use antialiasing
		/// </summary>
		public bool Antialias
		{
			get { return handler.Antialias; }
			set { handler.Antialias = value; }
		}

		/// <summary>
		/// Not yet implemented
		/// </summary>
		public Region ClipRegion {
			get { return handler.ClipRegion; }
			set { handler.ClipRegion = value; }
		}

		/// <summary>
		/// Gets or sets the interpolation mode for drawing images
		/// </summary>
		public ImageInterpolation ImageInterpolation {
			get { return handler.ImageInterpolation; }
			set { handler.ImageInterpolation = value; }
		}

		/// <summary>
		/// Flushes the drawing (for some platforms)
		/// </summary>
		/// <remarks>
		/// Flushing the drawing will force any undrawn changes to be shown to the user.  Typically when you are doing
		/// a lot of drawing, you may want to flush the changed periodically so that the user does not think the UI is unresponsive.
		/// 
		/// Some platforms may not have the concept of flushing the graphics, so this would do nothing
		/// </remarks>
		public void Flush ()
		{
			handler.Flush ();
		}
	}
}
