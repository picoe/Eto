using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Eto.Forms
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
	/// Typically one would use <see cref="PropertyBinding{T}"/> or <see cref="ColumnBinding{T}"/>
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
			if (!args.Cancel)
			{
				InternalSetValue(dataItem, (T)args.Value);
				OnChanged(new BindingChangedEventArgs(args.Value));
			}
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
				addChangeEvent: AddValueChangedHandler,
				removeChangeEvent: RemoveValueChangedHandler
			);
		}

		/// <summary>
		/// Converts the binding using the specified <paramref name="converter"/> object.
		/// </summary>
		/// <returns>A new binding that will be converted using the specified IValueConverter.</returns>
		/// <param name="converter">Converter object to use when converting to/from the value</param>
		/// <param name="conveterParameter">Parameter to pass to the converter.</param>
		/// <param name="culture">Culture to use for conversion, null to use invariant culture.</param>
		public IndirectBinding<TValue> Convert<TValue>(IValueConverter converter, object conveterParameter = null, CultureInfo culture = null)
		{
			culture = culture ?? CultureInfo.InvariantCulture;
			return Convert(
				val => (TValue)converter.Convert(val, typeof(TValue), conveterParameter, culture),
				val => (T)converter.ConvertBack(val, typeof(T), conveterParameter, culture)
			);
		}

		/// <summary>
		/// Converts the binding using the specified <paramref name="converter"/> object.
		/// </summary>
		/// <returns>A new binding that will be converted using the specified IValueConverter.</returns>
		/// <param name="converter">Converter object to use when converting to/from the value</param>
		/// <param name="propertyType">Type for the converter to convert to</param>
		/// <param name="conveterParameter">Parameter to pass to the converter.</param>
		/// <param name="culture">Culture to use for conversion, null to use invariant culture.</param>
		public IndirectBinding<object> Convert(IValueConverter converter, Type propertyType, object conveterParameter = null, CultureInfo culture = null)
		{
			culture = culture ?? CultureInfo.InvariantCulture;
			return Convert(
				val => converter.Convert(val, propertyType, conveterParameter, culture),
				val => (T)converter.ConvertBack(val, typeof(T), conveterParameter, culture)
			);
		}

		/// <summary>
		/// Casts this binding value to another (compatible) type.
		/// </summary>
		/// <typeparam name="TValue">The type to cast the values of this binding to.</typeparam>
		public IndirectBinding<TValue> Cast<TValue>()
		{
			return new DelegateBinding<object, TValue>(
				m =>
				{
					var value = (object)GetValue(m);
					if (value == null)
						return default(TValue);
					return (TValue)value;
				},
				(m, val) => SetValue(m, (T)(object)val),
				addChangeEvent: AddValueChangedHandler,
				removeChangeEvent: RemoveValueChangedHandler
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
		/// 	Binding.Property((MyModel m) => m.ChildObject).Child(c => c.SomeChildProperty);
		///		// or:
		/// 	Binding.Property((MyModel m) => m.ChildObject.SomeChildProperty);
		/// </code>
		/// </example>
		/// <returns>The binding to the child property accessed through the current binding.</returns>
		/// <param name="property">Property to bind to.</param>
		/// <typeparam name="TNewValue">The type of the child property value.</typeparam>
		public IndirectBinding<TNewValue> Child<TNewValue>(Expression<Func<T, TNewValue>> property)
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
		/// 	Binding.Property((MyModel m) => m.ChildObject).Child(Binding.Property("SomeChildProperty"));
		/// </code>
		/// </example>
		/// <returns>The binding to the child property accessed through the current binding.</returns>
		/// <param name="binding">Binding to get the child value from this binding.</param>
		/// <typeparam name="TNewValue">The type of the child property value.</typeparam>
		public IndirectBinding<TNewValue> Child<TNewValue>(IndirectBinding<TNewValue> binding)
		{
			return new IndirectChildBinding<T, TNewValue>(this, binding);
		}

		/// <summary>
		/// Casts this binding value to another (compatible) type, or returns the default if the types do not match.
		/// </summary>
		/// <typeparam name="TValue">The type to cast the values of this binding to.</typeparam>
		public IndirectBinding<TValue> OfType<TValue>(TValue defaultConvertedValue = default(TValue), T defaultValue = default(T))
		{
			return new DelegateBinding<object, TValue>(
				m =>
				{
					var val = GetValue(m);
					return val is TValue ? (TValue)(object)val : defaultConvertedValue;
				},
				(m, val) => SetValue(m, val is T ? (T)(object)val : defaultValue),
				addChangeEvent: AddValueChangedHandler,
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
				m =>
				{
					var val = GetValue(m);
					if (Equals(val, trueValue))
						return true;
					if (Equals(val, falseValue))
						return false;
					return null;
				},
				(m, val) =>
				{
					var typedVal = val == true ? trueValue : val == false ? falseValue : nullValue;
					SetValue(m, typedVal);
				},
				addChangeEvent: AddValueChangedHandler,
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
			return new DelegateBinding<object, bool?>(
				m =>
				{
					var val = GetValue(m);
					if (Equals(val, trueValue))
						return true;
					if (Equals(val, falseValue))
						return false;
					return null;
				},
				(m, val) =>
				{
					if (val == true)
						SetValue(m, trueValue);
					else if (val == false)
						SetValue(m, falseValue);
				},
				addChangeEvent: AddValueChangedHandler,
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
		public IndirectBinding<bool?> ToBool(T trueValue)
		{
			return new DelegateBinding<object, bool?>(
				m =>
				{
					var val = GetValue(m);
					if (Equals(val, trueValue))
						return true;
					return false;
				},
				(m, val) =>
				{
					if (val == true)
						SetValue(m, trueValue);
				},
				addChangeEvent: AddValueChangedHandler,
				removeChangeEvent: RemoveValueChangedHandler
			);
		}

		/// <summary>
		/// Converts the a binding to an enumeration to/from its string representation
		/// </summary>
		/// <returns>Binding to the string value of the enumeration.</returns>
		/// <param name="defaultValue">Default if the value is not valid or empty.</param>
		public IndirectBinding<string> EnumToString(T defaultValue = default(T))
		{
			var enumType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
			if (!enumType.IsEnum())
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Type of T ({0}) must be an enumeration type", typeof(T)));
			return new DelegateBinding<object, string>(
				m => System.Convert.ToString(GetValue(m)),
				(m, val) =>
				{
					var value = (!string.IsNullOrEmpty(val) && Enum.IsDefined(enumType, val))
						? (T)Enum.Parse(enumType, val)
						: defaultValue;
					SetValue(m, value);
				},
				addChangeEvent: AddValueChangedHandler,
				removeChangeEvent: RemoveValueChangedHandler
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
		public IndirectBinding<TType> ToType<TType>(Func<T, TType> invalidGetValue = null, Func<TType, T> invalidSetValue = null)
		{
			return new DelegateBinding<object, TType>(
				o =>
				{
					var val = GetValue(o);
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
				(o, val) =>
				{
					try
					{
						var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
						SetValue(o, (T)System.Convert.ChangeType(val, type));
					}
					catch
					{
						if (invalidSetValue == null)
							throw;
						SetValue(o, invalidSetValue(val));
					}
				},
				addChangeEvent: (o, eh) => AddValueChangedHandler(o, eh),
				removeChangeEvent: (o, eh) => RemoveValueChangedHandler(o, eh)
			);
		}

		/// <summary>
		/// Specifies that the binding should only respond to change events after a delay.
		/// </summary>
		/// <remarks>
		/// This is useful if the property/delegate that is bound is expensive to retrieve the new value,
		/// for example to dynamically generate a bitmap based on the state of the model, etc. 
		/// 
		/// The <paramref name="reset"/> boolean allows you to ensure that the binding is updated periodically when <c>false</c> (default), 
		/// or <c>true</c> to wait for the delay period after the last change event.
		/// </remarks>
		/// <param name="delay">The delay time span to wait after the value has changed before updating the binding.</param>
		/// <param name="reset"><c>true</c> to reset the delay every time the event is fired, <c>false</c> to trigger the change at least by the delay interval since the last time it was triggered</param>
		/// <returns>A binding that will delay the change event</returns>
		public IndirectBinding<T> AfterDelay(TimeSpan delay, bool reset = false)
		{
			return AfterDelay(delay.TotalSeconds, reset);
		}

		/// <summary>
		/// Specifies that the binding should only respond to change events after a delay.
		/// </summary>
		/// <remarks>
		/// This is useful if the property/delegate that is bound is expensive to retrieve the new value,
		/// for example to dynamically generate a bitmap based on the state of the model, etc. 
		/// 
		/// The <paramref name="reset"/> boolean allows you to ensure that the binding is updated periodically when <c>false</c> (default), 
		/// or <c>true</c> to wait for the delay period after the last change event.
		/// </remarks>
		/// <param name="delay">The delay, in seconds to wait after the value has changed before updating the binding.</param>
		/// <param name="reset"><c>true</c> to reset the delay every time the event is fired, <c>false</c> to trigger the change at least by the delay interval since the last time it was triggered</param>
		/// <returns>A binding that will delay the change event</returns>
		public IndirectBinding<T> AfterDelay(double delay, bool reset = false)
		{
			UITimer timer = null;
			EventArgs args = null;
			object sender = null;
			return new DelegateBinding<object, T>(
				m => GetValue(m),
				(m, val) => SetValue(m, val),
				addChangeEvent: (m, ev) =>
				{
					EventHandler<EventArgs> ev2 = (s, e) =>
					{
						args = e;
						sender = s;
						if (timer == null)
						{
							timer = new UITimer { Interval = delay };
							timer.Elapsed += (s2, e2) =>
							{
								timer.Stop();
								ev(sender, args);
							};
						}
						if (reset || !timer.Started)
							timer.Start();
					};
					AddValueChangedHandler(m, ev2);
				},
				removeChangeEvent: RemoveValueChangedHandler
			);
		}

		/// <summary>
		/// Catches any exceptions when setting the value of the binding
		/// </summary>
		/// <param name="exceptionHandler">Handler to call when setting the value, regardless of whether an exception occurs. Return true when the exception is handled, false to throw an exception.</param>
		/// <returns>The binding that catches any exception.</returns>
		public IndirectBinding<T> CatchException(Func<Exception, bool> exceptionHandler = null) => CatchException<Exception>(exceptionHandler);

		/// <summary>
		/// Catches any exceptions of the specified <typeparamref name="TException"/> when setting the value of the binding.
		/// </summary>
		/// <typeparam name="TException">Type of the exception to catch</typeparam>
		/// <param name="exceptionHandler">Handler to call when setting the value, regardless of whether an exception occurs. Return true when the exception is handled, false to throw an exception.</param>
		/// <returns>The binding that catches the specified exception.</returns>
		public IndirectBinding<T> CatchException<TException>(Func<TException, bool> exceptionHandler = null)
			where TException : Exception
		{
			return new DelegateBinding<object, T>(
				m => GetValue(m),
				(m, val) =>
				{
					try
					{
						SetValue(m, val);
						exceptionHandler?.Invoke(null);
					}
					catch (TException ex)
					{
						if (exceptionHandler?.Invoke(ex) == false)
							throw;
					}
				},
				addChangeEvent: AddValueChangedHandler,
				removeChangeEvent: RemoveValueChangedHandler
			);
		}
	}
}
