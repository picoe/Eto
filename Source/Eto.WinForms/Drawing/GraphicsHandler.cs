using System;
using System.ComponentModel;
using Eto.Drawing;
using sd = System.Drawing;
using sdd = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;
using System.Collections.Generic;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="Graphics.IHandler"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : WidgetHandler<System.Drawing.Graphics, Graphics>, Graphics.IHandler
	{
		Stack<sd.Drawing2D.Matrix> savedTransforms;

		/// <summary>
		/// Gets or sets a value indicating that the <see cref="DrawText"/> and <see cref="MeasureString"/> methods
		/// use compatible rendering (GDI+), or newer graphics-dependent (GDI) methods that have better cleartype rendering.
		/// Setting this to false will not allow the text to be fully transformed, so it is only recommended to turn
		/// off when rendering a custom control.
		/// Use via styles like so:
		/// <code>
		/// Eto.Style.Add&lt;GraphicsHandler&gt;(null, h => h.UseCompatibleTextRendering = false);
		/// </code>
		/// </summary>
		[DefaultValue(true)]
		public bool UseCompatibleTextRendering { get; set; }

		public bool IsRetained { get { return false; } }

		public static swf.TextFormatFlags DefaultTextFormat { get; private set; }

		public static sd.StringFormat DefaultStringFormat { get; private set; }

		static GraphicsHandler()
		{
			DefaultTextFormat = swf.TextFormatFlags.Left
			                    | swf.TextFormatFlags.NoPadding
			                    | swf.TextFormatFlags.NoClipping
			                    | swf.TextFormatFlags.PreserveGraphicsClipping
			                    | swf.TextFormatFlags.PreserveGraphicsTranslateTransform
			                    | swf.TextFormatFlags.NoPrefix;

			// Set the StringFormat
			DefaultStringFormat = new sd.StringFormat(sd.StringFormat.GenericTypographic);
			DefaultStringFormat.FormatFlags |=
				sd.StringFormatFlags.MeasureTrailingSpaces
				| sd.StringFormatFlags.NoWrap
				| sd.StringFormatFlags.NoClip;
		}

		ImageInterpolation imageInterpolation;

		public GraphicsHandler()
		{
			UseCompatibleTextRendering = true;
		}

		bool shouldDisposeGraphics = true;

		public GraphicsHandler(sd.Graphics graphics, bool shouldDisposeGraphics = true)
			: this()
		{
			this.Control = graphics;
			this.shouldDisposeGraphics = shouldDisposeGraphics;
		}

		protected override void Dispose(bool disposing)
		{
			if (!shouldDisposeGraphics)
				Control = null;

			base.Dispose(disposing);
		}

		public bool AntiAlias
		{
			get
			{
				return (this.Control.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.AntiAlias);
			}
			set
			{
				if (value)
					this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				else
					this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return imageInterpolation; }
			set
			{
				imageInterpolation = value;
				Control.InterpolationMode = value.ToSD();
			}
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get { return Control.PixelOffsetMode.ToEto(); }
			set { Control.PixelOffsetMode = value.ToSD(); }
		}

		public float PointsPerPixel
		{
			get { return 72f / Control.DpiX; }
		}

		public void CreateFromImage(Bitmap image)
		{
			Control = sd.Graphics.FromImage((sd.Image)image.ControlObject);
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetInitialState();
		}

		public void SetInitialState()
		{
			Control.PixelOffsetMode = sdd.PixelOffsetMode.None;
			Control.SmoothingMode = sdd.SmoothingMode.AntiAlias;
			Control.InterpolationMode = sdd.InterpolationMode.HighQualityBilinear;
		}

		public void Commit()
		{
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			this.Control.DrawLine(pen.ToSD(), startx, starty, endx, endy);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawRectangle(pen.ToSD(), x, y, width, height);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			Control.FillRectangle(brush.ToSD(new RectangleF(x, y, width, height)), x - 0.5f, y - 0.5f, width, height);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			Control.DrawEllipse(pen.ToSD(), x, y, width, height);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			Control.FillEllipse(brush.ToSD(new RectangleF(x, y, width, height)), x - 0.5f, y - 0.5f, width, height);
		}

		public float GetConvertedAngle(float initialAngle, float majorRadius, float minorRadius, bool circularToElliptical)
		{
			var angle = initialAngle;
			while (angle < 0)
				angle += 360.0f;
			var modAngle = angle % 360.0f;
			angle %= 90.0f;
			if (angle == 0)
				return initialAngle;
			var quadrant2 = (modAngle > 90 && modAngle <= 180);
			var quadrant3 = (modAngle > 180 && modAngle <= 270);
			var quadrant4 = (modAngle > 270 && modAngle <= 360);
			if (quadrant2 || quadrant4)
				angle = 90.0f - angle;
			angle = DegreeToRadian(angle);
			double functionReturnValue = 0;

			double dTan = 0;
			dTan = Math.Tan(angle);

			if (Math.Abs(dTan) < 1E-10 | Math.Abs(dTan) > 10000000000.0)
			{
				functionReturnValue = angle;

			}
			else if (circularToElliptical)
			{
				functionReturnValue = Math.Atan(dTan * majorRadius / minorRadius);
			}
			else
			{
				functionReturnValue = Math.Atan(dTan * minorRadius / majorRadius);
			}

			if (functionReturnValue < 0)
			{
				functionReturnValue = functionReturnValue + 2 * Math.PI;
			}
			var ret = RadianToDegree((float)functionReturnValue);

			// convert back to right quadrant
			if (quadrant2)
				ret = 180.0f - ret;
			else if (quadrant4)
				ret = 360.0f - ret;
			else if (quadrant3)
				ret += 180.0f;

			// get in the same range
			while (initialAngle < 0)
			{
				initialAngle += 360.0f;
				ret -= 360.0f;
			}
			while (initialAngle > 360)
			{
				initialAngle -= 360.0f;
				ret += 360.0f;
			}

			return ret;
		}

		float DegreeToRadian(float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		float RadianToDegree(float radians)
		{
			return radians * 180.0f / (float)Math.PI;
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (width != height)
			{
				var endAngle = startAngle + sweepAngle;
				startAngle = GetConvertedAngle(startAngle, width / 2, height / 2, false);
				endAngle = GetConvertedAngle(endAngle, width / 2, height / 2, false);
				sweepAngle = endAngle - startAngle;
			}
			Control.DrawArc(pen.ToSD(), x, y, width, height, startAngle, sweepAngle);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (width != height)
			{
				var endAngle = startAngle + sweepAngle;
				startAngle = GetConvertedAngle(startAngle, width / 2, height / 2, false);
				endAngle = GetConvertedAngle(endAngle, width / 2, height / 2, false);
				sweepAngle = endAngle - startAngle;
			}
			Control.FillPie(brush.ToSD(new RectangleF(x, y, width, height)), x - 0.5f, y - 0.5f, width, height, startAngle, sweepAngle);
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			var old = Control.PixelOffsetMode;
			Control.PixelOffsetMode = old == sdd.PixelOffsetMode.Half ? sdd.PixelOffsetMode.None : sdd.PixelOffsetMode.Half;
			Control.FillPath(brush.ToSD(path.Bounds), path.ToSD());
			Control.PixelOffsetMode = old;
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			Control.DrawPath(pen.ToSD(), path.ToSD());
		}

		public void DrawImage(Image image, float x, float y)
		{
			var handler = image.Handler as IWindowsImage;
			handler.DrawImage(this, x, y);
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			var handler = image.Handler as IWindowsImage;
			handler.DrawImage(this, x, y, width, height);
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			var handler = image.Handler as IWindowsImage;
			handler.DrawImage(this, source, destination);
		}

		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			if (UseCompatibleTextRendering)
			{
				Control.DrawString(text, (sd.Font)font.ControlObject, (sd.Brush)brush.ControlObject, x, y, DefaultStringFormat);
			}
			else
			{
				swf.TextRenderer.DrawText(Control, text, (sd.Font)font.ControlObject, new sd.Point((int)x, (int)y), brush.Color.ToSD(), DefaultTextFormat);
			}
		}

		public SizeF MeasureString(Font font, string text)
		{
			if (string.IsNullOrEmpty(text))
				return Size.Empty;

			var sdFont = FontHandler.GetControl(font);
			if (UseCompatibleTextRendering)
			{
				sd.CharacterRange[] ranges = { new sd.CharacterRange(0, text.Length) };
				DefaultStringFormat.SetMeasurableCharacterRanges(ranges);

				var regions = Control.MeasureCharacterRanges(text, sdFont, sd.RectangleF.Empty, DefaultStringFormat);
				var rect = regions[0].GetBounds(Control);

				return rect.Size.ToEto();
			}

			var size = swf.TextRenderer.MeasureText(Control, text, sdFont, sd.Size.Empty, DefaultTextFormat);
			return size.ToEto();
		}

		public void Flush()
		{
			Control.Flush();
		}

		public void TranslateTransform(float offsetX, float offsetY)
		{
			this.Control.TranslateTransform(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			this.Control.RotateTransform(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			this.Control.ScaleTransform(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			this.Control.MultiplyTransform((sd.Drawing2D.Matrix)matrix.ControlObject);
		}

		public void SaveTransform()
		{
			if (savedTransforms == null)
				savedTransforms = new Stack<sd.Drawing2D.Matrix>();

			savedTransforms.Push(Control.Transform);
		}

		public void RestoreTransform()
		{
			if (savedTransforms != null && savedTransforms.Count > 0)
			{
				var t = savedTransforms.Pop();

				Control.Transform = t;

				t.Dispose();
			}
		}

		public RectangleF ClipBounds
		{
			get { return this.Control.ClipBounds.ToEto(); }
		}

		public void SetClip(RectangleF rectangle)
		{
			this.Control.SetClip(rectangle.ToSD());
		}

		public void SetClip(IGraphicsPath path)
		{
			this.Control.SetClip(path.ToSD());
		}

		public void ResetClip()
		{
			this.Control.ResetClip();
		}

		public void Clear(SolidBrush brush)
		{
			if (brush != null)
				Control.Clear(brush.Color.ToSD());
			else
				Control.Clear(sd.Color.Transparent);
		}
	}
}
