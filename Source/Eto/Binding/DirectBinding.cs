using System;

namespace Eto
{
	/// <summary>
	/// Abstraction to get/set values from a provided object
	/// </summary>
	/// <remarks>
	/// This binding provides a way to get/set values of an object that is provided by the binding,
	/// and not passed in.
	/// 
	/// This differs from the <see cref="IndirectBinding{T}"/>, which requires that the caller pass in the
	/// object to get/set the value from/to.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class DirectBinding<T> : Binding
	{
		#region Events

		/// <summary>
		/// Identifier for the <see cref="DirectBinding{T}.DataValueChanged"/> event
		/// </summary>
		public const string DataValueChangedEvent = "ObjectBinding.DataValueChangedEvent";

		event EventHandler<EventArgs> _DataValueChanged;

		/// <summary>
		/// Event to handle when the value changes on the bound object
		/// </summary>
		public event EventHandler<EventArgs> DataValueChanged {
			add {
				var shouldHandle = _DataValueChanged == null;
				_DataValueChanged += value;
				if (shouldHandle)
					HandleEvent (DataValueChangedEvent);
			}
			remove {
				_DataValueChanged -= value;
				if (_DataValueChanged == null)
					RemoveEvent (DataValueChangedEvent);
			}
		}

		/// <summary>
		/// Handles the <see cref="DataValueChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors of this class should call this method when the value changes
		/// on the bound object. Make sure to also override the <see cref="M:Eto.Binding.HandleEvent"/> 
		/// and <see cref="M:Eto.Binding.RemoveEvent"/> methods to hook up/remove any event bindings 
		/// you need on the bound object.
		/// </remarks>
		protected virtual void OnDataValueChanged (EventArgs e)
		{
			if (_DataValueChanged != null)
				_DataValueChanged (this, e);
		}

		#endregion
	
		/// <summary>
		/// Gets or sets the value of this binding
		/// </summary>
		public abstract T DataValue { get; set; }

		/// <summary>
		/// Converts this binding's value to another value using delegates.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to cast one binding to another, perform logic when getting/setting a value from a particular
		/// binding, or get/set a preoperty of the value.
		/// </remarks>
		/// <typeparam name="TValue">Type of the value for the new binding</typeparam>
		/// <param name="getValue">Delegate to translate the value when getting it for the new binding</param>
		/// <param name="setValue">Delegate to translate the value when setting it to this binding</param>
		/// <returns>A new binding with the specified <typeparamref name="TValue"/> type.</returns>
		public DirectBinding<TValue> Convert<TValue>(Func<T, TValue> getValue, Action<TValue> setValue = null)
		{
			return new DelegateBinding<TValue>(
				() => getValue != null ? getValue(DataValue) : default(TValue),
				r => { if (setValue != null) setValue(r); },
				addChangeEvent: ev => DataValueChanged += ev,
				removeChangeEvent: ev => DataValueChanged -= ev
			);
		}

		/// <summary>
		/// Casts this binding value to another (compatible) type.
		/// </summary>
		/// <typeparam name="TValue">The type to cast the values of this binding to.</typeparam>
		public DirectBinding<TValue> Cast<TValue>()
		{
			return new DelegateBinding<TValue>(
				() => (TValue)(object)DataValue,
				val => DataValue = (T)(object)val,
				addChangeEvent: ev => DataValueChanged += ev,
				removeChangeEvent: ev => DataValueChanged -= ev
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
		public DirectBinding<bool?> ToBool(T trueValue, T falseValue, T nullValue)
		{
			return new DelegateBinding<bool?>(
				() => Equals(DataValue, trueValue),
				val => { DataValue = val == true ? trueValue : val == false ? falseValue : nullValue; },
				addChangeEvent: ev => DataValueChanged += ev,
				removeChangeEvent: ev => DataValueChanged -= ev
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
		/// <param name="falseValue">Value when the binding is false or null.</param>
		public DirectBinding<bool?> ToBool(T trueValue, T falseValue)
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
		/// <param name="trueValue">Value when the binding is true, false, or null.</param>
		public DirectBinding<bool?> ToBool(T trueValue)
		{
			return ToBool(trueValue, trueValue, trueValue);
		}
	}
}