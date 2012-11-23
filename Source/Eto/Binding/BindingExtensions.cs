using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Extensions for bindings
	/// </summary>
	public static class BindingExtensions
	{
		/// <summary>
		/// Adds a new dual binding between the widget and the specified object
		/// </summary>
		/// <param name="widget">Widget to add the binding to</param>
		/// <param name="widgetPropertyName">Property on the widget to update</param>
		/// <param name="source">Object to bind to</param>
		/// <param name="sourcePropertyName">Property on the source object to retrieve/set the value of</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding Bind (this Widget widget, string widgetPropertyName, object source, string sourcePropertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding (
				source,
				sourcePropertyName,
				widget,
				widgetPropertyName,
				mode
				);
			widget.Bindings.Add (binding);
			return binding;
		}
		
		/// <summary>
		/// Adds a new dual binding between the widget and the specified source binding
		/// </summary>
		/// <param name="widget">Widget to add the binding to</param>
		/// <param name="widgetPropertyName">Property on the widget to update</param>
		/// <param name="sourceBinding">Binding to get/set the value to from the widget</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding Bind (this Widget widget, string widgetPropertyName, DirectBinding sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding (
				sourceBinding,
				new ObjectBinding(widget, widgetPropertyName),
				mode
				);
			widget.Bindings.Add (binding);
			return binding;
		}
		
		/// <summary>
		/// Adds a new binding with the widget and the the widget's current data context 
		/// </summary>
		/// <remarks>
		/// This binds to a property of the <see cref="InstanceWidget.DataContext"/>, which will return the topmost value
		/// up the control hierarchy.  For example, you can set the DataContext of your form or panel, and then bind to properties
		/// of that context on any of the child controls such as a text box, etc.
		/// </remarks>
		/// <param name="widget">Widget to add the binding to</param>
		/// <param name="widgetPropertyName">Property on the widget to update</param>
		/// <param name="dataContextPropertyName">Property on the widget's <see cref="InstanceWidget.DataContext"/> to bind to the widget</param>
		/// <param name="mode">Mode of the binding</param>
		/// <param name="defaultWidgetValue">Default value to set to the widget when the value from the DataContext is null</param>
		/// <param name="defaultContextValue">Default value to set to the DataContext property when the widget value is null</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public static DualBinding Bind (this InstanceWidget widget, string widgetPropertyName, string dataContextPropertyName, DualBindingMode mode = DualBindingMode.TwoWay, object defaultWidgetValue = null, object defaultContextValue = null)
		{
			var contextBinding = new ObjectBinding(widget, "DataContext");
			var valueBinding = new ObjectBinding(contextBinding.DataValue, dataContextPropertyName);
			valueBinding.GettingNullValue = defaultWidgetValue;
			valueBinding.SettingNullValue = defaultContextValue;
			contextBinding.DataValueChanged += delegate {
				valueBinding.DataItem = contextBinding.DataValue;
			};
			var binding = new DualBinding (
				valueBinding,
				new ObjectBinding(widget, widgetPropertyName),
				mode
				);
			widget.Bindings.Add (contextBinding);
			widget.Bindings.Add (binding);
			return binding;
		}
	}
}
