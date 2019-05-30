using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Mode for updating the binding
	/// </summary>
	public enum BindingUpdateMode
	{
		/// <summary>
		/// Update the binding source (usually the model)
		/// </summary>
		Source,

		/// <summary>
		/// Update the binding destination (usually the control)
		/// </summary>
		Destination
	}

	/// <summary>
	/// Base binding interface
	/// </summary>
	/// <remarks>
	/// Binding provides a way to bind your data objects to control properties and grid values.
	/// This base class adds the ability to unbind the binding, or update it manually.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IBinding
	{
		/// <summary>
		/// Unbind this instance from its parent.
		/// </summary>
		/// <remarks>
		/// This typically will unregister any event handlers to properties so that the controls can be garbage collected.
		/// </remarks>
		void Unbind();

		/// <summary>
		/// Updates the binding from the source to the destination
		/// </summary>
		/// <remarks>
		/// Typically the source would be your custom class and the destination would be a UI control, but this is not
		/// always the case.
		/// </remarks>
		/// <param name="mode">Direction of the update</param>
		void Update(BindingUpdateMode mode = BindingUpdateMode.Source);
	}

	/// <summary>
	/// Base class for binding between a value and another
	/// </summary>
	/// <remarks>
	/// This is the base of any type of binding.  Some bindings may only be used to get/set a single
	/// value (e.g. <see cref="IndirectBinding{T}"/>), whereas the <see cref="DualBinding{T}"/> can link
	/// two objects' values together
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract partial class Binding : IBinding
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
		protected virtual void OnChanging(BindingChangingEventArgs e)
		{
			if (Changing != null)
				Changing(this, e);
		}

		/// <summary>
		/// Event to handle after the value has been set using this binding
		/// </summary>
		public event EventHandler<BindingChangedEventArgs> Changed;

		/// <summary>
		/// Handles the <see cref="Changed"/> event
		/// </summary>
		protected virtual void OnChanged(BindingChangedEventArgs e)
		{
			if (Changed != null)
				Changed(this, e);
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
		public virtual void Unbind()
		{
		}

		/// <summary>
		/// Updates the bound target object's value
		/// </summary>
		/// <remarks>
		/// Typically the source would be your custom class and the destination would be a UI control, but this is not
		/// always the case.
		/// </remarks>
		/// <param name="mode">Direction of the update</param>
		public virtual void Update(BindingUpdateMode mode = BindingUpdateMode.Destination)
		{
		}

		/// <summary>
		/// Called to handle an event for this binding
		/// </summary>
		/// <param name="id"></param>
		protected virtual void HandleEvent(string id)
		{
#if DEBUG
			throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "This binding does not support the {0} event", id));
#endif
		}

		/// <summary>
		/// Called to remove an event for this binding
		/// </summary>
		/// <param name="id"></param>
		protected virtual void RemoveEvent(string id)
		{
		}
	}
}

