using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using at = Android.Text;

namespace Eto.Android.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : WidgetHandler<ag.Canvas, Graphics>, Graphics.IHandler
	{
		// Android does not allow the clip region to expand, only shrink, except by saving/restoring
		// the drawing state stack. Eto interface requires being able to change the clip region at any time.
		// To work around this, without having to save our own stack of all transform/clip changes, we only 
		// save apply the clip region to the canvas right before drawing, and immediately undo it before any
		// methods which modify, save or restore the transform matrix.
		private RectangleF clipRect;
		private IMatrix clipMatrix;
		private Boolean isClipApplied;

		public GraphicsHandler(ag.Canvas canvas)
		{
			Control = canvas;
		}

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
			ApplyClip();

			Control.DrawLine(startx, starty, endx, endy, pen.ToAndroid());
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			ApplyClip();

			Control.DrawRect(x, y, x + width, y + height, pen.ToAndroid());
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			ApplyClip();

			Control.DrawRect(x, y, x + width, y + height, brush.ToAndroid());
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			ApplyClip();

			Control.DrawOval(new RectangleF(x, y, width, height).ToAndroid(), brush.ToAndroid());
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			ApplyClip();

			Control.DrawOval(new RectangleF(x, y, width, height).ToAndroid(), pen.ToAndroid());
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			ApplyClip();

			Control.DrawArc(new RectangleF(x, y, width, height).ToAndroid(), startAngle, sweepAngle, false, pen.ToAndroid());
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			ApplyClip();

			Control.DrawArc(new RectangleF(x, y, width, height).ToAndroid(), startAngle, sweepAngle, true, brush.ToAndroid());
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			ApplyClip();

			Control.DrawPath(path.ToAndroid(), brush.ToAndroid());
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			ApplyClip();

			Control.DrawPath(path.ToAndroid(), pen.ToAndroid());
		}

		public void DrawImage(Image image, float x, float y)
		{
			ApplyClip();

			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, x, y);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			ApplyClip();

			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, x, y, width, height);
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			ApplyClip();

			var handler = image.Handler as IAndroidImage;
			handler.DrawImage(this, source, destination);
		}

		public void DrawText(Font font, Brush brush, Single x, Single y, String text)
		{
			ApplyClip();

			var paint = GetTextPaint(font);
			paint.Color = brush.ToAndroid().Color; // this overwrites the color on the cached paint, but that's ok since it is only used here.

			// Android measures position at baseline
			Control.DrawText(text, x, y - paint.Ascent(), paint);
		}

		public void DrawText(FormattedText formattedText, PointF location)
		{
			ApplyClip();

			var text = formattedText.Text;

			if (String.IsNullOrEmpty(text)) // needed to avoid exception
				return;

			Control.Save(ag.SaveFlags.Matrix);
			Control.Translate(location.X, location.Y);

			var paint = GetTextPaint(formattedText);
			
			at.StaticLayout sl = at.StaticLayout.Builder.Obtain(text, 0, text.Length, paint, (Int32)formattedText.MaximumWidth)
				.SetAlignment(TranslateAlign(formattedText.Alignment))
				.SetIncludePad(false)
				.Build();

			sl.Draw(Control);

			Control.Restore();
		}

		private at.Layout.Alignment TranslateAlign(FormattedTextAlignment alignment)
		{
			switch(alignment)
			{
				case FormattedTextAlignment.Center:
					return at.Layout.Alignment.AlignCenter;

				case FormattedTextAlignment.Right:
					return at.Layout.Alignment.AlignOpposite;

				default:
					return at.Layout.Alignment.AlignNormal;
			}
		}

		public SizeF MeasureString(Font font, String text, Single width)
		{
			if (string.IsNullOrEmpty(text)) // needed to avoid exception
				return SizeF.Empty;

			var paint = GetTextPaint(font);

			at.StaticLayout sl = at.StaticLayout.Builder.Obtain(text, 0, text.Length, paint, (Int32)width)
				.SetIncludePad(false)
				.Build();

			Single Width = 0;

			for (Int32 lineIndex = 0; lineIndex < sl.LineCount; lineIndex++)
				if (sl.GetLineWidth(lineIndex) > Width)
					Width = sl.GetLineWidth(lineIndex);

			return new SizeF(Width, sl.Height);
		}

		public SizeF MeasureString(Font font, string text)
		{
			// See http://stackoverflow.com/questions/7549182/android-paint-measuretext-vs-gettextbounds

			if (string.IsNullOrEmpty(text)) // needed to avoid exception
				return SizeF.Empty;

			var paint = GetTextPaint(font);

			var W = paint.MeasureText(text);
			var H = -paint.Ascent() + paint.Descent();

			return new SizeF(W, H); 
		}

		/// <summary>
		/// Returns a Paint that is cached on the font.
		/// This is used by all font operations (across all Canvases) that use the same font.
		/// </summary>
		private at.TextPaint GetTextPaint(Font font)
		{
			var paint = (font.Handler as FontHandler).Paint;
			paint.AntiAlias = AntiAlias;
			return paint;
		}

		private at.TextPaint GetTextPaint(FormattedText ft)
		{
			var paint = GetTextPaint(ft.Font);
			paint.Color = ft.ForegroundBrush.ToAndroid().Color;
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
			UnapplyClip();
			Control.Translate(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			UnapplyClip();
			Control.Rotate(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			UnapplyClip();
			Control.Scale(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			UnapplyClip();
			Control.Concat(matrix.ToAndroid());
		}

		public void SaveTransform()
		{
			UnapplyClip();
			Control.Save(ag.SaveFlags.Matrix);
		}

		public void RestoreTransform()
		{
			UnapplyClip();
			Control.Restore();
		}

		public IMatrix CurrentTransform
		{
			get { return Control.Matrix.ToEto(); }
		}

		public RectangleF ClipBounds
		{
			get { return clipRect; }
		}

		public void SetClip(RectangleF rectangle)
		{
			UnapplyClip();
			clipRect = rectangle;
			clipMatrix = CurrentTransform;
		}

		public void SetClip(IGraphicsPath path)
		{
			// NOTE: This may not work with hardware acceleration.
			// See http://developer.android.com/guide/topics/graphics/hardware-accel.html#drawing-support
			// See http://stackoverflow.com/questions/16889815/canvas-clippath-only-works-on-android-emulator

			SetClip(path.Bounds);
		}

		public void ResetClip()
		{
			UnapplyClip();
			clipRect = RectangleF.Empty;
		}

		private void ApplyClip()
		{
			if (isClipApplied || clipRect.IsEmpty)
				return;

			// Get the absolute clipping rectangle as it would've been at the time it was set
			var AbsoluteClip = clipMatrix.TransformRectangle(clipRect);

			// Apply inverse of current transform (it will be re-applied by Android later to get back to the same absolute bounds)
			var UntransformedClip = CurrentTransform.Inverse().TransformRectangle(AbsoluteClip);

			Control.Save(ag.SaveFlags.Clip);

			Control.ClipRect(UntransformedClip.ToAndroid());
			isClipApplied = true;
		}

		private void UnapplyClip()
		{
			if (!isClipApplied)
				return;

			Control.Restore();
			isClipApplied = false;
		}

		public void Clear(SolidBrush brush)
		{
			ApplyClip();

			if (brush == null)
				brush = Brushes.Black;

			Control.DrawRect(0, 0, Control.Width, Control.Height, brush.ToAndroid());
		}

		public void DrawLines(Pen pen, IEnumerable<PointF> points)
		{
			ApplyClip();

			Control.DrawLines(points.SelectMany(p => new[] { p.X, p.Y }).ToArray(), pen.ToAndroid());
		}

		public void DrawPolygon(Pen pen, IEnumerable<PointF> points)
		{
			ApplyClip();

			var pts = points.SelectMany(p => new[] { p.X, p.Y });
			var first = points.First();
			pts = pts.Union(new[] { first.X, first.Y });
			Control.DrawLines(pts.ToArray(), pen.ToAndroid());
		}
	}
}
