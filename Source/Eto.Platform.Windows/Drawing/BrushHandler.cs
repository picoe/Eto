using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class BrushHandler : WidgetHandler<System.Drawing.Brush, Brush>, IBrush
	{
        public BrushHandler()
        {
        }

        public BrushHandler(SD.Brush brush)
        {
            this.Control = brush;
        }

        #region IBrush Members

        public void Create(RectangleF rectangle, Color c1, Color c2, float angle)
        {
            this.Control =
                new SD.Drawing2D.LinearGradientBrush(
                    Generator.Convert(rectangle),
                    Generator.Convert(c1),
                    Generator.Convert(c2),
                    angle);
        }

        public void Create(Bitmap b)
        {
            this.Control =
                new SD.TextureBrush(
                    (SD.Image)b.ControlObject);
        }

        public void Create(Color color)
        {
            this.Control = 
                new SD.SolidBrush(
                    Generator.Convert(color));
        }

        public void Create(Point point1, Point point2, Color color1, Color color2)
        {
            this.Control =
                new SD.Drawing2D.LinearGradientBrush(
                    Generator.Convert(point1),
                    Generator.Convert(point2),
                    Generator.Convert(color1),
                    Generator.Convert(color2));
        }

        public void Create(PointF point1, PointF point2, Color color1, Color color2)
        {
            this.Control =
                new SD.Drawing2D.LinearGradientBrush(
                    Generator.Convert(point1),
                    Generator.Convert(point2),
                    Generator.Convert(color1),
                    Generator.Convert(color2));
        }

        public void TranslateTransform(float x, float y)
        {
            ((SD.TextureBrush)this.Control).TranslateTransform(x, y);
        }

        #endregion
    }
}
