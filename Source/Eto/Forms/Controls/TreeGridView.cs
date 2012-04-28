using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	
	public interface ITreeGridStore<out T> : IDataStore<T>
		where T: ITreeGridItem
	{
	}

	public partial interface ITreeGridView : IGrid
	{
		ITreeGridStore<ITreeGridItem> DataStore { get; set; }

		ITreeGridItem SelectedItem { get; set; }
	}
	
	public class TreeViewItemEventArgs : EventArgs
	{
		public ITreeGridItem Item { get; private set; }
		
		public TreeViewItemEventArgs (ITreeGridItem item)
		{
			this.Item = item;
		}
	}

	public partial class TreeGridView : Grid
	{
		ITreeGridView inner;

		#region Events

		public event EventHandler<TreeViewItemEventArgs> Activated;

		public virtual void OnActivated (TreeViewItemEventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
		}

		#endregion

		public TreeGridView ()
			: this (Generator.Current)
		{
		}

		public TreeGridView (Generator g) : base(g, typeof(ITreeGridView), false)
		{
			inner = (ITreeGridView)Handler;
			Initialize ();
		}
		
		public ITreeGridItem SelectedItem {
			get { return inner.SelectedItem; }
			set { inner.SelectedItem = value; }
		}
		
		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return inner.DataStore; }
			set { inner.DataStore = value; }
		}

		public override IEnumerable<IGridItem> SelectedItems
		{
			get
			{
				if (DataStore == null)
					yield break;
				/*foreach (var row in SelectedRows) {
					yield return DataStore[row];
				}*/
			}
		}
		
	}
}