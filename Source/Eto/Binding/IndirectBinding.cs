using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Provides an indirect binding to an indeterminate source/destination
	/// </summary>
	/// <remarks>
	/// This binding does not directly bind to an object - you must pass the
	/// object to get/set the value.  The <see cref="DirectBinding"/> differs in 
	/// that it binds directly to an object.
	/// 
	/// The IndirectBinding is useful when you want to use the same binding on multiple
	/// objects, such as when binding cells in a <see cref="Forms.Grid"/>.
	/// 
	/// Typically one would use <see cref="PropertyBinding"/> or <see cref="ColumnBinding"/>
	/// which are ways to retrieve either a property value or column/index-based value.
	/// </remarks>
	public abstract class IndirectBinding : Binding
	{
		/// <summary>
		/// Gets the value from the specified object using this binding
		/// </summary>
		/// <remarks>
		/// When values are needed from this binding, this method will be called.
		/// Implementors of this binding would implement logic in <see cref="InternalGetValue"/>
		/// </remarks>
		/// <param name="dataItem">object to retrieve the value from</param>
		/// <returns>value from the specified object</returns>
		public object GetValue (object dataItem)
		{
			return InternalGetValue (dataItem);
		}
		
		/// <summary>
		/// Sets the specified value to an object using this binding
		/// </summary>
		/// <remarks>
		/// This is called to set the value to the object. Implementors of this binding
		/// woulc implement logic in <see cref="InternalSetValue"/>.
		/// </remarks>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set to the object</param>
		public void SetValue (object dataItem, object value)
		{ 
			var args = new BindingChangingEventArgs (value);
			OnChanging (args);
			InternalSetValue (dataItem, args.Value);
			OnChanged (new BindingChangedEventArgs (args.Value));
		}

		/// <summary>
		/// Implements the logic to get the value from the specified object
		/// </summary>
		/// <remarks>
		/// Implementors of this binding must implement this method to get the value from the specified object
		/// </remarks>
		/// <param name="dataItem">object to get the value from</param>
		/// <returns>value from this binding of the specified object</returns>
		protected abstract object InternalGetValue (object dataItem);
		
		/// <summary>
		/// Implements the logic to set the value to the specified object
		/// </summary>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set on the dataItem for this binding</param>
		protected abstract void InternalSetValue (object dataItem, object value);
		

		/// <summary>
		/// Adds a handler to trap when the value of this binding changes for the specified object
		/// </summary>
		/// <remarks>
		/// This is used to wire up events (or other mechanisms) to detect if the value is changed for a particular
		/// object.
		/// 
		/// This is typically used to fire the <see cref="DirectBinding.DataValueChanged"/> event (which is wired up automatically)
		/// </remarks>
		/// <param name="dataItem">object to hook up the value changed event for</param>
		/// <param name="handler">handler for when the value of this binding changes for the specified object</param>
		/// <returns>object to track the changed handler (must be passed to <see cref="RemoveValueChangedHandler"/> to remove)</returns>
		public virtual object AddValueChangedHandler (object dataItem, EventHandler<EventArgs> handler)
		{
			return null;
		}
			
		/// <summary>
		/// Removes the handler for the specified reference from <see cref="AddValueChangedHandler"/>
		/// </summary>
		/// <param name="bindingReference">Reference from the call to <see cref="AddValueChangedHandler"/></param>
		/// <param name="handler">Same handler that was set up during the <see cref="AddValueChangedHandler"/> call</param>
		public virtual void RemoveValueChangedHandler (object bindingReference, EventHandler<EventArgs> handler)
		{
		}
	}
}
