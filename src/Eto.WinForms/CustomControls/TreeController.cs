namespace Eto.CustomControls
{
	public interface ITreeHandler
	{
		ITreeGridItem SelectedItem { get; }
		void SelectRow(int row);
		bool AllowMultipleSelection { get; }

		bool PreResetTree(object item = null, int row = -1);
		void PostResetTree(object item = null, int row = -1);
	}

	public class TreeController : TreeSection, IList, INotifyCollectionChanged
	{
		readonly Dictionary<int, ITreeGridItem> cache = new Dictionary<int, ITreeGridItem>();

		public int ExpandResetThreshold { get; set; }
		public int CollapseResetThreshold { get; set; }

		public ITreeHandler Handler { get; set; }

		public event EventHandler<TreeGridViewItemCancelEventArgs> Expanding;
		public event EventHandler<TreeGridViewItemCancelEventArgs> Collapsing;
		public event EventHandler<TreeGridViewItemEventArgs> Expanded;
		public event EventHandler<TreeGridViewItemEventArgs> Collapsed;

		protected virtual void OnExpanding(TreeGridViewItemCancelEventArgs e)
		{
			Expanding?.Invoke(this, e);
		}

		protected virtual void OnCollapsing(TreeGridViewItemCancelEventArgs e)
		{
			Collapsing?.Invoke(this, e);
		}

		protected virtual void OnExpanded(TreeGridViewItemEventArgs e)
		{
			Expanded?.Invoke(this, e);
		}

		protected virtual void OnCollapsed(TreeGridViewItemEventArgs e)
		{
			Collapsed?.Invoke(this, e);
		}

		public override ITreeGridItem this[int row]
		{
			get
			{
				if (cache.TryGetValue(row, out var item))
					return item;
				item = base[row];
				cache[row] = item;
				return item;
			}
		}

		public override void InitializeItems(ITreeGridStore<ITreeGridItem> store)
		{
			base.InitializeItems(store);
			ResetCollection();
		}

		public bool ExpandRow(int row)
		{
			var args = new TreeGridViewItemCancelEventArgs(this[row]);
			OnExpanding(args);
			if (args.Cancel)
				return false;
			args.Item.Expanded = true;
			var controller = ExpandRowInternal(row);
			if (controller != null)
			{
				controller.ClearCache();
				if (controller.Count <= ExpandResetThreshold)
				{
					var preReset = Handler?.PreResetTree() ?? false;
					var startRow = controller.GetRealStartRow();
					for (int i = startRow; i < startRow + controller.Count; i++)
					{
						var item = this[i];
						OnTriggerCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i));
					}
					if (preReset)
						Handler?.PostResetTree();
				}
				else
				{
					ResetCollection();
				}
			}
			else
			{
				ResetCollection();
			}

			OnExpanded(new TreeGridViewItemEventArgs(args.Item));
			return true;
		}

		void ResetCollection()
		{
			var doPost = parent == null && Handler.PreResetTree();
			OnTriggerCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (doPost) Handler.PostResetTree();
		}

		public override void ClearCache()
		{
			cache.Clear();
			base.ClearCache();
		}

		public int IndexOf(ITreeGridItem item)
		{
			if (cache.ContainsValue(item))
			{
				var found = cache.First(r => ReferenceEquals(item, r.Value));
				return found.Key;
			}
			for (int i = 0; i < Count; i++)
			{
				if (ReferenceEquals(this[i], item))
					return i;
			}
			return -1;
		}

		bool ChildIsSelected(ITreeGridItem item)
		{
			var node = Handler.SelectedItem;

			while (node != null)
			{
				node = node.Parent;

				if (object.ReferenceEquals(node, item))
					return true;
			}
			return false;
		}


		public void ReloadItem(ITreeGridItem item)
		{
			var row = IndexOf(item);
			if (row < 0)
				return;
			var doPost = parent == null && Handler.PreResetTree(item, row);
			OnTriggerCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, row));
			OnTriggerCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, row));
			if (doPost) Handler.PostResetTree(item, row);
		}

		public bool CollapseRow(int row)
		{
			var args = new TreeGridViewItemCancelEventArgs(GetItemAtRow(row));
			OnCollapsing(args);
			if (args.Cancel)
				return false;
			var shouldSelect = !Handler.AllowMultipleSelection && ChildIsSelected(args.Item);
			var controller = CollapseSection(row);
			args.Item.Expanded = false;

			if (controller != null && controller.Count <= CollapseResetThreshold)
			{
				var parent = controller.parent;
				var startRow = controller.GetRealStartRow();
				var controllerCountOriginal = controller.Count;
				for (int i = controllerCountOriginal - 1; i >= 0; i--)
				{
					var item = controller[i];
					if (i < controllerCountOriginal - 1)
						cache[startRow] = controller[i + 1];
					else
						cache.Clear();

					// decrement counts
					var parentController = controller;
					while (parentController != null)
					{
						parentController.countCache = parentController.Count - 1;
						parentController = parentController.parent;
					}
					OnTriggerCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, startRow + i));
				}
				parent.Sections.Remove(controller);
				parent.ClearCache();
				ClearCache();
			}
			else
			{
				controller?.parent.Sections.Remove(controller);
				controller?.ClearCache();
				ResetCollection();
			}
			OnCollapsed(new TreeGridViewItemEventArgs(args.Item));

			if (shouldSelect)
				Handler.SelectRow(row);

			return true;
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(value as ITreeGridItem);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{

			}
		}

		public override void ReloadData()
		{
			base.ReloadData();
			ResetCollection();
		}

		public void ExpandToItem(ITreeGridItem value)
		{
			var parents = GetParents(value).Reverse();

			foreach (var item in parents)
			{
				var row = IndexOf(item);
				if (row >= 0 && !IsExpanded(row))
					ExpandRow(row);
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnTriggerCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}
	}
	public class TreeSection : ITreeGridStore<ITreeGridItem>
	{
		internal int? countCache;
		internal List<TreeSection> sections;
		internal TreeSection parent;
		
		internal int StartRow { get; private set; }

		internal List<TreeSection> Sections => sections ??= new List<TreeSection>();

		public ITreeGridStore<ITreeGridItem> Store { get; protected set; }


		internal int GetRealStartRow()
		{
			if (parent == null)
				return StartRow;
			var expandedRowsAbove = parent.Sections.Where(r => r.StartRow < StartRow).Sum(r => r.Count);
			return StartRow + expandedRowsAbove + parent.GetRealStartRow() + 1;
		}

		public virtual void InitializeItems(ITreeGridStore<ITreeGridItem> store)
		{
			ClearCache();
			sections?.Clear();
			Store = store;

			ResetSections();
		}

		protected void ResetSections()
		{
			Sections.Clear();
			if (Store != null)
			{
				for (int row = 0; row < Store.Count; row++)
				{
					var item = Store[row];
					if (item.Expandable && item.Expanded)
					{
						var children = (ITreeGridStore<ITreeGridItem>)item;
						var section = new TreeSection { StartRow = row, parent = this };
						section.InitializeItems(children);
						Sections.Add(section);
					}
				}
			}
		}

		public virtual void ReloadData()
		{
			ClearCache();
			ResetSections();
		}

		public virtual void ClearCache()
		{
			countCache = null;
			parent?.ClearCache();
		}

		public int LevelAtRow(int row)
		{
			if (sections == null || sections.Count == 0)
				return 0;
			foreach (var section in sections)
			{
				if (row <= section.StartRow)
				{
					return 0;
				}
				else
				{
					var count = section.Count;
					if (row <= section.StartRow + count)
					{
						return section.LevelAtRow(row - section.StartRow - 1) + 1;
					}
					row -= count;
				}
			}
			return 0;
		}


		public class TreeNode
		{
			public ITreeGridItem Item { get; set; }
			public int RowIndex { get; set; }
			public int Count { get; set; }
			public int Index { get; set; }
			public int Level { get; set; }
			public TreeNode Parent { get; set; }

			public bool IsFirstNode { get { return Index == 0; } }

			public bool IsLastNode { get { return Index == Count - 1; } }
		}

		public TreeNode GetNodeAtRow(int row)
		{
			return GetNodeAtRow(row, null, 0);
		}

		TreeNode GetNodeAtRow(int row, TreeNode parent, int level)
		{
			var node = new TreeNode { RowIndex = row, Parent = parent, Count = Store.Count, Level = level };
			if (sections == null || sections.Count == 0)
			{
				node.Item = Store[row];
				node.Index = row;
			}
			else
			{
				foreach (var section in sections)
				{
					if (row <= section.StartRow)
					{
						node.Item = Store[row];
						node.Index = row;
						break;
					}
					if (row <= section.StartRow + section.Count)
					{
						node.Index = section.StartRow;
						node.Item = section.Store as ITreeGridItem;
						return section.GetNodeAtRow(row - section.StartRow - 1, node, level + 1);
					}
					row -= section.Count;
				}
			}
			if (node.Item == null && row < Store.Count)
			{
				node.Item = Store[row];
				node.Index = row;
			}
			return node;
		}

		protected ITreeGridItem GetItemAtRow(int row)
		{
			if (Store == null) return null;

			ITreeGridItem item = null;
			if (sections == null || sections.Count == 0)
				item = Store[row];
			if (item == null)
			{
				foreach (var section in sections)
				{
					if (row <= section.StartRow)
					{
						item = Store[row];
						break;
					}
					if (row <= section.StartRow + section.Count)
					{
						item = section.GetItemAtRow(row - section.StartRow - 1);
						break;
					}
					row -= section.Count;
				}
			}
			if (item == null && row < Store.Count)
				item = Store[row];
			return item;
		}

		public bool IsExpanded(int row)
		{
			if (sections == null) return false;
			foreach (var section in sections)
			{
				if (row < section.StartRow)
					return false;
				if (row == section.StartRow)
				{
					return true;
				}
				if (row <= section.StartRow + section.Count)
				{
					return section.IsExpanded(row - section.StartRow - 1);
				}
				row -= section.Count;
			}
			return false;
		}

		protected TreeSection ExpandRowInternal(int row)
		{
			ITreeGridStore<ITreeGridItem> children = null;
			TreeSection childController = null;
			if (sections == null || sections.Count == 0)
			{
				children = (ITreeGridStore<ITreeGridItem>)Store[row];
				childController = new TreeSection { StartRow = row, parent = this };
				childController.InitializeItems(children);
				Sections.Add(childController);
				Sections.Sort((x, y) => x.StartRow.CompareTo(y.StartRow));
			}
			else
			{
				bool addTop = true;
				foreach (var section in sections)
				{
					if (row <= section.StartRow)
					{
						break;
					}
					if (row <= section.StartRow + section.Count)
					{
						childController = section.ExpandRowInternal(row - section.StartRow - 1);
						addTop = false;
						break;
					}
					row -= section.Count;
				}
				if (addTop && row < Store.Count)
				{
					children = (ITreeGridStore<ITreeGridItem>)Store[row];
					childController = new TreeSection { StartRow = row, parent = this };
					childController.InitializeItems(children);
					Sections.Add(childController);
					Sections.Sort((x, y) => x.StartRow.CompareTo(y.StartRow));
				}
			}
			return childController;
		}
		protected TreeSection CollapseSection(int row)
		{
			TreeSection result = null;
			if (sections != null && sections.Count > 0)
			{
				bool addTop = true;
				foreach (var section in sections)
				{
					if (row <= section.StartRow)
					{
						break;
					}
					if (row <= section.StartRow + section.Count)
					{
						addTop = false;
						result = section.CollapseSection(row - section.StartRow - 1);
						break;
					}
					row -= section.Count;
				}
				if (addTop && row < Store.Count)
				{
					result = Sections.FirstOrDefault(r => r.StartRow == row);
				}
			}
			return result;
		}

		public int Count
		{
			get
			{
				if (Store == null)
					return 0;
				if (countCache != null)
					return countCache.Value;
				if (sections != null)
					countCache = Store.Count + sections.Sum(r => r.Count);
				else
					countCache = Store.Count;
				return countCache.Value;
			}
		}

		public int Add(object value)
		{
			return 0;
		}

		public void Clear()
		{
		}

		public bool Contains(object value)
		{
			return true;
		}

		public void Insert(int index, object value)
		{

		}

		public bool IsFixedSize
		{
			get { return true; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Remove(object value)
		{

		}

		public void RemoveAt(int index)
		{

		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public virtual ITreeGridItem this[int index] => GetItemAtRow(index);

		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
			{
				yield return this[i];
			}
		}


		internal static IEnumerable<ITreeGridItem> GetParents(ITreeGridItem value)
		{
			ITreeGridItem parent = value.Parent;
			while (parent != null)
			{
				yield return parent;
				parent = parent.Parent;
			}
		}
	}
}
