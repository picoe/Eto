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
		/// Handler interface for the <see cref="SearchBox"/> control
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : TextBox.IHandler
		{
		}
	}
}
