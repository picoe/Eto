using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments for when a binding's value is changing
	/// </summary>
	/// <remarks>
	/// When handling the event, one could cancel setting the new value by setting the <see cref="CancelEventArgs.Cancel"/>
	/// property to false.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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
