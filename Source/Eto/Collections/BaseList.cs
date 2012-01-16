using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Collections
{
	public class BaseList<T> : IList<T>, ICollection<T>, IList
	{
		#region Members
		
		List<T> list;
		
		#endregion
		
		#region Properties

		public int Count
		{
			get { return list.Count; }
		}
		
		public bool IsReadOnly
		{
			get { return false; }
		}
		
		protected IList<T> InnerList
		{
			get { return list; }
		}

		#endregion
		
		#region Events
		
		public event EventHandler<EventArgs> Changed;
		
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null) Changed(this, e);
		}
		
		public event ListEventHandler<T> Added;
		
		protected virtual void OnAdded(ListEventArgs<T> e)
		{
			if (Added != null) Added(this, e);
		}
		
		public event ListEventHandler<T> Removed;
		
		protected virtual void OnRemoved(ListEventArgs<T> e)
		{
			if (Removed != null) Removed(this, e);
		}

		#endregion
		
		public BaseList()
		{
			list = new List<T>();
		}

		public BaseList(int capacity)
		{
			list = new List<T>(capacity);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}
		
		public int Capacity
		{
			get { return list.Capacity; }
			set { list.Capacity = value; }
		}
		
		public virtual void Clear()
		{
			list.Clear();
			OnChanged(EventArgs.Empty);
		}
		
		public virtual bool Contains(T item)
		{
			return list.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}
		
		public virtual void AddRange(IEnumerable<T> collection)
		{
			list.AddRange(collection);
			OnChanged(EventArgs.Empty);
		}
		
		public virtual void Add(T item)
		{
			list.Add(item);
			OnAdded(new ListEventArgs<T>(item, -1));
			OnChanged(EventArgs.Empty);
		}
		
		public virtual bool Remove(T item)
		{
			int index = list.IndexOf(item);
			if (index >= 0) {
				list.RemoveAt (index);
				OnRemoved(new ListEventArgs<T>(item, index));
				OnChanged(EventArgs.Empty);
				return true;
			}
			return false;
		}
		
		public virtual void RemoveAt(int index)
		{
			T item = this[index];
			OnRemoved(new ListEventArgs<T>(item, index));
			OnChanged(EventArgs.Empty);
			list.RemoveAt(index);
		}
		
		public virtual void Insert(int index, T item)
		{
			list.Insert(index, item);
			OnAdded(new ListEventArgs<T>(item, index));
			OnChanged(EventArgs.Empty);
		}
		
		public virtual T this[int index]
		{
			get { return list[index]; }
			set
			{
				T item = list[index];
				OnRemoved(new ListEventArgs<T>(item, index));
				list[index] = value;
				OnAdded(new ListEventArgs<T>(value, index));
				OnChanged(EventArgs.Empty);
			}
		}

		#region IList implementation
		int IList.Add (object value)
		{
			this.Add ((T)value);
			return this.Count - 1;
		}

		bool IList.Contains (object value)
		{
			return this.Contains ((T)value);
		}

		int IList.IndexOf (object value)
		{
			return this.IndexOf ((T)value);
		}

		void IList.Insert (int index, object value)
		{
			this.Insert (index, (T)value);
		}

		void IList.Remove (object value)
		{
			this.Remove ((T)value);
		}

		bool IList.IsFixedSize {
			get { return false; }
		}

		bool IList.IsReadOnly {
			get { return false; }
		}
		
		object IList.this[int index] {
			get { return list[index]; }
			set { this[index] = (T)value; }
		}
		#endregion



		#region ICollection implementation
		void ICollection.CopyTo (Array array, int index)
		{
			((ICollection)list).CopyTo (array, index);
		}

		int ICollection.Count {
			get { return this.Count; }
		}

		bool ICollection.IsSynchronized {
			get { return ((ICollection)list).IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return ((ICollection)list).SyncRoot; }
		}
		#endregion
	}
}


