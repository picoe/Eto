using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Eto.Forms
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
		/// <param name="toValue">Delegate to convert to the new value type.</param>
		/// <param name="fromValue">Delegate to convert from the value to the original binding's type.</param>
		/// <returns>A new binding with the specified <typeparamref name="TValue"/> type.</returns>
		public DirectBinding<TValue> Convert<TValue>(Func<T, TValue> toValue, Func<TValue, T> fromValue = null)
		{
			return new DelegateBinding<TValue>(
				() => toValue != null ? toValue(DataValue) : default(TValue),
				r => { if (fromValue != null) DataValue = fromValue(r); },
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
		/// Binds to the specified child <paramref name="property"/> expression.
		/// </summary>
		/// <remarks>
		/// This can be used to bind to properties of child objects of your view model, for example
		/// <code>model.SomeProperty.ChildProperty</code>.
		/// 
		/// This will automatically look up the changed event either by a [Property]Changed event or INotifyPropertyChanged implementation
		/// for each object in the heirarchy.
		/// 
		/// Note that you only really need to use this when you have an existing binding that you cannot change.
		/// See <see cref="Binding.Property{T,TValue}(Expression{Func{T,TValue}})"/> for an example of how to bind to child property values
		/// more directly.
		/// </remarks>
		/// <example>
		/// Use this like so:
		/// <code>
		/// 	public class MyChild { public SomeChildProperty { get; set; } }
		/// 	public class MyModel { public ChildObject { get; set; } }
		/// 
		/// 	var model = new MyModel();
		/// 	Binding.Property(model, (MyModel m) => m.ChildObject).Child(c => c.SomeChildProperty);
		/// </code>
		/// </example>
		/// <returns>The binding to the child property accessed through the current binding.</returns>
		/// <param name="property">Property to bind to.</param>
		/// <typeparam name="TValue">The type of the child property value.</typeparam>
		public DirectBinding<TValue> Child<TValue>(Expression<Func<T, TValue>> property)
		{
			return Child(Property(property));
		}

		/// <summary>
		/// Binds to the specified child <paramref name="binding"/> of this binding.
		/// </summary>
		/// <remarks>
		/// This can be used to bind to child objects of your view model, for example
		/// <code>model.SomeProperty.ChildProperty</code>.
		/// </remarks>
		/// <example>
		/// Use this like so:
		/// <code>
		/// 	public class MyChild { public SomeChildProperty { get; set; } }
		/// 	public class MyModel { public ChildObject { get; set; } }
		/// 
		/// 	var model = new MyModel();
		/// 	Binding.Property(model, (MyModel m) => m.ChildObject).Child(Binding.Property("SomeChildProperty"));
		/// </code>
		/// </example>
		/// <returns>The binding to the child property accessed through the current binding.</returns>
		/// <param name="binding">Binding to get the child value from this binding.</param>
		/// <typeparam name="TValue">The type of the child property value.</typeparam>
		public DirectBinding<TValue> Child<TValue>(IndirectBinding<TValue> binding)
		{
			object childBindingReference = null;
			EventHandler<EventArgs> eventHandler = null;
			void valueChanged(object sender, EventArgs e)
			{
				binding.RemoveValueChangedHandler(childBindingReference, eventHandler);
				eventHandler?.Invoke(sender, e);
				childBindingReference = binding.AddValueChangedHandler(DataValue, eventHandler);
			}
			void setValueStruct(TValue v)
			{
				object parentValue = DataValue;
				binding.SetValue(parentValue, v);
				DataValue = (T)parentValue;
			}
			void setValueObject(TValue v) => binding.SetValue(DataValue, v);
			var isStruct = typeof(T).GetTypeInfo().IsValueType;

			return new DelegateBinding<TValue>(
				() => binding.GetValue(DataValue),
				isStruct ? (Action<TValue>)setValueStruct : setValueObject,
				addChangeEvent: ev =>
				{
					eventHandler = ev;
					DataValueChanged += valueChanged;
					childBindingReference = binding.AddValueChangedHandler(DataValue, ev);
				},
				removeChangeEvent: ev =>
				{
					binding.RemoveValueChangedHandler(childBindingReference, ev);
					DataValueChanged -= valueChanged;
				}
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
				() =>
				{
					var val = DataValue;
					if (Equals(val, trueValue))
						return true;
					if (Equals(val, falseValue))
						return false;
					return null;
				},
				val =>
				{
					var typedVal = val == true ? trueValue : val == false ? falseValue : nullValue;
					DataValue = typedVal;
				},
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
			return new DelegateBinding<bool?>(
				() =>
				{
					var val = DataValue;
					if (Equals(val, trueValue))
						return true;
					if (Equals(val, falseValue))
						return false;
					return null;
				},
				val =>
				{
					if (val == true)
						DataValue = trueValue;
					else if (val == false)
						DataValue = falseValue;
				},
				addChangeEvent: eh => DataValueChanged += eh,
				removeChangeEvent: eh => DataValueChanged -= eh
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
		/// <param name="trueValue">Value when the binding is true, false, or null.</param>
		public DirectBinding<bool?> ToBool(T trueValue)
		{
			return new DelegateBinding<bool?>(
				() =>
				{
					var val = DataValue;
					if (Equals(val, trueValue))
						return true;
					return false;
				},
				val =>
				{
					if (val == true)
						DataValue = trueValue;
				},
				addChangeEvent: eh => DataValueChanged += eh,
				removeChangeEvent: eh => DataValueChanged -= eh
			);
		}

		/// <summary>
		/// Uses System.Convert.ChangeType to change the value of the binding to the specified type.
		/// </summary>
		/// <remarks>
		/// This has additional logic to deal with nullable types so they can be converted properly as well.
		/// </remarks>
		/// <param name="invalidGetValue">Delegate to get a value when it cannot be converted to the specified <typeparamref name="TType"/>. When null, an exception will be thrown when the value cannot be converted.</param>
		/// <param name="invalidSetValue">Delegate to set a value when it cannot be converted from the specified <typeparamref name="TType"/>. When null, an exception will be thrown when the value cannot be converted.</param>
		/// <typeparam name="TType">Type to convert the value to</typeparam>
		/// <returns>A binding of the new type that is a converted version of this binding</returns>
		public DirectBinding<TType> ToType<TType>(Func<T, TType> invalidGetValue = null, Func<TType, T> invalidSetValue = null)
		{
			return new DelegateBinding<TType>(
				() =>
				{
					var val = DataValue;
					try
					{
						var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
						return (TType)System.Convert.ChangeType(val, type);
					}
					catch
					{
						if (invalidGetValue == null)
							throw;
						return invalidGetValue(val);
					}
				},
				val =>
				{
					try
					{
						var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
						DataValue = (T)System.Convert.ChangeType(val, type);
					}
					catch
					{
						if (invalidSetValue == null)
							throw;
						DataValue = invalidSetValue(val);
					}
				},
				addChangeEvent: eh => DataValueChanged += eh,
				removeChangeEvent: eh => DataValueChanged -= eh
			);
		}

		/// <summary>
		/// Catches any exceptions when setting the value of the binding
		/// </summary>
		/// <param name="exceptionHandler">Handler to call when setting the value, regardless of whether an exception occurs. Return true when the exception is handled, false to throw an exception.</param>
		/// <returns>The binding that catches any exception.</returns>
		public DirectBinding<T> CatchException(Func<Exception, bool> exceptionHandler = null) => CatchException<Exception>(exceptionHandler);

		/// <summary>
		/// Catches any exceptions of the specified <typeparamref name="TException"/> when setting the value of the binding.
		/// </summary>
		/// <typeparam name="TException">Type of the exception to catch</typeparam>
		/// <param name="exceptionHandler">Handler to call when setting the value, regardless of whether an exception occurs. Return true when the exception is handled, false to throw an exception.</param>
		/// <returns>The binding that catches the specified exception.</returns>
		public DirectBinding<T> CatchException<TException>(Func<TException, bool> exceptionHandler = null)
			where TException: Exception
		{
			return new DelegateBinding<T>(
				() => DataValue,
				val =>
				{
					try
					{
						DataValue = val;
						exceptionHandler?.Invoke(null);
					}
					catch (TException ex)
					{
						if (exceptionHandler?.Invoke(ex) == false)
							throw;
					}
				},
				addChangeEvent: eh => DataValueChanged += eh,
				removeChangeEvent: eh => DataValueChanged -= eh
			);
	}

	}
}