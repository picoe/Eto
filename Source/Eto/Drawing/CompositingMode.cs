using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	// Summary:
	//     Specifies how the source colors are combined with the background colors.
	public enum CompositingMode
	{
		// Summary:
		//     Specifies that when a color is rendered, it is blended with the background
		//     color. The blend is determined by the alpha component of the color being
		//     rendered.
		SourceOver = 0,
		//
		// Summary:
		//     Specifies that when a color is rendered, it overwrites the background color.
		SourceCopy = 1,
	}
}
