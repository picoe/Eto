using System;
using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
    public class BrushHandler : WidgetHandler<swm.Brush, Brush>, IBrush
    {
        public void Create(RectangleF rectangle, Color c1, Color c2, float angle)
        {
            throw new NotImplementedException();
        }

        public void Create(Bitmap b)
        {
            throw new NotImplementedException();
        }

        public void Create(Color color)
        {
            throw new NotImplementedException();
        }

        public void Create(Point point1, Point point2, Color color1, Color color2)
        {
            throw new NotImplementedException();
        }

        public void Create(PointF point1, PointF point2, Color color1, Color color2)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(float x, float y)
        {
            throw new NotImplementedException();
        }
    }
}
