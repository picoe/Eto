using System;

namespace Eto.Forms
{
	/// <summary>
	/// An entry box for the user to enter a password without displaying the contents of the password while typed.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(PasswordBox.IHandler))]
	public class PasswordBox : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets a value indicating whether the value is read only
		/// </summary>
		/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets the maximum length of password the user can enter
		/// </summary>
		/// <value>The maximum length</value>
		public virtual int MaxLength
		{
			get { return Handler.MaxLength; }
			set { Handler.MaxLength = value; }
		}

		/// <summary>
		/// Gets or sets the password display character hint
		/// </summary>
		/// <remarks>
		/// Some platforms may not support changing the password display character
		/// </remarks>
		/// <value>The password character</value>
		public char PasswordChar
		{
			get { return Handler.PasswordChar; }
			set { Handler.PasswordChar = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="PasswordBox"/> control
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the value is read only
			/// </summary>
			/// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
			bool ReadOnly { get; set; }

			/// <summary>
			/// Gets or sets the maximum length of password the user can enter
			/// </summary>
			/// <value>The maximum length</value>
			int MaxLength { get; set; }

			/// <summary>
			/// Gets or sets the password display character hint
			/// </summary>
			/// <remarks>
			/// Some platforms may not support changing the password display character
			/// </remarks>
			/// <value>The password character</value>
			char PasswordChar { get; set; }
		}
	}
}
