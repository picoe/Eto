using System;
using System.Collections.Generic;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using Eto.Forms;
using System.Diagnostics;
#if WINFORMS
using Eto.WinForms.Forms.Controls;
#endif

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public partial class GraphicsHandler : WidgetHandler<sd.RenderTarget, Graphics>, Graphics.IHandler
	{
		bool hasBegan;
		bool rectClip;
		bool disposeControl = true;
		Bitmap image;
		float offset = 0.5f;
		float fillOffset;
		sd.Layer helperLayer;
		sd.Geometry clipGeometry;
		sd.LayerParameters? clipParams;
		RectangleF clipBounds;
#if WINFORMS
		DrawableHandler drawable;
#endif
		public float PointsPerPixel { get { return 72f / Control.DotsPerInch.Width; } }

		protected override bool DisposeControl { get { return disposeControl; } }

		protected sd.Layer HelperLayer
		{
			get { return helperLayer ?? (helperLayer = new sd.Layer(Control)); }
		}

		public GraphicsHandler()
		{
		}

		public GraphicsHandler(GraphicsHandler other)
		{
			Control = other.Control;
			disposeControl = false;
		}

#if WINFORMS
		public GraphicsHandler(DrawableHandler drawable)
		{
			this.drawable = drawable;
			CreateRenderTarget();

			//set hwnd target properties (permit to attach Direct2D to window)
			//target creation
			drawable.Control.SizeChanged += HandleSizeChanged;
		}
#endif
		void CreateRenderTarget()
		{
#if WINFORMS
			if (drawable != null)
			{
				var renderProp = CreateRenderProperties();
				renderProp.PixelFormat = new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Premultiplied);
				var winProp = new sd.HwndRenderTargetProperties
				{
					Hwnd = drawable.Control.Handle,
					PixelSize = drawable.ClientSize.ToDx(),
					PresentOptions = sd.PresentOptions.Immediately
				};

				Control = new sd.WindowRenderTarget(SDFactory.D2D1Factory, renderProp, winProp);
				return;
			}
#endif
			CreateWicTarget(); // this is executed in winforms if not created from a drawable, and always in Xaml.
		}

		private void CreateWicTarget()
		{
			if (image != null)
			{
				var renderProp = CreateRenderProperties();
				var imageHandler = image.Handler as BitmapHandler;
				renderProp.PixelFormat = new sd.PixelFormat(SharpDX.DXGI.Format.Unknown, sd.AlphaMode.Unknown);
				Control = new sd.WicRenderTarget(SDFactory.D2D1Factory, imageHandler.Control, renderProp);
			}
		}

		private static sd.RenderTargetProperties CreateRenderProperties()
		{
			var renderProp = new sd.RenderTargetProperties
			{
				DpiX = 0,
				DpiY = 0,
				MinLevel = sd.FeatureLevel.Level_DEFAULT,
				Type = sd.RenderTargetType.Default,
				Usage = sd.RenderTargetUsage.None
			};

			return renderProp;
		}

		void HandleSizeChanged(object sender, EventArgs e)
		{
#if WINFORMS
			var target = Control as sd.WindowRenderTarget;
			if (target == null)
				return;
			try
			{
				target.Resize(drawable.ClientSize.ToDx());
				drawable.Invalidate();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("Could not resize: {0}", ex));
			}
#else
			throw new NotImplementedException();
