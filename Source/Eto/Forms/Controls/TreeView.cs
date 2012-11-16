using System;
using System.Collections.Generic;
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

        ITreeItem GetNodeAt(Point point);

		void RefreshData ();

		void RefreshItem (ITreeItem item);

        bool LabelEdit { get; set; }

        bool IsExpanded(ITreeItem item);

        void Collapse(ITreeItem item);

        void AddTo(ITreeItem dest, ITreeItem item);

        bool AllowDrop { get; set; }

        void Expand(ITreeItem item);

        void Remove(ITreeItem item);

        void SetImage(TreeItem item, Image image);
    }

    public enum TreeViewAction
    {
        Unknown = 0,
        ByKeyboard = 1,
        ByMouse = 2,
        Collapse = 3,
        Expand = 4,
    }

	public class TreeViewItemEventArgs : EventArgs
	{
		public ITreeItem Item { get; private set; }
		
		public TreeViewItemEventArgs (ITreeItem item)
		{
			this.Item = item;
		}

        public bool CancelEdit { get; set; }

        public string Label { get; set; }

        public TreeViewAction Action { get; set; }
    }

    public class ItemDragEventArgs : EventArgs
    {
        public MouseButtons Buttons { get; set; }
        public object Item { get; set; }
    }

    public class TreeNodeMouseClickEventArgs : MouseEventArgs
    {
        public ITreeItem Item { get; private set; }

        public TreeNodeMouseClickEventArgs(
            MouseEventArgs e,
            ITreeItem item)
            :base(e.Buttons, e.Modifiers, e.Location)
        {
            this.Item = item;
        }
    }

	public class TreeViewItemCancelEventArgs : CancelEventArgs
	{
		public ITreeItem Item { get; private set; }
		
		public TreeViewItemCancelEventArgs (ITreeItem item)
		{
			this.Item = item;
		}
	}
	
	public partial class TreeView : Control
	{
		ITreeView handler;
		
		#region Events

		public const string ActivatedEvent = "TreeView.Activated";

		EventHandler<TreeViewItemEventArgs> _Activated;

		public event EventHandler<TreeViewItemEventArgs> Activated {
			add {
				HandleEvent (ActivatedEvent);
				_Activated += value;
			}
			remove { _Activated -= value; }
		}

		public virtual void OnActivated (TreeViewItemEventArgs e)
		{
			if (_Activated != null)
				_Activated (this, e);
		}

        #region SelectionChanged
        public const string SelectionChangedEvent = "TreeView.SelectionChanged";

        EventHandler<EventArgs> selectionChanged;

        public event EventHandler<EventArgs> SelectionChanged
        {
            add
            {
                HandleEvent(SelectionChangedEvent);
                selectionChanged += value;
            }
            remove { selectionChanged -= value; }
        }

        public virtual void OnSelectionChanged(EventArgs e)
        {
            if (selectionChanged != null)
                selectionChanged(this, e);
        }
        #endregion

        #region BeforeLabelEdit
        public const string BeforeLabelEditEvent = "TreeView.BeforeLabelEdit";

        EventHandler<TreeViewItemEventArgs> beforeLabelEdit;

        public event EventHandler<TreeViewItemEventArgs> BeforeLabelEdit
        {
            add
            {
                HandleEvent(BeforeLabelEditEvent);
                beforeLabelEdit += value;
            }
            remove { beforeLabelEdit -= value; }
        }

        public virtual void OnBeforeLabelEdit(TreeViewItemEventArgs e)
        {
            if (beforeLabelEdit != null)
                beforeLabelEdit(this, e);
        }
        #endregion

        #region AfterLabelEdit
        public const string AfterLabelEditEvent = "TreeView.AfterLabelEdit";

        EventHandler<TreeViewItemEventArgs> afterLabelEdit;

        public event EventHandler<TreeViewItemEventArgs> AfterLabelEdit
        {
            add
            {
                HandleEvent(AfterLabelEditEvent);
                afterLabelEdit += value;
            }
            remove { afterLabelEdit -= value; }
        }

        public virtual void OnAfterLabelEdit(TreeViewItemEventArgs e)
        {
            if (afterLabelEdit != null)
                afterLabelEdit(this, e);
        }
        #endregion
        
        #region NodeMouseClick
        public const string NodeMouseClickEvent = "TreeView.NodeMouseClick";

        EventHandler<TreeNodeMouseClickEventArgs> nodeMouseClick;

        public event EventHandler<TreeNodeMouseClickEventArgs> NodeMouseClick
        {
            add
            {
                HandleEvent(NodeMouseClickEvent);
                nodeMouseClick += value;
            }
            remove { nodeMouseClick -= value; }
        }

        public virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (nodeMouseClick != null)
                nodeMouseClick(this, e);
        }
        #endregion

        #region ItemDrag
        public const string ItemDragEvent = "TreeView.ItemDrag";

        EventHandler<ItemDragEventArgs> itemDrag;

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add
            {
                HandleEvent(ItemDragEvent);
                itemDrag += value;
            }
            remove { itemDrag -= value; }
        }

        public virtual void OnItemDrag(ItemDragEventArgs e)
        {
            if (itemDrag != null)
                itemDrag(this, e);
        }
        #endregion
			
		public const string ExpandingEvent = "TreeView.ExpandingEvent";

		event EventHandler<TreeViewItemCancelEventArgs> _Expanding;

		public event EventHandler<TreeViewItemCancelEventArgs> Expanding {
			add {
				_Expanding += value;
				HandleEvent (ExpandingEvent);
			}
			remove { _Expanding -= value; }
		}

		public virtual void OnExpanding (TreeViewItemCancelEventArgs e)
		{
			if (_Expanding != null)
				_Expanding (this, e);
		}

		public const string ExpandedEvent = "TreeView.ExpandedEvent";

		event EventHandler<TreeViewItemEventArgs> _Expanded;

		public event EventHandler<TreeViewItemEventArgs> Expanded {
			add {
				_Expanded += value;
				HandleEvent (ExpandedEvent);
			}
			remove { _Expanded -= value; }
		}

		public virtual void OnExpanded (TreeViewItemEventArgs e)
		{
			if (_Expanded != null)
				_Expanded (this, e);
		}
		
		public const string CollapsingEvent = "TreeView.CollapsingEvent";

		event EventHandler<TreeViewItemCancelEventArgs> _Collapsing;

		public event EventHandler<TreeViewItemCancelEventArgs> Collapsing {
			add {
				_Collapsing += value;
				HandleEvent (CollapsingEvent);
			}
			remove { _Collapsing -= value; }
		}

		public virtual void OnCollapsing (TreeViewItemCancelEventArgs e)
		{
			if (_Collapsing != null)
				_Collapsing (this, e);
		}
		
		public const string CollapsedEvent = "TreeView.CollapsedEvent";

		event EventHandler<TreeViewItemEventArgs> _Collapsed;

		public event EventHandler<TreeViewItemEventArgs> Collapsed {
			add {
				_Collapsed += value;
				HandleEvent (CollapsedEvent);
			}
			remove { _Collapsed -= value; }
		}

		public virtual void OnCollapsed (TreeViewItemEventArgs e)
		{
			if (_Collapsed != null)
				_Collapsed (this, e);
		}

		#endregion
		
		public TreeView () : this (Generator.Current)
		{
		}

		public TreeView (Generator g) : this (g, typeof(ITreeView))
		{
		}
		
		protected TreeView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ITreeView)Handler;
		}
		
		public ITreeItem SelectedItem {
			get { return handler.SelectedItem; }
            set
            {
                EnsureVisible(value);
                handler.SelectedItem = value;
            }
		}
		
		[Obsolete("Use DataStore property instead")]
		public ITreeStore TopNode {
			get { return this.DataStore; }
			set { this.DataStore = value; }
		}
		
		public ITreeStore DataStore {
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}

		public void RefreshData ()
		{
			handler.RefreshData ();
		}
		
		public void RefreshItem (ITreeItem item)
		{
			handler.RefreshItem (item);
		}
        public ITreeItem GetNodeAt(Point point)
        {
            return handler.GetNodeAt(point);
        }

        public bool LabelEdit
        {
            get { return handler.LabelEdit; }
            set { handler.LabelEdit = value; }
        }

        public bool IsExpanded(ITreeItem item)
        {
            return handler.IsExpanded(item);
        }

        public void Collapse(ITreeItem item)
        {
            handler.Collapse(item);
        }

        public void Expand(ITreeItem item)
        {
            handler.Expand(item);
        }

        public bool AllowDrop
        {
            get { return handler.AllowDrop; }
            set { handler.AllowDrop = value; }
        }

        public void Remove(ITreeItem item)
        {
            handler.Remove(item);
        }

        public void AddTo(ITreeItem dest, ITreeItem item)
        {
            handler.AddTo(
                dest,
                item);
        }

        public void SetImage(
            TreeItem item, 
            Image image)
        {
            handler.SetImage(
                item,
                image);
        }

        private void EnsureVisible(
            ITreeItem value)
        {
            if (value != null)
            {
                // expand the parents
                var items = new Stack<ITreeItem>();

                var temp = value;
                while (temp != null)
                {
                    // start with the parent
                    temp = temp.Parent;

                    if (temp != null)
                        items.Push(temp);
                }

                while (items.Count > 0)
                {
                    var item =
                        items.Pop();

                    if (item != null)
                        Expand(item);
                }
            }
        }
    }
}