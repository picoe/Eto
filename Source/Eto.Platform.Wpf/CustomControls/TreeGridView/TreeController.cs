using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Eto.Forms;

namespace Eto.Platform.Wpf.CustomControls.TreeGridView
{
	public interface ITreeHandler
	{
		void TriggerCollectionChanged (NotifyCollectionChangedEventArgs args);
	}

	public class TreeController : ITreeStore<ITreeItem>
	{
		Dictionary<int, ITreeItem> cache = new Dictionary<int, ITreeItem> ();

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

		public ITreeStore<ITreeItem> Store { get; set; }

		public ITreeHandler Handler { get; set; }

		public int IndexOf (ITreeItem item)
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

		public ITreeItem this[int row]
		{
			get
			{
				ITreeItem item;
				if (!cache.TryGetValue (row, out item)) {
					item = GetItemAtRow (row);
					cache[row] = item;
				}
				return item;
			}
		}

		ITreeItem GetItemAtRow (int row)
		{
			if (sections == null || sections.Count == 0)
				return Store[row];
			foreach (var section in sections) {
				if (row <= section.StartRow) {
					return Store[row];
				}
				else if (row <= section.StartRow + section.Count) {
					return section.GetItemAtRow (row - section.StartRow - 1);
				}
				else {
					row -= section.Count;
				}
			}
			if (row < Store.Count)
				return Store[row];
			return null;
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
			ExpandRowInternal (row);
		}

		ITreeStore<ITreeItem> ExpandRowInternal (int row)
		{
			ITreeStore<ITreeItem> children = null;
			var originalRow = row;
			if (sections == null || sections.Count == 0) {
				children = (ITreeStore<ITreeItem>)Store [row];
				Sections.Add (new TreeController { StartRow = row, Store = children });
			}
			else {
				bool addTop = true;
				foreach (var section in sections) {
					if (row <= section.StartRow) {
						break;
					}
					else if (row <= section.StartRow + section.Count) {
						children = section.ExpandRowInternal (row - section.StartRow - 1);
						addTop = false;
						break;
					}
					else {
						row -= section.Count;
					}
				}
				if (addTop && row < Store.Count) {
					children = (ITreeStore<ITreeItem>)Store [row];
					Sections.Add (new TreeController { StartRow = row, Store = children });
				}
			}
			Sections.Sort ((x, y) => x.StartRow.CompareTo (y.StartRow));
			if (Handler != null && children != null) {
				if (children.Count < 50) {
					var newlist = new Dictionary<int, ITreeItem> ();
					foreach (var item in cache) {
						newlist.Add (item.Key <= originalRow ? item.Key : item.Key + children.Count, item.Value);
					}
					cache = newlist;
					for (int i = originalRow + 1; i <= originalRow + children.Count; i++)
						Handler.TriggerCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add, this[i], i));
				}
				else {
					cache.Clear ();
					Handler.TriggerCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
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
			if (Handler != null) {
				Handler.TriggerCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
			}
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
	}
}
