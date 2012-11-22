using System;
using Eto.Drawing;
using SD = System.Drawing;
using SDD = System.Drawing.Drawing2D;
using SWF = System.Windows.Forms;
using StringFormat = System.Drawing.StringFormat;
using System.Collections.Generic;
using Eto.Cache;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsHandler : WidgetHandler<System.Drawing.Graphics, Graphics>, IGraphics
	{
        private static StringFormat StringFormat { get; set; }
        static GraphicsHandler()
        {
            // Set the StringFormat
            StringFormat =
                new StringFormat(
                    StringFormat.GenericTypographic);
            if (StringFormat != null)
            {
                StringFormat.FormatFlags |=
                    SD.StringFormatFlags.MeasureTrailingSpaces |
                    SD.StringFormatFlags.NoWrap |
                    SD.StringFormatFlags.NoClip;
            }
        }
        public override SD.Graphics Control
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
                    Control.PixelOffsetMode = SDD.PixelOffsetMode.Half; // was: None
                    //this.Control.CompositingMode = SDD.CompositingMode.SourceCopy;
                    //this.Control.SmoothingMode = SDD.SmoothingMode.HighQuality;
                    this.Control.SmoothingMode = SDD.SmoothingMode.AntiAlias;
                    Control.InterpolationMode = SDD.InterpolationMode.HighQualityBilinear; //.NearestNeighbor;
                }
            }
        }
		ImageInterpolation imageInterpolation;

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (SD.Graphics graphics)
		{
			this.Control = graphics;			
		}
		
		public bool Antialias {
			get {
				return (this.Control.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.AntiAlias);
			}
			set {
				if (value) this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				else this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			}
		}

		public ImageInterpolation ImageInterpolation {
			get { return imageInterpolation; }
			set {
				imageInterpolation = value;
				Control.InterpolationMode = Generator.Convert (value);
			}
		}

		public void CreateFromImage (Bitmap image)
		{
			Control = SD.Graphics.FromImage ((SD.Image)image.ControlObject);
			Control.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			this.ImageInterpolation = Eto.Drawing.ImageInterpolation.Default;
		}

		public void Commit ()
		{
		}

		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			if (startx == endx && starty == endy) {
				this.Control.FillRectangle (new SD.SolidBrush (Generator.Convert (color)), startx, starty, 1, 1);
			}
			else 
				this.Control.DrawLine (new SD.Pen (Generator.Convert (color)), startx, starty, endx, endy);
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			Control.DrawRectangle (new SD.Pen (Generator.Convert (color)), x, y, width-1, height-1);
		}

		public void FillRectangle (Color color, float x, float y, float width, float height)
		{
			Control.FillRectangle (new SD.SolidBrush (Generator.Convert (color)), x - 0.5f, y - 0.5f, width, height);
		}

		public void DrawEllipse (Color color, int x, int y, int width, int height)
		{
			Control.DrawEllipse (new SD.Pen (Generator.Convert (color)), x, y, width - 1, height - 1);
		}

		public void FillEllipse (Color color, int x, int y, int width, int height)
		{
			Control.FillEllipse (new SD.SolidBrush (Generator.Convert (color)), x - 0.5f, y - 0.5f, width, height);
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
			Control.FillPath (new SD.SolidBrush (Generator.Convert (color)), path.ControlObject as SD.Drawing2D.GraphicsPath);
		}
		
		public void DrawPath (Color color, GraphicsPath path)
		{
			Control.DrawPath (new SD.Pen(Generator.Convert (color)), path.ControlObject as SD.Drawing2D.GraphicsPath);
		}
		

		public void DrawImage (Image image, int x, int y)
		{
			Control.DrawImageUnscaled ((SD.Image)image.ControlObject, x, y);
		}

		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			Control.DrawImage ((SD.Image)image.ControlObject, x, y, width, height);
		}

		public void DrawImage (Image image, Rectangle source, Rectangle destination)
		{
			this.Control.DrawImage ((SD.Image)image.ControlObject, Generator.Convert (destination), Generator.Convert (source), SD.GraphicsUnit.Pixel);
		}

        public void DrawImage(Image image, PointF point)
        {
            this.Control.DrawImage(
                (SD.Image)image.ControlObject,
                Generator.Convert(point));
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            this.Control.DrawImage(
                (SD.Image)image.ControlObject,
                Generator.Convert(rect));
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            this.Control.DrawImage(
                (SD.Image)image.ControlObject,
                x, y, width, height);
        }

        public void DrawImage(Image image, RectangleF source, RectangleF destination)
        {
            this.Control.DrawImage(
                (SD.Image)image.ControlObject,
                Generator.Convert(destination), // Note that dest is before source
                Generator.Convert(source),
                SD.GraphicsUnit.Pixel);
        }

        public void DrawIcon(Icon icon, int x, int y, int width, int height)
		{
			Control.DrawIcon ((SD.Icon)icon.ControlObject, new SD.Rectangle (x, y, width, height));
		}

        public void DrawText(Font font, Color color, float x, float y, string text)
		{
            var brush =
                BrushCache.GetBrush(color);

            SD.Brush b = null;

            if (brush != null)
                b = brush.ControlObject
                    as SD.Brush;

            if (b != null)
                Control.DrawString(
                    text, 
                    (SD.Font)font.ControlObject, b, x, y, StringFormat);
		}

		public SizeF MeasureString (Font font, string text)
		{
#if FALSE
			/* BAD (but not really!?)
			 */
			return Generator.Convert(this.Control.MeasureString(text, (SD.Font)font.ControlObject, SD.PointF.Empty, StringFormat));
			/**
			if (string.IsNullOrEmpty(text)) return Size.Empty;
			
			var format = new SD.StringFormat (SD.StringFormat.GenericTypographic);
			SD.CharacterRange[] ranges = { new SD.CharacterRange (0, text.Length) };
		
			format.FormatFlags = SD.StringFormatFlags.MeasureTrailingSpaces | SD.StringFormatFlags.NoWrap; 
			format.SetMeasurableCharacterRanges (ranges);
		
			var sdfont = (SD.Font)font.ControlObject;
			var regions = this.Control.MeasureCharacterRanges (text, sdfont, SD.Rectangle.Empty, format);
			var rect = regions [0].GetBounds (this.Control);
			
			return Generator.Convert (rect.Size);
			//s.Width += 4;
			//return s;
			/**/
#else
            var result = SizeF.Empty;

            if (font != null &&
                !string.IsNullOrEmpty(text))
            {
                var sdFont =
                    Eto.Platform.Windows.Generator.Convert(
                        font);

                var graphics = this.Control;

                SD.CharacterRange[] characterRanges = 
                { 
                    new SD.CharacterRange(0, text.Length) 
                };

                StringFormat.SetMeasurableCharacterRanges(
                    characterRanges);

                var regions =
                    graphics.MeasureCharacterRanges(
                        text,
                        Eto.Platform.Windows.Generator.Convert(font),
                        new SD.Rectangle(0, 0, 10000, 10000),
                        StringFormat);

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
			// no need to flush
		}

        public void SetClip(RectangleF rect)
        {
            this.Control.SetClip(rect.ToRectangleF());
        }

        public void FillRectangle(Brush brush, RectangleF Rectangle)
        {
            this.Control.FillRectangle((SD.Brush)brush.ControlObject, Rectangle.ToRectangleF());
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            this.Control.FillRectangle((SD.Brush)brush.ControlObject, x, y, width, height);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            this.Control.FillPath(
                (SD.Brush)brush.ControlObject, 
                (SD.Drawing2D.GraphicsPath)path.ControlObject);
        }

        public double DpiX
        {
            get { return this.Control.DpiX; }
        }

        public double DpiY
        {
            get { return this.Control.DpiY; }
        }

        public RectangleF ClipBounds
        {
            get { return this.Control.ClipBounds.ToRectangleF(); }
        }

        public Matrix Transform
        {
            get
            {
                return new Matrix(
                    Generator.Current,
                    new MatrixHandler(
                        this.Control.Transform));
            }
            set
            {
                this.Control.Transform =
                    (SD.Drawing2D.Matrix)value.ControlObject;
            }
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

        public void MultiplyTransform(Matrix matrix)
        {
            this.Control.MultiplyTransform(
                (SD.Drawing2D.Matrix)matrix.ControlObject);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            this.Control.DrawRectangle(
                (SD.Pen)pen.ControlObject,
                x, y, width, height);
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            this.Control.DrawLine(
                (SD.Pen)pen.ControlObject,
                pt1.ToPointF(),
                pt2.ToPointF());
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            this.Control.DrawPath(
                (SD.Pen)pen.ControlObject,
                (SD.Drawing2D.GraphicsPath)path.ControlObject);
        }

        public void SetClip(Graphics graphics)
        {
            this.Control.SetClip(Generator.Convert(graphics));
        }


        private Stack<SD.Drawing2D.Matrix> savedTransforms;

        public void SaveTransform()
        {
            if (savedTransforms == null)
                savedTransforms =
                    new Stack<SD.Drawing2D.Matrix>();

            savedTransforms.Push(
                Control.Transform);
        }

        public void RestoreTransform()
        {
            if (savedTransforms != null &&
                savedTransforms.Count > 0)
            {
                var t =
                    savedTransforms.Pop();

                Control.Transform =
                    t;

                t.Dispose();
            }
        }
    }
}
