using System;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsHandler : WidgetHandler<System.Drawing.Graphics, Graphics>, IGraphics
	{
		ImageInterpolation imageInterpolation;

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (SD.Graphics graphics)
		{
			this.Control = graphics;
			Control.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			//this.Control.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			//this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			Control.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear; //.NearestNeighbor;
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
				Control.InterpolationMode = value.ToSD ();
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
				this.Control.FillRectangle (new SD.SolidBrush (color.ToSD ()), startx, starty, 1, 1);
			}
			else 
				this.Control.DrawLine (new SD.Pen (color.ToSD ()), startx, starty, endx, endy);
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			Control.DrawRectangle (new SD.Pen (color.ToSD ()), x, y, width-1, height-1);
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			Control.FillRectangle (new SD.SolidBrush (color.ToSD ()), x - 0.5f, y - 0.5f, width, height);
		}

		public void DrawEllipse (Color color, int x, int y, int width, int height)
		{
			Control.DrawEllipse (new SD.Pen (color.ToSD ()), x, y, width - 1, height - 1);
		}

		public void FillEllipse (Color color, int x, int y, int width, int height)
		{
			Control.FillEllipse (new SD.SolidBrush (color.ToSD ()), x - 0.5f, y - 0.5f, width, height);
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
			Control.FillPath (new SD.SolidBrush (color.ToSD ()), path.ControlObject as SD.Drawing2D.GraphicsPath);
		}
		
		public void DrawPath (Color color, GraphicsPath path)
		{
			Control.DrawPath (new SD.Pen(color.ToSD ()), path.ControlObject as SD.Drawing2D.GraphicsPath);
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
			this.Control.DrawImage ((SD.Image)image.ControlObject, destination.ToSD (), source.ToSD (), SD.GraphicsUnit.Pixel);
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			Control.DrawIcon ((SD.Icon)icon.ControlObject, new SD.Rectangle (x, y, width, height));
		}

		public Region ClipRegion {
			get { return null; }
			set { }
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			SD.Brush brush = new SD.SolidBrush (color.ToSD ());
			var format = new SD.StringFormat (SD.StringFormat.GenericTypographic);
			format.FormatFlags = SD.StringFormatFlags.MeasureTrailingSpaces | SD.StringFormatFlags.NoWrap;
			Control.DrawString (text, (SD.Font)font.ControlObject, brush, x, y, format);
			brush.Dispose ();
		}

		public SizeF MeasureString (Font font, string text)
		{
			/* BAD (but not really!?)
			 */
			var format = new SD.StringFormat (SD.StringFormat.GenericTypographic);
			format.FormatFlags = SD.StringFormatFlags.MeasureTrailingSpaces | SD.StringFormatFlags.NoWrap; 
			return this.Control.MeasureString(text, (SD.Font)font.ControlObject, SD.PointF.Empty, format).ToEto ();
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
		}

		public void Flush ()
		{
			// no need to flush
		}


	}
}
