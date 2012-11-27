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

		Bitmap image;
		sw.Size? dpi;

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (swm.Visual visual, swm.DrawingContext context, sw.Rect? clipRect)
		{
			this.visual = visual;
			
			this.Control = context;

			if (DPI != new sw.Size(1.0, 1.0))
				this.Control.PushTransform (new swm.ScaleTransform (DPI.Width, DPI.Height));

			if (clipRect != null)
				this.Control.PushClip (new swm.RectangleGeometry (clipRect.Value));
			this.ImageInterpolation = Eto.Drawing.ImageInterpolation.Default;
		}

        public bool IsRetainedMode { get { return true; } }

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
						dpi = new sw.Size (1 / m.M11, 1 / m.M11);
					}
					else
						dpi = new sw.Size (1.0, 1.0);
				}
				return dpi.Value;
			}
		}

		swm.Pen GetPen (Color color, double thickness = 1)
		{
			return new swm.Pen (new swm.SolidColorBrush (color.ToWpf ()), thickness);
		}

		void PushGuideLines (double x, double y, double width, double height)
		{
			Control.PushGuidelineSet(new swm.GuidelineSet(new double[] { x, x+width}, new double[] { y, y+height }));
		}

        // Helper method
        private void DrawRectangle(swm.Pen pen, float x, float y, float width, float height)
        {
            double t = pen.Thickness / 2;
            Control.DrawRectangle(null, pen, new sw.Rect(x + t, y + t, width - 1, height - 1));
        }

        public void DrawRectangle(Color color, int x, int y, int width, int height)
		{
            DrawRectangle(GetPen(color), x, y, width, height);
		}

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            DrawRectangle(pen.ControlObject as swm.Pen, x, y, width, height);            
        }

        // Helper method
        private void DrawLine(swm.Pen pen, float startx, float starty, float endx, float endy)
        {
            double t = pen.Thickness / 2;
            Control.DrawLine(pen, new sw.Point(startx + t, starty + t), new sw.Point(endx + t, endy + t));
        }

        public void DrawLine(Color color, int startx, int starty, int endx, int endy)
		{
			var pen = GetPen (color);
            DrawLine(pen, startx, starty, endx, endy);
		}

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen.ControlObject as swm.Pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        // Helper method
        private void FillRectangle(swm.Brush brush, float x, float y, float width, float height)
        {
            PushGuideLines(x, y, width, height);
            Control.DrawRectangle(brush, null, new sw.Rect(x, y, width, height));
            Control.Pop();
        }

        public void FillRectangle(Color color, float x, float y, float width, float height)
		{
            var brush = new swm.SolidColorBrush(color.ToWpf());
            FillRectangle(brush, x, y, width, height);
		}

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush.ControlObject as swm.Brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            FillRectangle(brush.ControlObject as swm.Brush, x, y, width, height);
        }

		public void DrawEllipse (Color color, int x, int y, int width, int height)
		{
			var pen = GetPen (color);
			double t = pen.Thickness / 2;
			Control.DrawEllipse (null, pen, new sw.Point(x + width / 2.0, y + height / 2.0), width / 2.0, height / 2.0);
		}

		public void FillEllipse (Color color, int x, int y, int width, int height)
		{
			PushGuideLines (x, y, width, height);
			var brush = new swm.SolidColorBrush (color.ToWpf ());
			Control.DrawEllipse (brush, null, new sw.Point (x + width / 2.0, y + height / 2.0), width / 2.0, height / 2.0);
			Control.Pop ();
		}

        // Helper method
        private void FillPath(swm.Brush brush, GraphicsPath path)
        {
            var geometry = ((GraphicsPathHandler)path.Handler).Control;
            Control.DrawGeometry(brush, null, geometry);
        }

        public void FillPath(Color color, GraphicsPath path)
		{
            var brush = new swm.SolidColorBrush(color.ToWpf());
            FillPath(brush, path);
		}

        public void FillPath(Brush brush, GraphicsPath path)
        {
            FillPath(brush.ControlObject as swm.Brush, path);
        }

        // Helper method
        private void DrawPath(swm.Pen pen, GraphicsPath path)
        {
            var geometry = ((GraphicsPathHandler)path.Handler).Control;
            Control.DrawGeometry(null, pen, geometry);
        }

        public void DrawPath(Color color, GraphicsPath path)
		{
            var pen = GetPen(color);
            DrawPath(pen, path);
		}

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            DrawPath(pen.ControlObject as swm.Pen, path);
        }

        public void DrawImage(Image image, int x, int y)
		{
			DrawImage (image, x, y, image.Size.Width, image.Size.Height);
		}

		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			var src = image.ControlObject as swm.ImageSource;
			Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { x , x  + width }, new double[] { y , y + height }));
			Control.DrawImage (src, new sw.Rect (x, y, width, height));
			Control.Pop ();
		}

		public void DrawImage (Image image, RectangleF source, RectangleF destination)
		{
			var src = image.ControlObject as swm.ImageSource;
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

        public void DrawImage(Image image, PointF p)
        {
            DrawImage(image, (int)p.X, (int)p.Y); // TODO: fix
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            // TODO: Fix
            DrawImage(image, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			var src = ((IconHandler)icon.Handler).Control;
			Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { x, x + width }, new double[] { y, y + height }));
			Control.DrawImage (src, new sw.Rect (x, y, width, height));
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
				switch (swm.RenderOptions.GetEdgeMode (visual)) {
					case swm.EdgeMode.Aliased:
						return false;
					case swm.EdgeMode.Unspecified:
						return true;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				swm.RenderOptions.SetEdgeMode (visual, value ? swm.EdgeMode.Unspecified : swm.EdgeMode.Aliased);
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return imageInterpolation; }
			set {
				imageInterpolation = value;
				swm.RenderOptions.SetBitmapScalingMode (visual, value.ToWpf ());
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				Close ();
			base.Dispose (disposing);
		}

        public void SetClip(RectangleF rect)
        {
            // should not be called since Wpf is retained mode.
            throw new InvalidOperationException(); 
        }

        public RectangleF ClipBounds
        {
            get
            {             
                // should not be called since Wpf is retained mode.
                throw new InvalidOperationException();
            }
        }

        private TransformStack transformStack;

        private TransformStack TransformStack
        {
            get
            {
                if (transformStack == null)
                    transformStack = new TransformStack(
                        this.Generator,
                        m =>
                        {
                            var matrixHandler =
                                m.Handler as MatrixHandler;

                            var matrix =
                                (swm.Matrix)
                                matrixHandler.Control;

                            var mt = new swm.MatrixTransform(
                                matrix);

                            Control.PushTransform(mt);

                        },
                        () => Control.Pop());

                return transformStack;
            }
        }

        public Matrix Transform
        {
            get
            {
                return TransformStack.Current;
            }
            set
            {
                TransformStack.Transform = value;
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

        public void MultiplyTransform(Matrix matrix)
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

        public void Clear(Color color)
        {
            // TODO
        }
    }
}
