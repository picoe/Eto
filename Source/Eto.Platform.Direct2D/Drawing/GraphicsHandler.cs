using System;
using System.Collections.Generic;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Platform.Windows;

namespace Eto.Platform.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : WidgetHandler<sd.RenderTarget, Graphics>, IGraphics
	{
		bool hasBegan;
		bool disposeControl;
		s.Color4? backColor;

		protected override bool DisposeControl { get { return disposeControl; } }

		public GraphicsHandler()
		{
		}

		public GraphicsHandler(GraphicsHandler other)
		{
			Control = other.Control;
			disposeControl = false;
		}

		public GraphicsHandler(DrawableHandler drawable)
		{
			var drawableControl = drawable.Control;

			backColor = drawable.BackgroundColor.ToDx();

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
				Hwnd = drawableControl.Handle,
				PixelSize = new s.Size2(drawableControl.ClientSize.Width, drawableControl.ClientSize.Height),
				PresentOptions = sd.PresentOptions.Immediately
			};

			//target creation
			var target = new sd.WindowRenderTarget(SDFactory.D2D1Factory, renderProp, winProp);

			drawableControl.SizeChanged += (s, e) => {
				try
				{
					target.Resize(new s.Size2(drawableControl.ClientSize.Width, drawableControl.ClientSize.Height));
					drawable.Invalidate();
				}
				catch (Exception)
				{
				}
			};

			Control = CurrentRenderTarget = target;
		}

		sd.Ellipse GetEllipse(float x, float y, float width, float height)
		{
			var rx = width / 2f;
			var ry = height / 2f;

			return new sd.Ellipse(center: new s.Vector2(x + rx, y + ry), radiusX: rx, radiusY: ry);
		}

		public bool AntiAlias { get; set; } // not used

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
			get { throw new NotImplementedException(); }
		}

		public void SetClip(RectangleF rect)
		{
			throw new NotImplementedException();
		}

		public void SetClip(IGraphicsPath path)
		{
			throw new NotImplementedException();
		}

		public void ResetClip()
		{
			throw new NotImplementedException();
		}

		public void TranslateTransform(float dx, float dy)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					s.Matrix3x2.Translation(dx, dy),
					Control.Transform);
		}

		public void RotateTransform(float angle)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					s.Matrix3x2.Rotation((float)(Conversions.DegreesToRadians(angle))),
					Control.Transform);
		}

		public void ScaleTransform(float sx, float sy)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					s.Matrix3x2.Scaling(sx, sy),
					Control.Transform);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Control.Transform =
				s.Matrix3x2.Multiply(
					(s.Matrix3x2)matrix.ControlObject,
					Control.Transform);
		}

		public void CreateFromImage(Bitmap image)
		{
			var b = image.ControlObject as sd.Bitmap;

			Control = new sd.BitmapRenderTarget(CurrentRenderTarget,
				sd.CompatibleRenderTargetOptions.None,
				new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Premultiplied));
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
				var width = textLayout.DetermineMinWidth();
				var height = font.LineHeight * 96 / 72.0f; // is this correct?
				return new SizeF(width, height);
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

		public PixelOffsetMode PixelOffsetMode { get; set; } // TODO

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			var pd = pen.ToPenData();
			Control.DrawRectangle(new s.RectangleF(x, y, width, height), pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			Control.FillRectangle(new s.RectangleF(x, y, width, height), brush.ToDx(Control));
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			var pd = pen.ToPenData();
			Control.DrawLine(
				new s.Vector2(startx, starty),
				new s.Vector2(endx, endy),
				pd.GetBrush(Control),
				pd.Width,
				pd.StrokeStyle);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			Control.FillEllipse(GetEllipse(x, y, width, height), brush.ToDx(Control));
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			var pd = pen.ToPenData();
			Control.DrawEllipse(GetEllipse(x, y, width, height), pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			Control.FillGeometry(path.ToGeometry(), brush.ToDx(Control));
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			var pd = pen.ToPenData();
			Control.DrawGeometry(path.ToGeometry(), pd.GetBrush(Control), pd.Width, pd.StrokeStyle);
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			var bmp = (sd.Bitmap)image.ControlObject;
			Control.DrawBitmap(bmp, destination.ToDx(), 1f, sd.BitmapInterpolationMode.Linear, source.ToDx());
		}

		public void DrawImage(Image image, float x, float y)
		{
			var bmp = (sd.Bitmap)image.ControlObject;
			var destination = new RectangleF(x, y, bmp.Size.Width, bmp.Size.Height);
			Control.DrawBitmap(bmp, destination.ToDx(), 1f, ImageInterpolation.ToDx());
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			var bmp = (sd.Bitmap)image.ControlObject;
			var destination = new RectangleF(x, y, width, height);
			Control.DrawBitmap(bmp, destination.ToDx(), 1f, ImageInterpolation.ToDx());
		}

		public object ControlObject
		{
			get { throw new NotImplementedException(); }
		}

		public void Clear(SolidBrush brush)
		{
			if (brush != null)
				Control.Clear(brush.Color.ToDx());
			else
				Control.Clear(backColor);
		}

		/// <summary>
		/// This is a HACK.
		/// Brushes, bitmaps and other resources are associated with a specific
		/// render target in D2D. Eto does not currently support this.
		/// </summary>
		public static sd.RenderTarget CurrentRenderTarget
		{
			get
			{
				if (currentRenderTarget == null)
				{
					// hack for now, use a temporary control to get the current target
					// ideally, each brush/etc will create itself when needed, not right away.
					// though, this may be difficult for things like a bitmap
					var ctl = new System.Windows.Forms.Control();
					var winProp = new sd.HwndRenderTargetProperties
					{
						Hwnd = ctl.Handle,
						PixelSize = new s.Size2(1000, 1000),
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
					currentRenderTarget = new sd.WindowRenderTarget(SDFactory.D2D1Factory, renderProp, winProp);
				}
				return currentRenderTarget;
			}
			set
			{
				currentRenderTarget = value;
			}
		}

		static sd.RenderTarget currentRenderTarget;

		public void BeginDrawing()
		{
			CurrentRenderTarget = Control;
			Control.BeginDraw();
			Control.Clear(backColor);
			hasBegan = true;
		}

		public void EndDrawing()
		{
			if (hasBegan)
			{
				Control.EndDraw();
				hasBegan = false;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && hasBegan)
				EndDrawing();
			base.Dispose(disposing);
		}
	}
}
