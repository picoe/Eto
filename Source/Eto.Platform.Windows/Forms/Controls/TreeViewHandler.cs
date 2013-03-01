using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TreeViewHandler : WindowsControl<swf.TreeView, TreeView>, ITreeView
	{
		ITreeStore top;
		ContextMenu contextMenu;
		Dictionary<Image, string> images = new Dictionary<Image, string> ();
		static string EmptyName = Guid.NewGuid ().ToString ();
		
		public TreeViewHandler ()
		{
			this.Control = new swf.TreeView ();
			
			this.Control.BeforeExpand += delegate(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
				var item = e.Node.Tag as ITreeItem;
				if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Name == EmptyName)
				{
					PopulateNodes (e.Node.Nodes, item);
				}
			};
			this.Control.AfterSelect += delegate(object sender, System.Windows.Forms.TreeViewEventArgs e) {
				Widget.OnSelectionChanged (EventArgs.Empty);
			};
		}

		public ITreeStore DataStore {
			get { return top; }
			set {
				top = value;
				this.Control.ImageList = null;
				images.Clear ();
				PopulateNodes (this.Control.Nodes, top);
			}
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set {
				contextMenu = value;
				if (contextMenu != null)
					this.Control.ContextMenuStrip = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					this.Control.ContextMenuStrip = null;
			}
		}

        public override void AttachEvent(string handler)
        {
            switch (handler)
            {
                case TreeView.ExpandingEvent:
                    this.Control.BeforeExpand += (sender, e) =>
                    {
                        var args = new TreeViewItemCancelEventArgs(e.Node.Tag as ITreeItem);
                        Widget.OnExpanding(args);
                        e.Cancel = args.Cancel;
                    };
                    break;
                case TreeView.ExpandedEvent:
                    this.Control.AfterExpand += (sender, e) =>
                    {
                        Widget.OnExpanded(new TreeViewItemEventArgs(e.Node.Tag as ITreeItem));
                    };
                    break;
                case TreeView.CollapsingEvent:
                    this.Control.BeforeCollapse += (sender, e) =>
                    {
                        var args = new TreeViewItemCancelEventArgs(e.Node.Tag as ITreeItem);
                        Widget.OnCollapsing(args);
                        e.Cancel = args.Cancel;
                    };
                    break;
                case TreeView.CollapsedEvent:
                    this.Control.AfterCollapse += (sender, e) =>
                    {
                        Widget.OnCollapsed(new TreeViewItemEventArgs(e.Node.Tag as ITreeItem));
                    };
                    break;
			case TreeView.ActivatedEvent:
				this.Control.KeyDown += (sender, e) => {
					if (e.KeyData == swf.Keys.Return && this.SelectedItem != null)
					{
						Widget.OnActivated (new TreeViewItemEventArgs(this.SelectedItem));
						e.Handled = true;
					}
				};
				this.Control.DoubleClick += (sender, e) => {
					if (this.SelectedItem != null)
					{
						Widget.OnActivated (new TreeViewItemEventArgs (this.SelectedItem));
					}
				};
				break;
                case TreeView.AfterLabelEditEvent:
                    Control.AfterLabelEdit += (s, e) =>
                    {
                        this.Widget.OnAfterLabelEdit(
                            e.ToEto());
                    };
                    break;
                case TreeView.BeforeLabelEditEvent:
                    Control.BeforeLabelEdit += (s, e) =>
                    {
                        this.Widget.OnBeforeLabelEdit(
                            e.ToEto());
                    };
                    break;
                case TreeView.NodeMouseClickEvent:
                    Control.NodeMouseClick += (s, e) =>
                    {
                        this.Widget.OnNodeMouseClick(
                            e.ToEto());
                    };
                    break;
                    
                default:
                    base.AttachEvent(handler);
                    break;
            }
        }
		
		void PopulateNodes (System.Windows.Forms.TreeNodeCollection nodes, ITreeStore item)
		{
            nodes.Clear();

			var count = item.Count;
			for (int i=0; i<count; i++) {
				var child = item[i];

                var node =
                    new swf.TreeNode()
                    {
                        Text = child.Text,
                        Tag = child,
                    };

                child.InternalTag = node;

                // First set the handler
                child.Handler = node;

                // Add to the collection
                nodes.Add(node);

                SetImage(
                    child as TreeItem,
                    child.Image);
				
				if (child.Expandable) {
					if (child.Expanded) {
						PopulateNodes (node.Nodes, child);
						node.Expand ();
					} else {
						node.Nodes.Add (EmptyName, string.Empty);
					}
				}
			}
		}
		
		string GetImageKey (Image image)
		{
			if (image == null)
				return null;
			
			if (this.Control.ImageList == null)
				this.Control.ImageList = new System.Windows.Forms.ImageList{ ColorDepth = swf.ColorDepth.Depth32Bit };
			string key;
			if (!images.TryGetValue (image, out key)) {
				key = Guid.NewGuid ().ToString ();
				this.Control.ImageList.AddImage (image, key);
			}
			return key;
		}

		public ITreeItem SelectedItem {
			get {
				var node = this.Control.SelectedNode;
				if (node == null)
					return null;
				return node.Tag as ITreeItem;
			}
			set 
            {
                if (value != null)
                {
                    // now select the node
                    var node =
                        value.InternalTag
                        as swf.TreeNode;

                    if (node != null)
                        this.Control.SelectedNode =
                            node;
                }
			}
		}


        public ITreeItem GetNodeAt(Point targetPoint)
        {
            return 
                this.Control.GetNodeAt(
                    targetPoint.ToSD())
                    .ToEto();
        }


        public bool LabelEdit
        {
            get { return this.Control.LabelEdit; }
            set { this.Control.LabelEdit = value; }
        }

        public bool AllowDrop
        {
            get { return this.Control.AllowDrop; }
            set { this.Control.AllowDrop = value; }
        }

        public bool IsExpanded(ITreeItem item)
        {
            var treeNode =
                GetTreeNode(item);

            return treeNode != null
                && treeNode.IsExpanded;
        }

        public void Collapse(ITreeItem item)
        {
            var treeNode =
                GetTreeNode(item);

            if (treeNode != null)
                treeNode.Collapse();
        }

        public void Expand(ITreeItem item)
        {
            var treeNode =
                GetTreeNode(item);

            if (treeNode != null)
                treeNode.Expand();
        }

        public void Remove(ITreeItem item)
        {
            var treeNode =
                GetTreeNode(item);

            if (treeNode != null)
                treeNode.Remove();
        }

        public void AddTo(ITreeItem dest, ITreeItem item)
        {
            var treeNode =
                GetTreeNode(item);

            var destNode =
                GetTreeNode(dest);

            if (treeNode != null &&
                destNode != null)
                treeNode.Nodes.Add(
                    destNode);
        }

        private swf.TreeNode GetTreeNode(
            ITreeItem item)
        {
            return
                item != null
                ? item.Handler as swf.TreeNode
                : null;
        }


        public void SetImage(
            TreeItem item, 
            Image image)
        {
            if (item != null)
            {
                item.Image =
                    image;

                // set the image key on the TreeNode
                var node =
                    GetTreeNode(
                        item);

                // If the node has already 
                // been constructed, set its image
                // key as well.
                if (node != null)
                {
                    var imageKey =
                        GetImageKey(
                            image);

                    // A workaround for a bug in the 
                    // swf treeview. If a null key is specified,
                    // it uses the first image, unless an
                    // image index greater than the number of 
                    // images in the image list is specified
                    // http://stackoverflow.com/questions/261660
                    if (imageKey == null)
                    {
                        node.ImageIndex = 10000;
                        node.SelectedImageIndex = 10000;
                    }
                    else
                    {
                        node.ImageKey = imageKey;
                        node.SelectedImageKey = imageKey;
                    }
                }
            }
        }

		public void RefreshData ()
		{
			this.Control.ImageList = null;
			images.Clear ();
			Control.BeginUpdate ();
			PopulateNodes (this.Control.Nodes, top);
			Control.EndUpdate ();
		}

		public void RefreshItem (ITreeItem item)
		{
			var nodes = Control.Nodes.Find (item.Key, true);
			var node = nodes.FirstOrDefault(r => object.Equals(item, r));
			if (node != null) {
				node.Text = item.Text;
				PopulateNodes (node.Nodes, item);
				if (node.IsExpanded != item.Expanded)
				{
					if (item.Expanded)
						node.Expand ();
					else
						node.Collapse ();
				}
			}
		}
    }
}

