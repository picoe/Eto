using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class GraphicsHandler : WidgetHandler<swm.DrawingContext, Graphics>, IGraphics
	{
		public GraphicsHandler (swm.DrawingContext context)
		{
			this.Control = context;
		}

		public void CreateFromImage (Bitmap image)
		{
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
		}

		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
		}

		public void FillPath (Color color, GraphicsPath path)
		{
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
		}

		public void DrawImage (IImage image, int x, int y)
		{
		}

		public void DrawImage (IImage image, int x, int y, int width, int height)
		{
		}

		public void DrawImage (IImage image, Rectangle source, Rectangle destination)
		{
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
		}

		public SizeF MeasureString (Font font, string text)
		{
			return SizeF.Empty;
		}

		public Region ClipRegion
		{
			get; set; 
		}

		public void Flush ()
		{
		}

		public bool Antialias
		{
			get; set; 
		}
	}
}
