namespace Eto.Wpf.Forms.Controls
{
	class EtoGridCollectionView : IList, INotifyCollectionChanged
	{
		IList _collection;
		swc.DataGrid _grid;
		swcp.DataGridRowsPresenter _presenter;

		public static IEnumerable Create(IEnumerable collection, swc.DataGrid grid)
		{
			if (collection is IList list && collection is INotifyCollectionChanged)
				return new EtoGridCollectionView(list, grid);
			return collection;
		}

		public EtoGridCollectionView(IList collection, swc.DataGrid grid)
		{
			_collection = collection;
			if (_collection is INotifyCollectionChanged collectionChanged)
				collectionChanged.CollectionChanged += OnCollectionChanged;
			_grid = grid;
		}

		public void Unregister()
		{
			if (_collection is INotifyCollectionChanged collectionChanged)
				collectionChanged.CollectionChanged -= OnCollectionChanged;
		}

		public object this[int index]
		{
			get => _collection[index];
			set => _collection[index] = value;
		}

		public bool IsReadOnly => _collection.IsReadOnly;
		public bool IsFixedSize => _collection.IsFixedSize;
		public int Count => _collection.Count;
		public object SyncRoot => _collection.SyncRoot;
		public bool IsSynchronized => _collection.IsSynchronized;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int Add(object value) => _collection.Add(value);

		public void Clear() => _collection.Clear();

		public bool Contains(object value) => _collection.Contains(value);

		public void CopyTo(Array array, int index) => _collection.CopyTo(array, index);

		public IEnumerator GetEnumerator() => _collection.GetEnumerator();

		public int IndexOf(object value) => _collection.IndexOf(value);

		public void Insert(int index, object value) => _collection.Insert(index, value);

		public void Remove(object value) => _collection.Remove(value);

		public void RemoveAt(int index) => _collection.RemoveAt(index);

		static readonly FieldInfo s_queueField = typeof(swc.ItemContainerGenerator).GetField("_recyclableContainers", BindingFlags.Instance | BindingFlags.NonPublic);

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Reset && s_queueField != null)
			{
				_presenter ??= _grid.FindChild<swcp.DataGridRowsPresenter>();
				if (_presenter != null)
				{
					var existingRows = new List<sw.DependencyObject>();

					IList list = _presenter.Children;
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is sw.DependencyObject child)
							existingRows.Add(child);
					}
					CollectionChanged?.Invoke(this, args);

					if (s_queueField.GetValue(_grid.ItemContainerGenerator) is Queue<sw.DependencyObject> queue)
					{
						for (int i = 0; i < existingRows.Count; i++)
						{
							queue.Enqueue(existingRows[i]);
						}
						return;
					}
				}
			}
			CollectionChanged?.Invoke(this, args);
		}
	}
}
