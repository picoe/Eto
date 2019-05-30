using System;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Helper extensions for <see cref="IBindable"/> objects to set up object bindings.
	/// </summary>
	public static class BindableExtensions
	{
		/// <summary>
		/// Adds a new dual binding between the control and the specified object
		/// </summary>
		/// <param name="bindable">Bindable object to add the binding to</param>
		/// <param name="propertyName">Property on the control to update</param>
		/// <param name="source">Object to bind to</param>
		/// <param name="sourcePropertyName">Property on the source object to retrieve/set the value of</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding<T> Bind<T>(this IBindable bindable, string propertyName, object source, string sourcePropertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding<T>(
				source,
				sourcePropertyName,
				bindable,
				propertyName,
				mode
			);
			bindable.Bindings.Add(binding);
			return binding;
		}

		/// <summary>
		/// Adds a new dual binding between the control and the specified source binding
		/// </summary>
		/// <param name="bindable">Bindable object to add the binding to</param>
		/// <param name="widgetPropertyName">Property on the control to update</param>
		/// <param name="sourceBinding">Binding to get/set the value to from the control</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding<T> Bind<T>(this IBindable bindable, string widgetPropertyName, DirectBinding<T> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding<T>(
				sourceBinding,
				new BindableBinding<IBindable,T>(bindable, Binding.Property<T>(widgetPropertyName)),
				mode
			);
			bindable.Bindings.Add(binding);
			return binding;
		}

		/// <summary>
		/// Adds a new binding with the control and the the control's current data context 
		/// </summary>
		/// <remarks>
		/// This binds to a property of the <see cref="IBindable.DataContext"/>, which will return the topmost value
		/// up the control hierarchy.  For example, you can set the DataContext of your form or panel, and then bind to properties
		/// of that context on any of the child controls such as a text box, etc.
		/// </remarks>
		/// <param name="bindable">Bindable object to add the binding to</param>
		/// <param name="controlPropertyName">Property on the control to update</param>
		/// <param name="dataContextPropertyName">Property on the control's <see cref="IBindable.DataContext"/> to bind to the control</param>
		/// <param name="mode">Mode of the binding</param>
		/// <param name="defaultControlValue">Default value to set to the control when the value from the DataContext is null</param>
		/// <param name="defaultContextValue">Default value to set to the DataContext property when the control value is null</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding<T> BindDataContext<T>(this IBindable bindable, string controlPropertyName, string dataContextPropertyName, DualBindingMode mode = DualBindingMode.TwoWay, T defaultControlValue = default(T), T defaultContextValue = default(T))
		{
			var dataContextBinding = Binding.Property<T>(dataContextPropertyName);
			var controlBinding = Binding.Property<T>(controlPropertyName);
			return BindDataContext(bindable, controlBinding, dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		/// <summary>
		/// Adds a new binding to the control with a direct value binding
		/// </summary>
		/// <param name="bindable">Bindable object to add the binding to</param>
		/// <param name="controlBinding">Binding to get/set the value from the control.</param>
		/// <param name="valueBinding">Value binding to get/set the value from another source.</param>
		/// <param name="mode">Mode of the binding</param>
		public static DualBinding<T> Bind<T>(this IBindable bindable, IndirectBinding<T> controlBinding, DirectBinding<T> valueBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new BindableBinding<IBindable,T>(bindable, controlBinding);
			return binding.Bind(sourceBinding: valueBinding, mode: mode);
		}

		/// <summary>
		/// Adds a new binding to the control with an indirect binding to the provided <paramref name="objectValue"/>
		/// </summary>
		/// <param name="bindable">Bindable object to add the binding to</param>
		/// <param name="controlBinding">Binding to get/set the value from the control.</param>
		/// <param name="objectValue">Object value to bind to.</param>
		/// <param name="objectBinding">Binding to get/set the value from the <paramref name="objectValue"/>.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value to set to the objectValue, if the value of the control property is null.</param>
		/// <param name="defaultContextValue">Default context value to set to the control, if the objectValue or value of the objectBinding is null.</param>
		public static DualBinding<T> Bind<T>(this IBindable bindable, IndirectBinding<T> controlBinding, object objectValue, IndirectBinding<T> objectBinding, DualBindingMode mode = DualBindingMode.TwoWay, T defaultControlValue = default(T), T defaultContextValue = default(T))
		{
			var valueBinding = new ObjectBinding<object,T>(objectValue, objectBinding) {
				SettingNullValue = defaultContextValue,
				GettingNullValue = defaultControlValue
			};
			return Bind(bindable, controlBinding, valueBinding, mode);
		}

		/// <summary>
		/// Adds a new binding from the control to its data context
		/// </summary>
		/// <param name="bindable">Bindable object to add the binding to</param>
		/// <param name="controlBinding">Binding to get/set the value from the control.</param>
		/// <param name="dataContextBinding">Binding to get/set the value from the <see cref="IBindable.DataContext"/>.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value to set to the objectValue, if the value of the control property is null.</param>
		/// <param name="defaultContextValue">Default context value to set to the control, if the objectValue or value of the objectBinding is null.</param>
		public static DualBinding<T> BindDataContext<T>(this IBindable bindable, IndirectBinding<T> controlBinding, IndirectBinding<T> dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, T defaultControlValue = default(T), T defaultContextValue = default(T))
		{
			var binding = new BindableBinding<IBindable, T>(bindable, controlBinding);
			return binding.BindDataContext(dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		/// <summary>
		/// Binds a control property to a <paramref name="source"/> property
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property.</param>
		/// <param name="source">Source object to get/set the value from.</param>
		/// <param name="sourceProperty">Source property from the data context.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <typeparam name="TWidget">The type of control.</typeparam>
		/// <typeparam name="TSource">The type of the source object.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> Bind<TWidget, TSource, TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, TSource source, Expression<Func<TSource, TValue>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			where TWidget : IBindable
		{
			return control.Bind(Binding.Property(controlProperty), source, Binding.Property(sourceProperty), mode);
		}

		/// <summary>
		/// Binds a control property to the <paramref name="source"/> object using the <paramref name="sourceBinding"/>.
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property expression.</param>
		/// <param name="source">Source object to bind to.</param>
		/// <param name="sourceBinding">Binding to get/set the value from the source.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <typeparam name="TWidget">The type of the control.</typeparam>
		/// <typeparam name="TSource">The type of the source object.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> Bind<TWidget, TSource, TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, TSource source, IndirectBinding<TValue> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
			where TWidget : IBindable
		{
			return control.Bind(Binding.Property(controlProperty), source, sourceBinding, mode);
		}

		/// <summary>
		/// Bind a control property to the specified <paramref name="sourceBinding"/> direct binding.
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property expression.</param>
		/// <param name="sourceBinding">Source binding to get/set the values.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <typeparam name="TWidget">The type of the control.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> Bind<TWidget, TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, DirectBinding<TValue> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
			where TWidget : IBindable
		{
			return control.Bind(Binding.Property(controlProperty), sourceBinding, mode);
		}

		/// <summary>
		/// Binds a control property to a <see cref="BindableWidget.DataContext"/> property
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property.</param>
		/// <param name="sourceProperty">Source property from the data context.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value, if the control value is null.</param>
		/// <param name="defaultContextValue">Default context value, if the context value is null.</param>
		/// <typeparam name="TWidget">The type of control.</typeparam>
		/// <typeparam name="TContext">The type of the data context object.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> BindDataContext<TWidget, TContext, TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, Expression<Func<TContext, TValue>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
			where TWidget : IBindable
		{
			IndirectBinding<TValue> controlBinding = Binding.Property(controlProperty);

			IndirectBinding<TValue> sourceBinding = Binding.Property(sourceProperty);

			return control.BindDataContext(controlBinding, sourceBinding, mode, defaultControlValue, defaultContextValue);
		}

		/// <summary>
		/// Binds a control property to a <see cref="BindableWidget.DataContext"/> property
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property.</param>
		/// <param name="sourceBinding">Source binding to get/set the value on the data context.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value, if the control value is null.</param>
		/// <param name="defaultContextValue">Default context value, if the context value is null.</param>
		/// <typeparam name="TWidget">The type of control.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> BindDataContext<TWidget, TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, IndirectBinding<TValue> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
			where TWidget : IBindable
		{
			return control.BindDataContext<TValue>(
				Binding.Property(controlProperty),
				sourceBinding,
				mode,
				defaultControlValue,
				defaultContextValue
			);
		}

		/// <summary>
		/// Gets a bindable binding with the inverse of the specified boolean value binding.
		/// </summary>
		/// <returns>A new binding to the inverse value.</returns>
		/// <param name="binding">Binding to invert.</param>
		/// <typeparam name="T">The type of the bindable object.</typeparam>
		public static BindableBinding<T, bool?> Inverse<T>(this BindableBinding<T, bool?> binding)
			where T: IBindable
		{
			return binding.Convert(c => !c, c => !c);
		}

		/// <summary>
		/// Gets a bindable binding with the inverse of the specified boolean value binding.
		/// </summary>
		/// <returns>A new binding to the inverse value.</returns>
		/// <param name="binding">Binding to invert.</param>
		/// <typeparam name="T">The type of the bindable object.</typeparam>
		public static BindableBinding<T, bool> Inverse<T>(this BindableBinding<T, bool> binding)
			where T: IBindable
		{
			return binding.Convert(c => !c, c => !c);
		}

		/// <summary>
		/// Gets a binding with the inverse of the specified nullable boolean value binding.
		/// </summary>
		/// <returns>A new binding to the inverse value.</returns>
		/// <param name="binding">Binding to invert.</param>
		public static DirectBinding<bool?> Inverse(this DirectBinding<bool?> binding)
		{
			return binding.Convert(c => !c, c => !c);
		}

		/// <summary>
		/// Gets a binding with the inverse of the specified nullable boolean value binding.
		/// </summary>
		/// <returns>A new binding to the inverse value.</returns>
		/// <param name="binding">Binding to invert.</param>
		public static DirectBinding<bool> Inverse(this DirectBinding<bool> binding)
		{
			return binding.Convert(c => !c, c => !c);
		}

		/// <summary>
		/// Gets a binding that returns a <paramref name="defaultValue"/> if the specified <paramref name="binding"/> returns a null value.
		/// </summary>
		/// <returns>A new binding that returns a non-nullable value.</returns>
		/// <param name="binding">Source of the binding to get the value.</param>
		/// <param name="defaultValue">Default value, or null to use <c>default(TValue)</c>.</param>
		/// <typeparam name="T">The type of the bindable object.</typeparam>
		/// <typeparam name="TValue">The value type to convert from nullable to non-nullable.</typeparam>
		public static BindableBinding<T, TValue> DefaultIfNull<T, TValue>(this BindableBinding<T, TValue?> binding, TValue? defaultValue = null)
			where T: IBindable
			where TValue: struct
		{
			return binding.Convert(c => c ?? defaultValue ?? default(TValue), c => c);
		}

		/// <summary>
		/// Gets a binding that returns a <paramref name="defaultValue"/> if the specified <paramref name="binding"/> returns a null value.
		/// </summary>
		/// <returns>A new binding that returns a non-null value.</returns>
		/// <param name="binding">Source of the binding to get the value.</param>
		/// <param name="defaultValue">Default value to return instead of null.</param>
		/// <typeparam name="T">The type of the bindable object.</typeparam>
		/// <typeparam name="TValue">The value type.</typeparam>
		public static BindableBinding<T, TValue> DefaultIfNull<T, TValue>(this BindableBinding<T, TValue> binding, TValue defaultValue)
			where T : IBindable
			where TValue : class
		{
			return binding.Convert(c => c ?? defaultValue, c => c);
		}
	}
}

