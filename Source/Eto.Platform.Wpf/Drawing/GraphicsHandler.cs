using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using sw = System.Windows;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class GraphicsHandler : WidgetHandler<swm.DrawingContext, Graphics>, IGraphics
	{
		swm.Visual visual;
		swm.DrawingVisual drawingVisual;

		Bitmap image;
		double? dpi;

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (swm.Visual visual, swm.DrawingContext context)
		{
			this.visual = visual;
			
			this.Control = context;

			if (DPI != 1.0)
				this.Control.PushTransform (new swm.ScaleTransform (DPI, DPI));
		}

		public void CreateFromImage (Bitmap image)
		{
			this.image = image;
			drawingVisual = new swm.DrawingVisual ();
			Control = drawingVisual.RenderOpen ();
			Control.DrawImage (image.ControlObject as swm.ImageSource, new sw.Rect (0, 0, image.Size.Width, image.Size.Height));

			visual = drawingVisual;
		}

		public double DPI
		{
			get
			{
				if (dpi == null) {
					swm.Matrix m = sw.PresentationSource.FromVisual (visual).CompositionTarget.TransformToDevice;
					dpi = 1 / m.M11;
				}
				return dpi.Value;
			}
		}

		swm.Pen GetPen (Color color, double thickness = 1)
		{
			return new swm.Pen (new swm.SolidColorBrush (Generator.Convert (color)), thickness);
		}

		void PushGuideLines (double x, double y, double width, double height)
		{
			Control.PushGuidelineSet(new swm.GuidelineSet(new double[] { x, x+width}, new double[] { y, y+height }));
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			var pen = GetPen(color);
			double t = pen.Thickness / 2;
			Control.DrawRectangle (null, pen, new sw.Rect (x + t, y + t, width - 1, height - 1));
		}

		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			var pen = GetPen (color);
			double t = pen.Thickness / 2;
			Control.DrawLine (pen, new sw.Point (startx + t, starty + t), new sw.Point (endx + t, endy + t));
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			PushGuideLines (x, y, width, height);
			var brush = new swm.SolidColorBrush (Generator.Convert (color));
			Control.DrawRectangle (brush, null, new sw.Rect (x, y, width, height));
			Control.Pop ();
		}

		public void FillPath (Color color, GraphicsPath path)
		{
			var figure = path.ControlObject as swm.PathFigure;
			var brush = new swm.SolidColorBrush (Generator.Convert (color));
			Control.DrawGeometry (brush, null, new swm.PathGeometry (new swm.PathFigure[] { figure }));
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			var figure = path.ControlObject as swm.PathFigure;
			var pen = GetPen (color);
			Control.DrawGeometry (null, pen, new swm.PathGeometry (new swm.PathFigure[] { figure }));
		}

		public void DrawImage (IImage image, int x, int y)
		{
			DrawImage (image, x, y, image.Size.Width, image.Size.Height);
		}

		public void DrawImage (IImage image, int x, int y, int width, int height)
		{
			var src = image.ControlObject as swm.ImageSource;
			Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { x , x  + width }, new double[] { y , y + height }));
			Control.DrawImage (src, new sw.Rect (x, y, width, height));
			Control.Pop ();
		}

		public void DrawImage (IImage image, Rectangle source, Rectangle destination)
		{
			var src = image.ControlObject as swm.ImageSource;
			//Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { destination.Left, destination.Right }, new double[] { destination.Top, destination.Bottom }));
			Control.PushClip (new swm.RectangleGeometry (Generator.Convert (destination)));
			if (source.Size != destination.Size) {
				// need to scale
			}
			Control.DrawImage (src, new sw.Rect(destination.X - source.X, destination.Y - source.Y, src.Width, src.Height));
			Control.Pop ();
			//Control.Pop ();
			//Control.Pop ();
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			var src = ((IconHandler)icon.Handler).Control;
			Control.PushGuidelineSet (new swm.GuidelineSet (new double[] { x, x + width }, new double[] { y, y + height }));
			Control.DrawImage (src, new sw.Rect (x, y, width, height));
			Control.Pop ();
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			//var formattedText = new swm.FormattedText(
			//Control.DrawText(
		}

		public SizeF MeasureString (Font font, string text)
		{
			return SizeF.Empty;
		}

		public Region ClipRegion
		{
			get; set; 
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

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				Close ();
			base.Dispose (disposing);
		}
	}
}
