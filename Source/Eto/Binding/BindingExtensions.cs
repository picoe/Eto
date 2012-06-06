using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public static class BindingExtensions
	{
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
		
		public static DualBinding Bind (this Widget widget, string widgetPropertyName, ObjectBinding sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding (
				sourceBinding,
				new ObjectSingleBinding(widget, widgetPropertyName),
				mode
			);
			widget.Bindings.Add (binding);
			return binding;
		}
	}
}
