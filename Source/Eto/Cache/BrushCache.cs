using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Cache
{
    /// <summary>
    /// A singleton app wide cache of brushes
    /// </summary>
    public static class BrushCache
    {
        private static Dictionary<Color, Brush> brushes =
            new Dictionary<Color, Brush>();

        public static Brush GetBrush(Color color)
        {
            Brush result = null;

            if (brushes != null &&
                !brushes.TryGetValue(color, out result))
            {
                result = new Brush(color);
                brushes[color] = result;
            }

            return result;
        }
    }
}
