using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the pixel offset modes of a <see cref="Graphics"/>
	/// </summary>
	/// <remarks>
	/// The pixel offset mode usually applies to all graphics operations such as 
	/// <see cref="Graphics.DrawLine"/>, <see cref="Graphics.DrawRectangle"/>, etc.
	/// </remarks>
	public enum PixelOffsetMode
	{
		/// <summary>
		/// Specifies that pixels are offset by half a pixel (-0.5)
		/// </summary>
		/// <remarks>
		/// This provides the best visual result by aligning to the pixel grid
		/// </remarks>
		Half,

		/// <summary>
		/// Specifies that pixels will not be offset and be relative to the center of each pixel
		/// </summary>
		/// <remarks>
		/// In this mode, vertical or horizontal lines that are not a fraction will typically be antialiased.
		/// </remarks>
		None
	}
}
