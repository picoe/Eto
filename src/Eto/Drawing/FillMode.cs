
namespace Eto.Drawing
{
	/// <summary>
	/// Mode for how a closed <see cref="IGraphicsPath"/> is filled
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum FillMode
	{
		/// <summary>
		/// Alternating / Even-Odd fill mode
		/// </summary>
		Alternate = 0,

		/// <summary>
		/// Winding fill mode
		/// </summary>
		Winding = 1,
	}
}
