using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Base class for binding between a value and another
	/// </summary>
	/// <remarks>
	/// This is the base of any type of binding.  Some bindings may only be used to get/set a single
	/// value (e.g. <see cref="IndirectBinding"/>), whereas the <see cref="DualBinding"/> can link
	/// two objects' values together
	/// </remarks>
	public abstract class Binding
	{
		#region Events

		/// <summary>
		/// Event to handle when the value is being set using this binding
		/// </summary>
		/// <remarks>
		/// This can be used to stop a value being updated based on custom logic
		/// </remarks>
		public event EventHandler<BindingChangingEventArgs> Changing;

		/// <summary>
		/// Handles the <see cref="Changing"/> event
		/// </summary>
		protected virtual void OnChanging (BindingChangingEventArgs e)
		{
			if (Changing != null)
				Changing (this, e);
		}

		/// <summary>
		/// Event to handle after the value has been set using this binding
		/// </summary>
		public event EventHandler<BindingChangedEventArgs> Changed;

		/// <summary>
		/// Handles the <see cref="Changed"/> event
		/// </summary>
		protected virtual void OnChanged (BindingChangedEventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}

		#endregion

		/// <summary>
		/// Unbind this from the target object(s)
		/// </summary>
		/// <remarks>
		/// Typically a binding may handle an event for when the target object(s) property values
		/// are changed.  This is called to unbind the binding from the objects so that they can be
		/// garbage collected
		/// </remarks>
		public virtual void Unbind ()
		{
		}
		
		/// <summary>
		/// Updates the bound target object's value
		/// </summary>
		public virtual void Update ()
		{
		}

		/// <summary>
		/// Called to handle an event for this binding
		/// </summary>
		/// <param name="id"></param>
		protected virtual void HandleEvent (string id)
		{
#if DEBUG
			throw new EtoException(string.Format ("This binding does not support the {0} event", id));
#endif
		}
		
		/// <summary>
		/// Called to remove an event for this binding
		/// </summary>
		/// <param name="id"></param>
		protected virtual void RemoveEvent (string id)
		{
		}
	}
}

