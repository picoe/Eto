using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Base interface for each type of brush supported in the system
	/// </summary>
	/// <remarks>
	/// The base brush interface is used to be able to pass any type of brush to graphics methods that take
	/// a brush as a parameter.
	/// </remarks>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IBrush : IDisposable
	{
		/// <summary>
		/// Platform-specific control object
		/// </summary>
		/// <remarks>
		/// This is used by platform code to get the native brush control.
		/// </remarks>
		object ControlObject { get; }
	}
}

