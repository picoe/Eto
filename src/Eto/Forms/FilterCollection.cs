using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for a control that can preserve selected items
	/// </summary>
	/// <remarks>
	/// A selection preserver is used to save and restore the selected items when the data store changes
	/// dramatically after a sort or server-side filtering.
	/// Controls should implement <see cref="ISelectableControl{T}"/> to create instances of the selection preserver.
	/// </remarks>
	public interface ISelectionPreserver : IDisposable
	{
		/// <summary>
		/// Gets or sets the selected items in the control.
		/// </summary>
		/// <value>The selected items.</value>
		IEnumerable<object> SelectedItems { get; set; }
	}

	/// <summary>
	/// Interface for an object that can select multiple items and rows.
	/// </summary>
	/// <seealso cref="SelectableFilterCollection{T}"/>
	public interface ISelectable<out T>
	{
		/// <summary>
		/// Gets the selected items.
		/// </summary>
		/// <value>The selected items.</value>
		IEnumerable<T> SelectedItems { get; }

		/// <summary>
		/// Gets or sets the selected rows.
		/// </summary>
		/// <value>The selected rows.</value>
		IEnumerable<int> SelectedRows { get; set; }

		/// <summary>
		/// Selects the specified <paramref name="row"/>.
		/// </summary>
		/// <param name="row">Row to select.</param>
		void SelectRow(int row);

		/// <summary>
		/// Unselects the specified <paramref name="row"/>.
		/// </summary>
		/// <param name="row">Row to unselected.</param>
		void UnselectRow(int row);

		/// <summary>
		/// Selects all rows represented in the data store.
		/// </summary>
		void SelectAll();

		/// <summary>
		/// Unselects all rows.
		/// </summary>
		void UnselectAll();
	}

	/// <summary>
	/// Interface for a control that can preserve its selection.
	/// </summary>
	public interface ISelectableControl<out T> : ISelectable<T>
	{
		/// <summary>
		/// Gets a new instance of a selection preserver.
		/// </summary>
		/// <remarks>
		/// This returns a selection preserver that can be used to save the selected items of a control.
		/// Typically, this is used when the selection may encompass items that are not visible in the control.
		/// </remarks>
		/// <value>The selection preserver.</value>
		ISelectionPreserver SelectionPreserver { get; }

		/// <summary>
		/// Occurs when the <see cref="ISelectable{T}.SelectedItems"/> is changed.
		/// </summary>
		event EventHandler<EventArgs> SelectedItemsChanged;
	}

	/// <summary>
	/// Collection that can filter/sort the items, and keep a selection of items in the original list. 
	/// </summary>
	/// <remarks>
	/// This collection is useful when you want the selection to act independant of the filter.
	/// This class will keep the selected items based on the original list, not based on the filtered view.
	/// 
	/// For example, if you select an item and set the filter that eliminates the item from the view, 
	/// the <see cref="SelectedItems"/> will still return the same selected item until it is unselected from the view
	/// or from this class directly.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SelectableFilterCollection<T> : FilterCollection<T>, ISelectable<T>
	{
		bool selectedAll;
		bool suppressSelectionChanged;
		List<T> selectedItems;
		Dictionary<T, int> viewToModel;
		Dictionary<int, int> modelToView;

		/// <summary>
		/// Gets the parent that this collection is attached to.
		/// </summary>
		/// <value>The parent selectable control.</value>
		public ISelectableControl<object> Parent { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SelectableFilterCollection{T}"/> class.
		/// </summary>
		/// <param name="parent">Parent to attach this instance to.</param>
		/// <param name="collection">Collection for the source of this collection.</param>
		public SelectableFilterCollection(ISelectableControl<object> parent, IList<T> collection)
			: base(collection)
		{
			Initialize(parent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SelectableFilterCollection{T}"/> class.
		/// </summary>
		/// <param name="parent">Parent to attach this instance to.</param>
		/// <param name="collection">Collection for the source of this collection.</param>
		public SelectableFilterCollection(ISelectableControl<object> parent, IEnumerable<T> collection)
			: base(collection)
		{
			Initialize(parent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SelectableFilterCollection{T}"/> class.
		/// </summary>
		/// <param name="parent">Parent to attach this instance to.</param>
		public SelectableFilterCollection(ISelectableControl<object> parent)
		{
			Initialize(parent);
		}

		void Initialize(ISelectableControl<object> parent)
		{
			Parent = parent;
			Parent.SelectedItemsChanged += HandleSelectionChanged;
			Change = () => new SelectionPreserverHelper { Collection = this, Preserver = Parent.SelectionPreserver };
		}

		class SelectionPreserverHelper : IDisposable
		{
			public SelectableFilterCollection<T> Collection { get; set; }

			public ISelectionPreserver Preserver { get; set; }

			public void Dispose()
			{
				Collection.suppressSelectionChanged = true;
				Preserver.SelectedItems = Collection.SelectedItems.OfType<object>();
				Preserver.Dispose();
				Collection.suppressSelectionChanged = false;
			}
		}

		/// <summary>
		/// Rebuilds the filtered/sorted view of this collection
		/// </summary>
		protected override void Rebuild()
		{
			base.Rebuild();

			// build a lookup from model to filtered view
			if (HasFilterOrSort)
			{
				viewToModel = new Dictionary<T, int>(Count);
				for (int i = 0; i < Count; i++)
				{
					viewToModel.Add(this[i], i);
				}

				modelToView = new Dictionary<int, int>(Count);
				for (int i = 0; i < Items.Count; i++)
				{
					int index;
					if (viewToModel.TryGetValue(Items[i], out index))
						modelToView.Add(i, index);
				}
			}
			else
			{
				viewToModel = null;
				modelToView = null;
			}
		}

		void HandleSelectionChanged(object sender, EventArgs e)
		{
			if (!suppressSelectionChanged)
			{
				selectedItems = Parent.SelectedItems.OfType<T>().ToList();
				OnSelectionChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Occurs when the selection changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectionChanged;

		/// <summary>
		/// Raises the <see cref="SelectionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		/// <value>The selected items.</value>
		public IEnumerable<T> SelectedItems
		{
			get { return selectedAll ? Items : selectedItems ?? Enumerable.Empty<T>(); }
		}

		/// <summary>
		/// Gets or sets the selected rows in the underlying list.
		/// </summary>
		/// <value>The selected rows in the underlying list.</value>
		public IEnumerable<int> SelectedRows
		{
			get
			{
				if (HasFilterOrSort)
				{
					if (selectedAll)
						return Enumerable.Range(0, Items.Count);
					if (selectedItems != null)
						return selectedItems.Select(r => viewToModel[r]);
					return Enumerable.Empty<int>();
				}
				return Parent.SelectedRows;
			}
			set
			{
				if (HasFilterOrSort)
				{
					selectedItems = value.Select(r => Items[r]).ToList();
					selectedAll = false;
				}
				else
					Parent.SelectedRows = value;
			}
		}

		/// <summary>
		/// Selects the specified <paramref name="row"/> in the underlying list.
		/// </summary>
		/// <param name="row">Row to select.</param>
		public void SelectRow(int row)
		{
			if (HasFilterOrSort)
			{
				if (!selectedAll)
				{
					var item = Items[row];
					if (selectedItems == null)
					{
						selectedItems = new List<T>() { item };
					}
					else
					{
						selectedItems.Add(item);
					}

					int viewRow;
					if (viewToModel.TryGetValue(item, out row) && modelToView.TryGetValue(row, out viewRow))
						Parent.SelectRow(viewRow);
				}
			}
			else
				Parent.SelectRow(row);
		}

		/// <summary>
		/// Unselects the specified <paramref name="row"/> in the underlying list.
		/// </summary>
		/// <param name="row">Row to unselect.</param>
		public void UnselectRow(int row)
		{
			selectedAll = false;
			if (HasFilterOrSort)
			{
				if (selectedAll)
					selectedItems = new List<T>(Items);
				if (selectedItems != null)
					selectedItems.Remove(Items[row]);
				int viewRow;
				if (modelToView.TryGetValue(row, out viewRow))
					Parent.UnselectRow(viewRow);
			}
			else
				Parent.UnselectRow(row);
		}

		/// <summary>
		/// Unselects all rows.
		/// </summary>
		public void UnselectAll()
		{
			selectedItems = null;
			selectedAll = false;
			Parent.UnselectAll();
		}

		/// <summary>
		/// Selects all rows in the underlying list.
		/// </summary>
		public void SelectAll()
		{
			selectedAll = true;
			selectedItems = null;
			Parent.SelectAll();
		}

		/// <summary>
		/// Clear this collection.
		/// </summary>
		public override void Clear()
		{
			if (selectedAll || (selectedItems != null && selectedItems.Count > 0))
			{
				selectedAll = false;
				selectedItems = null;
				OnSelectionChanged(EventArgs.Empty);
			}
			base.Clear();
		}

		/// <summary>
		/// Remove the specified item from this collection.
		/// </summary>
		/// <param name="item">Item to remove.</param>
		public override bool Remove(T item)
		{
			RemoveSelectedItem(item);
			return base.Remove(item);
		}

		void RemoveSelectedItem(T item)
		{
			if (selectedItems != null && selectedItems.Contains(item))
			{
				selectedItems.Remove(item);
				OnSelectionChanged(EventArgs.Empty);
			}
			else if (selectedAll)
			{
				selectedItems = new List<T>(Items);
				selectedItems.Remove(item);
				selectedAll = false;
				OnSelectionChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Removes the item at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">Index of the item to remove in this collection.</param>
		public override void RemoveAt(int index)
		{
			RemoveSelectedItem(this[index]);
			base.RemoveAt(index);
		}

		/// <inheritdoc/>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && selectedItems != null)
			{
				// removing an item from this collection directly, trigger selection changed if needed
				foreach (var item in e.OldItems)
				{
					if (selectedItems.Contains((T)item))
					{
						OnSelectionChanged(EventArgs.Empty);
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Collection that supports filtering and sorting
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FilterCollection<T> : IList<T>, IList, INotifyCollectionChanged
	{
		List<T> items;
		List<T> filtered;
		Func<T, bool> filter;
		Comparison<T> sort;
		EnumerableChangedHandler<T> changedHandler;
		IList<T> externalList;
		bool updating;
		UpdateObject updateObject;

		/// <summary>
		/// Gets the underlying list of items that the filtered collection is based off.
		/// If you change this list, you must call <see cref="Rebuild"/> to update the filtered collection.
		/// </summary>
		/// <value>The underlying items.</value>
		protected IList<T> Items { get { return items; } }

		/// <summary>
		/// Gets a value indicating whether this instance has filtering or sorting.
		/// </summary>
		/// <value><c>true</c> if this instance is filtered or sorted; otherwise, <c>false</c>.</value>
		protected bool HasFilterOrSort { get { return filtered != null; } }

		/// <summary>
		/// Gets or sets the delegate to create a change object each time the collection is filtered.
		/// </summary>
		/// <remarks>
		/// This is used so you can perform operations before or after a change has been made to the collection.
		/// The return value of the delegate is disposed after the change, allowing the object to perform any operations
		/// afterwards.
		/// </remarks>
		/// <value>The change delegate.</value>
		public Func<IDisposable> Change { get; set; }

		/// <summary>
		/// Creates a change object before any change is made to the filter collection.
		/// </summary>
		/// <remarks>
		/// This should be disposed after the change is completed.
		/// </remarks>
		/// <returns>The change instance.</returns>
		protected IDisposable CreateChange()
		{
			return Change != null ? Change() : null;
		}

		/// <summary>
		/// Gets or sets the filter delegate for items in this collection.
		/// </summary>
		/// <remarks>
		/// This will update this collection to contain only items that match the specified filter from the underlying list.
		/// This triggers a collection changed event, so any control that is using this collection as its data store should
		/// automatically update to show the new results.
		/// </remarks>
		/// <value>The filter delegate.</value>
		public Func<T, bool> Filter
		{
			get { return filter; }
			set
			{
				using (CreateChange())
				{
					filter = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the sort.
		/// </summary>
		/// <value>The sort.</value>
		public Comparison<T> Sort
		{
			get { return sort; }
			set
			{
				using (CreateChange())
				{
					sort = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Occurs when the collection is changed.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Raises the <see cref="CollectionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, e);
		}

		/// <summary>
		/// Rebuilds the filtered/sorted view of this collection
		/// </summary>
		protected virtual void Rebuild()
		{
			if (filter != null || sort != null)
			{
				filtered = filter == null ? new List<T>(items) : items.Where(filter).ToList();
				if (sort != null)
					filtered.Sort(sort);
			}
			else
				filtered = null;
		}

		/// <summary>
		/// Refreshes the list by applying the filter and sort to the contained items
		/// </summary>
		/// <remarks>
		/// This is useful when the state of the items change, or your filter delegate is dynamic and isn't set
		/// each time it changes.
		/// </remarks>
		public void Refresh()
		{
			Rebuild();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FilterCollection{T}"/> class with the specified <paramref name="collection"/>.
		/// </summary>
		/// <param name="collection">Collection of items as the source of this collection.</param>
		public FilterCollection(IEnumerable<T> collection)
		{
			var changed = collection as INotifyCollectionChanged;
			if (changed != null)
			{
				items = collection.ToList();
				changedHandler = new CollectionHandler { List = this };
				changedHandler.Register(collection);
			}
			else
			{
				items = new List<T>(collection);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FilterCollection{T}"/> class with the specified <paramref name="list"/>
		/// which will keep in sync with any changes to the filtered collection.
		/// </summary>
		/// <param name="list">List to keep in sync with this filtered collection.</param>
		public FilterCollection(IList<T> list)
			: this((IEnumerable<T>)list)
		{
			this.externalList = list;
		}

		void InsertItemRange(int index, IEnumerable<T> collection)
		{
			var collectionList = collection.ToList();
			items.InsertRange(index, collectionList);
			if (externalList != null)
			{
				foreach (var item in collectionList)
				{
					externalList.Insert(index++, item);
				}
			}
		}

		class UpdateObject : IDisposable
		{
			FilterCollection<T> parent;
			IDisposable change;
			public void Dispose()
			{
				parent.updating = false;
				change?.Dispose();
				change = null;
			}

			public UpdateObject(FilterCollection<T> parent)
			{
				this.parent = parent;
			}

			public void Start()
			{
				parent.updating = true;
				change = parent.CreateChange();
			}
		}

		IDisposable Update()
		{
			if (updateObject == null)
				updateObject = new UpdateObject(this);
			updateObject.Start();
			return updateObject;
		}

		class CollectionHandler : EnumerableChangedHandler<T>
		{
			public FilterCollection<T> List { get; set; }

			protected override void InitializeCollection()
			{
			}

			public override void AddRange(IEnumerable<T> items)
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					List.items.AddRange(items);
					List.Refresh();
				}
			}

			public override void InsertRange(int index, IEnumerable<T> items)
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					List.items.InsertRange(index, items);
					List.Refresh();
				}
			}

			public override void AddItem(T item)
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					List.items.Add(item);
					var matches = List.filter?.Invoke(item) != false;
					if (matches)
					{
						List.Rebuild();
						var filteredIndex = List.filtered?.IndexOf(item) ?? List.items.Count - 1;
						if (filteredIndex >= 0)
							List.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, filteredIndex));
					}
				}
			}

			public override void InsertItem(int index, T item)
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					List.items.Insert(index, item);
					var matches = List.filter?.Invoke(item) != false;
					if (matches)
					{
						List.Rebuild();

						var filteredIndex = List.filtered?.IndexOf(item) ?? index;
						if (filteredIndex >= 0)
							List.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, filteredIndex));
					}
				}
			}

			public override void RemoveItem(int index)
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					var item = List.items[index];
					var filteredIndex = List.filtered?.IndexOf(item) ?? index;

					List.items.RemoveAt(index);
					if (filteredIndex >= 0)
					{
						List.Rebuild();
						List.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, filteredIndex));
					}
				}
			}

			public override void RemoveAllItems()
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					List.items.Clear();
					List.filtered?.Clear();
					List.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}

			public override void Reset()
			{
				if (List.updating)
					return;
				using (List.CreateChange())
				{
					List.items.Clear();
					if (Collection != null)
				    	List.items.AddRange(Collection);
					List.Refresh();
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FilterCollection{T}"/> class.
		/// </summary>
		public FilterCollection()
		{
			items = new List<T>();
		}

		/// <summary>
		/// Adds the specified <paramref name="items"/> to the collection.
		/// </summary>
		/// <remarks>
		/// Any item that does not match the existing <see cref="Filter"/> will be only added to the underlying collection
		/// and not be visible in the filtered collection.
		/// </remarks>
		/// <param name="items">Items to add to the collection.</param>
		public void AddRange(IEnumerable<T> items)
		{
			if (filtered != null)
				InsertRange(Count, items);
			else
			{
				using (Update())
				{
					var itemList = items.ToList();
					this.items.AddRange(itemList);
					if (externalList != null)
					{
						foreach (var item in itemList)
						{
							externalList.Add(item);
						}
					}
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		/// <summary>
		/// Inserts the <paramref name="items"/> to the collection at the specified <paramref name="index"/>.
		/// </summary>
		/// <remarks>
		/// Any item that does not match the existing <see cref="Filter"/> will be only added to the underlying collection
		/// and not be visible in the filtered collection.
		/// </remarks>
		/// <param name="index">Index to start adding the items.</param>
		/// <param name="items">Items to add to the collection.</param>
		public void InsertRange(int index, IEnumerable<T> items)
		{
			using (Update())
			{
				if (filtered != null)
				{
					var enumerable = items as IList<T> ?? items.ToList();

					if (sort == null || enumerable.Count == 1)
					{
						T firstItem;
						if (filter != null)
						{
							var newItems = enumerable.Where(filter).ToArray();
							if (newItems.Length == 0)
								return; // filtered list did not change!

							firstItem = newItems[0];
							items = newItems;
						}
						else
							firstItem = (T)enumerable[0];

						if (index > 0)
						{
							var beforeItem = filtered[index - 1];
							var itemsIndex = this.items.IndexOf(beforeItem);
							InsertItemRange(itemsIndex, items);
						}
						else
						{
							InsertItemRange(0, items);
						}

						if (sort != null)
							index = filtered.IndexOf((T)enumerable[0]);
						InsertItemRange(index, items);
						Rebuild();
						var insertIndex = filtered.IndexOf(firstItem);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, insertIndex));
					}
					else if (enumerable.Count > 0)
					{
						if (sort != null)
							index = filtered.IndexOf((T)enumerable[0]);
						InsertItemRange(index, items);
						Refresh();
					}
				}
				else
				{
					InsertItemRange(index, items);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		#region IList<T> implementation

		/// <summary>
		/// Determines the index of a specific item in the collection.
		/// </summary>
		/// <returns>Index of the item if found, or -1 if not found.</returns>
		/// <param name="item">Item to find the index of.</param>
		public int IndexOf(T item)
		{
			return filtered != null ? filtered.IndexOf(item) : items.IndexOf(item);
		}

		/// <summary>
		/// Insert the item at the specified index.
		/// </summary>
		/// <param name="index">Index to insert at.</param>
		/// <param name="item">Item to insert.</param>
		public virtual void Insert(int index, T item)
		{
			using (Update())
			{
				if (filtered != null)
				{
					bool matchesFilter = filter == null || filter(item);
					bool directInsert = sort == null && matchesFilter;

					// insert item in the backing list before the item at the specified index.
					var beforeItem = filtered[index];
					var itemsIndex = items.IndexOf(beforeItem);
					items.Insert(itemsIndex, item);
					if (externalList != null)
						externalList.Insert(itemsIndex, item);
					if (directInsert)
						filtered.Insert(index, item);
					
					if (!directInsert)
						Rebuild();
					if (matchesFilter)
					{
						if (sort != null)
							index = filtered.IndexOf(item);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
					}
				}
				else
				{
					items.Insert(index, item);
					if (externalList != null)
						externalList.Insert(index, item);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
				}
			}
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		/// <param name="index">Index of the item to remove.</param>
		public virtual void RemoveAt(int index)
		{
			using (Update())
			{
				if (filtered != null)
				{
					var item = filtered[index];
					filtered.RemoveAt(index);
					items.Remove(item);
					if (externalList != null)
						externalList.Remove(item);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				}
				else
				{
					var item = items[index];
					items.RemoveAt(index);
					if (externalList != null)
						externalList.RemoveAt(index);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="FilterCollection{T}"/> at the specified index.
		/// </summary>
		/// <param name="index">Index of the item to get/set.</param>
		public virtual T this [int index]
		{
			get { return filtered != null ? filtered[index] : items[index]; }
			set
			{
				using (Update())
				{
					var oldIndex = index;
					var oldItem = this[index];
					if (filtered != null)
					{
						var itemsIndex = items.IndexOf(filtered[index]);
						items[itemsIndex] = value;
						if (externalList != null)
							externalList[itemsIndex] = value;
						Rebuild();
						index = filtered.IndexOf(value);
					}
					else
					{
						items[index] = value;
						if (externalList != null)
							externalList[index] = value;
					}
					if (index == oldIndex)
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
					else
					{
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, oldIndex));
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
					}
				}
			}
		}

		#endregion

		#region ICollection<T> implementation

		/// <summary>
		/// Add the specified item to the collection.
		/// </summary>
		/// <param name="item">Item to add.</param>
		public virtual void Add(T item)
		{
			using (Update())
			{
				items.Add(item);
				if (externalList != null)
					externalList.Add(item);

				Rebuild();
				var index = filtered != null ? filtered.IndexOf(item) : items.Count - 1;
				if (index >= 0)
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
			}
		}

		/// <summary>
		/// Clears the items from the collection.
		/// </summary>
		public virtual void Clear()
		{
			using (Update())
			{
				items.Clear();
				if (externalList != null)
					externalList.Clear();
				if (filtered != null)
					filtered.Clear();
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="item"/> is contained within this collection.
		/// </summary>
		/// <param name="item">Item to find.</param>
		public bool Contains(T item)
		{
			return filtered != null ? filtered.Contains(item) : items.Contains(item);
		}

		/// <summary>
		/// Copies the current filtered collection to the specified array.
		/// </summary>
		/// <param name="array">Array to copy to.</param>
		/// <param name="arrayIndex">Index in the array to start copying to.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (filtered != null)
				filtered.CopyTo(array, arrayIndex);
			else
				items.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove the specified item from the collection.
		/// </summary>
		/// <param name="item">Item to remove.</param>
		public virtual bool Remove(T item)
		{
			using (Update())
			{
				var index = IndexOf(item);
				if (items.Remove(item))
				{
					if (filtered != null)
						filtered.RemoveAt(index);
					if (externalList != null)
						externalList.Remove(item);
					if (index >= 0)
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets the count of items in the collection.
		/// </summary>
		/// <value>The count of items.</value>
		public int Count
		{
			get { return filtered != null ? filtered.Count : items.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether this collection is read only.
		/// </summary>
		/// <value><c>true</c> if this collection is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get { return ((IList<T>)items).IsReadOnly; }
		}

		#endregion

		#region IEnumerable<T> implementation

		/// <summary>
		/// Gets the enumerator for the collection.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return filtered != null ? filtered.GetEnumerator() : items.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IList implementation

		int IList.Add(object value)
		{
			Add((T)value);
			return -1;
		}

		bool IList.Contains(object value)
		{
			if (value is T val)
				return Contains(val);
			if (value == null)
				return Contains(default(T)); // null is valid
			return false;
		}

		int IList.IndexOf(object value)
		{
			if (value is T val)
				return IndexOf(val);
			if (value == null)
				return IndexOf(default(T)); // null is valid
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		void IList.Remove(object value)
		{
			if (value is T val)
				Remove(val);
			if (value == null)
				Remove(default(T)); // null is valid
		}

		bool IList.IsFixedSize
		{
			get { return ((IList)items).IsFixedSize; }
		}

		object IList.this [int index]
		{
			get { return this[index]; }
			set { this[index] = (T)value; }
		}

		#endregion

		#region ICollection implementation

		void ICollection.CopyTo(Array array, int index)
		{
			var collection = filtered ?? items;
			((ICollection)collection).CopyTo(array, index);
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return ((ICollection)items).SyncRoot; }
		}

		#endregion
	}
}

