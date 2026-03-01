namespace Eto.Forms;

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
	public object Value
	{
		get => InternalValue;
		set => InternalValue = value;
	}

	internal virtual object InternalValue { get; set; }
	
	/// <summary>
	/// Gets or sets the update mode for this change event
	/// This is used to determine if the change is being set to the source or target of the binding
	/// </summary>
	public BindingUpdateMode? UpdateMode { get; internal set; }

	/// <summary>
	/// Initializes a new instance of the BindingChangingEventArgs with the specifid value
	/// </summary>
	/// <param name="value"></param>
	public BindingChangingEventArgs(object value)
	{
		Value = value;
	}
	
	/// <summary>
	/// Initializes a new instance of the BindingChangingEventArgs with the specified value and update mode
	/// </summary>
	/// <param name="value">value that the binding is being set to</param>
	/// <param name="updateMode">update mode for this change event</param>
	public BindingChangingEventArgs(object value, BindingUpdateMode? updateMode)
	{
		Value = value;
		UpdateMode = updateMode;
	}

	/// <summary>
	/// Initializes a new instance of the BindingChangingEventArgs
	/// </summary>
	internal BindingChangingEventArgs()
	{
	}
}