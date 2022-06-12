
namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the pixel offset modes of a <see cref="Graphics"/>
	/// </summary>
	/// <remarks>
	/// The pixel offset mode applies to all Draw* and Fill* graphics operations such as 
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
		/// This simplifies drawing odd-width lines and fills by aligning them to the pixel boundary.
		/// Fills on this mode have crisp pixel-aligned boundaries, such as <see cref="Graphics.FillRectangle(Brush, RectangleF)" /> and variants.
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
		Half,
		/// <summary>
		/// Similar to None, specifies that pixels will be relative to the center of each pixel, but only for line drawing.
		/// </summary>
		/// <remarks>
		/// This simplifies drawing lines and fills with the same dimensions, as they will both be offset by 0.5 pixels.
		/// Note that in this mode, any fills will not be aligned on a pixel boundary, such as <see cref="Graphics.FillRectangle(Brush, RectangleF)" /> or other variants.
		/// </remarks>
		Aligned
	}
}
