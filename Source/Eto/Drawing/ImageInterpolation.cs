using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	/// <summary>
	/// Interpolation modes when drawing images using the <see cref="Graphics"/> object
	/// </summary>
	/// <seealso cref="Graphics.ImageInterpolation"/>
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
