using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class GraphicsHandler : WidgetHandler<Cairo.Context, Graphics>, IGraphics
	{
		Pango.Context pangoContext;
		Gtk.Widget widget;
		Gdk.Drawable drawable;
		Image image;
		Cairo.ImageSurface surface;
		double offset = 0.5;
		double inverseoffset = 0;
		PixelOffsetMode pixelOffsetMode = PixelOffsetMode.None;

		public PixelOffsetMode PixelOffsetMode
		{
			get { return pixelOffsetMode; }
			set
			{
				pixelOffsetMode = value;
				offset = value == PixelOffsetMode.None ? 0.5 : 0;
				inverseoffset = value == PixelOffsetMode.None ? 0 : 0.5;
			}
		}

		public double Offset { get { return offset; } }

		public double InverseOffset { get { return inverseoffset; } }

		float DegreeToRadian (float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (Cairo.Context context, Pango.Context pangoContext)
		{
			this.Control = context;
			this.pangoContext = pangoContext;
		}
		
		public GraphicsHandler (Gtk.Widget widget, Gdk.Drawable drawable)
		{
			this.widget = widget;
			this.drawable = drawable;
			this.Control = Gdk.CairoHelper.Create (drawable);
		}

		public bool IsRetained { get { return false; } }

		public bool Antialias
		{
			get { return Control.Antialias != Cairo.Antialias.None; }
			set
			{
				if (value)
					Control.Antialias = Cairo.Antialias.Default;
				else
					Control.Antialias = Cairo.Antialias.None;
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get;
			set;
		}

		public Pango.Context PangoContext
		{
			get
			{
				if (pangoContext == null && widget != null) {
					pangoContext = widget.PangoContext;
				}
				return pangoContext;
			}
		}

		public void CreateFromImage (Bitmap image)
		{
			this.image = image;
			var handler = (BitmapHandler)image.Handler;

			if (handler.Alpha)
				surface = new Cairo.ImageSurface (Cairo.Format.Argb32, image.Size.Width, image.Size.Height);
			else
				surface = new Cairo.ImageSurface (Cairo.Format.Rgb24, image.Size.Width, image.Size.Height);
			Control = new Cairo.Context (surface);
			Control.Save ();
			Control.Rectangle (0, 0, image.Size.Width, image.Size.Height);
			Gdk.CairoHelper.SetSourcePixbuf (Control, handler.Control, 0, 0);
			Control.Operator = Cairo.Operator.Source;
			Control.Fill ();
			Control.Restore ();
		}
		
		public void Flush ()
		{
			if (image != null) {
				var handler = (BitmapHandler)image.Handler;
				Gdk.Pixbuf pb = handler.GetPixbuf (Size.MaxValue);
				if (pb != null) {

					surface.Flush ();
					var bd = handler.Lock ();
					unsafe {
						byte* srcrow = (byte*)surface.DataPtr;
						byte* destrow = (byte*)bd.Data;
						for (int y=0; y<image.Size.Height; y++) {
							uint* src = (uint*)srcrow;
							uint* dest = (uint*)destrow;
							for (int x=0; x<image.Size.Width; x++) {
								*dest = bd.TranslateArgbToData (*src);
								dest++;
								src++;
							}
							destrow += bd.ScanWidth;
							srcrow += surface.Stride;
						}
					}
					handler.Unlock (bd);
				}
			}
			if (Control != null) {
				((IDisposable)Control).Dispose ();
				if (surface != null) {
					this.Control = new Cairo.Context (surface);
				} else if (drawable != null) {
					this.Control = Gdk.CairoHelper.Create (drawable);
				}
			}
		}

		public void DrawLine (Color color, float startx, float starty, float endx, float endy)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			if (startx != endx || starty != endy) {
				// to draw a line, it must move..
				Control.MoveTo (startx + offset, starty + offset);
				Control.LineTo (endx + offset, endy + offset);
				Control.LineCap = Cairo.LineCap.Square;
				Control.LineWidth = 1.0;
				Control.Stroke ();
			} else {
				// to draw one pixel, we must fill it
				Control.Rectangle (startx, starty, 1, 1);
				Control.Fill ();
			}
			Control.Restore ();
		}

		public void DrawRectangle (Color color, float x, float y, float width, float height)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			Control.Rectangle (x + offset, y + offset, width, height);
			Control.LineWidth = 1.0;
			Control.Stroke ();
			Control.Restore ();
		}

		public void FillRectangle (Color color, float x, float y, float width, float height)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			Control.Rectangle (x + inverseoffset, y + inverseoffset, width, height);
			Control.Fill ();
			Control.Restore ();
		}

		public void DrawEllipse (Color color, float x, float y, float width, float height)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			Control.Translate (x + width / 2 + offset, y + height / 2 + offset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			Control.Arc (0, 0, radius, 0, 2 * Math.PI);
			Control.LineWidth = 1.0;
			Control.Stroke ();
			Control.Restore ();
		}

		public void FillEllipse (Color color, float x, float y, float width, float height)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			Control.Translate (x + width / 2 + inverseoffset, y + height / 2 + inverseoffset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			Control.Arc (0, 0, radius, 0, 2 * Math.PI);
			Control.Fill ();
			Control.Restore ();
		}

		public void DrawArc (Color color, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			Control.Translate (x + width / 2 + offset, y + height / 2 + offset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			Control.Arc (0, 0, radius, DegreeToRadian (startAngle), DegreeToRadian (startAngle + sweepAngle));
			Control.LineWidth = 1.0;
			Control.Stroke ();
			Control.Restore ();
		}

		public void FillPie (Color color, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.Save ();
			Control.Color = color.ToCairo ();
			Control.Translate (x + width / 2 + inverseoffset, y + height / 2 + inverseoffset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			Control.Arc (0, 0, radius, DegreeToRadian (startAngle), DegreeToRadian (startAngle + sweepAngle));
			Control.LineTo (0, 0);
			Control.Fill ();
			Control.Restore ();
		}

		public void FillPath (Color color, GraphicsPath path)
		{
			Control.Save ();
			Control.Translate (inverseoffset, inverseoffset);
			Control.Color = color.ToCairo ();
			var pathHandler = path.Handler as GraphicsPathHandler;
			pathHandler.Apply (this);
			Control.Fill ();
			Control.Restore ();
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			Control.Save ();
			Control.Translate (offset, offset);
			Control.Color = color.ToCairo ();
			var pathHandler = path.Handler as GraphicsPathHandler;
			pathHandler.Apply (this);
			Control.LineCap = Cairo.LineCap.Square;
			Control.LineWidth = 1.0;
			Control.Stroke ();
			Control.Restore ();
		}

        public void DrawImage(Image image, PointF point)
		{
			((IImageHandler)image.Handler).DrawImage (this, point.X, point.Y);
		}

		public void DrawImage (Image image, RectangleF rect)
		{
			((IImageHandler)image.Handler).DrawImage (this, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImage (Image image, RectangleF source, RectangleF destination)
		{
			((IImageHandler)image.Handler).DrawImage (this, source, destination);
		}

		public void DrawText (Font font, Color color, float x, float y, string text)
		{
			using (var layout = new Pango.Layout (PangoContext)) {
				layout.FontDescription = ((FontHandler)font.Handler).Control;
				layout.SetText (text);
				int width, height;
				layout.GetPixelSize (out width, out height);
				Control.Save ();
				Control.Color = color.ToCairo ();
				Control.MoveTo (x, y);
				Pango.CairoHelper.LayoutPath (Control, layout);
				Control.Fill ();
				Control.Restore ();
			}
		}

		public SizeF MeasureString (Font font, string text)
		{
			using (var layout = new Pango.Layout (PangoContext)) {
				layout.FontDescription = ((FontHandler)font.Handler).Control;
				layout.SetText (text);
				int width, height;
				layout.GetPixelSize (out width, out height);
				layout.Dispose ();
				return new SizeF (width, height);
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (image != null)
				Flush ();
			
			base.Dispose (disposing);
		}

		public void SetClip (RectangleF rect)
		{
			throw new NotImplementedException ();
		}

		public void TranslateTransform (float dx, float dy)
		{
			Control.Translate (dx, dy);
		}

		public RectangleF ClipBounds
		{
			get { throw new NotImplementedException (); }
		}

		public void RotateTransform (float angle)
		{
			Control.Rotate (Conversions.DegreesToRadians (angle));
		}

		public void ScaleTransform (float sx, float sy)
		{
			Control.Scale (sx, sy);
		}

		public void MultiplyTransform (IMatrix matrix)
		{
			Control.Transform ((Cairo.Matrix)matrix.ControlObject);
		}

		public void SaveTransform ()
		{
			Control.Save ();
		}

		public void RestoreTransform ()
		{
			Control.Restore ();
		}

        public void SetClip(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(float dx, float dy)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public RectangleF ClipBounds
        {
            get { throw new NotImplementedException(); }
        }

        public void RotateTransform(float angle)
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy)
        {
            throw new NotImplementedException();
        }

        public void MultiplyTransform(Matrix matrix)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public void SaveTransform()
        {
            throw new NotImplementedException();
        }

        public void RestoreTransform()
        {
            throw new NotImplementedException();
        }

        public void Clear(Color color)
        {
            throw new NotImplementedException();
        }
    }
}
