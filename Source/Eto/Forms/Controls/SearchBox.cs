using System;

namespace Eto.Forms
{
	/// <summary>
	/// Search box control
	/// </summary>
	/// <remarks>
	/// The search box control is similar to a plain text box, but provides platform-specific styling.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(IHandler))]
	public class SearchBox: TextBox
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SearchBox"/> class.
		/// </summary>
		public SearchBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SearchBox"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		[Obsolete("Use default constructor instead")]
		public SearchBox(Generator generator) 
			: this(generator, typeof(IHandler))
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SearchBox"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler to create (must implement <see cref="IHandler"/>)></param>
		/// <param name="initialize">True to initialize the handler, or false if the caller should initialize</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SearchBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="SearchBox"/> control
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : TextBox.IHandler
		{
		}
	}
}
