using System;
using swm = System.Windows.Media;
using sw = System.Windows;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="Graphics"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : WidgetHandler<swm.DrawingContext, Graphics>, Graphics.IHandler
	{
		swm.Visual visual;
		swm.DrawingVisual drawingVisual;
		ImageInterpolation imageInterpolation;
		PixelOffsetMode pixelOffsetMode;
		double offset = 0.5;
		double inverseoffset;
		RectangleF? clipBounds;
		readonly RectangleF initialClip;
		IGraphicsPath clipPath;
		sw.Rect bounds;
		readonly bool disposeControl;

		Bitmap image;
		double? dpiScale;

		public GraphicsHandler()
		{
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get { return pixelOffsetMode; }
			set
			{
				pixelOffsetMode = value;
				offset = pixelOffsetMode == PixelOffsetMode.None ? 0.5 : 0;
				inverseoffset = pixelOffsetMode == PixelOffsetMode.None ? 0 : 0.5;
			}
		}

		public float PointsPerPixel
		{
			get { return (float)DPI * 72f / 96f; }
		}

		protected override bool DisposeControl { get { return disposeControl; } }

		public GraphicsHandler(swm.Visual visual, swm.DrawingContext context, sw.Rect bounds, RectangleF initialClip, bool shouldDispose = true)
		{
			disposeControl = shouldDispose;
			this.visual = visual;
			drawingVisual = visual as swm.DrawingVisual;

			Control = context;

			this.bounds = bounds;
			this.initialClip = initialClip;

			PushGuideLines(bounds);

			Control.PushClip(new swm.RectangleGeometry(bounds));

			ImageInterpolation = ImageInterpolation.Default;
		}

		public bool IsRetained { get { return true; } }

		public void CreateFromImage(Bitmap image)
		{
			this.image = image;
			bounds = new sw.Rect(0, 0, image.Size.Width, image.Size.Height);
			drawingVisual = new swm.DrawingVisual();
			visual = drawingVisual;
			Control = drawingVisual.RenderOpen();
			Control.DrawImage(image.ControlObject as swm.ImageSource, bounds);

			PushGuideLines(bounds);

			ImageInterpolation = ImageInterpolation.Default;
		}

		public double DPI
		{
			get
			{
				if (dpiScale == null)
				{
					var presentationSource = sw.PresentationSource.FromVisual(visual);
					if (presentationSource != null && presentationSource.CompositionTarget != null)
					{
						swm.Matrix m = presentationSource.CompositionTarget.TransformToDevice;
						dpiScale = m.M11;
					}
					else
						dpiScale = 1.0;
				}
				return dpiScale.Value;
			}
		}

		public void PushGuideLines(sw.Rect rect)
		{
			PushGuideLines(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void PushGuideLines(double x, double y, double width, double height)
		{
			Control.PushGuidelineSet(new swm.GuidelineSet(new double[] { x, x + width }, new double[] { y, y + height }));
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			PushGuideLines(x, y, width, height);
			Control.DrawRectangle(null, pen.ToWpf(true), new sw.Rect(x + offset, y + offset, width, height));
			Control.Pop();
		}


		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			var wpfPen = pen.ToWpf(true);
			Control.PushGuidelineSet(new swm.GuidelineSet(new double[] { startx, endx }, new double[] { starty, endy }));
			Control.DrawLine(wpfPen, new sw.Point(startx + offset, starty + offset), new sw.Point(endx + offset, endy + offset));
			Control.Pop();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			var wpfBrush = brush.ToWpf(true);
			PushGuideLines(x, y, width, height);
			Control.DrawRectangle(wpfBrush, null, new sw.Rect(x + inverseoffset, y + inverseoffset, width, height));
			Control.Pop();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawEllipse(null, pen.ToWpf(true), new sw.Point(x + width / 2.0 + offset, y + height / 2.0 + offset), width / 2.0, height / 2.0);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			PushGuideLines(x, y, width, height);
			Control.DrawEllipse(brush.ToWpf(true), null, new sw.Point(x + width / 2.0 + inverseoffset, y + height / 2.0 + inverseoffset), width / 2.0, height / 2.0);
			Control.Pop();
		}

		public static swm.Geometry CreateArcDrawing(sw.Rect rect, double startDegrees, double sweepDegrees, bool closed)
		{
			// degrees to radians conversion
			double startRadians = startDegrees * Math.PI / 180.0;
			double sweepRadians = sweepDegrees * Math.PI / 180.0;

			// x and y radius
			double dx = rect.Width / 2;
			double dy = rect.Height / 2;

			// determine the start point 
			double xs = rect.X + dx + (Math.Cos(startRadians) * dx);
			double ys = rect.Y + dy + (Math.Sin(startRadians) * dy);

			// determine the end point 
			double xe = rect.X + dx + (Math.Cos(startRadians + sweepRadians) * dx);
			double ye = rect.Y + dy + (Math.Sin(startRadians + sweepRadians) * dy);

			var centerPoint = new sw.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			// draw the arc into a stream geometry
			var streamGeom = new swm.StreamGeometry();
			using (var ctx = streamGeom.Open())
			{
				bool isLargeArc = Math.Abs(sweepDegrees) > 180;
				var sweepDirection = sweepDegrees < 0 ? swm.SweepDirection.Counterclockwise : swm.SweepDirection.Clockwise;

				if (closed)
				{
					ctx.BeginFigure(centerPoint, true, true);
					ctx.LineTo(new sw.Point(xs, ys), true, true);
				}
				else
					ctx.BeginFigure(new sw.Point(xs, ys), true, false);
				ctx.ArcTo(new sw.Point(xe, ye), new sw.Size(dx, dy), 0, isLargeArc, sweepDirection, true, false);
				if (closed)
					ctx.LineTo(centerPoint, true, true);
			}

			return streamGeom;
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (sweepAngle >= 360f)
				DrawEllipse(pen, x, y, width, height);
			else
			{
				var arc = CreateArcDrawing(new sw.Rect(x, y, width, height), startAngle, sweepAngle, false);
				Control.PushTransform(new swm.TranslateTransform(offset, offset));
				Control.DrawGeometry(null, pen.ToWpf(true), arc);
				Control.Pop();
			}
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (sweepAngle >= 360f)
				FillEllipse(brush, x, y, width, height);
			else
			{
				var arc = CreateArcDrawing(new sw.Rect(x, y, width, height), startAngle, sweepAngle, true);
				Control.PushTransform(new swm.TranslateTransform(inverseoffset, inverseoffset));
				Control.DrawGeometry(brush.ToWpf(true), null, arc);
				Control.Pop();
			}
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			Control.DrawGeometry(brush.ToWpf(true), null, path.ToWpf());
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			Control.PushTransform(new swm.TranslateTransform(offset, offset));
			Control.DrawGeometry(null, pen.ToWpf(true), path.ToWpf());
			Control.Pop();
		}

		public void DrawImage(Image image, float x, float y)
		{
			var size = image.Size;
			DrawImage(image, x, y, size.Width, size.Height);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			var src = image.ToWpf((int)Math.Max(width, height));
			//Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { x , x  + width }, new double[] { y , y + height }));
			Control.DrawImage(src, new sw.Rect(x + inverseoffset, y + inverseoffset, width, height));
			//Control.Pop ();
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			var src = image.ToWpf();
			Control.PushClip(new swm.RectangleGeometry(destination.ToWpf()));
			bool scale = source.Size != destination.Size;
			bool translate = source.X > 0 || source.Y > 0;
			double scalex = 1.0;
			double scaley = 1.0;
			if (scale)
			{
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				Control.PushTransform(new swm.ScaleTransform(scalex, scaley));
			}
			if (translate)
				Control.PushTransform(new swm.TranslateTransform(-source.X, -source.Y));
			var rect = new sw.Rect(destination.X / scalex, destination.Y / scaley, image.Size.Width, image.Size.Height);
			Control.DrawImage(src, rect);
			// pop for TranslateTransform
			if (translate)
				Control.Pop();
			// pop again for ScaleTransform
			if (scale)
				Control.Pop();
			// pop for PushClip
			Control.Pop();
		}

		public void DrawText(Font font, SolidBrush b, float x, float y, string text)
		{
			var fontHandler = font.Handler as FontHandler;
			if (fontHandler != null)
			{
				var brush = b.ToWpf();
				var formattedText = new swm.FormattedText(text, CultureInfo.CurrentUICulture, sw.FlowDirection.LeftToRight, fontHandler.WpfTypeface, fontHandler.PixelSize, brush);
				if (fontHandler.WpfTextDecorations != null)
					formattedText.SetTextDecorations(fontHandler.WpfTextDecorations, 0, text.Length);
				Control.DrawText(formattedText, new sw.Point(x, y));
			}
		}

		public SizeF MeasureString(Font font, string text)
		{
			var result = SizeF.Empty;

			var fontHandler = font.Handler as FontHandler;
			if (fontHandler != null)
			{
				var brush = new swm.SolidColorBrush(swm.Colors.White);
				var formattedText = new swm.FormattedText(text, CultureInfo.CurrentUICulture, sw.FlowDirection.LeftToRight, fontHandler.WpfTypeface, fontHandler.PixelSize, brush);
				result = new SizeF((float)formattedText.WidthIncludingTrailingWhitespace, (float)formattedText.Height);
			}

			return result;
		}

		public void Flush()
		{
			if (Close())
			{
				Control = drawingVisual.RenderOpen();
			}
		}

		bool Close()
		{
			if (image != null)
			{
				Control.Close();
				var handler = (BitmapHandler)image.Handler;
				var bmp = handler.Control;
				var newbmp = new swmi.RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, swm.PixelFormats.Pbgra32);
				newbmp.Render(visual);
				handler.SetBitmap(newbmp);
				return true;
			}
			return false;
		}

		public bool AntiAlias
		{
			get
			{
				switch (swm.RenderOptions.GetEdgeMode(visual))
				{
					case swm.EdgeMode.Aliased:
						return false;
					case swm.EdgeMode.Unspecified:
						return true;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				swm.RenderOptions.SetEdgeMode(visual, value ? swm.EdgeMode.Unspecified : swm.EdgeMode.Aliased);
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return imageInterpolation; }
			set
			{
				imageInterpolation = value;
				swm.RenderOptions.SetBitmapScalingMode(visual, value.ToWpf());
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Close();
			base.Dispose(disposing);
		}

		TransformStack transformStack;

		TransformStack TransformStack
		{
			get
			{
				if (transformStack == null)
					transformStack = new TransformStack(
						m => Control.PushTransform(m.ToWpfTransform()),
						Control.Pop);

				return transformStack;
			}
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			ReverseClip();
			TransformStack.TranslateTransform(offsetX, offsetY);
			ApplyClip();
		}

		public void RotateTransform(float angle)
		{
			ReverseClip();
			TransformStack.RotateTransform(angle);
			ApplyClip();
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			ReverseClip();
			TransformStack.ScaleTransform(scaleX, scaleY);
			ApplyClip();
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			ReverseClip();
			TransformStack.MultiplyTransform(matrix);
			ApplyClip();
		}

		public void SaveTransform()
		{
			TransformStack.SaveTransform();
		}

		public void RestoreTransform()
		{
			TransformStack.RestoreTransform();
		}

		protected void ReverseClip()
		{
			if (clipBounds != null || clipPath != null)
				Control.Pop();
		}

		protected void ApplyClip()
		{
			if (clipPath != null)
			{
				Control.PushClip(clipPath.ToWpf());
			}
			else if (clipBounds != null)
			{
				Control.PushClip(new swm.RectangleGeometry(clipBounds.Value.ToWpf()));
			}
		}

		public RectangleF ClipBounds
		{
			get
			{
				var translatedClip = clipBounds ?? initialClip;
				if (TransformStack.Current != null)
				{
					var invertedTransform = TransformStack.Current.Clone();
					invertedTransform.Invert();
					translatedClip = invertedTransform.TransformRectangle(translatedClip);
				}
				return translatedClip;
			}
		}

		public void SetClip(RectangleF rectangle)
		{
			ResetClip();
			clipBounds = rectangle;
			clipPath = null;
			ApplyClip();
		}

		public void SetClip(IGraphicsPath path)
		{
			ResetClip();
			clipPath = path.Clone(); // require a clone so changes to path don't affect current clip
			clipBounds = clipPath.ToWpf().Bounds.ToEtoF();
			ApplyClip();
		}

		public void ResetClip()
		{
			if (clipBounds != null)
			{
				Control.Pop();
				clipBounds = null;
				clipPath = null;
			}
		}

		public void Clear(SolidBrush brush)
		{
			var rect = clipBounds ?? initialClip;
			if (drawingVisual != null)
			{
				// bitmap
				Control.Close();
				var newbmp = new swmi.RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, swm.PixelFormats.Pbgra32);
				newbmp.Render(visual);

				swm.Geometry maskgeometry;
				if (clipPath != null)
				{
					maskgeometry = clipPath.ToWpf();
				}
				else
				{
					maskgeometry = new swm.RectangleGeometry(rect.ToWpf());
				}
				var boundsgeometry = new swm.RectangleGeometry(bounds);
				maskgeometry = swm.Geometry.Combine(boundsgeometry, maskgeometry, swm.GeometryCombineMode.Exclude, null);
				var dr = new swm.GeometryDrawing(swm.Brushes.Black, null, maskgeometry);
				var db = new swm.DrawingBrush(dr);
				//db.Transform = new swm.TranslateTransform (0.5, 0.5);

				Control = drawingVisual.RenderOpen();
				PushGuideLines(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				Control.PushOpacityMask(db);
				Control.DrawImage(newbmp, bounds);
				Control.Pop();

				TransformStack.PushAll();
				ApplyClip();
			}
			else
			{
				// drawable
				if (brush == null || brush.Color.A < 1.0f)
					Widget.FillRectangle(Brushes.Black, rect);
			}
			if (brush != null)
			{
				Widget.FillRectangle(brush, rect);
			}
		}
	}
}
