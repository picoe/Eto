using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	public class DataStoreVirtualCollection<T> : IList<T>, IList
		where T : class
	{
		const string ReadOnlyErrorMsg = "DataStoreVirtualCollection is a read-only collection.";
		IDataStore<T> store;

		public DataStoreVirtualCollection (IDataStore<T> store)
		{
			this.store = store;
		}

		#region IList<T> Members

		public int IndexOf (T item)
		{
			return this.IndexOf (item);
		}

		public void Insert (int index, T item)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		public void RemoveAt (int index)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		public T this [int index] {
			get {
				return this.store [index];
			}
			set {
				throw new NotSupportedException (ReadOnlyErrorMsg);
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add (T item)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		public void Clear ()
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		public bool Contains (T item)
		{
			return (this.IndexOf (item) != -1);
		}

		public void CopyTo (T[] array, int arrayIndex)
		{
			for (int i = 0; i < Count; i++) {
				array [arrayIndex + i] = this [i];
			}
		}

		public int Count {
			get { return this.store.Count; }
		}

		public bool IsReadOnly {
			get { return true; }
		}

		public bool Remove (T item)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator ()
		{
			return new DataStoreEnumerator (this);
		}

		#endregion

		#region IList Members

		public int Add (object value)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		public bool Contains (object value)
		{
			throw new NotImplementedException ();
		}

		public int IndexOf (object value)
		{
			int count = this.store.Count;
			for (int index = 0; index < count; ++index) {
				if (this.store [index].Equals (value))
					return index;
			}
			return -1;
		}

		public void Insert (int index, object value)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		public bool IsFixedSize {
			get { return true; }
		}

		public void Remove (object value)
		{
			throw new NotSupportedException (ReadOnlyErrorMsg);
		}

		object IList.this [int index] {
			get { return this [index]; }
			set {
				throw new NotSupportedException (ReadOnlyErrorMsg);
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo (Array array, int index)
		{
			for (int i = 0; i < Count; i++) {
				array.SetValue (this [i], index + i);
			}
		}

		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new DataStoreEnumerator (this);
		}

		#endregion

		#region Internal IEnumerator implementation

		private class DataStoreEnumerator : IEnumerator<T>
		{
			DataStoreVirtualCollection<T> collection;
			int cursor;

			public DataStoreEnumerator (DataStoreVirtualCollection<T> collection)
			{
				this.collection = collection;
				this.cursor = -1;
			}

			#region IEnumerator<T> Members

			public T Current {
				get { return this.collection [this.cursor]; }
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current {
				get { return this.Current; }
			}

			public bool MoveNext ()
			{
				this.cursor++;
				if (this.cursor == this.collection.Count)
					return false;
				return true;
			}

			public void Reset ()
			{
				this.cursor = -1;
			}

			#endregion

			#region IDisposable Members

			public void Dispose ()
			{
			}

			#endregion
		}

		#endregion
	}
}
