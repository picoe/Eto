using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Drawing
{
    /// <summary>
    /// Graphics context object for drawing operations
    /// </summary>
    /// <remarks>
    /// This class allows you to draw on either a <see cref="Bitmap"/> or a <see cref="T:Eto.Forms.Drawable"/> control.
    /// </remarks>
    /// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
    /// <license type="BSD-3">See LICENSE for full terms</license>
    [Handler(typeof(Graphics.IHandler))]
    public class Graphics : Widget
    {
        int currentTransformState;
        int minTransformState;

        new IHandler Handler { get { return (IHandler)base.Handler; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Eto.Drawing.Graphics"/> class.
        /// </summary>
        protected Graphics()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Graphics class with the specified platform <paramref name="handler"/>
        /// </summary>
        /// <param name="handler">Platform handler to use for this instance</param>
        public Graphics(IHandler handler)
            : base(handler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Generator class to draw on the given <paramref name="image"/>
        /// </summary>
        /// <param name="image">Image to draw on using this graphics context</param>
        public Graphics(Bitmap image)
        {
            Handler.CreateFromImage(image);
            Initialize();
        }

        /// <summary>
        /// Draws a 1 pixel wide line with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color of the line to draw</param>
        /// <param name="start">Starting location</param>
        /// <param name="end">Ending location</param>
        public void DrawLine(Color color, PointF start, PointF end)
        {
            using (var pen = new Pen(color))
                Handler.DrawLine(pen, start.X, start.Y, end.X, end.Y);
        }

        /// <summary>
        /// Draws a line with the specified <paramref name="pen"/>
        /// </summary>
        /// <param name="pen">Pen to draw the line with</param>
        /// <param name="start">Starting location</param>
        /// <param name="end">Ending location</param>
        public void DrawLine(Pen pen, PointF start, PointF end)
        {
            Handler.DrawLine(pen, start.X, start.Y, end.X, end.Y);
        }

        /// <summary>
        /// Draws a 1 pixel wide line with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color of the line</param>
        /// <param name="startx">X co-ordinate of the starting point</param>
        /// <param name="starty">Y co-ordinate of the starting point</param>
        /// <param name="endx">X co-ordinate of the ending point</param>
        /// <param name="endy">Y co-ordinate of the ending point</param>
        public void DrawLine(Color color, float startx, float starty, float endx, float endy)
        {
            using (var pen = new Pen(color))
                Handler.DrawLine(pen, startx, starty, endx, endy);
        }

        /// <summary>
        /// Draws a line with the specified <paramref name="pen"/>
        /// </summary>
        /// <param name="pen">Pen to draw the line with</param>
        /// <param name="startx">X co-ordinate of the starting point</param>
        /// <param name="starty">Y co-ordinate of the starting point</param>
        /// <param name="endx">X co-ordinate of the ending point</param>
        /// <param name="endy">Y co-ordinate of the ending point</param>
        public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
        {
            Handler.DrawLine(pen, startx, starty, endx, endy);
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="pen">Pen to outline the rectangle</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            Handler.DrawRectangle(pen, x, y, width, height);
        }

        /// <summary>
        /// Draws a 1 pixel wide outline of a rectangle with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color for the outline</param>
        /// <param name="rectangle">Where to draw the rectangle</param>
        public void DrawRectangle(Color color, RectangleF rectangle)
        {
            using (var pen = new Pen(color))
                Handler.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws a 1 pixel wide outline of a rectangle with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color for the outline</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public void DrawRectangle(Color color, float x, float y, float width, float height)
        {
            using (var pen = new Pen(color))
                Handler.DrawRectangle(pen, x, y, width, height);
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="pen">Pen to outline the rectangle</param>
        /// <param name="rectangle">Where to draw the rectangle</param>
        public void DrawRectangle(Pen pen, RectangleF rectangle)
        {
            Handler.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws an rectangle with colors on the top/left and bottom/right with the given <paramref name="width"/>
        /// </summary>
        /// <param name="topLeftColor">Color for top/left edges</param>
        /// <param name="bottomRightColor">Color for bottom/right edges</param>
        /// <param name="rectangle">Outside of inset rectangle to draw</param>
        /// <param name="width">Width of the rectangle, in pixels</param>
        public void DrawInsetRectangle(Color topLeftColor, Color bottomRightColor, RectangleF rectangle, int width = 1)
        {
            using (var topLeftPen = new Pen(topLeftColor))
            using (var bottomRightPen = new Pen(bottomRightColor))
                for (int i = 0; i < width; i++)
                {
                    DrawLine(topLeftPen, rectangle.TopLeft, rectangle.InnerTopRight);
                    DrawLine(topLeftPen, rectangle.TopLeft, rectangle.InnerBottomLeft);
                    DrawLine(bottomRightPen, rectangle.InnerBottomLeft, rectangle.InnerBottomRight);
                    DrawLine(bottomRightPen, rectangle.InnerTopRight, rectangle.InnerBottomRight);
                    rectangle.Inflate(-1, -1);
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
        public void FillRectangle(Color color, float x, float y, float width, float height)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillRectangle(brush, x, y, width, height);
        }

        /// <summary>
        /// Fills a rectangle with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the rectangle</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            Handler.FillRectangle(brush, x, y, width, height);
        }

        /// <summary>
        /// Fills a rectangle with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="rectangle">Location for the rectangle</param>
        public void FillRectangle(Color color, RectangleF rectangle)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Fills a rectangle with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the rectangle</param>
        /// <param name="rectangle">Location for the rectangle</param>
        public void FillRectangle(Brush brush, RectangleF rectangle)
        {
            Handler.FillRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Fills the specified <paramref name="rectangles"/>
        /// </summary>
        /// <param name="color">Color to fill the rectangles</param>
        /// <param name="rectangles">Enumeration of rectangles to fill</param>
        public void FillRectangles(Color color, IEnumerable<RectangleF> rectangles)
        {
            using (var brush = new SolidBrush(color))
                FillRectangles(brush, rectangles);
        }

        /// <summary>
        /// Fills the specified <paramref name="rectangles"/>
        /// </summary>
        /// <param name="brush">Brush to fill the rectangles</param>
        /// <param name="rectangles">Enumeration of rectangles to fill</param>
        public void FillRectangles(Brush brush, IEnumerable<RectangleF> rectangles)
        {
            foreach (var rectangle in rectangles)
            {
                Handler.FillRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        /// <summary>
        /// Fills an ellipse with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="rectangle">Location for the ellipse</param>
        public void FillEllipse(Color color, RectangleF rectangle)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillEllipse(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Fills an ellipse with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the ellipse</param>
        /// <param name="rectangle">Location for the ellipse</param>
        public void FillEllipse(Brush brush, RectangleF rectangle)
        {
            Handler.FillEllipse(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Fills an ellipse with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the ellipse</param>
        /// <param name="height">Height of the ellipse</param>
        public void FillEllipse(Color color, float x, float y, float width, float height)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillEllipse(brush, x, y, width, height);
        }

        /// <summary>
        /// Fills an ellipse with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the ellipse</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the ellipse</param>
        /// <param name="height">Height of the ellipse</param>
        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            Handler.FillEllipse(brush, x, y, width, height);
        }

        /// <summary>
        /// Draws a 1 pixel wide ellipse outline with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color to outline the ellipse</param>
        /// <param name="rectangle">Location for the ellipse</param>
        public void DrawEllipse(Color color, RectangleF rectangle)
        {
            using (var pen = new Pen(color))
                Handler.DrawEllipse(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws an ellipse outline with the specified <paramref name="pen"/>
        /// </summary>
        /// <param name="pen">Pen to outline the ellipse</param>
        /// <param name="rectangle">Location for the ellipse</param>
        public void DrawEllipse(Pen pen, RectangleF rectangle)
        {
            Handler.DrawEllipse(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws a 1 pixel wide ellipse outline with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the ellipse</param>
        /// <param name="height">Height of the ellipse</param>
        public void DrawEllipse(Color color, float x, float y, float width, float height)
        {
            using (var pen = new Pen(color))
                Handler.DrawEllipse(pen, x, y, width, height);
        }

        /// <summary>
        /// Draws an ellipse with the specified <paramref name="pen"/>
        /// </summary>
        /// <param name="pen">Pen to outline the ellipse</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        /// <param name="width">Width of the ellipse</param>
        /// <param name="height">Height of the ellipse</param>
        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            Handler.DrawEllipse(pen, x, y, width, height);
        }

        /// <summary>
        /// Draws a 1 pixel wide arc with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color of the arc</param>
        /// <param name="rectangle">Location of the arc</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the arc</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the arc</param>
        public void DrawArc(Color color, RectangleF rectangle, float startAngle, float sweepAngle)
        {
            using (var pen = new Pen(color))
                Handler.DrawArc(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws an arc with the specified <paramref name="pen"/>
        /// </summary>
        /// <param name="pen">Pen to draw the arc</param>
        /// <param name="rectangle">Location of the arc</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the arc</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the arc</param>
        public void DrawArc(Pen pen, RectangleF rectangle, float startAngle, float sweepAngle)
        {
            Handler.DrawArc(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a 1 pixel wide arc with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color of the arc</param>
        /// <param name="x">X co-ordinate of the upper left corner of the arc</param>
        /// <param name="y">Y co-ordinate of the upper left corner of the arc</param>
        /// <param name="width">Width of the arc</param>
        /// <param name="height">Height of the arc</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the arc</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the arc</param>
        public void DrawArc(Color color, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            using (var pen = new Pen(color))
                Handler.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws an arc with the specified <paramref name="pen"/>
        /// </summary>
        /// <param name="pen">Pen to draw the arc</param>
        /// <param name="x">X co-ordinate of the upper left corner of the arc</param>
        /// <param name="y">Y co-ordinate of the upper left corner of the arc</param>
        /// <param name="width">Width of the arc</param>
        /// <param name="height">Height of the arc</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the arc</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the arc</param>
        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            Handler.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a pie with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="rectangle">Location of the pie</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the pie</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the pie</param>
        public void FillPie(Color color, RectangleF rectangle, float startAngle, float sweepAngle)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillPie(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a pie with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the pie</param>
        /// <param name="rectangle">Location of the pie</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the pie</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the pie</param>
        public void FillPie(Brush brush, RectangleF rectangle, float startAngle, float sweepAngle)
        {
            Handler.FillPie(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a pie with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="x">X co-ordinate of the upper left corner of the pie</param>
        /// <param name="y">Y co-ordinate of the upper left corner of the pie</param>
        /// <param name="width">Width of the pie</param>
        /// <param name="height">Height of the pie</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the pie</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the pie</param>
        public void FillPie(Color color, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a pie with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the pie</param>
        /// <param name="x">X co-ordinate of the upper left corner of the pie</param>
        /// <param name="y">Y co-ordinate of the upper left corner of the pie</param>
        /// <param name="width">Width of the pie</param>
        /// <param name="height">Height of the pie</param>
        /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the pie</param>
        /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the pie</param>
        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            Handler.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a polygon defined by <paramref name="points"/> with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="points">Points of the polygon</param>
        public void FillPolygon(Color color, params PointF[] points)
        {
            var path = new GraphicsPath();
            path.AddLines(points);
            using (var brush = new SolidBrush(color))
                FillPath(brush, path);
        }

        /// <summary>
        /// Fills a polygon defined by <paramref name="points"/> with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to fill the polygon</param>
        /// <param name="points">Points of the polygon</param>
        public void FillPolygon(Brush brush, params PointF[] points)
        {
            var path = new GraphicsPath();
            path.AddLines(points);
            FillPath(brush, path);
        }

        /// <summary>
        /// Draws a 1 pixel wide outline of a polygon with the specified <paramref name="points"/>
        /// </summary>
        /// <param name="color">Color to draw the polygon lines</param>
        /// <param name="points">Points of the polygon</param>
        public void DrawPolygon(Color color, params PointF[] points)
        {
            using (var pen = new Pen(color))
                DrawPolygon(pen, points);
        }

        /// <summary>
        /// Draws an outline of a polygon with the specified <paramref name="points"/>
        /// </summary>
        /// <param name="pen">Pen to draw the polygon lines with</param>
        /// <param name="points">Points of the polygon</param>
        public void DrawPolygon(Pen pen, params PointF[] points)
        {
            Handler.DrawPolygon(pen, points);
        }

        /// <summary>
        /// Draws a 1 pixel wide set of lines with the specified <paramref name="points"/>
        /// </summary>
        /// <param name="color">Color to draw the lines</param>
        /// <param name="points">Points for each of the lines</param>
        public void DrawLines(Color color, params PointF[] points)
        {
            using (var pen = new Pen(color))
                DrawLines(pen, points);
        }

        /// <summary>
        /// Draws a 1 pixel wide set of lines with the specified <paramref name="points"/>
        /// </summary>
        /// <param name="color">Color to draw the lines</param>
        /// <param name="points">Points for each of the lines</param>
        public void DrawLines(Color color, IEnumerable<PointF> points)
        {
            using (var pen = new Pen(color))
                DrawLines(pen, points);
        }

        /// <summary>
        /// Draws a set of lines with the specified <paramref name="points"/>
        /// </summary>
        /// <param name="pen">Pen to draw the lines with</param>
        /// <param name="points">Points for each of the lines</param>
        public void DrawLines(Pen pen, params PointF[] points)
        {
            Handler.DrawLines(pen, points);
        }

        /// <summary>
        /// Draws a set of lines with the specified <paramref name="points"/>
        /// </summary>
        /// <param name="pen">Pen to draw the lines with</param>
        /// <param name="points">Points for each of the lines</param>
        public void DrawLines(Pen pen, IEnumerable<PointF> points)
        {
            Handler.DrawLines(pen, points);
        }

        /// <summary>
        /// Draws a 1 pixel outline of the specified <paramref name="path"/>
        /// </summary>
        /// <param name="color">Draw color</param>
        /// <param name="path">Path to draw</param>
        public void DrawPath(Color color, IGraphicsPath path)
        {
            using (var pen = new Pen(color))
                Handler.DrawPath(pen, path);
        }

        /// <summary>
        /// Draws the specified <paramref name="path"/>
        /// </summary>
        /// <param name="pen">Pen to outline the path</param>
        /// <param name="path">Path to draw</param>
        public void DrawPath(Pen pen, IGraphicsPath path)
        {
            Handler.DrawPath(pen, path);
        }

        /// <summary>
        /// Fills the specified <paramref name="path"/>
        /// </summary>
        /// <param name="color">Fill color</param>
        /// <param name="path">Path to fill</param>
        public void FillPath(Color color, IGraphicsPath path)
        {
            using (var brush = new SolidBrush(color))
                Handler.FillPath(brush, path);
        }

        /// <summary>
        /// Fills the specified <paramref name="path"/>
        /// </summary>
        /// <param name="brush">Brush to fill the path</param>
        /// <param name="path">Path to fill</param>
        public void FillPath(Brush brush, IGraphicsPath path)
        {
            Handler.FillPath(brush, path);
        }

        /// <summary>
        /// Draws the specified <paramref name="image"/> at a location with no scaling
        /// </summary>
        /// <param name="image">Image to draw</param>
        /// <param name="location">Location to draw the image</param>
        public void DrawImage(Image image, PointF location)
        {
            Handler.DrawImage(image, location.X, location.Y);
        }

        /// <summary>
        /// Draws the specified <paramref name="image"/> at a location with no scaling
        /// </summary>
        /// <param name="image">Image to draw</param>
        /// <param name="x">X co-ordinate</param>
        /// <param name="y">Y co-ordinate</param>
        public void DrawImage(Image image, float x, float y)
        {
            Handler.DrawImage(image, x, y);
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
        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            Handler.DrawImage(image, x, y, width, height);
        }

        /// <summary>
        /// Draws the specified <paramref name="image"/> in a rectangle
        /// </summary>
        /// <remarks>
        /// This will scale the image to the specified width and height using the <see cref="ImageInterpolation"/> mode
        /// </remarks>
        /// <param name="image">Image to draw</param>
        /// <param name="rectangle">Where to draw the image</param>
        public void DrawImage(Image image, RectangleF rectangle)
        {
            Handler.DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws the <paramref name="source"/> portion of an <paramref name="image"/>, scaling to the specified <paramref name="destination"/>
        /// </summary>
        /// <param name="image">Image to draw</param>
        /// <param name="source">Source rectangle of the image portion to draw</param>
        /// <param name="destination">Destination rectangle of where to draw the portion</param>
        public void DrawImage(Image image, RectangleF source, PointF destination)
        {
            Handler.DrawImage(image, source, new RectangleF(destination, source.Size));
        }

        /// <summary>
        /// Draws the <paramref name="source"/> portion of an <paramref name="image"/>, scaling to the specified <paramref name="destination"/>
        /// </summary>
        /// <param name="image">Image to draw</param>
        /// <param name="source">Source rectangle of the image portion to draw</param>
        /// <param name="destination">Destination rectangle of where to draw the portion</param>
        public void DrawImage(Image image, RectangleF source, RectangleF destination)
        {
            Handler.DrawImage(image, source, destination);
        }

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="brush"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="brush">Brush to stroke the text</param>
		/// <param name="x">X co-ordinate of where to start drawing the text</param>
		/// <param name="y">Y co-ordinate of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		// TODO: Remove in 3.0, kept in 2.x for ABI compatibility.
		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
			Handler.DrawText(font, brush, x, y, text);
        }

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="brush"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="brush">Brush to stroke the text</param>
		/// <param name="x">X co-ordinate of where to start drawing the text</param>
		/// <param name="y">Y co-ordinate of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		public void DrawText(Font font, Brush brush, float x, float y, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			Handler.DrawText(font, brush, x, y, text);
		}

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="color"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="color">Color of the text</param>
		/// <param name="x">X co-ordinate of where to start drawing the text</param>
		/// <param name="y">Y co-ordinate of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		public void DrawText(Font font, Color color, float x, float y, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            using (var brush = new SolidBrush(color))
				Handler.DrawText(font, brush, x, y, text);
		}

		FormattedText _sharedFormattedText;

        FormattedText SharedFormattedText => _sharedFormattedText ?? (_sharedFormattedText = new FormattedText());

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="brush"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="brush">Brush to stroke the text</param>
		/// <param name="location">Location of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		// TODO: Remove in 3.0, kept in 2.x for ABI compatibility.
		public void DrawText(Font font, SolidBrush brush, PointF location, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
			Handler.DrawText(font, brush, location.X, location.Y, text);
		}

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="brush"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="brush">Brush to stroke the text</param>
		/// <param name="location">Location of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		public void DrawText(Font font, Brush brush, PointF location, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			Handler.DrawText(font, brush, location.X, location.Y, text);
		}

		/// <summary>
		/// Draws text with the specified <paramref name="font"/>, <paramref name="color"/> and location
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="color">Color of the text</param>
		/// <param name="location">Location of where to start drawing the text</param>
		/// <param name="text">Text string to draw</param>
		public void DrawText(Font font, Color color, PointF location, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            using (var brush = new SolidBrush(color))
				DrawText(font, brush, location, text);
		}

		/// <summary>
		/// Draws text in a rectangle with the specified formatted text trimming
		/// </summary>
		/// <param name="font">Font to draw the text with</param>
		/// <param name="brush">Brush to fill the text path</param>
		/// <param name="frame">Frame for the text to be drawn into</param>
		/// <param name="text">String to draw</param>
		/// <param name="wrap">Wrap mode for the text</param>
		/// <param name="alignment">Alignment of the text in the specified rectangle</param>
		/// <param name="trimming">Trimming to apply if the text does not fit in the supplied rectangle</param>
		public void DrawText(Font font, Brush brush, RectangleF frame, string text, FormattedTextWrapMode wrap = FormattedTextWrapMode.Word, FormattedTextAlignment alignment = FormattedTextAlignment.Left, FormattedTextTrimming trimming = FormattedTextTrimming.WordEllipsis)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (frame.IsEmpty) return;

            var formattedText = SharedFormattedText;
            formattedText.Font = font;
            formattedText.ForegroundBrush = brush;
            formattedText.MaximumSize = frame.Size;
            formattedText.Text = text;
            formattedText.Trimming = trimming;
			formattedText.Wrap = wrap;
			formattedText.Alignment = alignment;
            DrawText(formattedText, frame.Location);
        }

		/// <summary>
		/// Draws formatted text at the specified location
		/// </summary>
		/// <param name="formattedText">Formatted text to draw</param>
		/// <param name="location">Location to draw the text</param>
        public void DrawText(FormattedText formattedText, PointF location)
        {
            Handler.DrawText(formattedText, location);
        }

        /// <summary>
        /// Measures the string with the given <paramref name="font"/>
        /// </summary>
        /// <param name="font">Font to measure with</param>
        /// <param name="text">Text string to measure</param>
        /// <returns>Size representing the dimensions of the entire text would take to draw given the specified <paramref name="font"/></returns>
        public virtual SizeF MeasureString(Font font, string text)
        {
            if (string.IsNullOrEmpty(text)) return SizeF.Empty; // handle null explicitly
            return Handler.MeasureString(font, text);
        }

        /// <summary>
        /// Gets or sets a value indicating that drawing operations will use anti-aliasing
        /// </summary>
        public bool AntiAlias
        {
            get { return Handler.AntiAlias; }
            set { Handler.AntiAlias = value; }
        }

        /// <summary>
        /// Gets or sets the interpolation mode for drawing images
        /// </summary>
        public ImageInterpolation ImageInterpolation
        {
            get { return Handler.ImageInterpolation; }
            set { Handler.ImageInterpolation = value; }
        }

        /// <summary>
        /// Gets the dots per inch of the current graphics context. Usually 96 for windows and 72 for other systems
        /// </summary>
        public float DPI
        {
            get { return 72f / Handler.PointsPerPixel; }
        }

        /// <summary>
        /// Gets the scale of points per pixel. Multiply by pixel size to get point value (e.g. to set font size in pixels).
        /// </summary>
        /// <remarks>
        /// A value of 1.0 indicates that one point equals one pixel.
        /// Windows is usually 0.75 (96 dpi) while other systems are usually 1.0 (e.g. linux, os x)
        /// </remarks>
        public float PointsPerPixel
        {
            get { return Handler.PointsPerPixel; }
        }

        /// <summary>
        /// Gets the scale of points to pixels. Multiply by point value to get pixel size
        /// </summary>
        /// <remarks>
        /// A value of 1.0 indicates that one pixel equals one point.
        /// Windows is usually 1 1/3 (96 dpi) while other systems are usually 1.0 (e.g. linux, os x)
        /// </remarks>
        public float PixelsPerPoint
        {
            get { return 1f / Handler.PointsPerPixel; }
        }

        /// <summary>
        /// Gets or sets the pixel offset mode for draw operations
        /// </summary>
        /// <value>The pixel offset mode.</value>
        public PixelOffsetMode PixelOffsetMode
        {
            get { return Handler.PixelOffsetMode; }
            set { Handler.PixelOffsetMode = value; }
        }

        /// <summary>
        /// Gets a value indicating the graphics sub-system is a retained system (e.g. WPF)
        /// </summary>
        /// <remarks>
        /// Retained mode systems may have different behaviour characteristics, which may impact how often the screen is updated
        /// or other code.
        /// </remarks>
        public bool IsRetained
        {
            get { return Handler.IsRetained; }
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
        public void Flush()
        {
            Handler.Flush();
        }

        /// <summary>
        /// Translates the origin of the co-ordinate system by the given offset
        /// </summary>
        /// <param name="offsetX">Offset to translate the X co-ordinate</param>
        /// <param name="offsetY">Offset to translate the Y co-ordinate</param>
        public void TranslateTransform(float offsetX, float offsetY)
        {
            Handler.TranslateTransform(offsetX, offsetY);
        }

        /// <summary>
        /// Translates the origin of the co-ordinate system by the given offset
        /// </summary>
        /// <param name="offset">Offset to translate the co-ordinate system by</param>
        public void TranslateTransform(PointF offset)
        {
            Handler.TranslateTransform(offset.X, offset.Y);
        }

        /// <summary>
        /// Rotates the co-ordinate system by the given <paramref name="angle"/>
        /// </summary>
        /// <param name="angle">Angle in degrees to rotate the co-ordinates</param>
        public void RotateTransform(float angle)
        {
            Handler.RotateTransform(angle);
        }

        /// <summary>
        /// Scales the co-ordinate system by a factor
        /// </summary>
        /// <param name="scale">Amount to scale in the horizontal and vertical axis</param>
        public void ScaleTransform(SizeF scale)
        {
            Handler.ScaleTransform(scale.Width, scale.Height);
        }

        /// <summary>
        /// Scales the co-ordinate system by a factor
        /// </summary>
        /// <param name="scaleX">Amount to scale the horizontal axis</param>
        /// <param name="scaleY">Amount to scale the vertical axis</param>
        public void ScaleTransform(float scaleX, float scaleY)
        {
            Handler.ScaleTransform(scaleX, scaleY);
        }

        /// <summary>
        /// Scales the co-ordinate system by a factor
        /// </summary>
        /// <param name="scale">Amount to scale in both the horizontal and vertical axis</param>
        public void ScaleTransform(float scale)
        {
            Handler.ScaleTransform(scale, scale);
        }

        /// <summary>
        /// Multiplies the co-ordinate system with the given <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Matrix to multiply the co-ordinate system with</param>
        public void MultiplyTransform(IMatrix matrix)
        {
            Handler.MultiplyTransform(matrix);
        }

        /// <summary>
        /// Maintains the current state of the graphics object.
        /// </summary>
        class TransformState : IDisposable
        {
            public Graphics Graphics;
            public int State;
            public int MinTransformState;

            public void Dispose()
            {
                Graphics.minTransformState = MinTransformState;
                while (Graphics.currentTransformState > State)
                {
                    Graphics.RestoreTransform();
                }
            }
        }

        /// <summary>
        /// Gets an object that will restore to the current transform state when disposed.
        /// </summary>
        /// <remarks>
        /// This saves the current transform state that can be changed by any of the transform calls.
        /// 
        /// This is useful when calling into untrusted code that can leave the transform unbalanced.
        /// It also ensures that RestoreTransform() requires a matching SaveTransform(), until the return value is disposed.
        /// Disposing the return value guarantees that the transform state is restored to when this method was called.  
        /// </remarks>
        /// <example>
        /// This example shows how you can reliably restore state in a code block:
        /// <code>
        /// using (graphics.SaveTransformState())
        /// {
        /// 	// ...
        /// 
        /// 	graphics.SaveTransform();
        /// 	graphics.MultiplyTransform(...);
        /// 
        /// 	// ... other messy drawing code that doesn't call RestoreTransform()
        /// }
        /// </code>
        /// </example>
        public IDisposable SaveTransformState()
        {
            Handler.SaveTransform();
            var state = new TransformState { Graphics = this, MinTransformState = minTransformState, State = currentTransformState };
            minTransformState = ++currentTransformState;
            return state;
        }

        /// <summary>
        /// Saves the current transform state
        /// </summary>
        /// <remarks>
        /// This saves the current transform state that can be changed by any of the transform calls, which can
        /// then be restored using <see cref="RestoreTransform"/>
        /// </remarks>
        public void SaveTransform()
        {
            currentTransformState++;
            Handler.SaveTransform();
        }

        /// <summary>
        /// Restores the transform state
        /// </summary>
        /// <remarks>
        /// This restores the transform state from a previous <see cref="SaveTransform"/> call.
        /// You must balance calls to SaveTransform() with calls to this method.
        /// </remarks>
        public void RestoreTransform()
        {
            if (currentTransformState <= minTransformState)
                throw new InvalidOperationException("No state to restore. RestoreTransform should be balanced with a corresponding call to SaveTranform.");
            currentTransformState--;
            Handler.RestoreTransform();
        }

        /// <summary>
        /// Gets a copy of the current transform.
        /// </summary>
        /// <remarks>
        /// The current transform is initially an identity matrix.
        /// Any transform operations performed on this object will change this value.
        /// </remarks>
        /// <value>The current transform matrix.</value>
        public IMatrix CurrentTransform
        {
            get { return Handler.CurrentTransform; }
        }

        /// <summary>
        /// Gets the bounds of the clipping region
        /// </summary>
        /// <remarks>
        /// This rectangle will encompass all parts of the clipping region, which may not be rectangular in shape
        /// </remarks>
        /// <value>The clip bounds applied to drawing operations</value>
        public RectangleF ClipBounds
        {
            get { return Handler.ClipBounds; }
        }

        /// <summary>
        /// Sets the clip region to the specified <paramref name="rectangle"/>
        /// </summary>
        /// <remarks>
        /// The previous clipping region will be cleared after this call.
        /// The rectangle specified is translated by the current transform, but is not affected by subsequent transform operations.
        /// </remarks>
        /// <param name="rectangle">Rectangle for the clipping region</param>
        public void SetClip(RectangleF rectangle)
        {
            Handler.SetClip(rectangle);
        }

        /// <summary>
        /// Sets the clip region to the specified <paramref name="path"/>
        /// </summary>
        /// <remarks>
        /// The previous clipping region will be cleared after this call
        /// The path specified is translated by the current transform, but is not affected by subsequent transform operations.
        /// </remarks>
        /// <param name="path">Path to specify the clip region</param>
        public void SetClip(IGraphicsPath path)
        {
            Handler.SetClip(path);
        }

        /// <summary>
        /// Resets the clip bounds to encompass the entire drawing area
        /// </summary>
        public void ResetClip()
        {
            Handler.ResetClip();
        }

        /// <summary>
        /// Returns true if the clip region intersects
        /// the specified rectangle.
        /// </summary>
        public virtual bool IsVisible(RectangleF rectangle)
        {
            return IsRetained || ClipBounds.Intersects(rectangle);
        }

        /// <summary>
        /// Resets all pixels in the <see cref="ClipBounds"/> region with the specified <paramref name="brush"/>
        /// </summary>
        /// <param name="brush">Brush to clear the graphics context</param>
        public void Clear(SolidBrush brush = null)
        {
            Handler.Clear(brush);
        }

        /// <summary>
        /// Resets all pixels in the <see cref="ClipBounds"/> region with the specified <paramref name="color"/>
        /// </summary>
        /// <param name="color">Color to clear the graphics context</param>
        public void Clear(Color color)
        {
            using (var brush = new SolidBrush(color))
                Clear(brush);
        }

        #region Handler

        /// <summary>
        /// Platform handler interface for the <see cref="Graphics"/> class
        /// </summary>
        /// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
        /// <license type="BSD-3">See LICENSE for full terms</license>
        [AutoInitialize(false)]
        public new interface IHandler : Widget.IHandler
        {
            /// <summary>
            /// Gets the scale of points to pixels. Multiply by desired pixel size to get point value (e.g. for font sizes)
            /// </summary>
            float PointsPerPixel { get; }

            /// <summary>
            /// Gets or sets the pixel offset mode for draw operations
            /// </summary>
            /// <value>The pixel offset mode.</value>
            PixelOffsetMode PixelOffsetMode { get; set; }

            /// <summary>
            /// Creates the graphics object for drawing on the specified <paramref name="image"/>
            /// </summary>
            /// <param name="image">Image to perform drawing operations on</param>
            void CreateFromImage(Bitmap image);

            /// <summary>
            /// Draws a line with the specified <paramref name="pen"/>
            /// </summary>
            /// <param name="pen">Pen to draw the line</param>
            /// <param name="startx">X co-ordinate of the starting point</param>
            /// <param name="starty">Y co-ordinate of the starting point</param>
            /// <param name="endx">X co-ordinate of the ending point</param>
            /// <param name="endy">Y co-ordinate of the ending point</param>
            void DrawLine(Pen pen, float startx, float starty, float endx, float endy);

            /// <summary>
            /// Draws a set of lines with the specified <paramref name="points"/>
            /// </summary>
            /// <param name="pen">Pen to draw the lines with</param>
            /// <param name="points">Points for each of the lines</param>
            void DrawLines(Pen pen, IEnumerable<PointF> points);

            /// <summary>
            /// Draws an outline of a polygon with the specified <paramref name="points"/>
            /// </summary>
            /// <param name="pen">Pen to draw the polygon lines with</param>
            /// <param name="points">Points of the polygon</param>
            void DrawPolygon(Pen pen, IEnumerable<PointF> points);

            /// <summary>
            /// Draws a rectangle outline
            /// </summary>
            /// <param name="pen">Pen to draw the rectangle</param>
            /// <param name="x">X co-ordinate</param>
            /// <param name="y">Y co-ordinate</param>
            /// <param name="width">Width of the rectangle</param>
            /// <param name="height">Height of the rectangle</param>
            void DrawRectangle(Pen pen, float x, float y, float width, float height);

            /// <summary>
            /// Fills a rectangle with the specified <paramref name="brush"/>
            /// </summary>
            /// <param name="brush">Brush to fill with</param>
            /// <param name="x">X co-ordinate</param>
            /// <param name="y">Y co-ordinate</param>
            /// <param name="width">Width of the rectangle</param>
            /// <param name="height">Height of the rectangle</param>
            void FillRectangle(Brush brush, float x, float y, float width, float height);

            /// <summary>
            /// Fills an ellipse with the specified <paramref name="brush"/>
            /// </summary>
            /// <param name="brush">Brush to fill the ellipse</param>
            /// <param name="x">X co-ordinate of the left side of the ellipse</param>
            /// <param name="y">Y co-ordinate of the top of the ellipse</param>
            /// <param name="width">Width of the ellipse</param>
            /// <param name="height">Height of the ellipse</param>
            void FillEllipse(Brush brush, float x, float y, float width, float height);

            /// <summary>
            /// Draws an outline of an ellipse with the specified <paramref name="pen"/>
            /// </summary>
            /// <param name="pen">Pen to outline the ellipse</param>
            /// <param name="x">X co-ordinate of the left side of the ellipse</param>
            /// <param name="y">Y co-ordinate of the top of the ellipse</param>
            /// <param name="width">Width of the ellipse</param>
            /// <param name="height">Height of the ellipse</param>
            void DrawEllipse(Pen pen, float x, float y, float width, float height);

            /// <summary>
            /// Draws an arc with the specified <paramref name="pen"/>
            /// </summary>
            /// <param name="pen">Pen to outline the arc</param>
            /// <param name="x">X co-ordinate of the upper left corner of the arc</param>
            /// <param name="y">Y co-ordinate of the upper left corner of the arc</param>
            /// <param name="width">Width of the arc</param>
            /// <param name="height">Height of the arc</param>
            /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the arc</param>
            /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the arc</param>
            void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

            /// <summary>
            /// Fills a pie with the specified <paramref name="brush"/>
            /// </summary>
            /// <param name="brush">Brush to fill the pie</param>
            /// <param name="x">X co-ordinate of the upper left corner of the pie</param>
            /// <param name="y">Y co-ordinate of the upper left corner of the pie</param>
            /// <param name="width">Width of the pie</param>
            /// <param name="height">Height of the pie</param>
            /// <param name="startAngle">Elliptical (skewed) angle in degrees from the x-axis to the starting point of the pie</param>
            /// <param name="sweepAngle">Angle in degrees from the <paramref name="startAngle"/> to the ending point of the pie</param>
            void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle);

            /// <summary>
            /// Fills the specified <paramref name="path"/>
            /// </summary>
            /// <param name="brush">Brush to fill the path</param>
            /// <param name="path">Path to fill</param>
            void FillPath(Brush brush, IGraphicsPath path);

            /// <summary>
            /// Draws the specified <paramref name="path"/>
            /// </summary>
            /// <param name="pen">Pen to outline the path</param>
            /// <param name="path">Path to draw</param>
            void DrawPath(Pen pen, IGraphicsPath path);

            /// <summary>
            /// Draws the specified <paramref name="image"/> at a location with no scaling
            /// </summary>
            /// <param name="image">Image to draw</param>
            /// <param name="x">X co-ordinate</param>
            /// <param name="y">Y co-ordinate</param>
            void DrawImage(Image image, float x, float y);

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
            void DrawImage(Image image, float x, float y, float width, float height);

            /// <summary>
            /// Draws the <paramref name="source"/> portion of an <paramref name="image"/>, scaling to the specified <paramref name="destination"/>
            /// </summary>
            /// <param name="image">Image to draw</param>
            /// <param name="source">Source rectangle of the image portion to draw</param>
            /// <param name="destination">Destination rectangle of where to draw the portion</param>
            void DrawImage(Image image, RectangleF source, RectangleF destination);

			/// <summary>
			/// Draws text with the specified <paramref name="font"/>, <paramref name="brush"/> and location
			/// </summary>
			/// <param name="font">Font to draw the text with</param>
			/// <param name="brush">A brush with the color of the text</param>
			/// <param name="x">X co-ordinate of where to start drawing the text</param>
			/// <param name="y">Y co-ordinate of where to start drawing the text</param>
			/// <param name="text">Text string to draw</param>
			void DrawText(Font font, Brush brush, float x, float y, string text);

			/// <summary>
			/// Draws formatted text at the specified location
			/// </summary>
			/// <param name="formattedText">Formatted text to draw</param>
			/// <param name="location">Location to draw the text</param>
			void DrawText(FormattedText formattedText, PointF location);

			/// <summary>
			/// Measures the string with the given <paramref name="font"/>
			/// </summary>
			/// <param name="font">Font to measure with</param>
			/// <param name="text">Text string to measure</param>
			/// <returns>Size representing the dimensions of the entire text would take to draw given the specified <paramref name="font"/></returns>
			SizeF MeasureString(Font font, string text);

            /// <summary>
            /// Flushes the drawing (for some platforms)
            /// </summary>
            /// <remarks>
            /// Flushing the drawing will force any undrawn changes to be shown to the user.  Typically when you are doing
            /// a lot of drawing, you may want to flush the changed periodically so that the user does not think the UI is unresponsive.
            /// </remarks>
            void Flush();

            /// <summary>
            /// Gets or sets a value indicating that drawing operations will use antialiasing
            /// </summary>
            bool AntiAlias { get; set; }

            /// <summary>
            /// Gets or sets the interpolation mode for drawing images
            /// </summary>
            ImageInterpolation ImageInterpolation { get; set; }

            /// <summary>
            /// Gets a value indicating the graphics sub-system is a retained system (e.g. WPF)
            /// </summary>
            /// <remarks>
            /// Retained mode systems may have different behaviour characteristics, which may impact how often the screen is updated
            /// or other code.
            /// </remarks>
            bool IsRetained { get; }

            /// <summary>
            /// Translates the origin of the co-ordinate system by the given offset
            /// </summary>
            /// <param name="offsetX">Offset to translate the X co-ordinate</param>
            /// <param name="offsetY">Offset to translate the Y co-ordinate</param>
            void TranslateTransform(float offsetX, float offsetY);

            /// <summary>
            /// Rotates the co-ordinate system by the given <paramref name="angle"/>
            /// </summary>
            /// <param name="angle">Angle in degrees to rotate the co-ordinates</param>
            void RotateTransform(float angle);

            /// <summary>
            /// Scales the co-ordinate system by a factor
            /// </summary>
            /// <param name="scaleX">Amount to scale the horizontal axis</param>
            /// <param name="scaleY">Amount to scale the vertical axis</param>
            void ScaleTransform(float scaleX, float scaleY);

            /// <summary>
            /// Multiplies the co-ordinate system with the given <paramref name="matrix"/>
            /// </summary>
            /// <param name="matrix">Matrix to multiply the co-ordinate system with</param>
            void MultiplyTransform(IMatrix matrix);

            /// <summary>
            /// Saves the current transform state
            /// </summary>
            /// <remarks>
            /// This saves the current transform state that can be changed by any of the transform calls, which can
            /// then be restored using <see cref="RestoreTransform"/>
            /// </remarks>
            void SaveTransform();

            /// <summary>
            /// Restores the transform state
            /// </summary>
            /// <remarks>
            /// This restores the transform state from a previous <see cref="SaveTransform"/> call.
            /// </remarks>
            void RestoreTransform();

            /// <summary>
            /// Gets a copy of the current transform.
            /// </summary>
            /// <remarks>
            /// The current transform is initially an identity matrix.
            /// Any transform operations performed on this object will change this value.
            /// </remarks>
            /// <value>The current transform matrix.</value>
            IMatrix CurrentTransform { get; }

            /// <summary>
            /// Gets the bounds of the clipping region
            /// </summary>
            /// <remarks>
            /// This rectangle will encompass all parts of the clipping region, which may not be rectangular in shape
            /// </remarks>
            /// <value>The clip bounds applied to drawing operations</value>
            RectangleF ClipBounds { get; }

            /// <summary>
            /// Sets the clip region to the specified <paramref name="rectangle"/>
            /// </summary>
            /// <remarks>
            /// The previous clipping region will be cleared after this call
            /// </remarks>
            /// <param name="rectangle">Rectangle for the clipping region</param>
            void SetClip(RectangleF rectangle);

            /// <summary>
            /// Sets the clip region to the specified <paramref name="path"/>
            /// </summary>
            /// <remarks>
            /// The previous clipping region will be cleared after this call
            /// </remarks>
            /// <param name="path">Path to specify the clip region</param>
            void SetClip(IGraphicsPath path);

            /// <summary>
            /// Resets the clip bounds to encompass the entire drawing area
            /// </summary>
            void ResetClip();

            /// <summary>
            /// Resets all pixels in the <see cref="ClipBounds"/> region with the specified <paramref name="brush"/>
            /// </summary>
            /// <param name="brush">Brush to clear the graphics context</param>
            void Clear(SolidBrush brush);

        }
        #endregion

    }
}
