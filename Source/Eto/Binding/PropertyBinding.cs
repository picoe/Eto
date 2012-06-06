using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public class PropertyBinding : SingleBinding
	{
		PropertyDescriptor property;
				
		public string Property { get; set; }
		
		public PropertyBinding (string property)
		{
			this.Property = property;
		}
		
		void EnsureProperty (object dataItem)
		{
			if (property == null && dataItem != null) {
				property = TypeDescriptor.GetProperties (dataItem).Find (Property, true);
			}
		}
		
		protected override object InternalGetValue (object dataItem)
		{
			EnsureProperty (dataItem);
			if (property != null) {
				return property.GetValue (dataItem);
			}
			return null;
		}
		
		protected override void InternalSetValue (object dataItem, object value)
		{
			EnsureProperty (dataItem);
			if (property != null) {
				property.SetValue (dataItem, value);
			}
		}
		
		class ValueChangedHandler
		{
			public PropertyBinding Binding { get; set; }
			
			public object DataItem { get; set; }
			
			public EventHandler<EventArgs> Handler { get; set; }
			
			public void HandlePropertyChanged (object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == Binding.Property)
					Handler (DataItem, EventArgs.Empty);
			}
		}
		
		public override object AddValueChangedHandler (object dataItem, EventHandler<EventArgs> handler)
		{
			if (dataItem == null)
				return false;
			var notify = dataItem as INotifyPropertyChanged;
			if (notify != null) {
				var helper = new ValueChangedHandler {
					Binding = this,
					DataItem = dataItem,
					Handler = handler
				};
				notify.PropertyChanged += helper.HandlePropertyChanged;
				return helper;
			} else {
				var type = dataItem.GetType ();
				var changedEvent = type.GetEvent (Property + "Changed");
				if (changedEvent != null)
					changedEvent.AddEventHandler (dataItem, handler);
				return dataItem;
			}
		}
		
		public override void RemoveValueChangedHandler (object bindingReference, EventHandler<EventArgs> handler)
		{
			var helper = bindingReference as ValueChangedHandler;
			if (helper != null) {
				var notify = helper.DataItem as INotifyPropertyChanged;
				notify.PropertyChanged -= helper.HandlePropertyChanged;
			} else {
				var dataItem = bindingReference;
				var type = dataItem.GetType ();
				var changedEvent = type.GetEvent (Property + "Changed");
				if (changedEvent != null)
					changedEvent.RemoveEventHandler (dataItem, handler);
			}
		}
	}
}
