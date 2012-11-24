using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Platform.Mac.Drawing
{
    public class BrushHandler : WidgetHandler<Brush>, IBrush
    {
        public Color Color { get; set; }

        public void Create(RectangleF rectangle, Color c1, Color c2, float angle)
        {
            
        }

        public void Create(Bitmap b)
        {
            
        }

        public void Create(Color color)
        {
            Color = color;
        }

        public void Create(Point point1, Point point2, Color color1, Color color2)
        {
            Color = color1; // TODO   
        }

        public void Create(PointF point1, PointF point2, Color color1, Color color2)
        {
            Color = color1; // TODO
        }

        public void TranslateTransform(float x, float y)
        {
            
        }
    }
}
