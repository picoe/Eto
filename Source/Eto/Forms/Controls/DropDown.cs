using System;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(DropDown.IHandler))]
	public class DropDown : ListControl
	{
		/// <summary>
		/// Handler interface for the <see cref="DropDown"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler
		{
		}
	}
}
