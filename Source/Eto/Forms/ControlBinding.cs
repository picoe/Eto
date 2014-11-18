using System;
using Eto.Forms;
using System.Linq.Expressions;

namespace Eto.Forms
{

	/// <summary>
	/// Binding object to easily bind a property of a <see cref="Control"/>.
	/// </summary>
	/// <remarks>
	/// This provides control-specific binding, such as binding to a <see cref="Control.DataContext"/>.
	/// Any bindings created using this will also add to the <see cref="Control.Bindings"/> collection to keep its
	/// reference.
	/// </remarks>
	public class ControlBinding<T,TValue> : ObjectBinding<T, TValue>
		where T: Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControlBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Data item to get/set the values from/to.</param>
		/// <param name="getValue">Delegate to get the value from the object.</param>
		/// <param name="setValue">Delegate to set the value to the object.</param>
		/// <param name="addChangeEvent">Delegate to add the change event.</param>
		/// <param name="removeChangeEvent">Delegate to remove the chang event.</param>
		public ControlBinding(T dataItem, Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null)
			: base(dataItem, getValue, setValue, addChangeEvent, removeChangeEvent)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Control the binding is attached to.</param>
		/// <param name="innerBinding">Inner binding.</param>
		public ControlBinding(T dataItem, IndirectBinding<TValue> innerBinding)
			: base(dataItem, innerBinding)
		{
		}

		/// <summary>
		/// Binds the specified <paramref name="sourceBinding"/> to this binding.
		/// </summary>
		/// <remarks>
		/// This creates a <see cref="DualBinding{TValue}"/> between the specified <paramref name="sourceBinding"/> and this binding.
		/// The binding is added to the <see cref="Control.Bindings"/> collection.
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
		/// Binds to a control's <see cref="Control.DataContext"/> using the specified <paramref name="dataContextBinding"/>.
		/// </summary>
		/// <remarks>
		/// This creates a <see cref="DualBinding{TValue}"/> between a binding to the specified <paramref name="dataContextBinding"/> and this binding.
		/// Since the data context changes, the binding passed for the data context binding is an indirect binding, in that it is reused.
		/// The binding is added to the <see cref="Control.Bindings"/> collection.
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
			var contextBinding = new ControlBinding<Control, object>(control, new DelegateBinding<Control, object>(w => w.DataContext, null, (w, h) => w.DataContextChanged += h, (w, h) => w.DataContextChanged -= h));
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
		/// Binds to a control's <see cref="Control.DataContext"/> using delegates to get/set the value.
		/// </summary>
		/// <remarks>
		/// This is a shortcut to use the <see cref="DelegateBinding{T,TValue}"/> to bind to a control's <see cref="Control.DataContext"/> property.
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
			return BindDataContext(new PropertyBinding<TValue>(propertyName), mode);
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
			var memberInfo = propertyExpression.GetMemberInfo();
			if (memberInfo == null)
			{
				var getValue = propertyExpression.Compile();
				return BindDataContext<TObject>(getValue, null, null, null, mode);
			}
			return BindDataContext(new PropertyBinding<TValue>(memberInfo.Member.Name), mode);
		}

		#region Obsolete

		/// <summary>
		/// Obsolete. Use BindDataContext(...) instead.
		/// </summary>
		[Obsolete("Use BindDataContext() instead")]
		public DualBinding<TValue> Bind(IndirectBinding<TValue> dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
		{
			return BindDataContext(dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		/// <summary>
		/// Obsolete. Use BindDataContext(...) instead.
		/// </summary>
		[Obsolete("Use BindDataContext<T> instead")]
		public DualBinding<TValue> Bind<TObject>(Func<TObject, TValue> getValue, Action<TObject, TValue> setValue = null, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			return BindDataContext(new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent, defaultGetValue, defaultSetValue), mode);
		}

		/// <summary>
		/// Obsolete. Use BindDataContext(...) instead.
		/// </summary>
		[Obsolete("Use BindDataContext<T> instead")]
		public DualBinding<TValue> Bind<TObject>(DelegateBinding<TObject, TValue> binding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(dataContextBinding: binding, mode: mode);
		}

		#endregion
	}
}
