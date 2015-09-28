using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(DropDown.IHandler))]
	public class DropDown : ListControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets a value indicating whether to show the control's border.
		/// </summary>
		/// <remarks>
		/// This is a hint to omit the border of the control and show it as plainly as possible.
		/// 
		/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
		/// </remarks>
		/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowBorder
		{
			get { return Handler.ShowBorder; }
			set { Handler.ShowBorder = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="DropDown"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether to show the control's border.
			/// </summary>
			/// <remarks>
			/// This is a hint to omit the border of the control and show it as plainly as possible.
			/// 
			/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
			/// </remarks>
			/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
			bool ShowBorder { get; set; }
		}
	}
}
