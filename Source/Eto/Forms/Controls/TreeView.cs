using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Eto.Forms
{
	public interface ITreeStore : IDataStore<ITreeItem>
	{

	}

	public partial interface ITreeView : IControl
	{
		ITreeStore DataStore { get; set; }

		ITreeItem SelectedItem { get; set; }
	}
	
	public class TreeViewItemEventArgs : CancelEventArgs
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
		
		public event EventHandler<EventArgs> SelectionChanged;
		
		public virtual void OnSelectionChanged (EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged (this, e);
		}
		
		public const string ExpandingEvent = "TreeGridView.ExpandingEvent";

		event EventHandler<TreeGridViewItemCancelEventArgs> _Expanding;

		public event EventHandler<TreeGridViewItemCancelEventArgs> Expanding
		{
			add
			{
				_Expanding += value;
				HandleEvent (ExpandingEvent);
			}
			remove { _Expanding -= value; }
		}

		public virtual void OnExpanding (TreeGridViewItemCancelEventArgs e)
		{
			if (_Expanding != null)
				_Expanding (this, e);
		}

		public const string ExpandedEvent = "TreeView.ExpandedEvent";

		event EventHandler<TreeGridViewItemEventArgs> _Expanded;

		public event EventHandler<TreeGridViewItemEventArgs> Expanded
		{
			add
			{
				_Expanded += value;
				HandleEvent (ExpandedEvent);
			}
			remove { _Expanded -= value; }
		}

		public virtual void OnExpanded (TreeGridViewItemEventArgs e)
		{
			if (_Expanded != null)
				_Expanded (this, e);
		}
		
		
		public const string CollapsingEvent = "TreeGridView.CollapsingEvent";

		event EventHandler<TreeGridViewItemCancelEventArgs> _Collapsing;

		public event EventHandler<TreeGridViewItemCancelEventArgs> Collapsing
		{
			add
			{
				_Collapsing += value;
				HandleEvent (CollapsingEvent);
			}
			remove { _Collapsing -= value; }
		}

		public virtual void OnCollapsing (TreeGridViewItemCancelEventArgs e)
		{
			if (_Collapsing != null)
				_Collapsing (this, e);
		}
		
		public const string CollapsedEvent = "TreeGridView.CollapsedEvent";

		event EventHandler<TreeGridViewItemEventArgs> _Collapsed;

		public event EventHandler<TreeGridViewItemEventArgs> Collapsed
		{
			add
			{
				_Collapsed += value;
				HandleEvent (CollapsedEvent);
			}
			remove { _Collapsed -= value; }
		}

		public virtual void OnCollapsed (TreeGridViewItemEventArgs e)
		{
			if (_Collapsed != null)
				_Collapsed (this, e);
		}

		#endregion
		
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
	}
}