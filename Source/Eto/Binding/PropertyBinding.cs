using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Indirect binding to get/set values using a property of a specified object
	/// </summary>
	/// <remarks>
	/// This is used when you are binding to a particular property of an object.
	/// 
	/// This can be used to get/set values from any object.  If you want to bind to a particular object
	/// directly, use the <see cref="ObjectBinding"/> with this class as its inner binding.
	/// </remarks>
	public class PropertyBinding : IndirectBinding
	{
		PropertyDescriptor descriptor;
		string property;
				
		/// <summary>
		/// Gets or sets the property in which to get/set values from for this binding
		/// </summary>
		public string Property
		{
			get { return property; }
			set {
				property = value;
				descriptor = null;
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="Property"/> specified is case-sensitive or not
		/// </summary>
		public bool IgnoreCase { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the PropertyBinding with the specified property
		/// </summary>
		/// <param name="property">Property to use to get/set values for this binding</param>
		/// <param name="ignoreCase">True to ignore case for the property, false to be case sensitive</param>
		public PropertyBinding (string property, bool ignoreCase = true)
		{
			this.Property = property;
			this.IgnoreCase = ignoreCase;
		}
		
		void EnsureProperty (object dataItem)
		{
			if (dataItem != null && (descriptor == null || !descriptor.ComponentType.IsAssignableFrom(dataItem.GetType()))) {
				descriptor = TypeDescriptor.GetProperties (dataItem).Find (Property, IgnoreCase);
			}
		}
		
		/// <summary>
		/// Implements the logic to get the value from the specified object
		/// </summary>
		/// <param name="dataItem">object to get the value from</param>
		/// <returns>value of the property from the specified dataItem object</returns>
		protected override object InternalGetValue (object dataItem)
		{
			EnsureProperty (dataItem);
			if (descriptor != null) {
				return descriptor.GetValue (dataItem);
			}
			return null;
		}
		
		/// <summary>
		/// Implements the logic to set the value on the specified object
		/// </summary>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set to the property of the specified dataItem object</param>
		protected override void InternalSetValue (object dataItem, object value)
		{
			EnsureProperty (dataItem);
			if (descriptor != null) {
				if (value != null && !descriptor.PropertyType.IsAssignableFrom (value.GetType ()))
				{
					value = Convert.ChangeType (value, descriptor.PropertyType);
				}
				descriptor.SetValue (dataItem, value);
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
		
		/// <summary>
		/// Wires an event handler to fire when the property of the dataItem is changed
		/// </summary>
		/// <param name="dataItem">object to detect changes on</param>
		/// <param name="handler">handler to fire when the property changes on the specified dataItem</param>
		/// <returns>binding reference used to track the event hookup, to pass to <see cref="RemoveValueChangedHandler"/> when removing the handler</returns>
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
				if (changedEvent != null) {
					try {
						changedEvent.AddEventHandler (dataItem, handler);
					}
					catch {}
				}
				return dataItem;
			}
		}

		/// <summary>
		/// Removes the handler for the specified reference from <see cref="AddValueChangedHandler"/>
		/// </summary>
		/// <param name="bindingReference">Reference from the call to <see cref="AddValueChangedHandler"/></param>
		/// <param name="handler">Same handler that was set up during the <see cref="AddValueChangedHandler"/> call</param>
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
				if (changedEvent != null) {
					try {
						changedEvent.RemoveEventHandler (dataItem, handler);
					}
					catch {}
				}
			}
		}
	}
}
