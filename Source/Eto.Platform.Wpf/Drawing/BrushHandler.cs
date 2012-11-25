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
            Control = new swm.SolidColorBrush(color.ToWpf());
        }

        public void Create(PointF point1, PointF point2, Color color1, Color color2)
        {
            Control = new swm.LinearGradientBrush(color1.ToWpf(), color2.ToWpf(), point1.ToWpf(), point2.ToWpf());
        }

        public void TranslateTransform(float x, float y)
        {
            throw new NotImplementedException();
        }
    }
}
