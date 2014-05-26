
namespace Eto.Drawing
{
	/// <summary>
	/// Specifies how lines are joined for a <see cref="Pen"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum PenLineJoin
	{
		/// <summary>
		/// Uses a miter to join lines, usually within a certain limit specified by <see cref="Pen.MiterLimit"/>
		/// </summary>
		Miter,

		/// <summary>
		/// Uses a bevel along the angle of the join
		/// </summary>
		Bevel,

		/// <summary>
		/// Uses a rounded edge to join lines
		/// </summary>
		Round
	}
}
