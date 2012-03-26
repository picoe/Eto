using System;
using System.Collections.Specialized;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Eto
{
	public abstract class CollectionChangedHandler<T, S>
		where S: class
	{
		public S DataStore { get; private set; }
		
		protected virtual void OnRegisterCollection (EventArgs e)
		{
		}
		
		protected virtual void OnUnregisterCollection (EventArgs e)
		{
		}
			
		public bool Register (S store)
		{
			this.DataStore = store;
			
			var notify = DataStore as INotifyCollectionChanged;
			if (notify != null) {
				notify.CollectionChanged += CollectionChanged;
			}
			OnRegisterCollection (EventArgs.Empty);
			return store != null;
		}
		
		public void Unregister ()
		{
			if (DataStore == null)
				return;
			
			var notify = DataStore as INotifyCollectionChanged;
			if (notify != null) {
				notify.CollectionChanged -= CollectionChanged;
			}
			OnUnregisterCollection (EventArgs.Empty);
			
			DataStore = null;
		}
			
		public virtual int IndexOf (T item)
		{
			var list = DataStore as IList;
			if (list != null)
				return list.IndexOf (item);
			else {
				return InternalIndexOf (item);
			}
		}
		
		protected abstract int InternalIndexOf (T item);
		
		public abstract void AddItem (T item);
			
		public abstract void InsertItem (int index, T item);
			
		public abstract void RemoveItem (int index);
			
		public abstract void RemoveAllItems ();
			
		public virtual void RemoveItem (T item)
		{
			var index = IndexOf (item);
			if (index >= 0)
				RemoveItem (index);
		}
			
		public virtual void AddRange (IEnumerable<T> items)
		{
			foreach (var item in items)
				AddItem (item);
		}
			
		public virtual void InsertRange (int index, IEnumerable<T> items)
		{
			foreach (var item in items)
				InsertItem (index++, item);
		}
			
		public virtual void RemoveRange (int index, int count)
		{
			for (int i = 0; i < count; i++)
				RemoveItem (index);
		}

		public virtual void RemoveRange (IEnumerable<T> items)
		{
			foreach (var item in items) 
				RemoveItem (item);
		}

		void CollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				if (e.NewStartingIndex != -1) 
					InsertRange (e.NewStartingIndex, e.NewItems.Cast<T> ());
				else
					AddRange (e.NewItems.Cast<T> ());
				break;
			case NotifyCollectionChangedAction.Move:
				RemoveRange (e.OldItems.Cast<T> ());
				InsertRange (e.NewStartingIndex, e.NewItems.Cast<T> ());
				break;
			case NotifyCollectionChangedAction.Remove:
				if (e.OldStartingIndex != -1)
					RemoveRange (e.OldStartingIndex, e.OldItems.Count);
				else
					RemoveRange (e.OldItems.Cast<T> ());
				break;
			case NotifyCollectionChangedAction.Replace:
				if (e.OldStartingIndex != -1) {
					RemoveRange (e.OldStartingIndex, e.OldItems.Count);
					InsertRange (e.OldStartingIndex, e.NewItems.Cast<T> ());
				} else {
					for (int i = 0; i < e.OldItems.Count; i++) {
						var index = IndexOf ((T)e.OldItems [i]);
						RemoveItem (index);
						InsertItem (index, (T)e.NewItems [i]);
					}
				}
				break;
			case NotifyCollectionChangedAction.Reset:
				RemoveAllItems ();
				break;
			}
		}
	}

	public abstract class EnumerableChangedHandler<T, S> : CollectionChangedHandler<T, S>
		where S: class, IEnumerable
	{
		protected override int InternalIndexOf (T item)
		{
			int index = 0;
			foreach (var child in DataStore) {
				if (object.ReferenceEquals (item, child)) 
					return index;
				index++;
			}
			return -1;
		}
		
		protected override void OnRegisterCollection (EventArgs e)
		{
			base.OnRegisterCollection (e);
			AddRange (DataStore.Cast<T>());
		}
	}
	
	public abstract class DataStoreChangedHandler<T, S> : CollectionChangedHandler<T, S>
		where S: class, IDataStore<T>
	{
		protected override void OnRegisterCollection (EventArgs e)
		{
			base.OnRegisterCollection (e);
			AddRange (DataStore.AsEnumerable ());
		}
		
		protected override int InternalIndexOf (T item)
		{
			for (int i = 0; i < DataStore.Count; i++) {
				if (object.ReferenceEquals (item, DataStore [i])) 
					return i;
			}
			return -1;
		}
	}
}

