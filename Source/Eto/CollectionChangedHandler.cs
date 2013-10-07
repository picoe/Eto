using System;
using System.Collections.Specialized;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Eto
{
	/// <summary>
	/// Class to help implement collection changed events on a data store
	/// </summary>
	/// <remarks>
	/// This is used for the platform handler of controls that use collections.
	/// This class helps detect changes to a collection so that the appropriate action
	/// can be taken to update the UI with the changes.
	/// 
	/// This is a simple helper that is much easier to implement than handling
	/// the <see cref="INotifyCollectionChanged.CollectionChanged"/> event directly.
	/// </remarks>
	/// <typeparam name="TItem">Type of the items in the collection</typeparam>
	/// <typeparam name="TCollection">Type of the collection</typeparam>
	public abstract class CollectionChangedHandler<TItem, TCollection>
		where TCollection: class
	{
		/// <summary>
		/// Gets the collection that this handler is observing
		/// </summary>
		public TCollection Collection { get; private set; }
		
		/// <summary>
		/// Called when the object has been registered (attached) to a collection
		/// </summary>
		protected virtual void OnRegisterCollection (EventArgs e)
		{
		}

		/// <summary>
		/// Called when the object has unregistered the collection
		/// </summary>
		protected virtual void OnUnregisterCollection (EventArgs e)
		{
			RemoveAllItems ();
		}
		
		/// <summary>
		/// Registers a specific collection to observe
		/// </summary>
		/// <param name="collection">collection to observe</param>
		/// <returns>true if the collection was registered, false otherwise</returns>
		public bool Register (TCollection collection)
		{
			Collection = collection;
			
			var notify = Collection as INotifyCollectionChanged;
			if (notify != null) {
				notify.CollectionChanged += CollectionChanged;
			}
			OnRegisterCollection (EventArgs.Empty);
			return notify != null;
		}
		
		/// <summary>
		/// Unregisters the current registered collection
		/// </summary>
		public void Unregister ()
		{
			if (Collection == null)
				return;
			
			var notify = Collection as INotifyCollectionChanged;
			if (notify != null) {
				notify.CollectionChanged -= CollectionChanged;
			}
			OnUnregisterCollection (EventArgs.Empty);
			
			Collection = null;
		}

		/// <summary>
		/// Gets the index of the specified item
		/// </summary>
		/// <param name="item">Item to find the index of</param>
		/// <returns>Index of the item if contained in the collection, otherwise -1</returns>
		public virtual int IndexOf (TItem item)
		{
			var list = Collection as IList;
			if (list != null)
				return list.IndexOf(item);
			return InternalIndexOf(item);
		}
		
		/// <summary>
		/// Gets the index of the item from the collection
		/// </summary>
		/// <remarks>
		/// Implementors should implement this to get the index of the item
		/// </remarks>
		/// <param name="item">Item to find the index</param>
		/// <returns>index of the item in the collection, or -1 if the item is not found</returns>
		protected abstract int InternalIndexOf (TItem item);
		
		/// <summary>
		/// Adds the item to the end of the collection
		/// </summary>
		/// <param name="item">Item to add to the collection</param>
		public abstract void AddItem (TItem item);
			
		/// <summary>
		/// Inserts an item at the specified index in the collection
		/// </summary>
		/// <param name="index">Index to insert the item to</param>
		/// <param name="item">Item to insert</param>
		public abstract void InsertItem (int index, TItem item);
			
		/// <summary>
		/// Removes the item at the specified index
		/// </summary>
		/// <param name="index">Index of the item to remove</param>
		public abstract void RemoveItem (int index);

		/// <summary>
		/// Removes all items from the collection
		/// </summary>
		public abstract void RemoveAllItems ();

		/// <summary>
		/// Removes the specified item
		/// </summary>
		/// <remarks>
		/// This will remove the item by finding the index and removing based on index.
		/// Implementors should override this method if there is a faster mechanism to do so.
		/// </remarks>
		/// <param name="item">Item to remove from the collection</param>
		public virtual void RemoveItem (TItem item)
		{
			var index = IndexOf (item);
			if (index >= 0)
				RemoveItem (index);
		}

		/// <summary>
		/// Adds multiple items to the end of the collection
		/// </summary>
		/// <remarks>
		/// This simply calls <see cref="AddItem"/> for each item in the list.  If there
		/// is a faster mechanism for doing so, implementors should override this method.
		/// 
		/// For example, sometimes adding a single item will update the UI for each item, this
		/// should be overridden so the UI is updated after all items have been added.
		/// </remarks>
		/// <param name="items">Enumeration of items to add to the end of the collection</param>
		public virtual void AddRange (IEnumerable<TItem> items)
		{
			foreach (var item in items)
				AddItem (item);
		}
		
		/// <summary>
		/// Inserts multiple items to the specified index in the collection
		/// </summary>
		/// <remarks>
		/// This simply calls <see cref="InsertItem"/> for each item in the list.  If there
		/// is a faster mechanism for doing so, implementors should override this method.
		/// 
		/// For example, sometimes inserting a single item will update the UI for each item, this
		/// should be overridden so the UI is updated after all items have been inserted.
		/// </remarks>
		/// <param name="index">Index to start adding the items</param>
		/// <param name="items">Enumeration of items to add</param>
		public virtual void InsertRange (int index, IEnumerable<TItem> items)
		{
			foreach (var item in items)
				InsertItem (index++, item);
		}

		/// <summary>
		/// Removes a specified count of items from the collection starting at the specified index
		/// </summary>
		/// <remarks>
		/// This simply calls <see cref="RemoveItem(int)"/> for each item to remove.  If there
		/// is a faster mechanism for doing so, implementors should override this method.
		/// 
		/// For example, sometimes removing a single item will update the UI for each item, this
		/// should be overridden so the UI is updated after all items have been removed.
		/// </remarks>
		/// <param name="index">Index to start removing the items from</param>
		/// <param name="count">Number of items to remove</param>
		public virtual void RemoveRange (int index, int count)
		{
			for (int i = 0; i < count; i++)
				RemoveItem (index);
		}

		/// <summary>
		/// Removes the specified items from the collection
		/// </summary>
		/// <remarks>
		/// This simply calls <see cref="RemoveItem(I)"/> for each item to remove.  If there
		/// is a faster mechanism for doing so, implementors should override this method.
		/// 
		/// For example, sometimes removing a single item will update the UI for each item, this
		/// should be overridden so the UI is updated after all items have been removed.
		/// </remarks>
		/// <param name="items">List of items to remove</param>
		public virtual void RemoveRange (IEnumerable<TItem> items)
		{
			foreach (var item in items) 
				RemoveItem (item);
		}

		void CollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				if (e.NewStartingIndex != -1) 
					InsertRange (e.NewStartingIndex, e.NewItems.Cast<TItem> ());
				else
					AddRange (e.NewItems.Cast<TItem> ());
				break;
			case NotifyCollectionChangedAction.Move:
				RemoveRange (e.OldItems.Cast<TItem> ());
				InsertRange (e.NewStartingIndex, e.NewItems.Cast<TItem> ());
				break;
			case NotifyCollectionChangedAction.Remove:
				if (e.OldStartingIndex != -1)
					RemoveRange (e.OldStartingIndex, e.OldItems.Count);
				else
					RemoveRange (e.OldItems.Cast<TItem> ());
				break;
			case NotifyCollectionChangedAction.Replace:
				if (e.OldStartingIndex != -1) {
					RemoveRange (e.OldStartingIndex, e.OldItems.Count);
					InsertRange (e.OldStartingIndex, e.NewItems.Cast<TItem> ());
				} else {
					for (int i = 0; i < e.OldItems.Count; i++) {
						var index = IndexOf ((TItem)e.OldItems [i]);
						RemoveItem (index);
						InsertItem (index, (TItem)e.NewItems [i]);
					}
				}
				break;
			case NotifyCollectionChangedAction.Reset:
				RemoveAllItems ();
				break;
			}
		}
	}

	/// <summary>
	/// Class to help implement change handling on an <see cref="IEnumerable"/>
	/// </summary>
	/// <remarks>
	/// This is used for the platform handler of controls that use collections.
	/// This class helps detect changes to a collection so that the appropriate action
	/// can be taken to update the UI with the changes.
	/// 
	/// Use this class as a base when you only have an <see cref="IEnumerable"/>.  If the object
	/// also implements <see cref="INotifyCollectionChanged"/> it will get changed events
	/// otherwise you must register a new collection each time.
	/// </remarks>
	/// <typeparam name="TItem">Type of each item in the enumerable</typeparam>
	/// <typeparam name="TCollection">Type of collection</typeparam>
	public abstract class EnumerableChangedHandler<TItem, TCollection> : CollectionChangedHandler<TItem, TCollection>
		where TCollection: class, IEnumerable
	{
		/// <summary>
		/// Implements the mechanism for finding the index of an item (the slow way)
		/// </summary>
		/// <remarks>
		/// If the collection object implements <see cref="IList"/>, this will not get called
		/// as it will call it's method of getting the index.  This is used as a fallback.
		/// </remarks>
		/// <param name="item">Item to find in the collection</param>
		/// <returns>Index of the item, or -1 if not found</returns>
		protected override int InternalIndexOf (TItem item)
		{
			int index = 0;
			foreach (var child in Collection) {
				if (object.ReferenceEquals (item, child)) 
					return index;
				index++;
			}
			return -1;
		}
		
		/// <summary>
		/// Called when the collection is registered
		/// </summary>
		protected override void OnRegisterCollection (EventArgs e)
		{
			base.OnRegisterCollection (e);
			AddRange (Collection.Cast<TItem>());
		}
	}
	
	/// <summary>
	/// Class to help implement change handling for a <see cref="IDataStore{T}"/>
	/// </summary>
	/// <remarks>
	/// This is used for the platform handler of controls that use collections.
	/// This class helps detect changes to a collection so that the appropriate action
	/// can be taken to update the UI with the changes.
	/// 
	/// Use this class as a base when you are detecting changes for an <see cref="IDataStore{T}"/>.
	/// If the object also implements <see cref="INotifyCollectionChanged"/>, it will get changed events.
	/// Otherwise, you must register a new collection each time.
	/// </remarks>
	/// <typeparam name="TItem">Type of items in the data store</typeparam>
	/// <typeparam name="TCollection">Type of the data store to detect changes on</typeparam>
	public abstract class DataStoreChangedHandler<TItem, TCollection> : CollectionChangedHandler<TItem, TCollection>, IEnumerable<TItem>
		where TCollection: class, IDataStore<TItem>
	{
		public IEnumerator<TItem> GetEnumerator ()
		{
			return new DataStoreVirtualCollection<TItem>(Collection).GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>
		/// Called when the collection is registered
		/// </summary>
		protected override void OnRegisterCollection (EventArgs e)
		{
			base.OnRegisterCollection (e);
			AddRange (Collection.AsEnumerable ());
		}

		/// <summary>
		/// Implements the mechanism for finding the index of an item (the slow way)
		/// </summary>
		/// <remarks>
		/// If the collection object implements <see cref="IList"/>, this will not get called
		/// as it will call it's method of getting the index.  This is used as a fallback.
		/// </remarks>
		/// <param name="item">Item to find in the collection</param>
		/// <returns>Index of the item, or -1 if not found</returns>
		protected override int InternalIndexOf (TItem item)
		{
			for (int i = 0; i < Collection.Count; i++) {
				if (object.ReferenceEquals (item, Collection [i])) 
					return i;
			}
			return -1;
		}
	}
}

