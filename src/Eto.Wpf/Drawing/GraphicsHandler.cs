using System;
using swm = System.Windows.Media;
using sw = System.Windows;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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
        swm.DrawingGroup group;
        swm.DrawingVisual drawingVisual;
        RectangleF? clipBounds;
        RectangleF initialClip;
        swm.PathGeometry clipPath;
        sw.Rect bounds;
        readonly bool disposeControl;
		bool isOffset;

		Bitmap image;
        swm.DrawingContext baseContext;

        public GraphicsHandler()
        {
        }

		static readonly object PixelOffsetMode_Key = new object();

        public PixelOffsetMode PixelOffsetMode
        {
            get { return Widget != null ? Widget.Properties.Get<PixelOffsetMode>(PixelOffsetMode_Key) : default(PixelOffsetMode); }
            set { Widget.Properties.Set(PixelOffsetMode_Key, value); }
        }

		void SetOffset(bool fill)
		{
			var requiresOffset = !fill && PixelOffsetMode == PixelOffsetMode.None;
			if (requiresOffset != isOffset)
			{
				RewindAll();
				isOffset = requiresOffset;
				ApplyAll();
			}
		}

        public float PointsPerPixel
		{
			get { return 72f / 96f; }
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

			Control.PushClip(new swm.RectangleGeometry(bounds));
        }

		public bool IsRetained { get { return true; } }

		public void CreateFromImage(Bitmap image)
		{
			this.image = image;
			initialClip = new RectangleF(0, 0, image.Width, image.Height);
			bounds = initialClip.ToWpf();
			visual = drawingVisual = new swm.DrawingVisual();
			Control = drawingVisual.RenderOpen();
			Control.DrawImage(image.ToWpf(1), bounds);
        }

		protected override void Initialize()
		{
			base.Initialize();

			ApplyAll();
		}

		static readonly object DPI_Key = new object();

        public double DPI
		{
			get
			{
				return Widget.Properties.Create<double>(DPI_Key, () =>
				{
					if (Win32.PerMonitorDpiSupported)
					{
						var window = visual.GetVisualParent<sw.Window>();
						if (window != null)
							return Win32.GetWindowDpi(new sw.Interop.WindowInteropHelper(window).Handle) / 96.0;
					}
					var presentationSource = sw.PresentationSource.FromVisual(visual);
					if (presentationSource != null && presentationSource.CompositionTarget != null)
					{
						swm.Matrix m = presentationSource.CompositionTarget.TransformToDevice;
						return m.M11;
					}
					return 1.0;
				});
			}
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			Control.DrawRectangle(null, pen.ToWpf(true), WpfExtensions.NormalizedRect(x, y, width, height));
		}


		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			SetOffset(false);
			var wpfPen = pen.ToWpf(true);
			Control.DrawLine(wpfPen, new sw.Point(startx, starty), new sw.Point(endx, endy));
		}

		public void DrawLines(Pen pen, IEnumerable<PointF> points)
		{
			using (var path = new GraphicsPath())
			{
				path.AddLines(points);
				DrawPath(pen, path);
			}
		}

		public void DrawPolygon(Pen pen, IEnumerable<PointF> points)
		{
			using (var path = new GraphicsPath())
			{
				path.AddLines(points);
				path.CloseFigure();
				DrawPath(pen, path);
			}
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			var wpfBrush = brush.ToWpf();
			Control.DrawRectangle(wpfBrush, null, WpfExtensions.NormalizedRect(x, y, width, height));
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			Control.DrawEllipse(null, pen.ToWpf(true), new sw.Point(x + width / 2.0, y + height / 2.0), width / 2.0, height / 2.0);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			Control.DrawEllipse(brush.ToWpf(), null, new sw.Point(x + width / 2.0, y + height / 2.0), width / 2.0, height / 2.0);
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
			SetOffset(false);
			if (sweepAngle >= 360f)
				DrawEllipse(pen, x, y, width, height);
			else
			{
				var arc = CreateArcDrawing(WpfExtensions.NormalizedRect(x, y, width, height), startAngle, sweepAngle, false);
				Control.DrawGeometry(null, pen.ToWpf(true), arc);
			}
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(true);
			if (sweepAngle >= 360f)
				FillEllipse(brush, x, y, width, height);
			else
			{
				var arc = CreateArcDrawing(WpfExtensions.NormalizedRect(x, y, width, height), startAngle, sweepAngle, true);
				Control.DrawGeometry(brush.ToWpf(), null, arc);
			}
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			SetOffset(true);
			Control.DrawGeometry(brush.ToWpf(), null, path.ToWpf());
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			SetOffset(false);
			Control.DrawGeometry(null, pen.ToWpf(true), path.ToWpf());
		}

		public void DrawImage(Image image, float x, float y)
		{
			var size = image.Size;
			DrawImage(image, x, y, size.Width, size.Height);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			SetOffset(true);
			var src = image.ToWpf((float)DPI, new Size((int)width, (int)height));
			var size = new SizeF((float)src.PixelWidth, (float)src.PixelHeight) / (float)DPI;

			if ((ImageInterpolation == ImageInterpolation.High || ImageInterpolation == ImageInterpolation.Default)
                && (width != size.Width || height != size.Height))
            {
                // better image quality by using transformed bitmap, plus it is actually faster
                src = new swmi.TransformedBitmap(
                    src,
                    new swm.ScaleTransform(width / size.Width * 96 / src.DpiX, height / size.Height * 96 / src.DpiY, 0, 0)
                    );
            }
            Control.DrawImage(src, WpfExtensions.NormalizedRect(x, y, width, height));
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			SetOffset(true);
			var src = image.ToWpf((float)DPI);
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

		public void DrawText(FormattedText formattedText, PointF location)
		{
			SetOffset(true);
			if (formattedText.Handler is FormattedTextHandler handler)
			{
				handler.DrawText(this, location);
			}
		}

		public void DrawText(Font font, Brush b, float x, float y, string text)
		{
			SetOffset(true);
			var fontHandler = font.Handler as FontHandler;
			if (fontHandler != null)
			{
				var brush = b.ToWpf();
#pragma warning disable CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'
				var formattedText = new swm.FormattedText(text, CultureInfo.CurrentUICulture, sw.FlowDirection.LeftToRight, fontHandler.WpfTypeface, fontHandler.WpfSize, brush);
#pragma warning restore CS0618 // Type or member is obsolete
				if (fontHandler.WpfTextDecorationsFrozen != null)
					formattedText.SetTextDecorations(fontHandler.WpfTextDecorationsFrozen, 0, text.Length);
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
#pragma warning disable CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'
				var formattedText = new swm.FormattedText(text, CultureInfo.CurrentUICulture, sw.FlowDirection.LeftToRight, fontHandler.WpfTypeface, fontHandler.WpfSize, brush);
#pragma warning restore CS0618 // Type or member is obsolete
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
			CloseGroup();
			if (image != null)
			{
				Control.Close();
				var handler = (BitmapHandler)image.Handler;
				var bmp = image.ToWpf();
				var newbmp = new swmi.RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, swm.PixelFormats.Pbgra32);
				newbmp.RenderWithCollect(visual);
				handler.SetBitmap(newbmp);
				return true;
			}
			return false;
		}

		public bool AntiAlias
		{
			get
			{
				switch (swm.RenderOptions.GetEdgeMode((sw.DependencyObject)group ?? visual))
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
				if (value != AntiAlias)
				{
					CloseGroup();
					if (value != AntiAlias)
					{
						CreateGroup();
						swm.RenderOptions.SetEdgeMode(group, value ? swm.EdgeMode.Unspecified : swm.EdgeMode.Aliased);
					}
				}
			}
		}

		static readonly object ImageInterpolation_Key = new object();

		public ImageInterpolation ImageInterpolation
		{
			get { return Widget.Properties.Get<ImageInterpolation>(ImageInterpolation_Key); }
			set
			{
				if (value != ImageInterpolation)
				{
					CloseGroup();
					CreateGroup();
					Widget.Properties.Set(ImageInterpolation_Key, value);
					swm.RenderOptions.SetBitmapScalingMode(group, value.ToWpf());
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
			base.Dispose(disposing);
		}

		void CreateGroup()
		{
			if (baseContext == null)
				baseContext = Control;
			RewindAll();
			group = new swm.DrawingGroup();
			Control = group.Open();
			ApplyAll();
		}

		void CloseGroup()
		{
			if (group != null && baseContext != null)
			{
				RewindAll();
				Control.Close();
				baseContext.DrawDrawing(group);
				Control = baseContext;
				group = null;
				baseContext = null;
				ApplyAll();
			}
		}

		TransformStack transforms;

		TransformStack Transforms
		{
			get
			{
				if (transforms == null)
					transforms = new TransformStack(
						m => Control.PushTransform(m.ToWpfTransform()),
						() => Control.Pop()
						);

				return transforms;
			}
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			Transforms.TranslateTransform(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			Transforms.RotateTransform(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Transforms.ScaleTransform(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Transforms.MultiplyTransform(matrix);
		}

		public void SaveTransform()
		{
			Transforms.SaveTransform();
		}

		public void RestoreTransform()
		{
			Transforms.RestoreTransform();
		}

		public IMatrix CurrentTransform
		{
			get { return transforms.Current != null ? transforms.Current.Clone() : Matrix.Create(); }
		}

		void RewindOffset()
		{
			if (isOffset)
				Control.Pop();
		}

		void ApplyOffset()
		{
			if (isOffset)
				Control.PushTransform(new swm.TranslateTransform(0.5, 0.5));
		}

		protected void RewindClip()
		{
			if (clipBounds != null || clipPath != null)
				Control.Pop();
		}

		protected void ApplyClip()
		{
			if (clipPath != null)
			{
				Control.PushClip(clipPath);
			}
			else if (clipBounds != null)
			{
				Control.PushClip(new swm.RectangleGeometry(clipBounds.Value.ToWpf()));
			}
		}

		void RewindAll()
		{
			RewindTransform();
			RewindClip();
			RewindOffset();
		}

		void ApplyAll()
		{
			ApplyOffset();
			ApplyClip();
			ApplyTransform();
		}

		void RewindTransform()
		{
			if (transforms != null)
				transforms.PopAll();
		}

		void ApplyTransform()
		{
			if (transforms != null)
				transforms.PushAll();
		}


		public RectangleF ClipBounds
		{
			get
			{
				var translatedClip = clipBounds ?? initialClip;
				if (transforms != null && transforms.Current != null)
				{
					var invertedTransform = transforms.Current.Clone();
					invertedTransform.Invert();
					translatedClip = invertedTransform.TransformRectangle(translatedClip);
				}
				return translatedClip;
			}
		}

		public void SetClip(RectangleF rectangle)
		{
			RewindTransform();
			RewindClip();
			if (transforms != null && transforms.Current != null)
				clipBounds = transforms.Current.TransformRectangle(rectangle);
			else
				clipBounds = rectangle;
			clipPath = null;
			ApplyClip();
			ApplyTransform();
		}

		public void SetClip(IGraphicsPath path)
		{
			RewindTransform();
			RewindClip();
			path = path.Clone();
			if (transforms != null && transforms.Current != null)
				path.Transform(transforms.Current);
            clipPath = path.ToWpf(); // require a clone so changes to path don't affect current clip
			clipBounds = clipPath.Bounds.ToEtoF();
			ApplyClip();
			ApplyTransform();
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
				CloseGroup();
				// bitmap
				Control.Close();
				var newbmp = new swmi.RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, swm.PixelFormats.Pbgra32);
				newbmp.RenderWithCollect(visual);

				swm.Geometry maskgeometry;
				if (clipPath != null)
				{
					maskgeometry = clipPath;
				}
				else
				{
					maskgeometry = new swm.RectangleGeometry(rect.ToWpf());
				}
				var boundsgeometry = new swm.RectangleGeometry(bounds);
				maskgeometry = swm.Geometry.Combine(boundsgeometry, maskgeometry, swm.GeometryCombineMode.Exclude, null);
				var dr = new swm.GeometryDrawing(swm.Brushes.Black, null, maskgeometry);
				var db = new swm.DrawingBrush(dr);

				visual = drawingVisual = new swm.DrawingVisual();
				Control = drawingVisual.RenderOpen();
				Control.PushOpacityMask(db);
				Control.DrawImage(newbmp, bounds);
				Control.Pop();

				ApplyAll();
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
