using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace Eto.Forms
{
	/// <summary>
	/// Indirect binding to get/set values using a property of a specified object
	/// </summary>
	/// <remarks>
	/// This is used when you are binding to a particular property of an object.
	/// 
	/// This can be used to get/set values from any object.  If you want to bind to a particular object
	/// directly, use the <see cref="ObjectBinding{T}"/> with this class as its inner binding.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PropertyBinding<T> : IndirectBinding<T>
	{
#if NETSTANDARD
		PropertyInfo descriptor;
		Type declaringType;
#else
		PropertyDescriptor descriptor;
#endif
		string property;

		/// <summary>
		/// Gets or sets the property in which to get/set values from for this binding
		/// </summary>
		public string Property
		{
			get { return property; }
			set
			{
				property = value;
				descriptor = null;
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="Property"/> specified is case-sensitive or not
		/// </summary>
		public bool IgnoreCase { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyBinding{T}"/> class.
		/// </summary>
		public PropertyBinding()
		{
		}

		/// <summary>
		/// Initializes a new instance of the PropertyBinding with the specified property
		/// </summary>
		/// <param name="property">Property to use to get/set values for this binding</param>
		/// <param name="ignoreCase">True to ignore case for the property, false to be case sensitive</param>
		public PropertyBinding(string property, bool ignoreCase = true)
		{
			this.Property = property;
			this.IgnoreCase = ignoreCase;
		}

		void EnsureProperty(object dataItem)
		{
			if (dataItem == null)
				return;

			if (
				// found previously, but incompatible type
				(descriptor != null && !declaringType.IsInstanceOfType(dataItem))
				// not found yet, and the type is different than last lookup
				|| (descriptor == null || declaringType != dataItem.GetType())
			)
			{
#if NETSTANDARD
				var dataItemType = dataItem.GetType();
				descriptor = null;
				// iterate to find non-public properties or with different case
				var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				foreach (var prop in dataItemType.GetRuntimeProperties())
				{
					if (string.Equals(prop.Name, Property, comparison))
					{
						descriptor = prop;
						break;
					}
				}
				declaringType = descriptor?.DeclaringType ?? dataItemType;
#else
				descriptor = TypeDescriptor.GetProperties(dataItem).Find(Property, IgnoreCase);
				declaringType = descriptor?.ComponentType ?? dataItemType;
#endif
			}
		}

		/// <summary>
		/// Determines whether the dataItem contains the property this binding is bound to.
		/// </summary>
		/// <returns><c>true</c> if the dataItem instance has the correct property to bind to, otherwise, <c>false</c>.</returns>
		/// <param name="dataItem">Data item to find the property.</param>
		protected bool HasProperty(object dataItem)
		{
			EnsureProperty(dataItem);
			return descriptor != null;
		}


		/// <summary>
		/// Implements the logic to get the value from the specified object
		/// </summary>
		/// <param name="dataItem">object to get the value from</param>
		/// <returns>value of the property from the specified dataItem object</returns>
		protected override T InternalGetValue(object dataItem)
		{
			EnsureProperty(dataItem);
			if (descriptor != null && dataItem != null
				#if NETSTANDARD
				&& descriptor.CanRead
				#endif
				)
			{
				var propertyType = typeof(T);
				object val = descriptor.GetValue(dataItem);
				if (val != null && !propertyType.IsInstanceOfType(val))
				{
					try
					{
						propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
						val = System.Convert.ChangeType(val, propertyType, CultureInfo.InvariantCulture);
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Could not convert object of type {val.GetType()} to {propertyType}\n{ex}");
						val = propertyType.GetTypeInfo().IsValueType ? Activator.CreateInstance(propertyType) : null;
					}
				}
				return (T)val;
			}
			return default(T);
		}

		/// <summary>
		/// Implements the logic to set the value on the specified object
		/// </summary>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set to the property of the specified dataItem object</param>
		protected override void InternalSetValue(object dataItem, T value)
		{
			EnsureProperty(dataItem);
			if (descriptor != null && dataItem != null
				#if NETSTANDARD
				&& descriptor.CanWrite
				#else
				&& !descriptor.IsReadOnly
				#endif
				)
			{
				var propertyType = descriptor.PropertyType;
				object val = value;
				if (val != null && !propertyType.IsInstanceOfType(val))
				{
					try
					{
#if NETSTANDARD
						propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
#endif
						val = System.Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Could not convert object of type {val.GetType()} to {propertyType}\n{ex}");
						val = propertyType.GetTypeInfo().IsValueType ? Activator.CreateInstance(propertyType) : null;
					}
				}
				descriptor.SetValue(dataItem, val);
			}
		}

		class ValueChangedHandler
		{
			public PropertyBinding<T> Binding { get; set; }

			public object DataItem { get; set; }

			public EventHandler<EventArgs> Handler { get; set; }

			public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == Binding.Property)
					Handler(DataItem, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Wires an event handler to fire when the property of the dataItem is changed
		/// </summary>
		/// <param name="dataItem">object to detect changes on</param>
		/// <param name="handler">handler to fire when the property changes on the specified dataItem</param>
		/// <returns>binding reference used to track the event hookup, to pass to <see cref="RemoveValueChangedHandler"/> when removing the handler</returns>
		public override object AddValueChangedHandler(object dataItem, EventHandler<EventArgs> handler)
		{
			if (dataItem == null)
				return false;
			var notify = dataItem as INotifyPropertyChanged;
			if (notify != null)
			{
				var helper = new ValueChangedHandler
				{
					Binding = this,
					DataItem = dataItem,
					Handler = handler
				};
				notify.PropertyChanged += helper.HandlePropertyChanged;
				return helper;
			}
			else
			{
				var type = dataItem.GetType();
				var changedEvent = type.GetRuntimeEvent(Property + "Changed");
				if (changedEvent != null)
				{
					try
					{
						changedEvent.AddEventHandler(dataItem, handler);
					}
					catch
					{
					}
				}
				return dataItem;
			}
		}

		/// <summary>
		/// Removes the handler for the specified reference from <see cref="AddValueChangedHandler"/>
		/// </summary>
		/// <param name="bindingReference">Reference from the call to <see cref="AddValueChangedHandler"/></param>
		/// <param name="handler">Same handler that was set up during the <see cref="AddValueChangedHandler"/> call</param>
		public override void RemoveValueChangedHandler(object bindingReference, EventHandler<EventArgs> handler)
		{
			var helper = bindingReference as ValueChangedHandler;
			if (helper != null)
			{
				var notify = (INotifyPropertyChanged)helper.DataItem;
				notify.PropertyChanged -= helper.HandlePropertyChanged;
			}
			else
			{
				var dataItem = bindingReference;
				if (dataItem == null)
					return;
				var type = dataItem.GetType();
				var changedEvent = type.GetRuntimeEvent(Property + "Changed");
				if (changedEvent != null)
				{
					try
					{
						changedEvent.RemoveEventHandler(dataItem, handler);
					}
					catch
					{
					}
				}
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Eto.Forms.PropertyBinding`1"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Eto.Forms.PropertyBinding`1"/>.</returns>
		public override string ToString()
		{
			return $"Property: {Property}";
		}
	}
}
