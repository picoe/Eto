using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface ITreeStore : IDataStore
	{
		int Count { get; }
		
		ITreeItem GetChild (int index);
	}

	public interface ITreeView : IControl
	{
		ITreeStore DataStore { get; set; }

		ITreeItem SelectedItem { get; set; }
		
		ContextMenu ContextMenu { get; set; }
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
		
		public TreeView ()
			: this (Generator.Current)
		{
		}

		public TreeView (Generator g) : base(g, typeof(ITreeView))
		{
			inner = (ITreeView)Handler;
		}
		
		public ITreeItem SelectedItem {
			get { return inner.SelectedItem; }
			set { inner.SelectedItem = value; }
		}
		
		[Obsolete("Use DataStore property instead")]
		public ITreeStore TopNode {
			get { return this.DataStore; }
			set { this.DataStore = value; }
		}
		
		public ITreeStore DataStore {
			get { return inner.DataStore; }
			set { inner.DataStore = value; }
		}
		
		public ContextMenu ContextMenu {
			get { return inner.ContextMenu; }
			set { inner.ContextMenu = value; }
		}
	}
}