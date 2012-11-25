using System;
using System.IO;
using System.Reflection;

namespace Eto.Drawing
{
    public interface IBrush : IInstanceWidget
	{
        // Creates a linear gradient brush
        void Create(RectangleF rectangle, Color c1, Color c2, float angle);

        void Create(Bitmap b);

        void Create(Color color);

        void Create(PointF point1, PointF point2, Color color1, Color color2);

        void TranslateTransform(float x, float y);
    }
	
	public class Brush : InstanceWidget
	{
		IBrush inner;
		
        public Brush(): this(Generator.Current)
        {
        }

        public Brush(Generator g)
            : base(g, typeof(IBrush))
        {
            inner = (IBrush)this.Handler;
        }

        public Brush(
            RectangleF rectangle, Color c1, Color c2, float angle)
            : this(Generator.Current)
        {
            inner.Create(
                rectangle, c1, c2, angle);
        }

        /// <summary>
        /// Texture brush
        /// </summary>
        /// <param name="b"></param>
        public Brush(
            Bitmap b)
            : this(Generator.Current)
        {
            inner.Create(b);
        }

        public Brush(Color color)
            : this(Generator.Current)
        {
            inner = (IBrush)Handler;
            inner.Create(color);
        }

        /// <summary>
        /// Valid only for texture brushes.
        /// </summary>
        public void TranslateTransform(float x, float y)
        {
            inner.TranslateTransform(x, y);
        }

        /// <summary>
        /// Linear Gradient
        /// </summary>
        public Brush(
            PointF point1, PointF point2, Color color1, Color color2)
            : this(Generator.Current)
        {
            inner.Create(point1, point2, color1, color2);            
        }
    }
}
