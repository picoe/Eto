
namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the pixel offset modes of a <see cref="Graphics"/>
	/// </summary>
	/// <remarks>
	/// The pixel offset mode usually applies to all graphics operations such as 
	/// <see cref="Graphics.DrawLine(Pen,PointF,PointF)"/>, <see cref="Graphics.DrawRectangle(Pen,RectangleF)"/>, etc.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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
