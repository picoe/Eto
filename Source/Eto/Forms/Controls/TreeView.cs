using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	
	public interface ITreeStore<out T> : IDataStore<T>
		where T: ITreeItem
	{
	}

	public partial interface ITreeView : IControl
	{
		ITreeStore<ITreeItem> DataStore { get; set; }

		ITreeItem SelectedItem { get; set; }
		
		bool ShowHeader { get; set; }
	}
	
	public class TreeViewItemEventArgs : EventArgs
	{
		public ITreeItem Item { get; private set; }
		
		public TreeViewItemEventArgs (ITreeItem item)
		{
			this.Item = item;
		}
	}

	public partial class TreeView : Control
	{
		ITreeView inner;

		#region Events

		public event EventHandler<TreeViewItemEventArgs> Activated;

		public virtual void OnActivated (TreeViewItemEventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
		}

		public const string SelectionChangedEvent = "GridView.SelectionChanged";

		event EventHandler<EventArgs> selectionChanged;

		public event EventHandler<EventArgs> SelectionChanged
		{
			add
			{
				selectionChanged += value;
				HandleEvent (SelectionChangedEvent);
			}
			remove { selectionChanged -= value; }
		}

		public virtual void OnSelectionChanged (EventArgs e)
		{
			if (selectionChanged != null)
				selectionChanged (this, e);
		}

		#endregion

		public TreeColumnCollection Columns { get; private set; }
		
		public TreeView ()
			: this (Generator.Current)
		{
		}

		public TreeView (Generator g) : base(g, typeof(ITreeView), false)
		{
			inner = (ITreeView)Handler;
			Columns = new TreeColumnCollection ();
			Initialize ();
		}
		
		public ITreeItem SelectedItem {
			get { return inner.SelectedItem; }
			set { inner.SelectedItem = value; }
		}
		
		[Obsolete("Use DataStore property instead")]
		public ITreeStore<ITreeItem> TopNode {
			get { return this.DataStore; }
			set { this.DataStore = value; }
		}
		
		public ITreeStore<ITreeItem> DataStore {
			get { return inner.DataStore; }
			set { inner.DataStore = value; }
		}
		
		public bool ShowHeader {
			get { return inner.ShowHeader; }
			set { inner.ShowHeader = value; }
		}
	}
}