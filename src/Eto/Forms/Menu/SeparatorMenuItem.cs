using System;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item to separate menu items
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(SeparatorMenuItem.IHandler))]
	public class SeparatorMenuItem : MenuItem
	{
		/// <summary>
		/// Handler interface for the <see cref="SeparatorMenuItem"/>
		/// </summary>
		public new interface IHandler : MenuItem.IHandler
		{
		}
	}
}