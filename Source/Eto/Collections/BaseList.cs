using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Collections
{
	public class BaseList<T> : IList<T>, ICollection<T>
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
			OnAdded(new ListEventArgs<T>(item));
			OnChanged(EventArgs.Empty);
		}
		
		public virtual bool Remove(T item)
		{
			bool ret = list.Remove(item);
			OnRemoved(new ListEventArgs<T>(item));
			OnChanged(EventArgs.Empty);
			return ret;
		}
		
		public virtual void RemoveAt(int index)
		{
			T item = this[index];
			OnRemoved(new ListEventArgs<T>(item));
			OnChanged(EventArgs.Empty);
			list.RemoveAt(index);
		}
		
		public virtual void Insert(int index, T item)
		{
			list.Insert(index, item);
			OnAdded(new ListEventArgs<T>(item));
			OnChanged(EventArgs.Empty);
		}
		
		public T this[int index]
		{
			get { return list[index]; }
			set
			{
				T item = list[index];
				OnRemoved(new ListEventArgs<T>(item));
				list[index] = value;
				OnAdded(new ListEventArgs<T>(value));
				OnChanged(EventArgs.Empty);
			}
		}
	}
}


