using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
    /// <summary>
    /// Similar to TextAlignment, but used by renderers
    /// since they do not natively support Justify.
    /// </summary>
    public enum StringAlignment
    {
        Near,
        Center,
        Far
    }
}
