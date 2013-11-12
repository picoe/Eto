using System.Collections.Generic;
using System;

namespace Eto
{
	public class PropertyStore : Dictionary<object, object>
	{
		public InstanceWidget Parent { get; private set; }

		public PropertyStore(InstanceWidget parent)
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
