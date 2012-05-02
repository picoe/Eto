using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Eto.Forms;
using System.Collections;

namespace Eto.Platform.Windows.CustomControls
{
	public interface ITreeHandler
	{
	}

	public class TreeController : ITreeGridStore<ITreeGridItem>, IList, INotifyCollectionChanged
	{
		Dictionary<int, ITreeGridItem> cache = new Dictionary<int, ITreeGridItem> ();

		int StartRow { get; set; }

		List<TreeController> sections;


		List<TreeController> Sections
		{
			get
			{
				if (sections == null) sections = new List<TreeController> ();
				return sections;
			}
		}

		public ITreeGridStore<ITreeGridItem> Store { get; set; }

		public void InitializeItems ()
		{
			for (int i = Count-1; i >= 0; i--) {
				var item = this[i];
				if (item.Expanded) {
					ExpandRowInternal (i, true);
				}
			}
		}

		public ITreeHandler Handler { get; set; }

		public int IndexOf (ITreeGridItem item)
		{
			if (cache.ContainsValue (item)) {
				var found = cache.First (r => object.ReferenceEquals (item, r.Value));
				return found.Key;
			}
			return -1;
		}

		public int LevelAtRow (int row)
		{
			if (sections == null || sections.Count == 0)
				return 0;
			foreach (var section in sections) {
				if (row <= section.StartRow) {
					return 0;
				}
				else {
					var count = section.Count;
					if (row <= section.StartRow + count) {
						return section.LevelAtRow (row - section.StartRow - 1) + 1;
					}
					else {
						row -= count;
					}
				}
			}
			return 0;
		}

		public ITreeGridItem this[int row]
		{
			get
			{
				ITreeGridItem item;
				if (!cache.TryGetValue (row, out item)) {
					item = GetItemAtRow (row);
					cache[row] = item;
				}
				return item;
			}
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

			public bool IsLastNode { get { return Index == Count-1; } }
		}

		public TreeNode GetNodeAtRow (int row)
		{
			return GetNodeAtRow (row, null, 0);
		}

		TreeNode GetNodeAtRow (int row, TreeNode parent, int level)
		{
			var node = new TreeNode { RowIndex = row, Parent = parent, Count = Store.Count, Level = level };
			if (sections == null || sections.Count == 0) {
				node.Item = Store[row];
				node.Index = row;
			}
			else {
				foreach (var section in sections) {
					if (row <= section.StartRow) {
						node.Item = Store[row];
						node.Index = row;
						break;
					}
					else if (row <= section.StartRow + section.Count) {
						node.Index = section.StartRow;
						node.Item = section.Store as ITreeGridItem;
						return section.GetNodeAtRow (row - section.StartRow - 1, node, level + 1);
					}
					else {
						row -= section.Count;
					}
				}
			}
			if (node.Item == null && row < Store.Count) {
				node.Item = Store[row];
				node.Index = row;
			}
			return node;
		}


		ITreeGridItem GetItemAtRow (int row)
		{
			ITreeGridItem item = null;
			if (sections == null || sections.Count == 0)
				item = Store[row];
			if (item == null) {
				foreach (var section in sections) {
					if (row <= section.StartRow) {
						item = Store[row];
						break;
					}
					else if (row <= section.StartRow + section.Count) {
						item = section.GetItemAtRow (row - section.StartRow - 1);
						break;
					}
					else {
						row -= section.Count;
					}
				}
			}
			if (item == null && row < Store.Count)
				item = Store[row];
			return item;
		}

		public bool IsExpanded (int row)
		{
			if (sections == null) return false;
			foreach (var section in sections) {
				if (row < section.StartRow)
					return false;
				else if (row == section.StartRow) {
					return true;
				}
				else if (row <= section.StartRow + section.Count) {
					return section.IsExpanded (row - section.StartRow - 1);
				}
				else {
					row -= section.Count;
				}
			}
			return false;
		}

		public void ExpandRow (int row)
		{
			ExpandRowInternal (row, false);
		}

		ITreeGridStore<ITreeGridItem> ExpandRowInternal (int row, bool init)
		{
			ITreeGridStore<ITreeGridItem> children = null;
			var originalRow = row;
			if (sections == null || sections.Count == 0) {
				children = (ITreeGridStore<ITreeGridItem>)Store [row];
				var childController = new TreeController { StartRow = row, Store = children };
				if (init) childController.InitializeItems ();
				Sections.Add (childController);
			}
			else {
				bool addTop = true;
				foreach (var section in sections) {
					if (row <= section.StartRow) {
						break;
					}
					else if (row <= section.StartRow + section.Count) {
						children = section.ExpandRowInternal (row - section.StartRow - 1, init);
						addTop = false;
						break;
					}
					else {
						row -= section.Count;
					}
				}
				if (addTop && row < Store.Count) {
					children = (ITreeGridStore<ITreeGridItem>)Store [row];
					var childController = new TreeController { StartRow = row, Store = children };
					if (init) childController.InitializeItems ();
					Sections.Add (childController);
				}
			}
			Sections.Sort ((x, y) => x.StartRow.CompareTo (y.StartRow));
			if (children != null) {
				if (children.Count < 50) {
					var newlist = new Dictionary<int, ITreeGridItem> ();
					foreach (var item in cache) {
						newlist.Add (item.Key <= originalRow ? item.Key : item.Key + children.Count, item.Value);
					}
					cache = newlist;
					for (int i = originalRow + 1; i <= originalRow + children.Count; i++)
						OnTriggerCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add, this[i], i));
				}
				else {
					cache.Clear ();
					OnTriggerCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
				}
			}
			return children;
		}

		public void CollapseRow (int row)
		{
			if (sections != null && sections.Count > 0) {
				bool addTop = true;
				foreach (var section in sections) {
					if (row <= section.StartRow) {
						break;
					}
					else if (row <= section.StartRow + section.Count) {
						section.CollapseRow (row - section.StartRow - 1);
						addTop = false;
					}
					else {
						row -= section.Count;
					}
				}
				if (addTop && row < Store.Count)
					Sections.RemoveAll (r => r.StartRow == row);
			}
			cache.Clear ();
			OnTriggerCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
		}

		public int Count
		{
			get
			{
				if (sections != null)
					return this.Store.Count + sections.Sum (r => r.Count);
				else
					return this.Store.Count;
			}
		}

		public int Add (object value)
		{
			return 0;
		}

		public void Clear ()
		{
		}

		public bool Contains (object value)
		{
			return true;
		}

		int IList.IndexOf (object value)
		{
			return IndexOf (value as ITreeGridItem);
		}

		public void Insert (int index, object value)
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

		public void Remove (object value)
		{
			
		}

		public void RemoveAt (int index)
		{
			
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				
			}
		}

		public void CopyTo (Array array, int index)
		{
			throw new NotImplementedException ();
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public IEnumerator GetEnumerator ()
		{
			for (int i = 0; i < Count; i++) {
				yield return this[i];
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnTriggerCollectionChanged (NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
				CollectionChanged (this, args);
		}
	}
}
