using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines a pen to be used with the <see cref="Graphics"/> drawing methods
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IPen : IDisposable
	{
		/// <summary>
		/// Gets or sets the color of the pen
		/// </summary>
		Color Color { get; set; }

		/// <summary>
		/// Gets or sets the thickness (width) of the pen
		/// </summary>
		float Thickness { get; set; }

		/// <summary>
		/// Gets or sets the style of line join for the pen
		/// </summary>
		PenLineJoin LineJoin { get; set; }

		/// <summary>
		/// Gets or sets the line cap style
		/// </summary>
		PenLineCap LineCap { get; set; }

		/// <summary>
		/// Gets or sets the miter limit for the pen
		/// </summary>
		/// <remarks>
		/// The miter limit specifies the maximum allowed ratio of miter lenth to stroke length in which a 
		/// miter will be converted to a bevel.  The default miter limit is 10.
		/// </remarks>
		float MiterLimit { get; set; }

		/// <summary>
		/// Gets the platform-specific control for this pen
		/// </summary>
		object ControlObject { get; }
	}

	/// <summary>
	/// Handler for the <see cref="IPen"/> interface
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IPenHandler : IPen
	{
		/// <summary>
		/// Creates a new pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color for the new pen</param>
		/// <param name="thickness">Thickness of the new pen</param>
		void Create (Color color, float thickness);
	}

	/// <summary>
	/// Methods to create <see cref="IPen"/> objects for use with drawing methods in <see cref="Graphics"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Pen
	{
		/// <summary>
		/// Creates a new pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color for the new pen</param>
		/// <param name="thickness">Thickness of the new pen</param>
		/// <param name="generator">Generator to create the pen for</param>
		public static IPen Create (Color color, float thickness = 1f, Generator generator = null)
		{
			var handler = (generator ?? Generator.Current).Create<IPenHandler> ();
			handler.Create (color, thickness);
			return handler;
		}
	}
}

