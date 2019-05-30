
namespace Eto.Drawing
{
	/// <summary>
	/// Specifies the line cap for a <see cref="Pen"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum PenLineCap
	{
		/// <summary>
		/// Lines have a square cap, that is the same size as the width of the pen
		/// </summary>
		Square,

		/// <summary>
		/// Lines are capped exactly at the ending points of the line
		/// </summary>
		Butt,

		/// <summary>
		/// Lines have a rounded cap, which is equal to the width of the pen
		/// </summary>
		Round
	}
}