#endif
		}

		sd.Ellipse GetEllipse(float x, float y, float width, float height)
		{
			var rx = width / 2f;
			var ry = height / 2f;

			return new sd.Ellipse(center: new s.Vector2(x + rx, y + ry), radiusX: rx, radiusY: ry);
		}

		public bool AntiAlias
		{
			get { return Control != null && Control.AntialiasMode == sd.AntialiasMode.PerPrimitive; }
			set
			{
				if (Control != null)
					Control.AntialiasMode = value ? sd.AntialiasMode.PerPrimitive : sd.AntialiasMode.Aliased;
			}		
		}

		public ImageInterpolation ImageInterpolation { get; set; }

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
			get
			{
				if (Control != null)
				{
					// not very efficient, but works
					var transform = Control.Transform;
					transform.Invert();
					var start = s.Matrix3x2.TransformPoint(transform, clipBounds.Location.ToDx()).ToEto();
					var end = s.Matrix3x2.TransformPoint(transform, clipBounds.EndLocation.ToDx()).ToEto();
					return new RectangleF(start, end);
				}
				else
					return new RectangleF();
			}
		}

		public void SetClip(RectangleF rect)
		{
			ResetClip();
			clipBounds = rect;
			rectClip = true;
			Control.PushAxisAlignedClip(rect.ToDx(), Control.AntialiasMode);
		}

		public void SetClip(IGraphicsPath path)
		{
			ResetClip();
			clipBounds = path.Bounds;
			var parameters = new sd.LayerParameters
			{
				ContentBounds = clipBounds.ToDx(),
				GeometricMask = clipGeometry = path.ToGeometry(),
				MaskAntialiasMode = Control.AntialiasMode,
				MaskTransform = s.Matrix3x2.Identity,
				Opacity = 1f
			};
			clipParams = parameters;
			Control.PushLayer(ref parameters, HelperLayer);
		}

		public void ResetClip()
		{
			if (clipParams != null)
			{
				Control.PopLayer();
				clipBounds = new RectangleF(Control.Size.ToEto());
				clipParams = null;
			}
			if (rectClip)
			{
				Control.PopAxisAlignedClip();
				rectClip = false;
				clipBounds = new RectangleF(Control.Size.ToEto());
			}
		}

		public void TranslateTransform(float dx, float dy)
		{
			Control.Transform = s.Matrix3x2.Multiply(s.Matrix3x2.Translation(dx, dy), Control.Transform);
		}

		public void RotateTransform(float angle)
		{
			Control.Transform = s.Matrix3x2.Multiply(s.Matrix3x2.Rotation((float)(Conversions.DegreesToRadians(angle))), Control.Transform);
		}

		public void ScaleTransform(float sx, float sy)
		{
			Control.Transform = s.Matrix3x2.Multiply(s.Matrix3x2.Scaling(sx, sy), Control.Transform);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Control.Transform = s.Matrix3x2.Multiply((s.Matrix3x2)matrix.ControlObject, Control.Transform);
		}

		Stack<s.Matrix3x2> transformStack;

		public void SaveTransform()
		{
			if (transformStack == null)
				transformStack = new Stack<s.Matrix3x2>();
			transformStack.Push(Control.Transform);
		}

		public void RestoreTransform()
		{
			Control.Transform = transformStack.Pop();
		}

		public void CreateFromImage(Bitmap image)
		{
			this.image = image;
			CreateWicTarget();
			BeginDrawing();
		}

		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			using (var textLayout = GetTextLayout(font, text))
			{
				Control.DrawTextLayout(new s.Vector2(x, y), textLayout, brush.ToDx(Control));
			}
		}

		public SizeF MeasureString(Font font, string text)
		{
			using (var textLayout = GetTextLayout(font, text))
			{
				var metrics = textLayout.Metrics;
				return new SizeF(metrics.WidthIncludingTrailingWhitespace, metrics.Height);
			}
		}

		static sw.TextLayout GetTextLayout(Font font, string text)
		{
			var fontHandler = (FontHandler)font.Handler;
			var textLayout = new sw.TextLayout(SDFactory.DirectWriteFactory, text, fontHandler.TextFormat, float.MaxValue, float.MaxValue);
			return textLayout;
		}

		public void Flush()
		{
			Control.Flush();
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get { return offset == 0f ? PixelOffsetMode.None : PixelOffsetMode.Half; }
			set
			{
				if (value == PixelOffsetMode.None)
				{
					offset = .5f;
					fillOffset = 0f;
				}
				else
				{
					offset = 0f;
					fillOffset = -.5f;
				}
			}
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			var pd = pen.ToPenData();
			Control.DrawRectangle(new s.RectangleF(x + offset, y + offset, width, height), pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			Control.FillRectangle(new s.RectangleF(x + fillOffset, y + fillOffset, width, height), brush.ToDx(Control));
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			var pd = pen.ToPenData();
			Control.DrawLine(
				new s.Vector2(startx + offset, starty + offset),
				new s.Vector2(endx + offset, endy + offset),
				pd.GetBrush(Control),
				pd.Width,
				pd.StrokeStyle);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			Control.FillEllipse(GetEllipse(x + fillOffset, y + fillOffset, width, height), brush.ToDx(Control));
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			var pd = pen.ToPenData();
			Control.DrawEllipse(GetEllipse(x + offset, y + offset, width, height), pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			PointF start;
			var arc = CreateArc(x + offset, y + offset, width, height, startAngle, sweepAngle, out start);
			var path = new sd.PathGeometry(SDFactory.D2D1Factory);
			var sink = path.Open();
			sink.BeginFigure(start.ToDx(), sd.FigureBegin.Hollow);
			sink.AddArc(arc);
			sink.EndFigure(sd.FigureEnd.Open);
			sink.Close();
			sink.Dispose();
			var pd = pen.ToPenData();
			Control.DrawGeometry(path, pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
		}

		internal static sd.ArcSegment CreateArc(float x, float y, float width, float height, float startAngle, float sweepAngle, out PointF start)
		{
			// degrees to radians conversion
			float startRadians = startAngle * (float)Math.PI / 180.0f;
			float sweepRadians = sweepAngle * (float)Math.PI / 180.0f;

			// x and y radius
			float dx = width / 2;
			float dy = height / 2;

			// determine the start point 
			float xs = x + dx + ((float)Math.Cos(startRadians) * dx);
			float ys = y + dy + ((float)Math.Sin(startRadians) * dy);

			// determine the end point 
			float xe = x + dx + ((float)Math.Cos(startRadians + sweepRadians) * dx);
			float ye = y + dy + ((float)Math.Sin(startRadians + sweepRadians) * dy);

			bool isLargeArc = Math.Abs(sweepAngle) > 180;
			bool isClockwise = sweepAngle >= 0 && Math.Abs(sweepAngle) < 360;
			start = new PointF(xs, ys);
			return new sd.ArcSegment
			{
				Point = new s.Vector2(xe, ye),
				Size = new s.Size2F(dx, dy),
				SweepDirection = isClockwise ? sd.SweepDirection.Clockwise : sd.SweepDirection.CounterClockwise,
				ArcSize = isLargeArc ? sd.ArcSize.Large : sd.ArcSize.Small
			};
		}


		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			PointF start;
			x += fillOffset;
			y += fillOffset;
			var arc = CreateArc(x, y, width, height, startAngle, sweepAngle, out start);
			var path = new sd.PathGeometry(SDFactory.D2D1Factory);
			var sink = path.Open();
			var center = new s.Vector2(x + width / 2, y + height / 2);
			sink.BeginFigure(center, sd.FigureBegin.Filled);
			sink.AddLine(start.ToDx());
			sink.AddArc(arc);
			sink.AddLine(center);
			sink.EndFigure(sd.FigureEnd.Open);
			sink.Close();
			sink.Dispose();
			Control.FillGeometry(path, brush.ToDx(Control));
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			SaveTransform();
			TranslateTransform(-fillOffset, -fillOffset);
			Control.FillGeometry(path.ToGeometry(), brush.ToDx(Control));
			RestoreTransform();
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			var pd = pen.ToPenData();
			SaveTransform();
			TranslateTransform(offset, offset);
			Control.DrawGeometry(path.ToGeometry(), pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
			RestoreTransform();
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			var bmp = image.ToDx(Control);
			Control.DrawBitmap(bmp, destination.ToDx(), 1f, sd.BitmapInterpolationMode.Linear, source.ToDx());
		}

		public void DrawImage(Image image, float x, float y)
		{
			var bmp = image.ToDx(Control);
			if (bmp != null)
				Control.DrawBitmap(bmp, new s.RectangleF(x, y, bmp.Size.Width, bmp.Size.Height), 1f, ImageInterpolation.ToDx());
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			var bmp = image.ToDx(Control);
			Control.DrawBitmap(bmp, new s.RectangleF(x, y, width, height), 1f, ImageInterpolation.ToDx());
		}

		public object ControlObject
		{
			get { throw new NotImplementedException(); }
		}

		public void Clear(SolidBrush brush)
		{
			if (Control != null)
			{
				var color = brush != null ? brush.Color : Colors.Transparent;
				// drawing to an image, so we can clear to transparent
				if (image != null)
				{
					if (clipParams != null)
					{
						// can't clear the current layer otherwise it will not be applied to main layer
						// This creates a copy of the current context, inverses the current clip, and draws the image back clipping
						// the cleared path.
						
						// end clip layer and current drawing session
						Control.PopLayer();
						Control.EndDraw();

						// create a copy of the current state
						var copy = image.Clone();
						var bmp = copy.ToDx(Control);

						Control.BeginDraw();

						// clear existing contents
						Control.Clear(null);
						var size = Control.Size;

						// create an inverse geometry
						var inverse = new sd.PathGeometry(SDFactory.D2D1Factory);
						var sink = inverse.Open();
						var bounds = new s.RectangleF(0, 0, size.Width, size.Height);
						var geom = new sd.RectangleGeometry(SDFactory.D2D1Factory, bounds);
						geom.Combine(clipGeometry, sd.CombineMode.Exclude, sink);
						sink.Close();

						// create a new mask layer with inverse geometry
						var parameters = new sd.LayerParameters
						{
							ContentBounds = bounds,
							GeometricMask = inverse,
							MaskAntialiasMode = Control.AntialiasMode,
							MaskTransform = s.Matrix3x2.Identity,
							Opacity = 1f
						};
						Control.PushLayer(ref parameters, HelperLayer);

						// draw bitmap of contents back, clipping to the inverse of the clip region
						Control.DrawBitmap(bmp, 1f, sd.BitmapInterpolationMode.NearestNeighbor);
						Control.PopLayer();

						// restore our clip path
						parameters = clipParams.Value;
						Control.PushLayer(ref parameters, HelperLayer);

						copy.Dispose();
					}
				}
				else
				{
					// alpha is not supported on a drawable, so blend with black as the base color.
					color = Color.Blend(Colors.Black, color);
				}

				Control.Clear(color.ToDx());
			}
		}

		static sd.RenderTarget globalRenderTarget;
		/// <summary>
		/// This is a HACK.
		/// Brushes, bitmaps and other resources are associated with a specific
		/// render target in D2D. Eto does not currently support this.
		/// </summary>
		public static sd.RenderTarget CurrentRenderTarget
		{
			get
			{
				if (globalRenderTarget == null)
				{
#if WINFORMS
					// hack for now, use a temporary control to get the current target
					// ideally, each brush/etc will create itself when needed, not right away.
					// though, this may be difficult for things like a bitmap
					var ctl = new System.Windows.Forms.Control();
					var winProp = new sd.HwndRenderTargetProperties
					{
						Hwnd = ctl.Handle,
						PixelSize = new s.Size2(2000, 2000),
						PresentOptions = sd.PresentOptions.Immediately
					};
					var renderProp = new sd.RenderTargetProperties
					{
						DpiX = 0,
						DpiY = 0,
						MinLevel = sd.FeatureLevel.Level_10,
						PixelFormat = new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Premultiplied),
						Type = sd.RenderTargetType.Hardware,
						Usage = sd.RenderTargetUsage.None
					};
					globalRenderTarget = new sd.WindowRenderTarget(SDFactory.D2D1Factory, renderProp, winProp);
#else
					throw new NotImplementedException();
#endif
				}
				return currentRenderTarget ?? globalRenderTarget;
			}
			set
			{
				currentRenderTarget = value;
			}
		}

		static sd.RenderTarget currentRenderTarget;

		public void BeginDrawing(RectangleF? clipRect = null)
		{
			if (Control != null)
			{
				CurrentRenderTarget = Control;
				Control.BeginDraw();
				Control.Transform = s.Matrix3x2.Identity;
				if (transformStack != null)
					transformStack.Clear();
				ResetClip();
				clipBounds = new RectangleF(Control.Size.ToEto());
				if (clipRect != null)
					Control.PushAxisAlignedClip(clipRect.Value.ToDx(), SharpDX.Direct2D1.AntialiasMode.PerPrimitive);
				hasBegan = true;
			}
		}

		public void EndDrawing(bool popClip = false)
		{
			if (hasBegan)
			{
				ResetClip();
				CurrentRenderTarget = null;

				if (popClip)
					Control.PopAxisAlignedClip();

				Control.EndDraw();
				hasBegan = false;

				if (image != null)
				{
					var imageHandler = image.Handler as BitmapHandler;
					imageHandler.Reset();
				}
			}
		}

		public void PerformDrawing(RectangleF? clipRect, Action draw)
		{
			bool recreated = false;
			do
			{
				try
				{
					recreated = false;
					BeginDrawing(clipRect);
					draw();
					EndDrawing(clipRect != null);
				}
				catch (s.SharpDXException ex)
				{
					if (ex.ResultCode == 0x8899000C) // D2DERR_RECREATE_TARGET
					{
						Debug.WriteLine("Recreating targets");
						// need to recreate render target
						CreateRenderTarget();
						CurrentRenderTarget = Control;
						globalRenderTarget = null;
						recreated = true;
					}
				}
			}
			while (recreated);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				EndDrawing();
			base.Dispose(disposing);
		}
	}
}
