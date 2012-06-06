using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public abstract class SingleBinding : Binding
	{
		public event EventHandler<BindingChangingEventArgs> Changing;

		protected virtual void OnChanging (BindingChangingEventArgs e)
		{
			if (Changing != null)
				Changing (this, e);
		}
		
		public event EventHandler<BindingChangedEventArgs> Changed;

		protected virtual void OnChanged (BindingChangedEventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}

		public object GetValue (object dataItem)
		{
			return InternalGetValue (dataItem);
		}
		
		public void SetValue (object dataItem, object value)
		{
			var args = new BindingChangingEventArgs (value);
			OnChanging (args);
			InternalSetValue (dataItem, args.Value);
			OnChanged (new BindingChangedEventArgs (args.Value));
		}

		protected abstract object InternalGetValue (object dataItem);
		
		protected abstract void InternalSetValue (object dataItem, object value);
		
		public virtual object AddValueChangedHandler (object dataItem, EventHandler<EventArgs> handler)
		{
			return null;
		}
			
		public virtual void RemoveValueChangedHandler (object bindingReference, EventHandler<EventArgs> handler)
		{
		}
	}
}
