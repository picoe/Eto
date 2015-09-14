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
	/// Collection of items that implements the <see cref="IDataStore{T}"/> interface
	/// </summary>
	public class DataStoreCollection<T> : ExtendedObservableCollection<T>
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
}
