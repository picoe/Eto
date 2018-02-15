using System;
using Eto.Forms;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Binding object to easily bind a property of a <see cref="IBindable"/> object, such as a <see cref="Eto.Forms.Control"/>.
	/// </summary>
	/// <remarks>
	/// This provides control-specific binding, such as binding to a <see cref="IBindable.DataContext"/>.
	/// Any bindings created using this will also add to the <see cref="IBindable.Bindings"/> collection to keep its
	/// reference.
	/// </remarks>
	public class BindableBinding<T,TValue> : ObjectBinding<T, TValue>
		where T: IBindable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControlBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Data item to get/set the values from/to.</param>
		/// <param name="getValue">Delegate to get the value from the object.</param>
		/// <param name="setValue">Delegate to set the value to the object.</param>
		/// <param name="addChangeEvent">Delegate to add the change event.</param>
		/// <param name="removeChangeEvent">Delegate to remove the chang event.</param>
		public BindableBinding(T dataItem, Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null)
			: base(dataItem, getValue, setValue, addChangeEvent, removeChangeEvent)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Control the binding is attached to.</param>
		/// <param name="innerBinding">Inner binding.</param>
		public BindableBinding(T dataItem, IndirectBinding<TValue> innerBinding)
			: base(dataItem, innerBinding)
		{
		}

		/// <summary>
		/// Binds the specified <paramref name="sourceBinding"/> to this binding.
		/// </summary>
		/// <remarks>
		/// This creates a <see cref="DualBinding{TValue}"/> between the specified <paramref name="sourceBinding"/> and this binding.
		/// The binding is added to the <see cref="IBindable.Bindings"/> collection.
		/// </remarks>
		/// <param name="sourceBinding">Source binding to bind from.</param>
		/// <param name="mode">Dual binding mode.</param>
		public override DualBinding<TValue> Bind(DirectBinding<TValue> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = base.Bind(sourceBinding, mode);
			if (DataItem != null)
				DataItem.Bindings.Add(binding);
			return binding;
		}

		/// <summary>
		/// Binds to an object's <see cref="IBindable.DataContext"/> using the specified <paramref name="dataContextBinding"/>.
		/// </summary>
		/// <remarks>
		/// This creates a <see cref="DualBinding{TValue}"/> between a binding to the specified <paramref name="dataContextBinding"/> and this binding.
		/// Since the data context changes, the binding passed for the data context binding is an indirect binding, in that it is reused.
		/// The binding is added to the <see cref="IBindable.Bindings"/> collection.
		/// </remarks>
		/// <returns>A new dual binding that binds the <paramref name="dataContextBinding"/> to this control binding.</returns>
		/// <param name="dataContextBinding">Binding to get/set values from/to the control's data context.</param>
		/// <param name="mode">Dual binding mode.</param>
		/// <param name="defaultControlValue">Default control value.</param>
		/// <param name="defaultContextValue">Default context value.</param>
		public DualBinding<TValue> BindDataContext(IndirectBinding<TValue> dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
		{
			var control = DataItem;
			if (control == null)
				throw new InvalidOperationException("Binding must be attached to a control");
			var contextBinding = new BindableBinding<IBindable, object>(control, Binding.Delegate((IBindable w) => w.DataContext, null, (w, h) => w.DataContextChanged += h, (w, h) => w.DataContextChanged -= h));
			var valueBinding = new ObjectBinding<object, TValue>(control.DataContext, dataContextBinding)
			{
				GettingNullValue = defaultControlValue,
				SettingNullValue = defaultContextValue,
				DataItem = contextBinding.DataValue
			};
			DualBinding<TValue> binding = Bind(sourceBinding: valueBinding, mode: mode);
			contextBinding.DataValueChanged += delegate
			{
				((ObjectBinding<object, TValue>)binding.Source).DataItem = contextBinding.DataValue;
			};
			control.Bindings.Add(contextBinding);
			return binding;
		}

		/// <summary>
		/// Binds to a control's <see cref="IBindable.DataContext"/> using delegates to get/set the value.
		/// </summary>
		/// <remarks>
		/// This is a shortcut to use the <see cref="DelegateBinding{T,TValue}"/> to bind to a control's <see cref="IBindable.DataContext"/> property.
		/// When the data context type is <typeparamref name="TValue"/>, then the delegates will be called to get/set the value.
		/// Otherwise, if the data context is null or a different type, the <paramref name="defaultGetValue"/> will be used.
		/// </remarks>
		/// <returns>A new dual binding that binds the control to this object binding.</returns>
		/// <param name="getValue">Delegate to get the value from the data context.</param>
		/// <param name="setValue">Delegate to set the value to the data context when changed.</param>
		/// <param name="addChangeEvent">Delegate to add a change event on the data context.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event from the data context.</param>
		/// <param name="mode">Dual binding mode.</param>
		/// <param name="defaultGetValue">Default get value.</param>
		/// <param name="defaultSetValue">Default set value.</param>
		/// <typeparam name="TObject">Type of the data context object to bind with.</typeparam>
		public DualBinding<TValue> BindDataContext<TObject>(Func<TObject, TValue> getValue, Action<TObject, TValue> setValue, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			return BindDataContext(new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent, defaultGetValue, defaultSetValue), mode);
		}

		/// <summary>
		/// Binds to the specified <paramref name="propertyName"/> of the current data context.
		/// </summary>
		/// <remarks>
		/// This is a shortcut to using the <see cref="PropertyBinding{TValue}"/>.
		/// This has the advantage of registering automatically to <see cref="System.ComponentModel.INotifyPropertyChanged"/> 
		/// or to an event named after the property with a "Changed" suffix.
		/// </remarks>
		/// <returns>The binding between the data context and this binding.</returns>
		/// <param name="propertyName">Name of the property on the data context to bind to.</param>
		/// <param name="mode">Direction of the binding.</param>
		public DualBinding<TValue> BindDataContext(string propertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return BindDataContext(Property<TValue>(propertyName), mode);
		}

		/// <summary>
		/// Binds to a specified property of the control's current data context.
		/// </summary>
		/// <remarks>
		/// This has the advantage of registering automatically to <see cref="System.ComponentModel.INotifyPropertyChanged"/> 
		/// or to an event named after the property with a "Changed" suffix, if the expression is a property.
		/// When the expression does not evaluate to a property, it will not be able to bind to the changed events and will
		/// use the expression as a delegate directly.
		/// </remarks>
		/// <typeparam name="TObject">Type of the data context to bind to</typeparam>
		/// <param name="propertyExpression">Expression for a property of the data context, or a non-property expression with no change event binding.</param>
		/// <param name="mode">Direction of the binding</param>
		/// <returns>The binding between the data context and this binding</returns>
		public DualBinding<TValue> BindDataContext<TObject>(Expression<Func<TObject, TValue>> propertyExpression, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return BindDataContext(Property(propertyExpression), mode);
		}

		/// <summary>
		/// Converts this binding's value to another value using delegates.
		/// </summary>
		/// <remarks>This is useful when you want to cast one binding to another, perform logic when getting/setting a value from a particular
		/// binding, or get/set a preoperty of the value.</remarks>
		/// <typeparam name="TNewValue">Type of the value for the new binding</typeparam>
		/// <param name="toValue">Delegate to convert to the new value type.</param>
		/// <param name="fromValue">Delegate to convert from the value to the original binding's type.</param>
		public new BindableBinding<T, TNewValue> Convert<TNewValue>(Func<TValue, TNewValue> toValue, Func<TNewValue, TValue> fromValue = null)
		{
			return new BindableBinding<T, TNewValue>(
				DataItem,
				Delegate<T, TNewValue>(
					c => toValue != null ? toValue(DataValue) : default(TNewValue),
					(c,v) => { if (fromValue != null) DataValue = fromValue(v); },
					addChangeEvent: (c, ev) => DataValueChanged += ev,
					removeChangeEvent: (c, ev) => DataValueChanged -= ev
				)
			);
		}

		/// <summary>
		/// Casts this binding value to another (compatible) type.
		/// </summary>
		/// <typeparam name="TNewValue">The type to cast the values of this binding to.</typeparam>
		public new BindableBinding<T, TNewValue> Cast<TNewValue>()
		{
			return new BindableBinding<T, TNewValue>(
				DataItem,
				Delegate<T, TNewValue>(
					c => (TNewValue)(object)DataValue,
					(c, v) => DataValue = (TValue)(object)v,
					addChangeEvent: (c, ev) => DataValueChanged += ev,
					removeChangeEvent: (c, ev) => DataValueChanged -= ev
				)
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
		/// <typeparam name="TNewValue">The type of the child property value.</typeparam>
		public BindableBinding<T, TNewValue> Child<TNewValue>(Expression<Func<T, TNewValue>> property)
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
		/// <typeparam name="TNewValue">The type of the child property value.</typeparam>
		public new BindableBinding<T, TNewValue> Child<TNewValue>(IndirectBinding<TNewValue> binding)
		{
			object childBindingReference = null;
			EventHandler<EventArgs> eventHandler = null;
			EventHandler<EventArgs> valueChanged = (sender, e) =>
			{
				binding.RemoveValueChangedHandler(childBindingReference, eventHandler);
				eventHandler?.Invoke(sender, e);
				childBindingReference = binding.AddValueChangedHandler(DataValue, eventHandler);
			};
			return new BindableBinding<T, TNewValue>(
				DataItem,
				c => binding.GetValue(DataValue),
				(c, v) => binding.SetValue(DataValue, v),
				addChangeEvent: (c, ev) =>
				{
					eventHandler = ev;
					DataValueChanged += valueChanged;
					childBindingReference = binding.AddValueChangedHandler(DataValue, ev);
				},
				removeChangeEvent: (c, ev) =>
				{
					binding.RemoveValueChangedHandler(childBindingReference, ev);
					DataValueChanged -= valueChanged;
				}
			);
		}


		/// <summary>
		/// Catches any exceptions when setting the value of the binding
		/// </summary>
		/// <param name="exceptionHandler">Handler to call when setting the value, regardless of whether an exception occurs. Return true when the exception is handled, false to throw an exception.</param>
		/// <returns>The binding that catches any exception.</returns>
		public new BindableBinding<T, TValue> CatchException(Func<Exception, bool> exceptionHandler = null) => CatchException<Exception>(exceptionHandler);

		/// <summary>
		/// Catches any exceptions of the specified <typeparamref name="TException"/> when setting the value of the binding.
		/// </summary>
		/// <typeparam name="TException">Type of the exception to catch</typeparam>
		/// <param name="exceptionHandler">Handler to call when setting the value, regardless of whether an exception occurs. Return true when the exception is handled, false to throw an exception.</param>
		/// <returns>The binding that catches the specified exception.</returns>
		public new BindableBinding<T, TValue> CatchException<TException>(Func<TException, bool> exceptionHandler = null)
			where TException : Exception
		{
			return new BindableBinding<T, TValue>(
				DataItem,
				Delegate<T, TValue>(
					c => DataValue,
					(c, v) =>
					{
						try
						{
							DataValue = v;
							exceptionHandler?.Invoke(null);
						}
						catch (TException ex)
						{
							if (exceptionHandler?.Invoke(ex) == false)
								throw;
						}
					},
					addChangeEvent: (c, ev) => DataValueChanged += ev,
					removeChangeEvent: (c, ev) => DataValueChanged -= ev
				)
			);
		}
	}
}
