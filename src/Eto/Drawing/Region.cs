using System;


namespace Eto.Drawing
{

	/// <summary>
	/// Defines a region to use for clipping
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Region.IHandler))]
	public class Region : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Excludes the specified <paramref name="rectangle"/> from the region
		/// </summary>
		/// <param name="rectangle">Rectangle to exclude</param>
		public void Exclude (Rectangle rectangle)
		{
			Handler.Exclude (rectangle);
		}

		/// <summary>
		/// Resets the region
		/// </summary>
		public void Reset ()
		{
			Handler.Reset ();
		}

		/// <summary>
		/// Sets the specified <paramref name="rectangle"/> in the region
		/// </summary>
		/// <param name="rectangle">Rectangle to set the region to</param>
		public void Set (Rectangle rectangle)
		{
			Handler.Set (rectangle);
		}

		/// <summary>
		/// Handler interface for the <see cref="Region"/> class
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Excludes the specified <paramref name="rectangle"/> from the region
			/// </summary>
			/// <param name="rectangle">Rectangle to exclude</param>
			void Exclude (Rectangle rectangle);

			/// <summary>
			/// Resets the region
			/// </summary>
			void Reset ();

			/// <summary>
			/// Sets the specified <paramref name="rectangle"/> in the region
			/// </summary>
			/// <param name="rectangle">Rectangle to set the region to</param>
			void Set (Rectangle rectangle);
		}

	}

}
