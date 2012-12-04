using System;
using Eto.Drawing;
using sd = System.Drawing;
using sdd = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsHandler : WidgetHandler<System.Drawing.Graphics, Graphics>, IGraphics
	{
		public bool IsRetained { get { return false; } }

		static sd.StringFormat defaultStringFormat;

		static GraphicsHandler ()
		{
			// Set the StringFormat
			defaultStringFormat = new sd.StringFormat (sd.StringFormat.GenericTypographic);
			defaultStringFormat.FormatFlags |= 
				sd.StringFormatFlags.MeasureTrailingSpaces
				| sd.StringFormatFlags.NoWrap
				| sd.StringFormatFlags.NoClip;
		}

		ImageInterpolation imageInterpolation;

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (sd.Graphics graphics)
		{
			this.Control = graphics;
		}
		
		public bool Antialias {
			get {
				return (this.Control.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.AntiAlias);
			}
			set {
				if (value)
					this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				else
					this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			}
		}

		public ImageInterpolation ImageInterpolation {
			get { return imageInterpolation; }
			set {
				imageInterpolation = value;
				Control.InterpolationMode = value.ToSD ();
			}
		}

		public PixelOffsetMode PixelOffsetMode {
			get { return Control.PixelOffsetMode.ToEto (); }
			set { Control.PixelOffsetMode = value.ToSD (); }
		}

		public void CreateFromImage (Bitmap image)
		{
			Control = sd.Graphics.FromImage ((sd.Image)image.ControlObject);
		}

		public override void Initialize ()
		{
			base.Initialize ();

			Control.PixelOffsetMode = sdd.PixelOffsetMode.None;
			Control.SmoothingMode = sdd.SmoothingMode.AntiAlias;
			Control.InterpolationMode = sdd.InterpolationMode.HighQualityBilinear;
		}

		public void Commit ()
		{
		}

		public void DrawLine (Color color, float startx, float starty, float endx, float endy)
		{
			if (startx == endx && starty == endy)
				this.Control.FillRectangle (new sd.SolidBrush (color.ToSD ()), startx, starty, 1, 1);
			else
				this.Control.DrawLine (new sd.Pen (color.ToSD ()), startx, starty, endx, endy);
		}

		public void DrawRectangle (Color color, float x, float y, float width, float height)
		{
			Control.DrawRectangle (new sd.Pen (color.ToSD ()), x, y, width, height);
		}

		public void FillRectangle (Color color, float x, float y, float width, float height)
		{
			Control.FillRectangle (new sd.SolidBrush (color.ToSD ()), x - 0.5f, y - 0.5f, width, height);
		}

		public void DrawEllipse (Color color, float x, float y, float width, float height)
		{
			Control.DrawEllipse (new sd.Pen (color.ToSD ()), x, y, width, height);
		}

		public void FillEllipse (Color color, float x, float y, float width, float height)
		{
			Control.FillEllipse (new sd.SolidBrush (color.ToSD ()), x - 0.5f, y - 0.5f, width, height);
		}

		public float GetConvertedAngle (float initialAngle, float majorRadius, float minorRadius, bool circularToElliptical)
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
			angle = DegreeToRadian (angle);
			double functionReturnValue = 0;

			double dTan = 0;
			dTan = Math.Tan (angle);

			if (Math.Abs (dTan) < 1E-10 | Math.Abs (dTan) > 10000000000.0) {
				functionReturnValue = angle;

			} else if (circularToElliptical) {
				functionReturnValue = Math.Atan (dTan * majorRadius / minorRadius);
			} else {
				functionReturnValue = Math.Atan (dTan * minorRadius / majorRadius);
			}

			if (functionReturnValue < 0) {
				functionReturnValue = functionReturnValue + 2 * Math.PI;
			}
			var ret = RadianToDegree ((float)functionReturnValue);

			// convert back to right quadrant
			if (quadrant2)
				ret = 180.0f - ret;
			else if (quadrant4)
				ret = 360.0f - ret;
			else if (quadrant3)
				ret += 180.0f;

			// get in the same range
			while (initialAngle < 0) {
				initialAngle += 360.0f;
				ret -= 360.0f;
			}
			while (initialAngle > 360) {
				initialAngle -= 360.0f;
				ret += 360.0f;
			}

			return ret;	
		}

		float DegreeToRadian (float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		float RadianToDegree (float radians)
		{
			return radians * 180.0f / (float)Math.PI;
		}

		public void DrawArc (Color color, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (width != height) {
				var endAngle = startAngle + sweepAngle;
				startAngle = GetConvertedAngle (startAngle, width / 2, height / 2, false);
				endAngle = GetConvertedAngle (endAngle, width / 2, height / 2, false);
				sweepAngle = endAngle - startAngle;
			}
			Control.DrawArc (new sd.Pen (color.ToSD ()), x, y, width, height, startAngle, sweepAngle);
		}

		public void FillPie (Color color, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (width != height) {
				var endAngle = startAngle + sweepAngle;
				startAngle = GetConvertedAngle (startAngle, width / 2, height / 2, false);
				endAngle = GetConvertedAngle (endAngle, width / 2, height / 2, false);
				sweepAngle = endAngle - startAngle;
			}
			Control.FillPie (new sd.SolidBrush (color.ToSD ()), x - 0.5f, y - 0.5f, width, height, startAngle, sweepAngle);
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
			var old = Control.PixelOffsetMode;
			Control.PixelOffsetMode = old == sdd.PixelOffsetMode.Half ? sdd.PixelOffsetMode.None : sdd.PixelOffsetMode.Half;
			Control.FillPath (new sd.SolidBrush (color.ToSD ()), GraphicsPathHandler.GetControl (path));
			Control.PixelOffsetMode = old;
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			Control.DrawPath (new sd.Pen (color.ToSD ()), GraphicsPathHandler.GetControl (path));
		}

		public void DrawImage (Image image, float x, float y)
		{
			var handler = image.Handler as IWindowsImage;
			handler.DrawImage (this, x, y);
		}

		public void DrawImage (Image image, float x, float y, float width, float height)
		{
			var handler = image.Handler as IWindowsImage;
			handler.DrawImage (this, x, y, width, height);
		}

		public void DrawImage (Image image, RectangleF source, RectangleF destination)
		{
			var handler = image.Handler as IWindowsImage;
			handler.DrawImage (this, source, destination);
		}

		public void DrawText (Font font, Color color, float x, float y, string text)
		{
			sd.Brush brush = new sd.SolidBrush (color.ToSD ());
			Control.DrawString (text, (sd.Font)font.ControlObject, brush, x, y, defaultStringFormat);
			brush.Dispose ();
		}

		public SizeF MeasureString (Font font, string text)
		{
			/* BAD (but not really!?)
			 *
			return this.Control.MeasureString (text, FontHandler.GetControl (font), sd.PointF.Empty, defaultStringFormat).ToEto ();
			/**/
			if (string.IsNullOrEmpty (text))
				return Size.Empty;
			sd.CharacterRange[] ranges = { new sd.CharacterRange (0, text.Length) };
			defaultStringFormat.SetMeasurableCharacterRanges (ranges);

			var regions = this.Control.MeasureCharacterRanges (text, FontHandler.GetControl (font), sd.Rectangle.Empty, defaultStringFormat);
			var rect = regions [0].GetBounds (this.Control);

			return rect.Size.ToEto ();
			/**/
		}

		public void Flush ()
		{
			Control.Flush ();
		}
	}
}
