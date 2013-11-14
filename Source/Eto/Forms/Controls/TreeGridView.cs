using System;
using System.Collections.Generic;
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

		public TreeGridViewItemEventArgs(ITreeGridItem item)
		{
			this.Item = item;
		}
	}

	public class TreeGridViewItemCancelEventArgs : CancelEventArgs
	{
		public ITreeGridItem Item { get; private set; }

		public TreeGridViewItemCancelEventArgs(ITreeGridItem item)
		{
			this.Item = item;
		}
	}

	public partial class TreeGridView : Grid
	{
		new ITreeGridView Handler { get { return (ITreeGridView)base.Handler; } }

		#region Events

		public event EventHandler<TreeGridViewItemEventArgs> Activated
		{
			add { Properties.AddEvent(ActivatedKey, value); }
			remove { Properties.RemoveEvent(ActivatedKey, value); }
		}

		static readonly object ActivatedKey = new object();

		public virtual void OnActivated(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ActivatedKey, this, e);
		}

		public const string ExpandingEvent = "TreeGridView.ExpandingEvent";

		public event EventHandler<TreeGridViewItemCancelEventArgs> Expanding
		{
			add { Properties.AddHandlerEvent(ExpandingEvent, value); }
			remove { Properties.RemoveEvent(ExpandingEvent, value); }
		}

		public virtual void OnExpanding(TreeGridViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(ExpandingEvent, this, e);
		}

		public const string ExpandedEvent = "TreeGridView.ExpandedEvent";

		public event EventHandler<TreeGridViewItemEventArgs> Expanded
		{
			add { Properties.AddHandlerEvent(ExpandedEvent, value); }
			remove { Properties.RemoveEvent(ExpandedEvent, value); }
		}

		public virtual void OnExpanded(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ExpandedEvent, this, e);
		}

		public const string CollapsingEvent = "TreeGridView.CollapsingEvent";

		public event EventHandler<TreeGridViewItemCancelEventArgs> Collapsing
		{
			add { Properties.AddHandlerEvent(CollapsingEvent, value); }
			remove { Properties.RemoveEvent(CollapsingEvent, value); }
		}

		public virtual void OnCollapsing(TreeGridViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(CollapsingEvent, this, e);
		}

		public const string CollapsedEvent = "TreeGridView.CollapsedEvent";

		public event EventHandler<TreeGridViewItemEventArgs> Collapsed
		{
			add { Properties.AddHandlerEvent(CollapsedEvent, value); }
			remove { Properties.RemoveEvent(CollapsedEvent, value); }
		}

		public virtual void OnCollapsed(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(CollapsedEvent, this, e);
		}

		public const string SelectedItemChangedEvent = "TreeGridView.SelectedItemChanged";

		public event EventHandler<EventArgs> SelectedItemChanged
		{
			add { Properties.AddHandlerEvent(SelectedItemChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectedItemChangedEvent, value); }
		}

		public virtual void OnSelectedItemChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedItemChangedEvent, this, e);
		}

		#endregion

		static TreeGridView()
		{
			EventLookup.Register(typeof(TreeGridView), "OnExpanding", TreeGridView.ExpandingEvent);
			EventLookup.Register(typeof(TreeGridView), "OnExpanded", TreeGridView.ExpandedEvent);
			EventLookup.Register(typeof(TreeGridView), "OnCollapsing", TreeGridView.CollapsingEvent);
			EventLookup.Register(typeof(TreeGridView), "OnCollapsed", TreeGridView.CollapsedEvent);
			EventLookup.Register(typeof(TreeGridView), "OnSelectedItemChanged", TreeGridView.SelectedItemChangedEvent);
		}

		public TreeGridView()
			: this((Generator)null)
		{
		}

		public TreeGridView(Generator generator) : this(generator, typeof(ITreeGridView))
		{
		}

		protected TreeGridView(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public new ITreeGridItem SelectedItem
		{
			get { return Handler.SelectedItem; }
			set { Handler.SelectedItem = value; }
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		public override IEnumerable<object> SelectedItems
		{
			get
			{
				if (DataStore == null)
					yield break;
				foreach (var row in SelectedRows)
				{
					yield return DataStore[row];
				}
			}
		}
	}
}