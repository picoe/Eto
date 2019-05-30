using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for controls with common functionality
	/// </summary>
	/// <remarks>
	/// Currently provides a way to change the font for controls.
	/// 
	/// Any control with textual input or display should derive from this class.
	/// Any container or specialized control where there are multiple fonts, etc should define their own properties.
	/// </remarks>
	public abstract class CommonControl : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CommonControl"/> class.
		/// </summary>
		protected CommonControl()
		{
		}

		/// <summary>
		/// Gets or sets the font for the text of the control
		/// </summary>
		/// <value>The text font.</value>
		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="CommonControl"/>.
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the font for the text of the control
			/// </summary>
			/// <value>The text font.</value>
			Font Font { get; set; }
		}
	}
}

