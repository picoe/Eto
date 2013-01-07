using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Cache
{
    /// <summary>
    /// A singleton app wide cache of brushes
    /// </summary>
    public static class BrushCache
    {
        struct CacheKey
        {
            public Color Color;
            public Generator Generator;
        }

        private static Dictionary<CacheKey, Brush> brushes =
            new Dictionary<CacheKey, Brush>();

        public static Brush GetBrush(
            Generator generator,
            Color color)
        {
            Brush result = null;

            var key =
                new CacheKey
                {
                    Generator = generator ?? Generator.Current,
                    Color = color,
                };

            if (brushes != null &&
                !brushes.TryGetValue(key, out result))
            {
                result = new SolidBrush(color, generator);
                brushes[key] = result;
            }

            return result;
        }
    }
}
