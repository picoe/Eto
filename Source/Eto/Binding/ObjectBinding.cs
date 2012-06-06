using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public abstract class ObjectBinding : Binding
	{
		public const string DataValueChangedEvent = "ObjectBinding.DataValueChangedEvent";

		event EventHandler<EventArgs> _DataValueChanged;

		public event EventHandler<EventArgs> DataValueChanged {
			add {
				_DataValueChanged += value;
				HandleEvent (DataValueChangedEvent);
			}
			remove {
				_DataValueChanged -= value;
				if (_DataValueChanged == null)
					RemoveEvent (DataValueChangedEvent);
			}
		}

		public virtual void OnDataValueChanged (EventArgs e)
		{
			if (_DataValueChanged != null)
				_DataValueChanged (this, e);
		}
		
		public virtual event EventHandler<BindingChangingEventArgs> Changing;
		public virtual event EventHandler<BindingChangedEventArgs> Changed;
		
		public abstract object DataItem { get; }
		
		public abstract object GetValue ();
		
		public abstract void SetValue (object value);
	}
}