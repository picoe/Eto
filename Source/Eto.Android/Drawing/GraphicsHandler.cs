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
	/// <summary>
	/// Handler for <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : WidgetHandler<ag.Canvas, Graphics>, Graphics.IHandler
	{
		public GraphicsHandler()
		{
		}

		public float PointsPerPixel { get { return 1f; } }
		// TODO

		public PixelOffsetMode PixelOffsetMode { get; set; }
		// TODO

		public void CreateFromImage(Bitmap image)
		{
			Control = new ag.Canvas((ag.Bitmap)image.ControlObject);
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			Control.DrawLine(startx, starty, endx, endy, pen.ToAndroid());
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawRect(new RectangleF(x, y, width, height).ToAndroid(), pen.ToAndroid());
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			Control.DrawRect(new RectangleF(x, y, width, height).ToAndroid(), brush.ToAndroid());
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			Control.DrawOval(new RectangleF(x, y, width, height).ToAndroid(), brush.ToAndroid());
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawOval(new RectangleF(x, y, width, height).ToAndroid(), pen.ToAndroid());
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.DrawArc(new RectangleF(x, y, width, height).ToAndroid(), startAngle, sweepAngle, false, pen.ToAndroid());
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.DrawArc(new RectangleF(x, y, width, height).ToAndroid(), startAngle, sweepAngle, true, brush.ToAndroid());
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			Control.DrawPath(path.ToAndroid(), brush.ToAndroid());
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			Control.DrawPath(path.ToAndroid(), pen.ToAndroid());
		}

		public void DrawImage(Image image, float x, float y)
		{
			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, x, y);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, x, y, width, height);
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, source, destination);
		}


		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			var paint = GetTextPaint(font);
			paint.Color = brush.ToAndroid().Color; // this overwrites the color on the cached paint, but that's ok since it is only used here.
			Control.DrawText(text, x, y, paint);
		}

		public SizeF MeasureString(Font font, string text)
		{
			if (string.IsNullOrEmpty(text)) // needed to avoid exception
				return SizeF.Empty;			
			var paint = GetTextPaint(font);

			// See http://stackoverflow.com/questions/7549182/android-paint-measuretext-vs-gettextbounds
			var bounds = new ag.Rect();
			paint.GetTextBounds(text, 0, text.Length, bounds);
			
			// TODO: see the above article; the width may be truncated to the nearest integer.
			return new SizeF(bounds.Width(), bounds.Height()); 
		}

		/// <summary>
		/// Returns a Paint that is cached on the font.
		/// This is used by all font operations (across all Canvases) that use the same font.
		/// </summary>
		private ag.Paint GetTextPaint(Font font)
		{
			var paint = (font.Handler as FontHandler).Paint;
			paint.AntiAlias = AntiAlias;
			return paint;
		}

		public void Flush()
		{			
		}

		// The ANTI_ALIAS flag on Paint (not Canvas) causes it to render antialiased.
		// SUBPIXEL_TEXT_FLAG is currently unsupported on Android.
		// See http://stackoverflow.com/questions/4740565/meaning-of-some-paint-constants-in-android
		public bool AntiAlias { get; set; }

		// TODO: setting the FILTER_BITMAP_FLAG on Paint (not Canvas)
		// causes it to do a bilinear interpolation.
		public ImageInterpolation ImageInterpolation { get; set; }

		public bool IsRetained
		{
			get { return false; }
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			Control.Translate(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			Control.Rotate(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.Scale(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Control.Concat(matrix.ToAndroid());
		}

		public void SaveTransform()
		{
			Control.Save(ag.SaveFlags.Matrix);
		}

		public void RestoreTransform()
		{
			Control.Restore();
		}

		public IMatrix CurrentTransform
		{
			get { return Control.Matrix.ToEto(); }
		}

		public RectangleF ClipBounds
		{
			get { return Control.ClipBounds.ToEto(); }
		}

		public void SetClip(RectangleF rectangle)
		{
			Control.ClipRect(rectangle.ToAndroid(), ag.Region.Op.Replace);
		}

		public void SetClip(IGraphicsPath path)
		{
			// NOTE: This may not work with hardware acceleration.
			// See http://developer.android.com/guide/topics/graphics/hardware-accel.html#drawing-support
			// See http://stackoverflow.com/questions/16889815/canvas-clippath-only-works-on-android-emulator
			Control.ClipPath(path.ToAndroid(), ag.Region.Op.Replace);
		}

		public void ResetClip()
		{
			Control.ClipRect(new ag.Rect(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
		}

		public void Clear(SolidBrush brush)
		{
			throw new NotImplementedException();
		}

		public void DrawLines(Pen pen, IEnumerable<PointF> points)
		{
			Control.DrawLines(points.SelectMany(p => new[] { p.X, p.Y }).ToArray(), pen.ToAndroid());
		}

		public void DrawPolygon(Pen pen, IEnumerable<PointF> points)
		{
			var pts = points.SelectMany(p => new[] { p.X, p.Y });
			var first = points.First();
			pts = pts.Union(new[] { first.X, first.Y });
			Control.DrawLines(pts.ToArray(), pen.ToAndroid());
		}
	}
}