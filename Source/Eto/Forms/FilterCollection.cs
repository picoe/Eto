using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	#region Obsolete

	/// <summary>
	/// Used as a hook to obsolete functionality in <see cref="GridView.Filter"/> and <see cref="GridView.SortComparer"/>.
	/// Remove when obsolete code is removed.
	/// </summary>
	interface IFilterableSource<in T>
	{
		Func<T, bool> Filter { get; }

		Comparison<T> Sort { get; }
	}

	/// <summary>
	/// Used as a hook to obsolete functionality in <see cref="GridView.Filter"/> and <see cref="GridView.SortComparer"/>
	/// Remove when obsolete code is removed.
	/// </summary>
	interface IFilterable<out T>
	{
		Func<T, bool> Filter { set; }

		Comparison<T> Sort { set; }
	}

	#endregion

	public interface ISelectionPreserver : IDisposable
	{
		IEnumerable<object> SelectedItems { get; set; }
	}

	/// <summary>
	/// Interface for an object that can select multiple items and rows.
	/// </summary>
	/// <seealso cref="SelectableFilterCollection{T}"/>
	public interface ISelectable<out T>
	{
		IEnumerable<T> SelectedItems { get; }

		IEnumerable<int> SelectedRows { get; set; }

		void SelectRow(int row);

		void UnselectRow(int row);

		void SelectAll();

		void UnselectAll();
	}

	public interface ISelectableControl<out T> : ISelectable<T>
	{
		Func<ISelectionPreserver> SelectionPreserver { get; }

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

		public ISelectableControl<object> Parent { get; private set; }

		public SelectableFilterCollection(ISelectableControl<object> parent, IList<T> collection)
			: base(collection)
		{
			Initialize(parent);
		}

		public SelectableFilterCollection(ISelectableControl<object> parent, IEnumerable<T> collection)
			: base(collection)
		{
			Initialize(parent);
		}

		public SelectableFilterCollection(ISelectableControl<object> parent)
		{
			Initialize(parent);
		}

		void Initialize(ISelectableControl<object> parent)
		{
			Parent = parent;
			Parent.SelectedItemsChanged += HandleSelectionChanged;
			Change = () => new SelectionPreserverHelper { Collection = this, Preserver = Parent.SelectionPreserver() };
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

		public event EventHandler<EventArgs> SelectionChanged;

		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}

		public IEnumerable<T> SelectedItems
		{
			get { return selectedAll ? Items : selectedItems ?? Enumerable.Empty<T>(); }
		}

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

		public void SelectRow(int row)
		{
			if (HasFilterOrSort)
			{
				if (!selectedAll)
				{
					var item = Items[row];
					selectedItems.Add(item);
					int viewRow;
					if (viewToModel.TryGetValue(item, out row) && modelToView.TryGetValue(row, out viewRow))
						Parent.SelectRow(viewRow);
				}
			}
			else
				Parent.SelectRow(row);
		}

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

		public void UnselectAll()
		{
			selectedItems = null;
			selectedAll = false;
			Parent.UnselectAll();
		}

		public void SelectAll()
		{
			selectedAll = true;
			selectedItems = null;
			Parent.SelectAll();
		}

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

		public override void RemoveAt(int index)
		{
			RemoveSelectedItem(this[index]);
			base.RemoveAt(index);
		}
	}

	/// <summary>
	/// Collection that supports filtering and sorting
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FilterCollection<T> : IList<T>, IList, INotifyCollectionChanged, IFilterableSource<T>, IFilterable<T>
	{
		List<T> items;
		List<T> filtered;
		Func<T, bool> filter;
		SortComparer sort;
		CollectionHandler changedHandler;

		public IList<T> Items { get { return items; } }

		protected bool HasFilterOrSort { get { return filtered != null; } }

		class SortComparer : IComparer<T>
		{
			public Comparison<T> Comparison { get; set; }

			public int Compare(T x, T y)
			{
				return Comparison(x, y);
			}
		}

		public Func<IDisposable> Change { get; set; }

		protected IDisposable CreateChange()
		{
			return Change != null ? Change() : null;
		}

		public Func<T, bool> Filter
		{
			get { return filter; }
			set
			{
				using (CreateChange())
				{
					filter = value;
					Rebuild();
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public Comparison<T> Sort
		{
			get { return sort != null ? sort.Comparison : null; }
			set
			{
				using (CreateChange())
				{
					sort = value != null ? new SortComparer { Comparison = value } : null;
					Rebuild();
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, e);
		}

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

		public FilterCollection(IEnumerable<T> collection)
		{
			var changed = collection as INotifyCollectionChanged;
			if (changed != null)
			{
				changedHandler = new CollectionHandler { List = this };
				changedHandler.Register(collection);
			}
			else
				items = new List<T>(collection);
		}

		protected class CollectionHandler : EnumerableChangedHandler<T>
		{
			public FilterCollection<T> List { get; set; }

			protected override void InitializeCollection()
			{
				List.items = Collection != null ? new List<T>(Collection) : new List<T>();
			}

			public override void AddRange(IEnumerable<T> items)
			{
				List.AddRange(items);
			}

			public override void InsertRange(int index, IEnumerable<T> items)
			{
				List.InsertRange(index, items);
			}

			public override void AddItem(T item)
			{
				List.Add(item);
			}

			public override void InsertItem(int index, T item)
			{
				List.Insert(index, item);
			}

			public override void RemoveItem(int index)
			{
				List.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				List.Clear();
			}
		}

		public FilterCollection()
		{
			items = new List<T>();
		}

		public void AddRange(IEnumerable<T> items)
		{
			if (filtered != null)
				InsertRange(Count, items);
			else
			{
				using (CreateChange())
				{
					this.items.AddRange(items);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public void InsertRange(int index, IEnumerable<T> items)
		{
			using (CreateChange())
			{
				if (filtered != null)
				{
					var enumerable = items as IList<T> ?? items.ToArray();

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
							this.items.InsertRange(itemsIndex, items);
						}
						else
						{
							this.items.InsertRange(0, items);
						}

						if (sort != null)
							index = filtered.IndexOf((T)enumerable[0]);
						this.items.InsertRange(index, items);
						Rebuild();
						var insertIndex = filtered.IndexOf(firstItem);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, insertIndex));
					}
					else if (enumerable.Count > 0)
					{
						if (sort != null)
							index = filtered.IndexOf((T)enumerable[0]);
						this.items.InsertRange(index, items);
						Rebuild();
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					}
				}
				else
				{
					this.items.InsertRange(index, items);
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

		public virtual void Insert(int index, T item)
		{
			using (CreateChange())
			{
				if (filtered != null)
				{
					bool matchesFilter = filter == null || filter(item);
					bool directInsert = sort == null && matchesFilter;
					if (index > 0)
					{
						var beforeItem = filtered[index - 1];
						var itemsIndex = items.IndexOf(beforeItem);
						items.Insert(itemsIndex, item);
						if (directInsert)
							filtered.Insert(index, item);
					}
					else
					{
						items.Insert(0, item);
						if (directInsert)
							filtered.Insert(0, item);
					}
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
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
				}
			}
		}

		public virtual void RemoveAt(int index)
		{
			using (CreateChange())
			{
				if (filtered != null)
				{
					var item = filtered[index];
					filtered.RemoveAt(index);
					items.Remove(item);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				}
				else
				{
					var item = items[index];
					items.RemoveAt(index);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				}
			}
		}

		public virtual T this [int index]
		{
			get { return filtered != null ? filtered[index] : items[index]; }
			set
			{
				using (CreateChange())
				{
					var oldItem = this[index];
					if (filtered != null)
					{
						var itemsIndex = items.IndexOf(filtered[index]);
						items[itemsIndex] = value;
						Rebuild();
						index = filtered.IndexOf(value);
					}
					else
						items[index] = value;
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
				}
			}
		}

		#endregion

		#region ICollection<T> implementation

		public virtual void Add(T item)
		{
			using (CreateChange())
			{
				items.Add(item);

				Rebuild();
				var index = (filtered != null ? filtered.Count : items.Count) - 1;
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
			}
		}

		public virtual void Clear()
		{
			using (CreateChange())
			{
				items.Clear();
				if (filtered != null)
					filtered.Clear();
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		public bool Contains(T item)
		{
			return filtered != null ? filtered.Contains(item) : items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (filtered != null)
				filtered.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(T item)
		{
			using (CreateChange())
			{
				var index = IndexOf(item);
				if (filtered != null)
					filtered.Remove(item);
				var removed = items.Remove(item);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				return removed;
			}
		}

		public int Count
		{
			get { return filtered != null ? filtered.Count : items.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((IList<T>)items).IsReadOnly; }
		}

		#endregion

		#region IEnumerable<T> implementation

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
			return Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		void IList.Remove(object value)
		{
			Remove((T)value);
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
			throw new NotImplementedException();
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

		#region IFilterable implementation

		/*
		Func<object, bool> IFilterable.Filter
		{
			get { return Filter != null ? new Func<object, bool>(o => Filter((T)o)) : null; }
			set
			{
				Filter = o => value(o);
			}
		}


		Comparison<object> IFilterable.Sort2
		{
			get
			{
				return Sort != null ? new Comparison<object>((x,y) => Sort((T)x, (T)y)) : null;
			}
			set
			{
				Sort = value;
			}
		}*/


		#endregion

	}
}

