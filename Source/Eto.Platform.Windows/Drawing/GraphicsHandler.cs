using System;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class RegionHandler : Region
	{
		SD.Region region;

		public RegionHandler (SD.Region region)
		{
			this.region = region;
		}

		public override object ControlObject {
			get { return region; }
		}

		public override void Exclude (Rectangle rect)
		{
			region.Exclude (Generator.Convert (rect));
		}

		public override void Reset ()
		{
			region.MakeInfinite ();
		}

		public override void Set (Rectangle rect)
		{
			region.MakeEmpty ();
			region.Complement (Generator.Convert (rect));
		}
	}

	public class GraphicsHandler : WidgetHandler<SD.Graphics, Graphics>, IGraphics
	{
		
		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (SD.Graphics graphics)
		{
			this.Control = graphics;
			Control.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			//this.Control.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			//this.Control.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			Control.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear; //.NearestNeighbor;
		}

		public void CreateFromImage (Bitmap image)
		{
			Control = SD.Graphics.FromImage ((SD.Image)image.ControlObject);
		}

		public void Commit ()
		{
		}

		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			this.Control.DrawLine (new SD.Pen (Generator.Convert (color)), startx, starty, endx, endy);
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			Control.DrawRectangle (new SD.Pen (Generator.Convert (color)), x, y, width, height);
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			Control.FillRectangle (new SD.SolidBrush (Generator.Convert (color)), x, y, width, height);
		}

		public void DrawImage (IImage image, int x, int y)
		{
			Control.DrawImageUnscaled ((SD.Image)image.ControlObject, x, y);
		}

		public void DrawImage (IImage image, int x, int y, int width, int height)
		{
			Control.DrawImage ((SD.Image)image.ControlObject, x, y, width, height);
		}

		public void DrawImage (IImage image, Rectangle source, Rectangle destination)
		{
			this.Control.DrawImage ((SD.Image)image.ControlObject, Generator.Convert (destination), Generator.Convert (source), SD.GraphicsUnit.Pixel);
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			Control.DrawIcon ((SD.Icon)icon.ControlObject, new SD.Rectangle (x, y, width, height));
		}

		public Region ClipRegion {
			get { return new RegionHandler (Control.Clip); }
			set { Control.Clip = (SD.Region)((RegionHandler)value).ControlObject; }
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			SD.Brush brush = new SD.SolidBrush (Generator.Convert (color));
			Control.DrawString (text, (SD.Font)font.ControlObject, brush, x, y);
			brush.Dispose ();
		}

		public SizeF MeasureString (Font font, string text)
		{
			// BAD: return Generator.Convert(this.Control.MeasureString(text, (SD.Font)font.ControlObject));
			if (string.IsNullOrEmpty(text)) return Size.Empty;
			
			var format = new SD.StringFormat ();
			SD.CharacterRange[] ranges = { new SD.CharacterRange (0, text.Length) };
		
			format.FormatFlags = SD.StringFormatFlags.MeasureTrailingSpaces | SD.StringFormatFlags.NoWrap;
			format.SetMeasurableCharacterRanges (ranges);
		
			var sdfont = (SD.Font)font.ControlObject;
			var rect = new SD.RectangleF(0, 0, 10000, 1000);
			var regions = this.Control.MeasureCharacterRanges (text, sdfont, rect, format);
			rect = regions [0].GetBounds (this.Control);
			
			return Generator.Convert (rect.Size);
		}

		public void Flush ()
		{
			// no need to flush
		}


	}
}
