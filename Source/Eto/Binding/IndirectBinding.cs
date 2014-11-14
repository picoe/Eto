using System;

namespace Eto
{
	/// <summary>
	/// Base interface for an indirect binding.
	/// </summary>
	/// <remarks>
	/// An indirect binding is passed the data item to get/set values on the object.
	/// This is used for things like the <see cref="Forms.Grid"/>, <see cref="Forms.ListBox"/>, etc when
	/// binding to values for each item in the data store.
	/// </remarks>
	public interface IIndirectBinding<T> : IBinding
	{
		/// <summary>
		/// Gets the value from the specified object using this binding
		/// </summary>
		/// <remarks>
		/// When values are needed from this binding, this method will be called.
		/// </remarks>
		/// <param name="dataItem">object to retrieve the value from</param>
		/// <returns>value from the specified object</returns>
		T GetValue(object dataItem);

		/// <summary>
		/// Sets the specified value to an object using this binding
		/// </summary>
		/// <remarks>
		/// This is called to set the value to the object.
		/// </remarks>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set to the object</param>
		void SetValue(object dataItem, T value);
	}

	/// <summary>
	/// Provides an indirect binding to an indeterminate source/destination
	/// </summary>
	/// <remarks>
	/// This binding does not directly bind to an object - you must pass the
	/// object to get/set the value.  The <see cref="DirectBinding{T}"/> differs in 
	/// that it binds directly to an object.
	/// 
	/// The IndirectBinding is useful when you want to use the same binding on multiple
	/// objects, such as when binding cells in a <see cref="Forms.Grid"/>.
	/// 
	/// Typically one would use <see cref="PropertyBinding"/> or <see cref="ColumnBinding{T}"/>
	/// which are ways to retrieve either a property value or column/index-based value.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class IndirectBinding<T> : Binding, IIndirectBinding<T>
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
		public T GetValue(object dataItem)
		{
			return InternalGetValue(dataItem);
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
		public void SetValue(object dataItem, T value)
		{ 
			var args = new BindingChangingEventArgs(value);
			OnChanging(args);
			InternalSetValue(dataItem, (T)args.Value);
			OnChanged(new BindingChangedEventArgs(args.Value));
		}

		/// <summary>
		/// Implements the logic to get the value from the specified object
		/// </summary>
		/// <remarks>
		/// Implementors of this binding must implement this method to get the value from the specified object
		/// </remarks>
		/// <param name="dataItem">object to get the value from</param>
		/// <returns>value from this binding of the specified object</returns>
		protected abstract T InternalGetValue(object dataItem);

		/// <summary>
		/// Implements the logic to set the value to the specified object
		/// </summary>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set on the dataItem for this binding</param>
		protected abstract void InternalSetValue(object dataItem, T value);


		/// <summary>
		/// Adds a handler to trap when the value of this binding changes for the specified object
		/// </summary>
		/// <remarks>
		/// This is used to wire up events (or other mechanisms) to detect if the value is changed for a particular
		/// object.
		/// 
		/// This is typically used to fire the <see cref="DirectBinding{T}.DataValueChanged"/> event (which is wired up automatically)
		/// </remarks>
		/// <param name="dataItem">object to hook up the value changed event for</param>
		/// <param name="handler">handler for when the value of this binding changes for the specified object</param>
		/// <returns>object to track the changed handler (must be passed to <see cref="RemoveValueChangedHandler"/> to remove)</returns>
		public virtual object AddValueChangedHandler(object dataItem, EventHandler<EventArgs> handler)
		{
			return null;
		}

		/// <summary>
		/// Removes the handler for the specified reference from <see cref="AddValueChangedHandler"/>
		/// </summary>
		/// <param name="bindingReference">Reference from the call to <see cref="AddValueChangedHandler"/></param>
		/// <param name="handler">Same handler that was set up during the <see cref="AddValueChangedHandler"/> call</param>
		public virtual void RemoveValueChangedHandler(object bindingReference, EventHandler<EventArgs> handler)
		{
		}

		/// <summary>
		/// Converts this binding's value to another value using delegates.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to cast one binding to another, perform logic when getting/setting a value from a particular
		/// binding, or get/set a preoperty of the value.
		/// </remarks>
		/// <param name="toValue">Delegate to convert to the new value type.</param>
		/// <param name="fromValue">Delegate to convert from the value to the original binding's type.</param>
		/// <typeparam name="TValue">The type to convert to.</typeparam>
		public IndirectBinding<TValue> Convert<TValue>(Func<T, TValue> toValue, Func<TValue, T> fromValue = null)
		{
			return new DelegateBinding<object, TValue>(
				m => toValue != null ? toValue(GetValue(m)) : default(TValue),
				(m, val) => { if (fromValue != null) SetValue(m, fromValue(val)); },
				addChangeEvent: (m, ev) => AddValueChangedHandler(m, ev),
				removeChangeEvent: RemoveValueChangedHandler
			);
		}

		/// <summary>
		/// Casts this binding value to another (compatible) type.
		/// </summary>
		/// <typeparam name="TValue">The type to cast the values of this binding to.</typeparam>
		public IndirectBinding<TValue> Cast<TValue>()
		{
			return new DelegateBinding<object, TValue>(
				m => (TValue)(object)GetValue(m),
				(m, val) => SetValue(m, (T)(object)val),
				addChangeEvent: (m, ev) => AddValueChangedHandler(m, ev),
				removeChangeEvent: RemoveValueChangedHandler
			);
		}

		/// <summary>
		/// Converts this binding to return a nullable boolean binding
		/// </summary>
		/// <remarks>
		/// This is useful when converting a binding to be used for a checkbox's Checked binding for example.
		/// When the binding's value matches the <paramref name="trueValue"/>, it will return true.
		/// </remarks>
		/// <returns>Boolean binding.</returns>
		/// <param name="trueValue">Value when the binding is true.</param>
		/// <param name="falseValue">Value when the binding is false.</param>
		/// <param name="nullValue">Value when the binding is null.</param>
		public IndirectBinding<bool?> ToBool(T trueValue, T falseValue, T nullValue)
		{
			return new DelegateBinding<object, bool?>(
				m => Equals(GetValue(m), trueValue),
				(m, val) => SetValue(m, val == true ? trueValue : val != null ? falseValue : nullValue),
				addChangeEvent: (m, ev) => AddValueChangedHandler(m, ev),
				removeChangeEvent: RemoveValueChangedHandler
			);
		}

		/// <summary>
		/// Converts this binding to return a nullable boolean binding
		/// </summary>
		/// <remarks>
		/// This is useful when converting a binding to be used for a checkbox's Checked binding for example.
		/// When the binding's value matches the <paramref name="trueValue"/>, it will return true.
		/// </remarks>
		/// <returns>Boolean binding.</returns>
		/// <param name="trueValue">Value when the binding is true.</param>
		/// <param name="falseValue">Value when the binding is false.</param>
		public IndirectBinding<bool?> ToBool(T trueValue, T falseValue)
		{
			return ToBool(trueValue, falseValue, falseValue);
		}

		/// <summary>
		/// Converts this binding to return a nullable boolean binding
		/// </summary>
		/// <remarks>
		/// This is useful when converting a binding to be used for a checkbox's Checked binding for example.
		/// When the binding's value matches the <paramref name="trueValue"/>, it will return true.
		/// </remarks>
		/// <returns>Boolean binding.</returns>
		/// <param name="trueValue">Value when the binding is true.</param>
		public IndirectBinding<bool?> ToBool(T trueValue)
		{
			return ToBool(trueValue, trueValue, trueValue);
		}
	}
}
