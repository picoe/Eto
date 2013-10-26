using System;
using System.Linq.Expressions;
using Eto.Forms;

namespace Eto
{
	/// <summary>
	/// Extensions for bindings
	/// </summary>
	public static class BindingExtensions
	{
		/// <summary>
		/// Adds a new dual binding between the control and the specified object
		/// </summary>
		/// <param name="control">Control to add the binding to</param>
		/// <param name="propertyName">Property on the control to update</param>
		/// <param name="source">Object to bind to</param>
		/// <param name="sourcePropertyName">Property on the source object to retrieve/set the value of</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding Bind(this Control control, string propertyName, object source, string sourcePropertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding(
				source,
				sourcePropertyName,
				control,
				propertyName,
				mode
			);
			control.Bindings.Add(binding);
			return binding;
		}
		
		/// <summary>
		/// Adds a new dual binding between the control and the specified source binding
		/// </summary>
		/// <param name="control">Control to add the binding to</param>
		/// <param name="widgetPropertyName">Property on the control to update</param>
		/// <param name="sourceBinding">Binding to get/set the value to from the control</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding Bind(this Control control, string widgetPropertyName, DirectBinding sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding(
				sourceBinding,
				new ObjectBinding(control, widgetPropertyName),
				mode
			);
			control.Bindings.Add(binding);
			return binding;
		}
		
		/// <summary>
		/// Adds a new binding with the control and the the control's current data context 
		/// </summary>
		/// <remarks>
		/// This binds to a property of the <see cref="Control.DataContext"/>, which will return the topmost value
		/// up the control hierarchy.  For example, you can set the DataContext of your form or panel, and then bind to properties
		/// of that context on any of the child controls such as a text box, etc.
		/// </remarks>
		/// <param name="control">Control to add the binding to</param>
		/// <param name="controlPropertyName">Property on the control to update</param>
		/// <param name="dataContextPropertyName">Property on the control's <see cref="Control.DataContext"/> to bind to the control</param>
		/// <param name="mode">Mode of the binding</param>
		/// <param name="defaultControlValue">Default value to set to the control when the value from the DataContext is null</param>
		/// <param name="defaultContextValue">Default value to set to the DataContext property when the control value is null</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding Bind(this Control control, string controlPropertyName, string dataContextPropertyName, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
		{
			var dataContextBinding = new PropertyBinding(dataContextPropertyName);
			var controlBinding = new PropertyBinding(controlPropertyName);
			return Bind(control, controlBinding, dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		public static DualBinding Bind<W,WP,S,SP>(this W control, Expression<Func<W,WP>> controlProperty, S source, Expression<Func<S, SP>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			where W: Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return Bind(control, controlExpression.Member.Name, source, sourceExpression.Member.Name, mode);
		}

		public static DualBinding Bind<W, WP, SP, DC>(this W control, Expression<Func<W, WP>> controlProperty, Expression<Func<DC, SP>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
			where W : Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return Bind(control, controlExpression.Member.Name, sourceExpression.Member.Name, mode, defaultControlValue, defaultContextValue);
		}

		public static DualBinding Bind(this Control control, IndirectBinding controlBinding, DirectBinding valueBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(controlBinding: new ObjectBinding(control, controlBinding), valueBinding: valueBinding, mode: mode);
		}

		public static DualBinding Bind(this Control control, IndirectBinding controlBinding, object objectValue, IndirectBinding objectBinding, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
		{
			var valueBinding = new ObjectBinding(objectValue, objectBinding) {
				SettingNullValue = defaultContextValue,
				GettingNullValue = defaultControlValue
			};
			return Bind(control, controlBinding, valueBinding, mode);
		}

		public static DualBinding Bind(this Control control, IndirectBinding controlBinding, IndirectBinding dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
		{
			return Bind(new ObjectBinding(control, controlBinding), dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		public static DualBinding Bind(this ObjectBinding controlBinding, DirectBinding valueBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding(
				valueBinding,
				controlBinding,
				mode
			);
			var control = controlBinding.DataItem as Control;
			if (control != null)
				control.Bindings.Add(binding);
			return binding;
		}

		public static DualBinding Bind(this ObjectBinding controlBinding, IndirectBinding dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
		{
			var control = controlBinding.DataItem as Control;
			if (control == null)
				throw new ArgumentOutOfRangeException("controlBinding", "Binding must be attached to a control");
			var contextBinding = new ObjectBinding(control, new DelegateBinding<Control, object>(w => w.DataContext, null, (w, h) => w.DataContextChanged += h, (w, h) => w.DataContextChanged -= h));
			var valueBinding = new ObjectBinding(control.DataContext, dataContextBinding) {
				GettingNullValue = defaultControlValue,
				SettingNullValue = defaultContextValue
			};
			DualBinding binding = Bind(controlBinding: controlBinding, valueBinding: valueBinding, mode: mode);
			contextBinding.DataValueChanged += delegate
			{
				((ObjectBinding)binding.Source).DataItem = contextBinding.DataValue;
			};
			control.Bindings.Add(contextBinding);
			return binding;
		}
	}
}