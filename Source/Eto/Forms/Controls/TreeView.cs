using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ITreeStore : IDataStore<ITreeItem>
	{
	}

	public partial interface ITreeView : IControl
	{
		ITreeStore DataStore { get; set; }

		ITreeItem SelectedItem { get; set; }

		void RefreshData();

		void RefreshItem(ITreeItem item);

		ITreeItem GetNodeAt(PointF point);

		bool LabelEdit { get; set; }

		Color TextColor { get; set; }
	}

	public class TreeViewItemEventArgs : EventArgs
	{
		public ITreeItem Item { get; private set; }

		public TreeViewItemEventArgs(ITreeItem item)
		{
			this.Item = item;
		}
	}

	public class TreeViewItemCancelEventArgs : CancelEventArgs
	{
		public ITreeItem Item { get; private set; }

		public TreeViewItemCancelEventArgs(ITreeItem item)
		{
			this.Item = item;
		}
	}

	public class TreeViewItemEditEventArgs : TreeViewItemCancelEventArgs
	{
		public string Label { get; set; }

		public TreeViewItemEditEventArgs(ITreeItem item, string label)
			: base(item)
		{
			this.Label = label;
		}
	}

	public partial class TreeView : Control
	{
		new ITreeView Handler { get { return (ITreeView)base.Handler; } }

		#region Events

		public const string ActivatedEvent = "TreeView.Activated";

		public event EventHandler<TreeViewItemEventArgs> Activated
		{
			add { Properties.AddHandlerEvent(ActivatedEvent, value); }
			remove { Properties.RemoveEvent(ActivatedEvent, value); }
		}

		public virtual void OnActivated(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(ActivatedEvent, this, e);
		}

		public const string SelectionChangedEvent = "TreeView.SelectionChangedEvent";

		public event EventHandler<EventArgs> SelectionChanged
		{
			add { Properties.AddHandlerEvent(SelectionChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectionChangedEvent, value); }
		}

		public virtual void OnSelectionChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectionChangedEvent, this, e);
		}

		public const string ExpandingEvent = "TreeView.ExpandingEvent";

		public event EventHandler<TreeViewItemCancelEventArgs> Expanding
		{
			add { Properties.AddHandlerEvent(ExpandingEvent, value); }
			remove { Properties.RemoveEvent(ExpandingEvent, value); }
		}

		public virtual void OnExpanding(TreeViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(ExpandingEvent, this, e);
		}

		public const string ExpandedEvent = "TreeView.ExpandedEvent";

		public event EventHandler<TreeViewItemEventArgs> Expanded
		{
			add { Properties.AddHandlerEvent(ExpandedEvent, value); }
			remove { Properties.RemoveEvent(ExpandedEvent, value); }
		}

		public virtual void OnExpanded(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(ExpandedEvent, this, e);
		}

		public const string CollapsingEvent = "TreeView.CollapsingEvent";

		public event EventHandler<TreeViewItemCancelEventArgs> Collapsing
		{
			add { Properties.AddHandlerEvent(CollapsingEvent, value); }
			remove { Properties.RemoveEvent(CollapsingEvent, value); }
		}

		public virtual void OnCollapsing(TreeViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(CollapsingEvent, this, e);
		}

		public const string CollapsedEvent = "TreeView.CollapsedEvent";

		public event EventHandler<TreeViewItemEventArgs> Collapsed
		{
			add { Properties.AddHandlerEvent(CollapsedEvent, value); }
			remove { Properties.RemoveEvent(CollapsedEvent, value); }
		}

		public virtual void OnCollapsed(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(CollapsedEvent, this, e);
		}

		public const string LabelEditedEvent = "TreeView.LabelEdited";

		public event EventHandler<TreeViewItemEditEventArgs> LabelEdited
		{
			add { Properties.AddHandlerEvent(LabelEditedEvent, value); }
			remove { Properties.RemoveEvent(LabelEditedEvent, value); }
		}

		public virtual void OnLabelEdited(TreeViewItemEditEventArgs e)
		{
			Properties.TriggerEvent(LabelEditedEvent, this, e);
		}

		public const string LabelEditingEvent = "TreeView.LabelEditing";

		public event EventHandler<TreeViewItemCancelEventArgs> LabelEditing
		{
			add { Properties.AddHandlerEvent(LabelEditingEvent, value); }
			remove { Properties.RemoveEvent(LabelEditingEvent, value); }
		}

		public virtual void OnLabelEditing(TreeViewItemCancelEventArgs e)
		{
			Properties.TriggerEvent(LabelEditingEvent, this, e);
		}

		public const string NodeMouseClickEvent = "TreeView.NodeMouseClick";

		public event EventHandler<TreeViewItemEventArgs> NodeMouseClick
		{
			add { Properties.AddHandlerEvent(NodeMouseClickEvent, value); }
			remove { Properties.RemoveEvent(NodeMouseClickEvent, value); }
		}

		public virtual void OnNodeMouseClick(TreeViewItemEventArgs e)
		{
			Properties.TriggerEvent(NodeMouseClickEvent, this, e);
		}

		#endregion

		static TreeView()
		{
			EventLookup.Register(typeof(TreeView), "OnActivated", TreeView.ActivatedEvent);
			EventLookup.Register(typeof(TreeView), "OnSelectionChanged", TreeView.SelectionChangedEvent);
			EventLookup.Register(typeof(TreeView), "OnExpanding", TreeView.ExpandingEvent);
			EventLookup.Register(typeof(TreeView), "OnExpanded", TreeView.ExpandedEvent);
			EventLookup.Register(typeof(TreeView), "OnCollapsing", TreeView.CollapsingEvent);
			EventLookup.Register(typeof(TreeView), "OnCollapsed", TreeView.CollapsedEvent);
			EventLookup.Register(typeof(TreeView), "OnLabelEdited", TreeView.LabelEditedEvent);
			EventLookup.Register(typeof(TreeView), "OnLabelEditing", TreeView.LabelEditingEvent);
			EventLookup.Register(typeof(TreeView), "OnNodeMouseClick", TreeView.NodeMouseClickEvent);
		}

		public TreeView()
			: this((Generator)null)
		{
		}

		public TreeView(Generator generator) : this(generator, typeof(ITreeView))
		{
		}

		protected TreeView(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public ITreeItem SelectedItem
		{
			get { return Handler.SelectedItem; }
			set { Handler.SelectedItem = value; }
		}

		[Obsolete("Use DataStore property instead")]
		public ITreeStore TopNode
		{
			get { return DataStore; }
			set { DataStore = value; }
		}

		public ITreeStore DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}

		public void RefreshData()
		{
			Handler.RefreshData();
		}

		public void RefreshItem(ITreeItem item)
		{
			Handler.RefreshItem(item);
		}

		public ITreeItem GetNodeAt(PointF point)
		{
			return Handler.GetNodeAt(point);
		}

		public bool LabelEdit
		{
			get { return Handler.LabelEdit; }
			set { Handler.LabelEdit = value; }
		}
	}
}