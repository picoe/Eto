
namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the pixel offset modes of a <see cref="Graphics"/>
	/// </summary>
	/// <remarks>
	/// The pixel offset mode applies to all Draw* graphics operations such as 
	/// <see cref="Graphics.DrawLine(Pen,PointF,PointF)"/>, <see cref="Graphics.DrawRectangle(Pen,RectangleF)"/>, etc.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum PixelOffsetMode
	{
		/// <summary>
		/// Specifies that pixels will not be offset and be relative to the center of each pixel.
		/// </summary>
		/// <remarks>
		/// This simplifies drawing odd-width lines by aligning them to the pixel boundary.
		/// </remarks>
		None,
		/// <summary>
		/// Specifies that pixels are offset by half a pixel (-0.5) in both the horizontal and vertical axes. Use for high speed.
		/// </summary>
		/// <remarks>
		/// In this mode, the center of lines will be at the point between logical pixels and can improve performance with some platforms. 
		/// Horizontal or vertical lines that are not a fraction will typically be antialiased.
		/// For example, to draw a 1px wide horizontal line without antialiasing, you would have to draw at a 0.5 offset in the vertical axis.
		/// 
		/// Only Draw operations are offset with this mode, Fill operations will not be offset. (new in 2.1)
		/// For example, filling a rectangle from 10, 10 to 20, 20 will not be antialiased and fall on (logical) pixel boundaries.
		/// </remarks>
		Half
	}
}
