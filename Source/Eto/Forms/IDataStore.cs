using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;

namespace Eto.Forms
{
	/// <summary>
	/// Base data store interface to bind to a collection of objects of a particular type.
	/// </summary>
	/// <remarks>
	/// Note that you should use an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> if you want
	/// the control to respond to changes of the collection.
	/// </remarks>
	public interface IDataStore<out T>
	{
		/// <summary>
		/// Gets the number of items in this data store.
		/// </summary>
		/// <value>The count of items.</value>
		int Count { get; }

		/// <summary>
		/// Gets the object at the specified index.
		/// </summary>
		/// <param name="index">Index of the item to get.</param>
		T this [int index] { get; }
	}

	/// <summary>
	/// Interface for an object-based data store
	/// </summary>
	[Obsolete("No longer needed, use standard collections instead")]
	public interface IDataStore : IDataStore<object>
	{
	}

	/// <summary>
	/// Extensions for <see cref="IDataStore{T}"/>
	/// </summary>
	[Obsolete("No longer needed/provided")]
	public static class DataStoreExtensions
	{
		/// <summary>
		/// Enumerates all items in a <see cref="IDataStore{T}"/>
		/// </summary>
		/// <returns>The enumerable of all items.</returns>
		/// <param name="store">Data store to enumerate.</param>
		/// <typeparam name="T">The item type in the data store.</typeparam>
		[Obsolete("No longer needed/provided")]
		public static IEnumerable<T> AsEnumerable<T>(this IDataStore<T> store)
		{
			if (store == null)
				yield break;
			for (int i = 0; i < store.Count; i++)
				yield return store[i];
		}
	}

	/// <summary>
	/// Collection of items that implements the <see cref="IDataStore{T}"/> interface
	/// </summary>
	public class DataStoreCollection<T> : ExtendedObservableCollection<T>, IDataStore<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection{T}"/> class.
		/// </summary>
		public DataStoreCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection{T}"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to initialize the collection with.</param>
		public DataStoreCollection(IEnumerable<T> items)
			: base(items)
		{
		}
	}

	/// <summary>
	/// A data store of plain objects, that can be bound to a grid view.
	/// </summary>
	[Obsolete("Use standard colletions instead")]
	public class DataStoreCollection : DataStoreCollection<object>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection"/> class.
		/// </summary>
		public DataStoreCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DataStoreCollection"/> class with the specified items.
		/// </summary>
		/// <param name="items">Items to populate the data store with initially.</param>
		public DataStoreCollection(IEnumerable<object> items)
			: base(items)
		{
		}
	}
}
