using System;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a spinning indeterminate progress spinner wheel
	/// </summary>
	/// <remarks>
	/// Use the <see cref="Control.Enabled"/> property to control whether the spinner is active or not.
	/// </remarks>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Spinner.IHandler))]
	public class Spinner : Control
	{
		/// <summary>
		/// Initializes a new instance of the Spinner class
		/// </summary>
		public Spinner()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Spinner class with the specified generator
		/// </summary>
		/// <param name="generator">Generator to create the spinner from</param>
		[Obsolete("Use default constructor instead")]
		public Spinner(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="Spinner"/> control
		/// </summary>
		/// <copyright>(c) 2013 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public interface IHandler : Control.IHandler
		{
		}
	}
}

