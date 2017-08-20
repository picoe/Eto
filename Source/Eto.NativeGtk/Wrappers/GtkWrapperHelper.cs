using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

static partial class GtkWrapper
{
    public const int GTK_POS_LEFT = 0;
    public const int GTK_POS_RIGHT = 1;
    public const int GTK_POS_TOP = 2;
    public const int GTK_POS_BOTTOM = 3;

    public const int GTK_RESPONSE_NONE = -1;
    public const int GTK_RESPONSE_REJECT = -2;
    public const int GTK_RESPONSE_ACCEPT = -3;
    public const int GTK_RESPONSE_DELETE_EVENT = -4;
    public const int GTK_RESPONSE_OK = -5;
    public const int GTK_RESPONSE_CANCEL = -6;
    public const int GTK_RESPONSE_CLOSE = -7;
    public const int GTK_RESPONSE_YES = -8;
    public const int GTK_RESPONSE_NO = -9;
    public const int GTK_RESPONSE_APPLY = -10;
    public const int GTK_RESPONSE_HELP = -11;

    public const int PANGO_WRAP_WORD = 0;
    public const int PANGO_WRAP_CHAR = 1;
    public const int PANGO_WRAP_WORD_CHAR = 2;

    public const int GTK_ALIGN_FILL = 0;
    public const int GTK_ALIGN_START = 1;
    public const int GTK_ALIGN_END = 2;
    public const int GTK_ALIGN_CENTER = 3;
    public const int GTK_ALIGN_BASELINE = 4;

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

    public struct GtkAllocation
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public struct GdkRectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public static double[] ToDouble(this Eto.Drawing.Color color)
    {
        return new double[] { color.R, color.G, color.B, color.A };
    }

    public static GtkWrapper.RGBA ToNativeRGBA(this Eto.Drawing.Color color)
    {
        return new RGBA { Alpha = color.A, Blue = color.B, Green = color.G, Red = color.R };
    }
}
