using System.Collections.Generic;
using System;

namespace Eto
{
	/// <summary>
	/// A storage for properties and events of a class
	/// </summary>
	/// <remarks>
	/// This is used by <see cref="InstanceWidget"/> object to minimize the footprint of each instance.
	/// For example, the <see cref="Forms.Control"/> class has around 20 events, each would take up to 4 bytes on a 32 bit 
	/// system for a total overhead of 80 bytes per instance.
	/// Most of the events won't be handled on most controls, so using a dictionary can dramatically reduce the size.
	/// </remarks>
	public class PropertyStore : Dictionary<object, object>
	{
		/// <summary>
		/// Gets the parent widget that this property store is attached to
		/// </summary>
		/// <remarks>
		/// This is used to attach/remove events
		/// </remarks>
		/// <value>The parent widget</value>
		public InstanceWidget Parent { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.PropertyStore"/> class.
		/// </summary>
		/// <param name="parent">Parent to attach the properties to</param>
		internal PropertyStore(InstanceWidget parent)
		{
			this.Parent = parent;
		}

		public T Get<T>(object key)
		{
			object value;
			return TryGetValue(key, out value) ? (T)value : default(T);
		}

		public T Create<T>(object key)
			where T: new()
		{
			object value;
			if (!TryGetValue(key, out value))
			{
				value = new T();
				Add(key, value);
			}
			return (T)value;
		}

		public void AddEvent(object key, Delegate value)
		{
			object existingDelegate;
			if (TryGetValue(key, out existingDelegate))
				this[key] = Delegate.Combine((Delegate)existingDelegate, value);
			else
			{
				Add(key, value);
			}
		}

		public void AddHandlerEvent(string key, Delegate value)
		{
			object existingDelegate;
			if (TryGetValue(key, out existingDelegate))
				this[key] = Delegate.Combine((Delegate)existingDelegate, value);
			else
			{
				if (!EventLookup.IsDefault(Parent, key))
					Parent.HandleDefaultEvents(key);
				Add(key, value);
			}
		}

		public void RemoveEvent(object key, Delegate value)
		{
			object existingDelegate;
			if (TryGetValue(key, out existingDelegate))
			{
				this[key] = Delegate.Remove((Delegate)existingDelegate, value);
			}
		}

		public void TriggerEvent<T>(object key, object sender, T args)
			where T: EventArgs
		{
			object existingDelegate;
			if (TryGetValue(key, out existingDelegate) && existingDelegate != null)
			{
				((EventHandler<T>)existingDelegate)(sender, args);
			}
		}
	}
}
