using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Translates an <see cref="IDataStore{T}"/> to a read-only <see cref="IList{T}"/>
	/// </summary>
	/// <remarks>
	/// This is typically used to pass the data store to controls that require a standard collection
	/// </remarks>
	public class DataStoreVirtualCollection<T> : IList<T>, IList
	{
		const string ReadOnlyErrorMsg = "DataStoreVirtualCollection is a read-only collection.";
		readonly IDataStore<T> store;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreVirtualCollection{T}"/> class.
		/// </summary>
		/// <param name="store">Store.</param>
		public DataStoreVirtualCollection(IDataStore<T> store)
		{
			this.store = store;
		}

		#region IList<T> Members

		/// <summary>
		/// Determines the index of a specific item in the collection.
		/// </summary>
		/// <returns>The index of the item if found, or -1 if not found</returns>
		/// <param name="item">Item to find the index</param>
		public int IndexOf(T item)
		{
			return IndexOf(item);
		}

		/// <summary>
		/// Inserts an item at the specified index. This collection is read-only so this throws an exception.
		/// </summary>
		/// <param name="index">Index to add the item</param>
		/// <param name="item">Item to add</param>
		public void Insert(int index, T item)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		/// <summary>
		/// Removes the item at the specified index. This collection is read-only so this throws an exception.
		/// </summary>
		/// <param name="index">Index of the item to remove</param>
		public void RemoveAt(int index)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		/// <summary>
		/// Gets or sets the item at the specified index. This collection is read-only so setting the item throws an exception.
		/// </summary>
		/// <param name="index">Index.</param>
		public T this [int index]
		{
			get { return store[index]; }
			set
			{
				throw new NotSupportedException(ReadOnlyErrorMsg);
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Adds an item to the current collection. This collection is read-only so this throws an exception.
		/// </summary>
		/// <param name="item">The item to add to the current collection.</param>
		public void Add(T item)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		/// <summary>
		/// Clears all items from the collection. This collection is read-only so this throws an exception.
		/// </summary>
		public void Clear()
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		/// <Docs>The object to locate in the current collection.</Docs>
		/// <para>Determines whether the current collection contains a specific value.</para>
		/// <summary>
		/// Determines whether the current collection contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the current collection.</param>
		public bool Contains(T item)
		{
			return (IndexOf(item) != -1);
		}

		/// <summary>
		/// Copies the contents of the collection to the specified array starting at the specified index
		/// </summary>
		/// <param name="array">Array to copy to</param>
		/// <param name="arrayIndex">Index in the array to start copying to</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < Count; i++)
			{
				array[arrayIndex + i] = this[i];
			}
		}

		/// <summary>
		/// Gets the count of items in this collection
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return store == null ? 0 : store.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// Remove the specified item. This collection is read-only so this throws an exception.
		/// </summary>
		/// <param name="item">Item to remove</param>
		public bool Remove(T item)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Gets the enumerator for the collection
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return new DataStoreEnumerator(this);
		}

		#endregion

		#region IList Members

		/// <summary>
		/// Adds an item to the current collection.
		/// </summary>
		/// <param name="value">The item to add to the current collection</param>
		public int Add(object value)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		/// <summary>
		/// Determines whether the current collection contains a specific value.
		/// </summary>
		/// <param name="value">The object to locate in the current collection.</param>
		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines the index of a specific item in the current instance.
		/// </summary>
		/// <returns>Index of the item if found, or -1 if not in the collection</returns>
		/// <param name="value">Value to find</param>
		public int IndexOf(object value)
		{
			int count = store.Count;
			for (int index = 0; index < count; ++index)
			{
				if (store[index].Equals(value))
					return index;
			}
			return -1;
		}

		/// <summary>
		/// Insert a value into the collection with the specified index
		/// </summary>
		/// <param name="index">Index to add the item</param>
		/// <param name="value">Value to add</param>
		public void Insert(int index, object value)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is fixed size.
		/// </summary>
		/// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
		public bool IsFixedSize
		{
			get { return true; }
		}

		/// <summary>
		/// Removes the first occurrence of an item from the current collection.
		/// </summary>
		/// <param name="value">The item to remove from the current collection.</param>
		public void Remove(object value)
		{
			throw new NotSupportedException(ReadOnlyErrorMsg);
		}

		object IList.this [int index]
		{
			get { return this[index]; }
			set
			{
				throw new NotSupportedException(ReadOnlyErrorMsg);
			}
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// Copies the contents of the collection to the specified array starting at the specified index
		/// </summary>
		/// <param name="array">Array to copy to</param>
		/// <param name="index">Index in the array to start copying to</param>
		public void CopyTo(Array array, int index)
		{
			for (int i = 0; i < Count; i++)
			{
				array.SetValue(this[i], index + i);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is synchronized.
		/// </summary>
		/// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the sync root.
		/// </summary>
		/// <value>The sync root.</value>
		public object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DataStoreEnumerator(this);
		}

		#endregion

		#region Internal IEnumerator implementation

		class DataStoreEnumerator : IEnumerator<T>
		{
			readonly DataStoreVirtualCollection<T> collection;
			int cursor;

			public DataStoreEnumerator(DataStoreVirtualCollection<T> collection)
			{
				this.collection = collection;
				this.cursor = -1;
			}

			#region IEnumerator<T> Members

			public T Current
			{
				get { return collection[cursor]; }
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				cursor++;
				return cursor != collection.Count;
			}

			public void Reset()
			{
				cursor = -1;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion
		}

		#endregion
	}
}
