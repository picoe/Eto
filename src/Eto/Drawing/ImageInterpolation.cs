
namespace Eto.Drawing
{
	/// <summary>
	/// Interpolation modes when drawing images using the <see cref="Graphics"/> object
	/// </summary>
	/// <seealso cref="Graphics.ImageInterpolation"/>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ImageInterpolation
	{
		/// <summary>
		/// Default interplation mode - usually a balance between quality vs. performance
		/// </summary>
		Default,

		/// <summary>
		/// No interpolation (also known as nearest neighbour)
		/// </summary>
		None,

		/// <summary>
		/// Low interpolation quality (usually fastest)
		/// </summary>
		Low,

		/// <summary>
		/// Medium interpolation quality slower than <see cref="Low"/>, but better quality
		/// </summary>
		Medium,

		/// <summary>
		/// Highest interpolation quality - slowest but best quality
		/// </summary>
		High
	}

}
