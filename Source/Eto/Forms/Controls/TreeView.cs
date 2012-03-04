using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	
	public interface ITreeStore<out T> : IDataStore<T>
		where T: ITreeItem
	{
	}

	public interface ITreeView : IControl
	{
		ITreeStore<ITreeItem> DataStore { get; set; }

		ITreeItem SelectedItem { get; set; }
		
		ContextMenu ContextMenu { get; set; }
		
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

	public class TreeView : Control
	{
		ITreeView inner;
		
		public event EventHandler<TreeViewItemEventArgs> Activated;

		public virtual void OnActivated (TreeViewItemEventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
		}
		
		public event EventHandler<EventArgs> SelectionChanged;
		
		public virtual void OnSelectionChanged (EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged (this, e);
		}
		
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
		
		public ContextMenu ContextMenu {
			get { return inner.ContextMenu; }
			set { inner.ContextMenu = value; }
		}
		
		public bool ShowHeader {
			get { return inner.ShowHeader; }
			set { inner.ShowHeader = value; }
		}
	}
}