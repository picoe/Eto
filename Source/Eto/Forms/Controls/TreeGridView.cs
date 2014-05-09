using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Eto.Forms
{
	public interface ITreeGridStore<out T> : IDataStore<T>
		where T: ITreeGridItem
	{
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

	[Handler(typeof(TreeGridView.IHandler))]
	public partial class TreeGridView : Grid
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		public event EventHandler<TreeGridViewItemEventArgs> Activated
		{
			add { Properties.AddEvent(ActivatedKey, value); }
			remove { Properties.RemoveEvent(ActivatedKey, value); }
		}

		static readonly object ActivatedKey = new object();

		protected virtual void OnActivated(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ActivatedKey, this, e);
		}

		public const string ExpandingEvent = "TreeGridView.ExpandingEvent";

		public event EventHandler<TreeGridViewItemCancelEventArgs> Expanding
		{
			add { Properties.AddHandlerEvent(ExpandingEvent, value); }
			remove { Properties.RemoveEvent(ExpandingEvent, value); }
		}

		protected virtual void OnExpanding(TreeGridViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(ExpandingEvent, this, e);
		}

		public const string ExpandedEvent = "TreeGridView.ExpandedEvent";

		public event EventHandler<TreeGridViewItemEventArgs> Expanded
		{
			add { Properties.AddHandlerEvent(ExpandedEvent, value); }
			remove { Properties.RemoveEvent(ExpandedEvent, value); }
		}

		protected virtual void OnExpanded(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(ExpandedEvent, this, e);
		}

		public const string CollapsingEvent = "TreeGridView.CollapsingEvent";

		public event EventHandler<TreeGridViewItemCancelEventArgs> Collapsing
		{
			add { Properties.AddHandlerEvent(CollapsingEvent, value); }
			remove { Properties.RemoveEvent(CollapsingEvent, value); }
		}

		protected virtual void OnCollapsing(TreeGridViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(CollapsingEvent, this, e);
		}

		public const string CollapsedEvent = "TreeGridView.CollapsedEvent";

		public event EventHandler<TreeGridViewItemEventArgs> Collapsed
		{
			add { Properties.AddHandlerEvent(CollapsedEvent, value); }
			remove { Properties.RemoveEvent(CollapsedEvent, value); }
		}

		protected virtual void OnCollapsed(TreeGridViewItemEventArgs e)
		{
			Properties.TriggerEvent(CollapsedEvent, this, e);
		}

		public const string SelectedItemChangedEvent = "TreeGridView.SelectedItemChanged";

		public event EventHandler<EventArgs> SelectedItemChanged
		{
			add { Properties.AddHandlerEvent(SelectedItemChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectedItemChangedEvent, value); }
		}

		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedItemChangedEvent, this, e);
		}

		#endregion

		static TreeGridView()
		{
			EventLookup.Register<TreeGridView>(c => c.OnExpanding(null), TreeGridView.ExpandingEvent);
			EventLookup.Register<TreeGridView>(c => c.OnExpanded(null), TreeGridView.ExpandedEvent);
			EventLookup.Register<TreeGridView>(c => c.OnCollapsing(null), TreeGridView.CollapsingEvent);
			EventLookup.Register<TreeGridView>(c => c.OnCollapsed(null), TreeGridView.CollapsedEvent);
			EventLookup.Register<TreeGridView>(c => c.OnSelectedItemChanged(null), TreeGridView.SelectedItemChangedEvent);
		}

		public TreeGridView()
		{
		}

		[Obsolete("Use default constructor instead")]
		public TreeGridView(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
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

		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Grid.ICallback
		{
			void OnActivated(TreeGridView widget, TreeGridViewItemEventArgs e);
			void OnExpanding(TreeGridView widget, TreeGridViewItemCancelEventArgs e);
			void OnExpanded(TreeGridView widget, TreeGridViewItemEventArgs e);
			void OnCollapsing(TreeGridView widget, TreeGridViewItemCancelEventArgs e);
			void OnCollapsed(TreeGridView widget, TreeGridViewItemEventArgs e);
			void OnSelectedItemChanged(TreeGridView widget, EventArgs e);
		}

		protected class Callback : Grid.Callback, ICallback
		{
			public void OnActivated(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				widget.OnActivated(e);
			}
			public void OnExpanding(TreeGridView widget, TreeGridViewItemCancelEventArgs e)
			{
				widget.OnExpanding(e);
			}
			public void OnExpanded(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				widget.OnExpanded(e);
			}
			public void OnCollapsing(TreeGridView widget, TreeGridViewItemCancelEventArgs e)
			{
				widget.OnCollapsing(e);
			}
			public void OnCollapsed(TreeGridView widget, TreeGridViewItemEventArgs e)
			{
				widget.OnCollapsed(e);
			}
			public void OnSelectedItemChanged(TreeGridView widget, EventArgs e)
			{
				widget.OnSelectedItemChanged(e);
			}
		}

		public interface IHandler : Grid.IHandler, IContextMenuHost
		{
			ITreeGridStore<ITreeGridItem> DataStore { get; set; }

			ITreeGridItem SelectedItem { get; set; }
		}
	}
}