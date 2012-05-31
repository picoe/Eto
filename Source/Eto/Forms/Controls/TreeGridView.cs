using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
	
	public class TreeGridViewItemEventArgs : EventArgs
	{
		public ITreeGridItem Item { get; private set; }

		public TreeGridViewItemEventArgs (ITreeGridItem item)
		{
			this.Item = item;
		}
	}

	public class TreeGridViewItemCancelEventArgs : CancelEventArgs
	{
		public ITreeGridItem Item { get; private set; }

		public TreeGridViewItemCancelEventArgs (ITreeGridItem item)
		{
			this.Item = item;
		}
	}
	
	public partial class TreeGridView : Grid
	{
		ITreeGridView handler;

		#region Events

		public event EventHandler<TreeGridViewItemEventArgs> Activated;

		public virtual void OnActivated (TreeGridViewItemEventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
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

		public const string ExpandedEvent = "TreeGridView.ExpandedEvent";

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

		public TreeGridView ()
			: this (Generator.Current)
		{
		}

		public TreeGridView (Generator g) : this (g, typeof(ITreeGridView))
		{
		}
		
		protected TreeGridView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ITreeGridView)Handler;
			if (initialize)
				Initialize ();
		}

		
		public ITreeGridItem SelectedItem {
			get { return handler.SelectedItem; }
			set { handler.SelectedItem = value; }
		}
		
		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
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