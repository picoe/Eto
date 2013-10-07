using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class GraphicsHandler : WidgetHandler<Cairo.Context, Graphics>, IGraphics
	{
		Pango.Context pangoContext;
		Gtk.Widget widget;
		Image image;
		Cairo.ImageSurface surface;
		double offset = 0.5;
		double inverseoffset = 0;
		PixelOffsetMode pixelOffsetMode = PixelOffsetMode.None;
		RectangleF? clipBounds;
		IGraphicsPath clipPath;
		bool disposeControl = true;
#if GTK2
		Gdk.Drawable drawable;

		public GraphicsHandler (Gtk.Widget widget, Gdk.Drawable drawable)
		{
			this.widget = widget;
			this.drawable = drawable;
			this.Control = Gdk.CairoHelper.Create (drawable);
		}
#else
		public GraphicsHandler (Gtk.Widget widget, Gdk.Window drawable)
		{
			this.widget = widget;
			this.Control = Gdk.CairoHelper.Create (drawable);
		}
#endif

		public GraphicsHandler (Cairo.Context context, Pango.Context pangoContext, bool dispose = true)
		{
			this.Control = context;
			this.pangoContext = pangoContext;
			this.disposeControl = dispose;
		}

		protected override bool DisposeControl { get { return disposeControl; } }

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

		public GraphicsHandler ()
		{
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
#if GTK2
			if (Control != null) {
				((IDisposable)Control).Dispose ();
				if (surface != null) {
					this.Control = new Cairo.Context (surface);
				} else if (drawable != null) {
					this.Control = Gdk.CairoHelper.Create (drawable);
				}
			}
#endif
		}

		public void DrawLine (Pen pen, float startx, float starty, float endx, float endy)
		{
			Control.MoveTo (startx + offset, starty + offset);
			Control.LineTo (endx + offset, endy + offset);
			pen.Apply (this);
		}

		public void DrawRectangle (Pen pen, float x, float y, float width, float height)
		{
			Control.Rectangle (x + offset, y + offset, width, height);
			pen.Apply (this);
		}

		public void FillRectangle (Brush brush, float x, float y, float width, float height)
		{
			Control.Rectangle (x + inverseoffset, y + inverseoffset, width, height);
			Control.Save ();
			brush.Apply (this);
			Control.Restore ();
		}

		public void DrawEllipse (Pen pen, float x, float y, float width, float height)
		{
			Control.Save ();
			Control.Translate (x + width / 2 + offset, y + height / 2 + offset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			Control.Arc (0, 0, radius, 0, 2 * Math.PI);
			Control.Restore ();
			pen.Apply (this);
		}

		public void FillEllipse (Brush brush, float x, float y, float width, float height)
		{
			Control.Save ();
			Control.Translate (x + width / 2 + inverseoffset, y + height / 2 + inverseoffset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			Control.Arc (0, 0, radius, 0, 2 * Math.PI);
			Control.Restore ();
			Control.Save ();
			brush.Apply (this);
			Control.Restore ();
		}

		public void DrawArc (Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.Save ();
			Control.Translate (x + width / 2 + offset, y + height / 2 + offset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			if (sweepAngle < 0)
				Control.ArcNegative(0, 0, radius, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle));
			else
				Control.Arc (0, 0, radius, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle));
			Control.Restore ();
			pen.Apply (this);
		}

		public void FillPie (Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.Save ();
			Control.Translate (x + width / 2 + inverseoffset, y + height / 2 + inverseoffset);
			double radius = Math.Max (width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale (1.0, height / width);
			else
				Control.Scale (width / height, 1.0);
			if (sweepAngle < 0)
				Control.ArcNegative(0, 0, radius, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle));
			else
				Control.Arc (0, 0, radius, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle));
			Control.LineTo (0, 0);
			Control.Restore ();
			Control.Save ();
			brush.Apply (this);
			Control.Restore ();
		}

		public void FillPath (Brush brush, IGraphicsPath path)
		{
			Control.Save ();
			Control.Translate (inverseoffset, inverseoffset);
			path.Apply (Control);
			Control.Restore ();
			Control.Save ();
			Control.FillRule = path.FillMode.ToCairo ();
			brush.Apply (this);
			Control.Restore ();
		}

		public void DrawPath (Pen pen, IGraphicsPath path)
		{
			Control.Save ();
			Control.Translate (offset, offset);
			path.Apply (Control);
			Control.Restore ();
			pen.Apply (this);
		}

		public void DrawImage (Image image, float x, float y)
		{
			((IImageHandler)image.Handler).DrawImage (this, x, y);
		}

		public void DrawImage (Image image, float x, float y, float width, float height)
		{
			((IImageHandler)image.Handler).DrawImage (this, x, y, width, height);
		}

		public void DrawImage (Image image, RectangleF source, RectangleF destination)
		{
			((IImageHandler)image.Handler).DrawImage (this, source, destination);
		}

		Pango.Layout CreateLayout ()
		{
			if (PangoContext != null)
				return new Pango.Layout (PangoContext);
			else
				return Pango.CairoHelper.CreateLayout (Control);
		}

		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			using (var layout = CreateLayout ()) {
				layout.FontDescription = ((FontHandler)font.Handler).Control;
				layout.SetText (text);
				Control.Save ();
				Control.Color = brush.Color.ToCairo ();
				Control.MoveTo (x, y);
				Pango.CairoHelper.LayoutPath (Control, layout);
				Control.Fill ();
				Control.Restore ();
			}
		}

		public SizeF MeasureString (Font font, string text)
		{
			using (var layout = CreateLayout ()) {
				layout.FontDescription = ((FontHandler)font.Handler).Control;
				layout.SetText (text);
				int width, height;
				layout.GetPixelSize (out width, out height);
				return new SizeF (width, height);
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (image != null)
			{
				Flush();
				image = null;
			}
			if (surface != null)
			{
				surface.Dispose();
				surface = null;
			}

			base.Dispose (disposing);
		}

		public void TranslateTransform (float offsetX, float offsetY)
		{
			Control.Translate (offsetX, offsetY);
		}

		public void RotateTransform (float angle)
		{
			Control.Rotate (Conversions.DegreesToRadians (angle));
		}

		public void ScaleTransform (float scaleX, float scaleY)
		{
			Control.Scale (scaleX, scaleY);
		}

		public void MultiplyTransform (IMatrix matrix)
		{
			Control.Transform (matrix.ToCairo ());
		}

		public void SaveTransform ()
		{
			ReverseClip ();
			Control.Save ();
			ApplyClip ();
		}

		public void RestoreTransform ()
		{
			Control.Restore ();
			ApplyClip ();
		}

		void ReverseClip ()
		{
			if (clipBounds != null)
				Control.ResetClip ();
		}

		void ApplyClip ()
		{
			if (clipPath != null) {
				clipPath.Apply (Control);
				Control.Clip ();
			} else if (clipBounds != null) {
				Control.Rectangle (clipBounds.Value.ToCairo ());
				Control.Clip ();
			}
		}

		public RectangleF ClipBounds
		{
			get { return clipBounds ?? RectangleF.Empty; }
		}

		public void SetClip (RectangleF rectangle)
		{
			ResetClip ();
			clipBounds = rectangle;
			ApplyClip ();
		}

		public void SetClip (IGraphicsPath path)
		{
			ResetClip ();
			clipPath = path;
			clipBounds = path.Bounds;
			ApplyClip ();
		}

		public void ResetClip ()
		{
			clipBounds = null;
			Control.ResetClip ();
		}

		public void Clear (SolidBrush brush)
		{
			Control.Save ();
			Control.Operator = Cairo.Operator.Clear;
			Control.Paint ();
			Control.Restore ();
			Control.Save ();
			if (brush != null) {
				brush.Apply (this);
				Control.Paint ();
			}
			Control.Restore ();
		}
	}
}
