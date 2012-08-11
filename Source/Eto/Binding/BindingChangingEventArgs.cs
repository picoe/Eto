using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Arguments for when a binding's value is changing
	/// </summary>
	/// <remarks>
	/// When handling the event, one could cancel setting the new value by setting the <see cref="BindingChangingEventArgs.Cancel"/>
	/// property to false.
	/// </remarks>
	public class BindingChangingEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Proposed value to set to the binding
		/// </summary>
		public object Value { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the BindingChangingEventArgs
		/// </summary>
		/// <param name="value"></param>
		public BindingChangingEventArgs (object value)
		{
			this.Value = value;
		}
	}
	
}
