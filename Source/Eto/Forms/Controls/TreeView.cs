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
		EventHandler<TreeViewItemEventArgs> _Activated;

		public event EventHandler<TreeViewItemEventArgs> Activated
		{
			add
			{
				HandleEvent(ActivatedEvent);
				_Activated += value;
			}
			remove { _Activated -= value; }
		}

		public virtual void OnActivated(TreeViewItemEventArgs e)
		{
			if (_Activated != null)
				_Activated(this, e);
		}

		public const string SelectionChangedEvent = "TreeView.SelectionChangedEvent";

		event EventHandler<EventArgs> _SelectionChanged;

		public event EventHandler<EventArgs> SelectionChanged
		{
			add
			{
				_SelectionChanged += value;
				HandleEvent(SelectionChangedEvent);
			}
			remove
			{
				_SelectionChanged -= value;
			}
		}

		public virtual void OnSelectionChanged(EventArgs e)
		{
			if (_SelectionChanged != null)
				_SelectionChanged(this, e);
		}

		public const string ExpandingEvent = "TreeView.ExpandingEvent";

		event EventHandler<TreeViewItemCancelEventArgs> _Expanding;

		public event EventHandler<TreeViewItemCancelEventArgs> Expanding
		{
			add
			{
				_Expanding += value;
				HandleEvent(ExpandingEvent);
			}
			remove { _Expanding -= value; }
		}

		public virtual void OnExpanding(TreeViewItemCancelEventArgs e)
		{
			if (_Expanding != null)
				_Expanding(this, e);
		}

		public const string ExpandedEvent = "TreeView.ExpandedEvent";

		event EventHandler<TreeViewItemEventArgs> _Expanded;

		public event EventHandler<TreeViewItemEventArgs> Expanded
		{
			add
			{
				_Expanded += value;
				HandleEvent(ExpandedEvent);
			}
			remove { _Expanded -= value; }
		}

		public virtual void OnExpanded(TreeViewItemEventArgs e)
		{
			if (_Expanded != null)
				_Expanded(this, e);
		}

		public const string CollapsingEvent = "TreeView.CollapsingEvent";

		event EventHandler<TreeViewItemCancelEventArgs> _Collapsing;

		public event EventHandler<TreeViewItemCancelEventArgs> Collapsing
		{
			add
			{
				_Collapsing += value;
				HandleEvent(CollapsingEvent);
			}
			remove { _Collapsing -= value; }
		}

		public virtual void OnCollapsing(TreeViewItemCancelEventArgs e)
		{
			if (_Collapsing != null)
				_Collapsing(this, e);
		}

		public const string CollapsedEvent = "TreeView.CollapsedEvent";

		event EventHandler<TreeViewItemEventArgs> _Collapsed;

		public event EventHandler<TreeViewItemEventArgs> Collapsed
		{
			add
			{
				_Collapsed += value;
				HandleEvent(CollapsedEvent);
			}
			remove { _Collapsed -= value; }
		}

		public virtual void OnCollapsed(TreeViewItemEventArgs e)
		{
			if (_Collapsed != null)
				_Collapsed(this, e);
		}

		public const string LabelEditedEvent = "TreeView.LabelEdited";

		event EventHandler<TreeViewItemEditEventArgs> labelEdited;

		public event EventHandler<TreeViewItemEditEventArgs> LabelEdited
		{
			add
			{
				labelEdited += value;
				HandleEvent(LabelEditedEvent);
			}
			remove { labelEdited -= value; }
		}

		public virtual void OnLabelEdited(TreeViewItemEditEventArgs e)
		{
			if (labelEdited != null)
				labelEdited(this, e);
		}

		public const string LabelEditingEvent = "TreeView.LabelEditing";

		event EventHandler<TreeViewItemCancelEventArgs> labelEditing;

		public event EventHandler<TreeViewItemCancelEventArgs> LabelEditing
		{
			add
			{
				labelEditing += value;
				HandleEvent(LabelEditingEvent);
			}
			remove { labelEditing -= value; }
		}

		public virtual void OnLabelEditing(TreeViewItemCancelEventArgs e)
		{
			if (labelEditing != null)
				labelEditing(this, e);
		}

		public const string NodeMouseClickEvent = "TreeView.NodeMouseClick";

		event EventHandler<TreeViewItemEventArgs> _NodeMouseClick;

		public event EventHandler<TreeViewItemEventArgs> NodeMouseClick
		{
			add
			{
				_NodeMouseClick += value;
				HandleEvent(NodeMouseClickEvent);
			}
			remove { _NodeMouseClick -= value; }
		}

		public virtual void OnNodeMouseClick(TreeViewItemEventArgs e)
		{
			if (_NodeMouseClick != null)
				_NodeMouseClick(this, e);
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

		public TreeView() : this (Generator.Current)
		{
		}

		public TreeView(Generator g) : this (g, typeof(ITreeView))
		{
		}

		protected TreeView(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
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