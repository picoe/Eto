using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Arguments to handle when a binding value has changed
	/// </summary>
	public class BindingChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the value that was set to the binding
		/// </summary>
		public object Value { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the BindingChangedEventArgs
		/// </summary>
		/// <param name="value">value that the binding was set to</param>
		public BindingChangedEventArgs (object value)
		{
			this.Value = value;
		}
	}
}
