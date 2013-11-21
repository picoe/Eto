using System;
using System.Collections.Generic;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using swf = System.Windows.Forms;
using Size2 = SharpDX.DrawingSize; // This has been changed to Size2 in more recent versions of SharpDx

namespace Eto.Platform.Direct2D.Drawing
{
	public class GraphicsHandler : WidgetHandler<sd.RenderTarget, Graphics>, IGraphics2
	{
		#region Constructors

		public GraphicsHandler(sd.RenderTarget target)
		{
			this.Control = target;
		}

		public GraphicsHandler()
		{
		}

		#endregion

		private sd.Brush GetBrush(Pen pen)
		{
			var color = pen.Color;
			return new sd.SolidColorBrush(this.Control, color.ToSD());
		}

		private sd.Brush GetBrush(Color color)
		{
			return (sd.Brush)Brushes.Cached(color, this.Generator).ControlObject;
		}

		private sd.Ellipse GetEllipse(float x, float y, float width, float height)
		{
			var rx = width / 2f;
			var ry = height / 2f;

			return new sd.Ellipse(center: new s.DrawingPointF(x + rx, y + ry), radiusX: rx, radiusY: ry);
		}

		public bool AntiAlias { get; set; } // not used

		public ImageInterpolation ImageInterpolation { get; set; } // TODO

		public bool IsRetained
		{
			get { return false; }
		}

		public double DpiX
		{
			get { return Control.DotsPerInch.Width; }
		}

		public double DpiY
		{
			get { return Control.DotsPerInch.Height; }
		}

		public RectangleF ClipBounds
		{
			get { throw new NotImplementedException(); }
		}

		void IGraphics.SetClip(RectangleF rect)
		{
			throw new NotImplementedException();
		}

		void IGraphics.SetClip(IGraphicsPath path)
		{
			throw new NotImplementedException();
		}

		void IGraphics.ResetClip()
		{
			throw new NotImplementedException();
		}

		void IGraphics.TranslateTransform(float dx, float dy)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					s.Matrix3x2.Translation(dx, dy),
					Control.Transform);
		}

		void IGraphics.RotateTransform(float angle)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					s.Matrix3x2.Rotation((float)(Conversions.DegreesToRadians(angle))),
					Control.Transform);
		}

		void IGraphics.ScaleTransform(float sx, float sy)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					s.Matrix3x2.Scaling(sx, sy),
					Control.Transform);
		}

		void IGraphics.MultiplyTransform(IMatrix matrix)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					(s.Matrix3x2)matrix.ControlObject,
					Control.Transform);
		}

		void IGraphics.CreateFromImage(Bitmap image)
		{
			throw new NotImplementedException();
		}

		void IGraphics.CreateFromDrawable(Eto.Forms.Drawable drawable)
		{
			var o = drawable.ControlObject as swf.Control;

			var etoDrawable = o as Eto.Platform.Windows.DrawableHandler.EtoDrawable;
			if (etoDrawable != null)
			{
				etoDrawable.SetStyle(swf.ControlStyles.SupportsTransparentBackColor | swf.ControlStyles.DoubleBuffer, false);
				etoDrawable.SetStyle(swf.ControlStyles.AllPaintingInWmPaint | swf.ControlStyles.Opaque, true);
			}

			var renderProp = new sd.RenderTargetProperties
			{
				DpiX = 0,
				DpiY = 0,
				MinLevel = sd.FeatureLevel.Level_10,
				PixelFormat = new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Premultiplied),
				Type = sd.RenderTargetType.Hardware,
				Usage = sd.RenderTargetUsage.None
			};

			//set hwnd target properties (permit to attach Direct2D to window)
			var winProp = new sd.HwndRenderTargetProperties
			{
				Hwnd = o.Handle,
				PixelSize = new Size2(o.ClientSize.Width, o.ClientSize.Height),
				PresentOptions = sd.PresentOptions.Immediately
			};

			//target creation
			var target = new sd.WindowRenderTarget(SDFactory.D2D1Factory, renderProp, winProp);

			o.SizeChanged += (s, e) => {
				target.Resize(new Size2(o.ClientSize.Width, o.ClientSize.Height));
				drawable.Invalidate();
			};

			this.Control = target;
		}

		sd.Geometry GetGeometry(IGraphicsPath path)
		{
			throw new NotImplementedException(); // TODO
		}

		void IGraphics.DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			throw new NotImplementedException();
		}

		public SizeF MeasureString(Font font, string text)
		{
			throw new NotImplementedException();
		}

		void IGraphics.Flush()
		{
			Control.Flush();
		}

		private Stack<s.Matrix3x2> transformStack = null;

		void IGraphics.SaveTransform()
		{
			if (transformStack == null)
				transformStack = new Stack<s.Matrix3x2>();

			transformStack.Push(Control.Transform);
		}

		void IGraphics.RestoreTransform()
		{
			Control.Transform =
				transformStack.Pop();
		}

#if TODO
		void IGraphics.Clear(Color color)
		{
			Control.Clear(color.ToWpf());
		}
#endif

		public PixelOffsetMode PixelOffsetMode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		void IGraphics.DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawRectangle(
				new s.DrawingRectangleF(x, y, width, height),
				GetBrush(pen));
		}

		void IGraphics.FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			Control.FillRectangle(
				new s.DrawingRectangleF(x, y, width, height),
				brush.ControlObject as sd.Brush);
		}

		void IGraphics.DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			Control.DrawLine(
				new s.DrawingPointF(startx, starty),
				new s.DrawingPointF(endx, endy),
				GetBrush(pen));
		}

		void IGraphics.FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			Control.FillEllipse(
				GetEllipse(x, y, width, height),
				brush.ControlObject as sd.Brush);
		}

		void IGraphics.DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawEllipse(
				GetEllipse(x, y, width, height),
				GetBrush(pen));
		}

		void IGraphics.DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		void IGraphics.FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		void IGraphics.FillPath(Brush brush, IGraphicsPath path)
		{
			Control.FillGeometry(GetGeometry(path), (sd.Brush)brush.ControlObject);
		}

		void IGraphics.DrawPath(Pen pen, IGraphicsPath path)
		{
			Control.DrawGeometry(GetGeometry(path), GetBrush(pen));
		}

		void IGraphics.DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			throw new NotImplementedException();
		}

		void IGraphics.DrawImage(Image image, float x, float y)
		{
			throw new NotImplementedException();
		}

		void IGraphics.DrawImage(Image image, float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public object ControlObject
		{
			get { throw new NotImplementedException(); }
		}

		public void Clear(SolidBrush brush)
		{
			Control.Clear(brush.Color.ToSD());
		}

		public void BeginDrawing()
		{
			Control.BeginDraw();
			// BUGBUG: Remove this once Clear() works.
			Control.Clear(Colors.Black.ToSD());
		}

		public void EndDrawing()
		{
			Control.EndDraw();
		}
	}
}
