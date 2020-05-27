using System;
using Eto.Drawing;
using System.Collections.Generic;
using GLib;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Drawing
{
	public class GraphicsHandler : WidgetHandler<Cairo.Context, Graphics>, Graphics.IHandler
	{
		Pango.Context pangoContext;
		readonly Gtk.Widget widget;
		Image image;
		Cairo.ImageSurface surface;
		RectangleF? clipBounds;
		IGraphicsPath clipPath;
		Cairo.Matrix currentTransform = new Cairo.Matrix();
		Stack<Cairo.Matrix> transforms;
		bool disposeControl = true;
		bool isOffset;
#if GTK2
		readonly Gdk.Drawable drawable;

		public GraphicsHandler(Gtk.Widget widget, Gdk.Drawable drawable, bool dispose = true)
		{
			this.widget = widget;
			this.drawable = drawable;
			this.Control = Gdk.CairoHelper.Create(drawable);
			disposeControl = dispose;
		}
#else
		public GraphicsHandler (Gtk.Widget widget, Gdk.Window drawable)
		{
			this.widget = widget;
			this.Control = Gdk.CairoHelper.Create (drawable);
		}

		public GraphicsHandler(Gtk.Widget widget, Cairo.Context context)
		{
			this.widget = widget;
			this.Control = context;
		}
#endif
		public GraphicsHandler(Cairo.Context context, Pango.Context pangoContext, bool dispose = true)
		{
			this.Control = context;
			this.pangoContext = pangoContext;
			this.disposeControl = dispose;
		}

		public GraphicsHandler()
		{
		}

		protected override bool DisposeControl { get { return disposeControl; } }

		static readonly object PixelOffsetMode_Key = new object();

		public PixelOffsetMode PixelOffsetMode
		{
			get { return Widget.Properties.Get<PixelOffsetMode>(PixelOffsetMode_Key); }
			set { Widget.Properties.Set(PixelOffsetMode_Key, value); }
		}

		void SetOffset(bool fill)
		{
			var requiresOffset = !fill && PixelOffsetMode == PixelOffsetMode.None;
			if (requiresOffset != isOffset)
			{
				ReverseAll();
				isOffset = requiresOffset;
				ApplyAll();
			}
		}

		public float PointsPerPixel { get { return PangoContext != null ? 72f / (float)Pango.CairoHelper.ContextGetResolution(PangoContext) : 72f / 96f; } }

		public bool IsRetained { get { return false; } }

		public bool AntiAlias
		{
			get { return Control.Antialias != Cairo.Antialias.None; }
			set
			{
				if (AntiAlias != value)
				{
					ReverseAll();
					Control.Antialias = value ? Cairo.Antialias.Default : Cairo.Antialias.None;
					ApplyAll();
				}
			}
		}

		static readonly object ImageInterpolation_Key = new object();

		public ImageInterpolation ImageInterpolation
		{
			get { return Widget.Properties.Get<ImageInterpolation>(ImageInterpolation_Key); }
			set { Widget.Properties.Set(ImageInterpolation_Key, value); }
		}

		public Pango.Context PangoContext
		{
			get
			{
				if (pangoContext == null && widget != null)
				{
					pangoContext = widget.PangoContext;
				}
				return pangoContext;
			}
		}

		public void CreateFromImage(Bitmap image)
		{
			this.image = image;
			var handler = (BitmapHandler)image.Handler;

			surface = handler.Surface;
			if (surface == null)
			{
				handler.FixupAlpha();
				var format = handler.Alpha ? Cairo.Format.Argb32 : Cairo.Format.Rgb24;
				surface = new Cairo.ImageSurface(format, image.Size.Width, image.Size.Height);
				Control = new Cairo.Context(surface);
				Control.Save();
				Control.Rectangle(0, 0, image.Size.Width, image.Size.Height);
				Gdk.CairoHelper.SetSourcePixbuf(Control, handler.Control, 0, 0);
				Control.Operator = Cairo.Operator.Source;
				Control.Fill();
				Control.Restore();
			}
			else
			{
				Control = new Cairo.Context(surface);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			currentTransform.InitIdentity();

			ApplyAll();
		}

		public void Flush() => Flush(true);

		void Flush(bool recreate)
		{
#if GTK2
			if (Control != null)
			{
				// Analysis disable once RedundantCast - backward compatibility
				((IDisposable)Control).Dispose();
				if (recreate)
				{
					if (surface != null)
					{
						Control = new Cairo.Context(surface);
					}
					else if (drawable != null)
					{
						Control = Gdk.CairoHelper.Create(drawable);
					}
				}
			}
#endif
			if (image != null)
			{
				var handler = (BitmapHandler)image.Handler;

				surface.Flush();
				handler.Surface = surface;
				if (!recreate)
					surface = null;
			}
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			SetOffset(false);
			Control.Save();
			Control.MoveTo(startx, starty);
			Control.LineTo(endx, endy);
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void DrawLines(Pen pen, IEnumerable<PointF> points)
		{
			SetOffset(false);
			Control.Save();
			bool first = true;
			foreach (var point in points)
			{
				if (!first)
				{
					Control.LineTo(point.X, point.Y);
					continue;
				}

				Control.MoveTo(point.X, point.Y);
				first = false;
			}
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void DrawPolygon(Pen pen, IEnumerable<PointF> points)
		{
			SetOffset(false);
			Control.Save();
			bool first = true;
			foreach (var point in points)
			{
				if (!first)
				{
					Control.LineTo(point.X, point.Y);
					continue;
				}

				Control.MoveTo(point.X, point.Y);
				first = false;
			}
			Control.ClosePath();
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			Control.Save();
			Control.Rectangle(x, y, width, height);
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			Control.Rectangle(x, y, width, height);
			Control.Save();
			brush.Apply(this);
			Control.Fill();
			Control.Restore();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			Control.Save();
			Control.Translate(x + width / 2, y + height / 2);
			double radius = Math.Max(width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale(1.0, height / width);
			else
				Control.Scale(width / height, 1.0);
			Control.Arc(0, 0, radius, 0, 2 * Math.PI);
			Control.Restore();
			Control.Save();
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			Control.Save();
			Control.Translate(x + width / 2, y + height / 2);
			double radius = Math.Max(width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale(1.0, height / width);
			else
				Control.Scale(width / height, 1.0);
			Control.Arc(0, 0, radius, 0, 2 * Math.PI);
			Control.Restore();
			Control.Save();
			brush.Apply(this);
			Control.Fill();
			Control.Restore();
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(false);
			Control.Save();
			Control.Translate(x + width / 2, y + height / 2);
			double radius = Math.Max(width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale(1.0, height / width);
			else
				Control.Scale(width / height, 1.0);
			if (sweepAngle < 0)
				Control.ArcNegative(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
			else
				Control.Arc(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
			Control.Restore();
			Control.Save();
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(true);
			Control.Save();
			Control.Translate(x + width / 2, y + height / 2);
			double radius = Math.Max(width / 2.0, height / 2.0);
			if (width > height)
				Control.Scale(1.0, height / width);
			else
				Control.Scale(width / height, 1.0);
			if (sweepAngle < 0)
				Control.ArcNegative(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
			else
				Control.Arc(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
			Control.LineTo(0, 0);
			Control.Restore();
			Control.Save();
			brush.Apply(this);
			Control.Fill();
			Control.Restore();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			SetOffset(true);
			Control.Save();
			path.Apply(Control);
			Control.FillRule = path.FillMode.ToCairo();
			brush.Apply(this);
			Control.Fill();
			Control.Restore();
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			SetOffset(false);
			Control.Save();
			path.Apply(Control);
			pen.Apply(this);
			Control.Stroke();
			Control.Restore();
		}

		public void DrawImage(Image image, float x, float y)
		{
			SetOffset(true);
			((IImageHandler)image.Handler).DrawImage(this, x, y);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			SetOffset(true);
			((IImageHandler)image.Handler).DrawImage(this, x, y, width, height);
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			SetOffset(true);
			((IImageHandler)image.Handler).DrawImage(this, source, destination);
		}

		Pango.Layout CreateLayout()
		{
			return PangoContext != null ? new Pango.Layout(PangoContext) : Pango.CairoHelper.CreateLayout(Control);
		}

		public void DrawText(Font font, Brush brush, float x, float y, string text)
		{
			var oldAA = AntiAlias;
			AntiAlias = true;
			SetOffset(true);
			using (var layout = CreateLayout())
			{
				font.Apply(layout);
				layout.SetText(text);
				Control.Save();
				brush.Apply(this);
				Control.MoveTo(x, y);
				Pango.CairoHelper.LayoutPath(Control, layout);
				Control.Fill();
				Control.Restore();
			}
			AntiAlias = oldAA;
		}
		public void DrawText(FormattedText formattedText, PointF location)
		{
			var oldAA = AntiAlias;
			AntiAlias = true;
			SetOffset(true);
			using (var layout = CreateLayout())
			{
				var handler = (FormattedTextHandler)formattedText.Handler;
				handler.Draw(this, layout, Control, location);
			}
			AntiAlias = oldAA;
		}

		public SizeF MeasureString(Font font, string text)
		{
			using (var layout = CreateLayout())
			{
				font.Apply(layout);
				layout.SetText(text);
				int width, height;
				layout.GetPixelSize(out width, out height);
				return new SizeF(width, height);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				ReverseAll();
			if (image != null)
			{
				Flush(false);
				image = null;
			}
			if (surface != null)
			{
				// Analysis disable once RedundantCast - backward compatibility
				((IDisposable)surface).Dispose();
				surface = null;
			}

			base.Dispose(disposing);
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			Control.Translate(offsetX, offsetY);
			var cairoMatrix = new Cairo.Matrix();
			cairoMatrix.InitTranslate(offsetX, offsetY);
			currentTransform = Cairo.Matrix.Multiply(cairoMatrix, currentTransform);
		}

		public void RotateTransform(float angle)
		{
			var radians = Conversions.DegreesToRadians(angle);
            Control.Rotate(radians);
			var cairoMatrix = new Cairo.Matrix();
			cairoMatrix.InitRotate(radians);
			currentTransform = Cairo.Matrix.Multiply(cairoMatrix, currentTransform);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.Scale(scaleX, scaleY);
			var cairoMatrix = new Cairo.Matrix();
			cairoMatrix.InitScale(scaleX, scaleY);
			currentTransform = Cairo.Matrix.Multiply(cairoMatrix, currentTransform);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			var cairoMatrix = matrix.ToCairo();
            Control.Transform(cairoMatrix);
			
			currentTransform = Cairo.Matrix.Multiply(cairoMatrix, currentTransform);
		}

		public void SaveTransform()
		{
			if (transforms == null)
				transforms = new Stack<Cairo.Matrix>();
			transforms.Push(currentTransform);
		}

		public void RestoreTransform()
		{
			if (transforms == null || transforms.Count == 0)
				throw new InvalidOperationException("No corresponding SaveTransform");
			ReverseTransform();
			currentTransform = transforms.Pop();
			ApplyTransform();
		}

		public IMatrix CurrentTransform
		{
			get { return currentTransform.ToEto(); }
		}

		void ReverseOffset()
		{
			if (isOffset)
				Control.Restore();
		}

		void ApplyOffset()
		{
			if (isOffset)
			{
				Control.Save();
				Control.Translate(0.5, 0.5);
			}
		}

		void ReverseClip()
		{
			if (clipBounds != null)
				Control.ResetClip();
		}

		void ApplyClip()
		{
			if (clipPath != null)
			{
				clipPath.Apply(Control);
				Control.Clip();
			}
			else if (clipBounds != null)
			{
				Control.Rectangle(clipBounds.Value.ToCairo());
				Control.Clip();
			}
		}

		void ReverseTransform()
		{
			Control.Restore();
		}

		void ApplyTransform()
		{
			Control.Save();
			Control.Transform(currentTransform);
		}

		void ReverseAll()
		{
			ReverseTransform();
			ReverseClip();
			ReverseOffset();
		}

		void ApplyAll()
		{
			ApplyOffset();
			ApplyClip();
			ApplyTransform();
		}

#if !GTK2
		public static bool GetClipRectangle(Cairo.Context cr, ref Gdk.Rectangle rect)
		{
#if GTKCORE
			return Gdk.CairoHelper.GetClipRectangle(cr, out rect);
#else
			IntPtr intPtr = Marshaller.StructureToPtrAlloc(rect);
			bool flag = NativeMethods.gdk_cairo_get_clip_rectangle((cr != null) ? cr.Handle : IntPtr.Zero, intPtr);
			bool result = flag;
			rect = (Gdk.Rectangle)Marshal.PtrToStructure(intPtr, typeof(Gdk.Rectangle));
			Marshal.FreeHGlobal(intPtr);
			return result;
#endif
		}
#endif

		public RectangleF ClipBounds
		{
			get
			{
#if GTK2
				var bounds = clipBounds ?? (widget != null ? (RectangleF)widget.Allocation.ToEto() : RectangleF.Empty);
				var matrix = Control.Matrix;
				if (matrix.IsIdentity())
					return bounds;
				var etoMatrix = matrix.ToEto();
				etoMatrix.Invert();
				return etoMatrix.TransformRectangle(bounds);
#else
				var bounds = clipBounds;
				if (bounds == null)
				{
					var rect = new Gdk.Rectangle();
					if (GetClipRectangle(Control, ref rect))
						bounds = rect.ToEto();
					else
						bounds = widget?.Allocation.ToEto() ?? RectangleF.Empty;
				}
				var matrix = Control.Matrix;
				if (matrix.IsIdentity())
					return bounds.Value;
				var etoMatrix = matrix.ToEto();
				etoMatrix.Invert();
				return etoMatrix.TransformRectangle(bounds.Value);
#endif
			}
		}

		public void SetClip(RectangleF rectangle)
		{
			ReverseTransform();
			ResetClip();
			clipBounds = currentTransform.ToEto().TransformRectangle(rectangle);
			clipPath = null;
			ApplyClip();
			ApplyTransform();
		}

		public void SetClip(IGraphicsPath path)
		{
			ReverseTransform();
			ResetClip();
			path = path.Clone();
			path.Transform(currentTransform.ToEto());
			clipPath = path;
			clipBounds = path.Bounds;
			ApplyClip();
			ApplyTransform();
		}

		public void ResetClip()
		{
			clipBounds = null;
			Control.ResetClip();
		}

		public void Clear(SolidBrush brush)
		{
			Control.Save();
			Control.Operator = Cairo.Operator.Clear;
			Control.Paint();
			Control.Restore();
			Control.Save();
			if (brush != null)
			{
				brush.Apply(this);
				Control.Fill();
				Control.Paint();
			}
			Control.Restore();
		}
	}
}