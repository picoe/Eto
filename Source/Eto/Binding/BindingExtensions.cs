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
		/// <param name="sourceBinding">Binding to</param>
		/// <param name="mode"></param>
		/// <returns></returns>
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
		
		public static DualBinding Bind (this InstanceWidget widget, string widgetPropertyName, string dataContextPropertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var contextBinding = new ObjectBinding(widget, "DataContext");
			var valueBinding = new ObjectBinding(contextBinding.DataValue, dataContextPropertyName);
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
