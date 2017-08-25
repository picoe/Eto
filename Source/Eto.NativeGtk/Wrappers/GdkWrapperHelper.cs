using System;

static partial class GdkWrapper
{
    public struct RGBA
    {
        public double Red;
        public double Green;
        public double Blue;
        public double Alpha;

        public Eto.Drawing.Color ToColor()
        {
            return new Eto.Drawing.Color((float)Red, (float)Green, (float)Blue, (float)Alpha);
        }

        public double[] ToDouble()
        {
            return new[] { Red, Green, Blue, Alpha };
        }
    }

    public static RGBA ToNativeRGBA(this Eto.Drawing.Color color)
    {
        return new RGBA { Alpha = color.A, Blue = color.B, Green = color.G, Red = color.R };
    }

    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}
