using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using sw = System.Windows;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Platform.Wpf.Drawing
{
	public class GraphicsHandler : WidgetHandler<swm.DrawingContext, Graphics>, IGraphics
	{
		swm.Visual visual;
		swm.DrawingVisual drawingVisual;
		ImageInterpolation imageInterpolation;
        PixelOffsetMode pixelOffsetMode;
        double offset = 0.5;
        double inverseoffset = 0;

		Bitmap image;
		sw.Size? dpi;

		public GraphicsHandler ()
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

		public GraphicsHandler (swm.Visual visual, swm.DrawingContext context, sw.Rect? clipRect)
		{
			this.visual = visual;
			
			this.Control = context;

			//if (DPI != new sw.Size(1.0, 1.0))
			//	this.Control.PushTransform (new swm.ScaleTransform (DPI.Width, DPI.Height));
			var dpi = DPI;
			//offset = dpi.Width / 2;
			//this.Control.PushTransform (new swm.TranslateTransform (0, -0.5));
			if (clipRect != null) {
				var r = clipRect.Value;
				
				//PushGuideLines (r.X, r.Y, r.Width, r.Height);
				//r = new sw.Rect (r.X, r.Y, r.Width + 0.5, r.Height + 0.5);
				//this.Control.PushClip (new swm.RectangleGeometry (r));
				this.Control.PushClip (new swm.RectangleGeometry (new sw.Rect (r.X - 0.5, r.Y - 0.5, r.Width + 1, r.Height + 1)));

				this.Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { r.Left, r.Right }, new double[] { r.Top, r.Y + r.Bottom }));
			}
			this.ImageInterpolation = Eto.Drawing.ImageInterpolation.Default;
		}

        public bool IsRetained { get { return true; } }

		public void CreateFromImage (Bitmap image)
		{
			this.image = image;
			drawingVisual = new swm.DrawingVisual ();
			Control = drawingVisual.RenderOpen ();
			Control.DrawImage (image.ControlObject as swm.ImageSource, new sw.Rect (0, 0, image.Size.Width, image.Size.Height));

			visual = drawingVisual;
			this.ImageInterpolation = Eto.Drawing.ImageInterpolation.Default;
		}

		public sw.Size DPI
		{
			get
			{
				if (dpi == null) {
					var presentationSource = sw.PresentationSource.FromVisual (visual);
					if (presentationSource != null) {
						swm.Matrix m = presentationSource.CompositionTarget.TransformToDevice;
						dpi = new sw.Size (m.M11, m.M11);
					} else
						dpi = new sw.Size (1.0, 1.0);
				}
				return dpi.Value;
			}
		}

		public void PushGuideLines (double x, double y, double width, double height)
		{
            Control.PushGuidelineSet(new swm.GuidelineSet(new double[] { x, x + width }, new double[] { y, y + height }));
        }

        public void DrawRectangle(IPen pen, float x, float y, float width, float height)
		{
			PushGuideLines (x, y, width, height);
			Control.DrawRectangle (null, pen.ToWpf (), new sw.Rect (x + offset, y + offset, width, height));
			Control.Pop ();
		}


        public void DrawLine(IPen pen, float startx, float starty, float endx, float endy)
        {
			var wpfPen = pen.ToWpf ();
			Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { startx, endx }, new double[] { starty, endy }));
			Control.DrawLine (wpfPen, new sw.Point (startx + offset, starty + offset), new sw.Point (endx + offset, endy + offset));
			Control.Pop ();
		}

        public void FillRectangle(IBrush brush, float x, float y, float width, float height)
        {
			var wpfBrush = brush.ToWpf ();
			PushGuideLines (x, y, width, height);
			Control.DrawRectangle (wpfBrush, null, new sw.Rect (x + inverseoffset, y + inverseoffset, width, height));
			Control.Pop ();
        }

        public void DrawEllipse(IPen pen, float x, float y, float width, float height)
        {
            Control.DrawEllipse(null, pen.ToWpf (), new sw.Point(x + width / 2.0 + offset, y + height / 2.0 + offset), width / 2.0, height / 2.0);
        }

        public void FillEllipse(IBrush brush, float x, float y, float width, float height)
        {
            //PushGuideLines(x, y, width, height);
            Control.DrawEllipse(brush.ToWpf (), null, new sw.Point(x + width / 2.0 + inverseoffset, y + height / 2.0 + inverseoffset), width / 2.0, height / 2.0);
            //Control.Pop();
        }

        static swm.Geometry CreateArcDrawing(sw.Rect rect, double startDegrees, double sweepDegrees, bool closed)
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
                    ctx.BeginFigure(new sw.Point(xs, ys), false, false);
                ctx.ArcTo(new sw.Point(xe, ye), new sw.Size(dx, dy), 0, isLargeArc, sweepDirection, true, false);
                if (closed)
                    ctx.LineTo(centerPoint, true, true);
            }

            return streamGeom;
        }

        public void DrawArc(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var arc = CreateArcDrawing(new sw.Rect(x, y, width, height), startAngle, sweepAngle, false);
            Control.PushTransform(new swm.TranslateTransform(offset, offset));
            Control.DrawGeometry(null, pen.ToWpf (), arc);
            Control.Pop();
        }

        public void FillPie(IBrush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var arc = CreateArcDrawing(new sw.Rect(x, y, width, height), startAngle, sweepAngle, true);
            Control.PushTransform(new swm.TranslateTransform(inverseoffset, inverseoffset));
            Control.DrawGeometry(brush.ToWpf (), null, arc);
            Control.Pop();
        }

		public void FillPath(IBrush brush, GraphicsPath path)
		{
			var geometry = ((GraphicsPathHandler)path.Handler).Control;
			Control.DrawGeometry (brush.ToWpf (), null, geometry);
		}

        public void DrawPath(IPen pen, GraphicsPath path)
		{
			var geometry = ((GraphicsPathHandler)path.Handler).Control;
			Control.PushTransform (new swm.TranslateTransform (offset, offset));
			Control.DrawGeometry (null, pen.ToWpf (), geometry);
			Control.Pop ();
		}

		public void DrawImage (Image image, float x, float y)
		{
			var size = image.Size;
			DrawImage (image, x, y, size.Width, size.Height);
		}

		public void DrawImage (Image image, float x, float y, float width, float height)
		{
			var src = image.ToWpf ((int)Math.Max(width, height));
			//Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { x , x  + width }, new double[] { y , y + height }));
			Control.DrawImage (src, new sw.Rect (x + inverseoffset, y + inverseoffset, width, height));
			//Control.Pop ();
		}

		public void DrawImage (Image image, RectangleF source, RectangleF destination)
		{
			var src = image.ToWpf ();
			Control.PushClip (new swm.RectangleGeometry (destination.ToWpf ()));
			bool scaled = false;
			double scalex = 1.0;
			double scaley = 1.0;
			if (source.Size != destination.Size) {
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				Control.PushTransform (new swm.ScaleTransform (scalex, scaley));
				scaled = true;
			}
			Control.DrawImage (src, new sw.Rect((destination.X / scalex) - source.X, (destination.Y / scaley) - source.Y, destination.Width / scalex, destination.Height / scaley));
            // pop once for PushClip
			Control.Pop ();
            // pop again for PushTransform
			if (scaled)
				Control.Pop ();
		}

        public void DrawText(Font font, Color color, float x, float y, string text)
		{
			var fontHandler = font.Handler as FontHandler;
            if (fontHandler != null)
            {
                var brush = new swm.SolidColorBrush(color.ToWpf());
                var formattedText = new swm.FormattedText(text, CultureInfo.CurrentUICulture, sw.FlowDirection.LeftToRight, fontHandler.WpfTypeface, fontHandler.PixelSize, brush);
                Control.DrawText(formattedText, new sw.Point(x, y));
            }
		}

		public SizeF MeasureString (Font font, string text)
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

		public void Flush ()
		{
			if (Close ()) {
				Control = drawingVisual.RenderOpen ();
			}
		}

		bool Close ()
		{
			if (image != null) {
				Control.Close ();
				var handler = image.Handler as BitmapHandler;
				var bmp = handler.Control;
				var newbmp = new swmi.RenderTargetBitmap (bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, swm.PixelFormats.Pbgra32);
				newbmp.Render (visual);
				handler.SetBitmap (newbmp);
				return true;
			}
			return false;
		}

        public bool Antialias
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

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				Close ();
			base.Dispose (disposing);
		}

        TransformStack transformStack;

        TransformStack TransformStack
        {
            get
            {
                if (transformStack == null)
                    transformStack = new TransformStack(
                        this.Generator,
                        m =>
                        {
                            var matrix = (swm.Matrix)m.ControlObject;

                            var mt = new swm.MatrixTransform(matrix);

                            Control.PushTransform(mt);

                        },
                        // we ignore the m parameter below
                        // since wpf only supports popping
                        // the stack
                        () => Control.Pop());

                return transformStack;
            }
        }

        public void TranslateTransform(float dx, float dy)
        {
            TransformStack.TranslateTransform(dx, dy);
        }

        public void RotateTransform(float angle)
        {
            TransformStack.RotateTransform(angle);
        }

        public void ScaleTransform(float sx, float sy)
        {
            TransformStack.ScaleTransform(sx, sy);            
        }

        public void MultiplyTransform(IMatrix matrix)
        {
            TransformStack.MultiplyTransform(matrix);
        }

        public void SaveTransform()
        {
            TransformStack.SaveTransform();
        }

        public void RestoreTransform()
        {
            TransformStack.RestoreTransform();
        }
    }
}
