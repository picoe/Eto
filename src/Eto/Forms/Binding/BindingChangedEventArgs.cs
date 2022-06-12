using System;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments to handle when a binding value has changed
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BindingChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the value that was set to the binding
		/// </summary>
		public object Value => InternalValue;

		internal virtual object InternalValue { get; set; }

		/// <summary>
		/// Initializes a new instance of the BindingChangedEventArgs with the specified value
		/// </summary>
		/// <param name="value">value that the binding was set to</param>
		public BindingChangedEventArgs(object value)
		{
			this.InternalValue = value;
		}

		/// <summary>
		/// Initializes a new instance of the BindingChangedEventArgs
		/// </summary>
		internal BindingChangedEventArgs()
		{
		}
	}
}
