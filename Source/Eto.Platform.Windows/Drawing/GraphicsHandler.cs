using System;
using Eto.Drawing;
using sd = System.Drawing;
using sdd = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;
using System.Collections.Generic;
using Eto.Cache;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsHandler : WidgetHandler<System.Drawing.Graphics, Graphics>, IGraphics
	{
		Stack<sd.Drawing2D.Matrix> savedTransforms;

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


        public override sd.Graphics Control
        {
            get
            {
                return base.Control;
            }
            protected set
            {
                base.Control = value;
                if (Control != null)
                {
                    Control.PixelOffsetMode = sdd.PixelOffsetMode.Half; // was: None
                    //this.Control.CompositingMode = sdd.CompositingMode.SourceCopy;
                    //this.Control.SmoothingMode = sdd.SmoothingMode.HighQuality;
                    this.Control.SmoothingMode = sdd.SmoothingMode.AntiAlias;
                    Control.InterpolationMode = sdd.InterpolationMode.HighQualityBilinear; //.NearestNeighbor;
                }
            }
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

        public void FillPath(Color color, GraphicsPath path)
        {
            var old = Control.PixelOffsetMode;
            Control.PixelOffsetMode = old == sdd.PixelOffsetMode.Half ? sdd.PixelOffsetMode.None : sdd.PixelOffsetMode.Half;
            Control.FillPath(new sd.SolidBrush(color.ToSD()), GraphicsPathHandler.GetControl(path));
            Control.PixelOffsetMode = old;
        }

        public void DrawPath(Color color, GraphicsPath path)
        {
            Control.DrawPath(new sd.Pen(color.ToSD()), GraphicsPathHandler.GetControl(path));
        }

        public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
            var handler = image.Handler as IWindowsImage;
            handler.DrawImage(this, source, destination);
		}

        public void DrawImage(Image image, PointF point)
        {
            this.Control.DrawImage(
                (sd.Image)image.ControlObject,
                point.ToSD());
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            this.Control.DrawImage(
                (sd.Image)image.ControlObject,
                rect.ToSD());
        }

        public void DrawText(Font font, Color color, float x, float y, string text)
		{
            var brush =
                BrushCache.GetBrush(font.Generator, color);

            sd.Brush b = null;

            if (brush != null)
                b = brush.ControlObject
                    as sd.Brush;

            if (b != null)
                Control.DrawString(text, (sd.Font)font.ControlObject, b, x, y, defaultStringFormat);
		}

		public SizeF MeasureString (Font font, string text)
		{
#if FALSE
			/* BAD (but not really!?)
			 *
			return this.Control.MeasureString (text, FontHandler.GetControl (font), sd.PointF.Empty, defaultStringFormat).ToEto ();
			/*

				return Size.Empty;
			sd.CharacterRange[] ranges = { new sd.CharacterRange (0, text.Length) };
			defaultStringFormat.SetMeasurableCharacterRanges (ranges);

			var regions = this.Control.MeasureCharacterRanges (text, FontHandler.GetControl (font), sd.Rectangle.Empty, defaultStringFormat);
			var rect = regions [0].GetBounds (this.Control);

			return rect.Size.ToEto ();
			*/
#else
            var result = SizeF.Empty;

            if (font != null &&
                !string.IsNullOrEmpty(text))
            {
                var sdFont =
                    font.ToSD();

                var graphics = this.Control;

                sd.CharacterRange[] characterRanges = 
                { 
                    new sd.CharacterRange(0, text.Length) 
                };

                defaultStringFormat.SetMeasurableCharacterRanges(
                    characterRanges);

                var regions =
                    graphics.MeasureCharacterRanges(
                        text,
                        sdFont,
                        new sd.Rectangle(0, 0, 10000, 10000),
                        defaultStringFormat);

                if (regions != null &&
                    regions.Length > 0)
                {
                    var bounds =
                        regions[0].GetBounds(
                            graphics);

                    result =
                        bounds.Size.ToSizeF();
                }
            }

            return result;
        }
#endif

		public void Flush ()
		{
			Control.Flush ();
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.Control.TranslateTransform(dx, dy);
		}

		public void RotateTransform(float angle)
		{
			this.Control.RotateTransform(angle);
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.Control.ScaleTransform(sx, sy);
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

        public void SetClip(RectangleF rect)
        {
            this.Control.SetClip(rect.ToRectangleF());
        }

        public void FillRectangle(Brush brush, RectangleF Rectangle)
        {
            this.Control.FillRectangle((sd.Brush)brush.ControlObject, Rectangle.ToRectangleF());
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            this.Control.FillRectangle((sd.Brush)brush.ControlObject, x, y, width, height);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            this.Control.FillPath(
                (sd.Brush)brush.ControlObject, 
                (sd.Drawing2D.GraphicsPath)path.ControlObject);
        }
        public RectangleF ClipBounds
        {
            get { return this.Control.ClipBounds.ToRectangleF(); }
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            this.Control.DrawRectangle(
                (sd.Pen)pen.ControlObject,
                x, y, width, height);
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            this.Control.DrawLine(
                (sd.Pen)pen.ControlObject,
                pt1.ToPointF(),
                pt2.ToPointF());
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            this.Control.DrawPath(
                (sd.Pen)pen.ControlObject,
                (sd.Drawing2D.GraphicsPath)path.ControlObject);
        }

        public void Clear(Color color)
        {
            Control.Clear(color.ToSD());
        }
    }
}
