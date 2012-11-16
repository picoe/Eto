using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Cache
{
    struct PenKey
    {
        public float Width { get; set; }
        public Color Color { get; set; }
        public PenAlignment PenAlignment { get; set; }
        public DashStyle DashStyle { get; set; }
    }

    /// <summary>
    /// A singleton app wide cache of pens
    /// </summary>
    public static class PenCache
    {
        private static Dictionary<PenKey, Pen> pens =
            new Dictionary<PenKey, Pen>();

        public static Pen GetPen(Color color)
        {
            return GetPen(
                color,
                1f,
                PenAlignment.Center,
                DashStyle.Solid);
        }

        public static Pen GetPen(
            Color color,
            float width)
        {
            return GetPen(
                color,
                width,
                PenAlignment.Center,
                DashStyle.Solid);
        }

        public static Pen GetPen(
            Color color,
            float width,
            PenAlignment alignment,
            DashStyle dashStyle)
        {
            Pen result = null;

            var key = new PenKey()
            {
                Color = color,
                Width = width,
                DashStyle = dashStyle,
                PenAlignment = alignment
            };

            if (pens != null &&
                !pens.TryGetValue(key, out result))
            {
                result = new Pen(Generator.Current, color, width, alignment, dashStyle);

                pens[key] = result;
            }

            return result;
        }
    }
}
