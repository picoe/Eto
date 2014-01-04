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
		readonly Dictionary<Image, string> images = new Dictionary<Image, string>();
		static readonly string EmptyName = Guid.NewGuid().ToString();
		bool ignoreExpandCollapseEvents = true;

		public TreeViewHandler()
		{
			this.Control = new swf.TreeView
			{
				HideSelection = false
			};

			this.Control.BeforeExpand += delegate(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
			{
				var item = e.Node.Tag as ITreeItem;
				if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Name == EmptyName)
				{
					PopulateNodes(e.Node.Nodes, item);
				}
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(TreeView.ExpandedEvent);
			HandleEvent(TreeView.CollapsedEvent);
		}

		public ITreeStore DataStore
		{
			get { return top; }
			set
			{
				top = value;
				Control.ImageList = null;
				images.Clear();
				Control.BeginUpdate();
				PopulateNodes(Control.Nodes, top);
				Control.EndUpdate();
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeView.ExpandingEvent:
					Control.BeforeExpand += (sender, e) =>
					{
						if (ignoreExpandCollapseEvents)
							return;
						var item = e.Node.Tag as ITreeItem;
						if (!item.Expanded)
						{
							var args = new TreeViewItemCancelEventArgs(item);
							Widget.OnExpanding(args);
							e.Cancel = args.Cancel;
						}
					};
					break;
				case TreeView.ExpandedEvent:
					Control.AfterExpand += (sender, e) =>
					{
						if (ignoreExpandCollapseEvents)
							return;
						var item = e.Node.Tag as ITreeItem;
						if (!item.Expanded)
						{
							item.Expanded = true;
							Widget.OnExpanded(new TreeViewItemEventArgs(item));
						}
					};
					break;
				case TreeView.CollapsingEvent:
					Control.BeforeCollapse += (sender, e) =>
					{
						if (ignoreExpandCollapseEvents)
							return;
						var item = e.Node.Tag as ITreeItem;
						if (item.Expanded)
						{
							var args = new TreeViewItemCancelEventArgs(item);
							Widget.OnCollapsing(args);
							e.Cancel = args.Cancel;
						}
					};
					break;
				case TreeView.CollapsedEvent:
					Control.AfterCollapse += (sender, e) =>
					{
						if (ignoreExpandCollapseEvents)
							return;
						var item = e.Node.Tag as ITreeItem;
						if (item.Expanded)
						{
							item.Expanded = false;
							Widget.OnCollapsed(new TreeViewItemEventArgs(item));
						}
					};
					break;
				case TreeView.ActivatedEvent:
					Control.KeyDown += (sender, e) =>
					{
						if (SelectedItem != null)
						{
							if (e.KeyData == swf.Keys.Return)
							{
								Widget.OnActivated(new TreeViewItemEventArgs(SelectedItem));
								e.Handled = true;
								if (LabelEdit)
									Control.SelectedNode.BeginEdit();
							}
							else if (e.KeyData == swf.Keys.F2 && LabelEdit)
							{
								Control.SelectedNode.BeginEdit();
							}
						}
					};

					Control.NodeMouseDoubleClick += (sender, e) =>
					{
						if (e.Button == swf.MouseButtons.Left && SelectedItem != null)
						{
							if (LabelEdit)
								Control.SelectedNode.BeginEdit();
							else
								Widget.OnActivated(new TreeViewItemEventArgs(SelectedItem));
						}
					};
					break;
				case TreeView.LabelEditedEvent:
					Control.AfterLabelEdit += (s, e) =>
					{
						var args = new TreeViewItemEditEventArgs(e.Node.Tag as ITreeItem, e.Label);
						Widget.OnLabelEdited(args);
						if (!args.Cancel)
							args.Item.Text = e.Label;
						e.CancelEdit = args.Cancel;
					};
					break;
				case TreeView.LabelEditingEvent:
					Control.BeforeLabelEdit += (s, e) =>
					{
						var args = new TreeViewItemCancelEventArgs(e.Node.Tag as ITreeItem);
						Widget.OnLabelEditing(args);
						e.CancelEdit = args.Cancel;
					};
					break;
				case TreeView.NodeMouseClickEvent:
					Control.NodeMouseClick += (s, e) => Widget.OnNodeMouseClick(new TreeViewItemEventArgs(e.Node.Tag as ITreeItem));
					break;
				case TreeView.SelectionChangedEvent:
					Control.AfterSelect += (sender, e) => Widget.OnSelectionChanged(EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void PopulateNodes(System.Windows.Forms.TreeNodeCollection nodes, ITreeStore item)
		{
			if (Widget.Loaded)
				ignoreExpandCollapseEvents = true;
			PerformPopulateNodes(nodes, item);
			if (Widget.Loaded)
				ignoreExpandCollapseEvents = false;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			ignoreExpandCollapseEvents = false;
		}

		void PerformPopulateNodes(System.Windows.Forms.TreeNodeCollection nodes, ITreeStore item)
		{
			nodes.Clear();
			var count = item.Count;
			for (int i = 0; i < count; i++)
			{
				var child = item[i];
				var node = new swf.TreeNode
				{
					Text = child.Text,
					Name = child.Key,
					Tag = child,
				};
				SetImage(child, node);

				if (child.Expandable)
				{
					if (child.Expanded)
					{
						PerformPopulateNodes(node.Nodes, child);
						node.Expand();
					}
					else
					{
						node.Nodes.Add(EmptyName, string.Empty);
					}
				}

				nodes.Add(node);
			}
		}

		string GetImageKey(Image image)
		{
			if (image == null)
				return null;

			if (Control.ImageList == null)
				Control.ImageList = new System.Windows.Forms.ImageList { ColorDepth = swf.ColorDepth.Depth32Bit };
			string key;
			if (!images.TryGetValue(image, out key))
			{
				key = Guid.NewGuid().ToString();
				Control.ImageList.AddImage(image, key);
			}
			return key;
		}

		public ITreeItem SelectedItem
		{
			get
			{
				var node = Control.SelectedNode;
				return node == null ? null : node.Tag as ITreeItem;
			}
			set
			{
				if (value != null)
				{
					// now select the node
					var node = GetTreeNode(value);

					if (node != null)
						Control.SelectedNode = node;
				}
			}
		}

		public ITreeItem GetNodeAt(PointF targetPoint)
		{
			var item = Control.GetNodeAt(targetPoint.ToSDPoint());
			return item != null ? item.Tag as ITreeItem : null;
		}

		public Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public bool LabelEdit
		{
			get { return Control.LabelEdit; }
			set { Control.LabelEdit = value; }
		}

		public bool AllowDrop
		{
			get { return Control.AllowDrop; }
			set { Control.AllowDrop = value; }
		}

		public void AddTo(ITreeItem dest, ITreeItem item)
		{
			var treeNode = GetTreeNode(item);

			var destNode = GetTreeNode(dest);

			if (treeNode != null && destNode != null)
				treeNode.Nodes.Add(destNode);
		}

		IEnumerable<swf.TreeNode> EnumerateNodes(swf.TreeNodeCollection nodes)
		{
			foreach (swf.TreeNode node in nodes)
			{
				yield return node;
				foreach (var child in EnumerateNodes(node.Nodes))
					yield return child;
			}
		}

		swf.TreeNode GetTreeNode(ITreeItem item)
		{
			if (item.Key == null)
			{
				return EnumerateNodes(Control.Nodes).FirstOrDefault(r => object.ReferenceEquals(r.Tag, item));
			}
			else
			{
				var nodes = Control.Nodes.Find(item.Key, true);
				return nodes.FirstOrDefault(r => object.ReferenceEquals(item, r.Tag));
			}
		}

		void SetImage(ITreeItem item, swf.TreeNode node)
		{
			if (item != null)
			{
				// set the image key on the TreeNode
				node = node ?? GetTreeNode(item);

				// If the node has already 
				// been constructed, set its image
				// key as well.
				if (node != null)
				{
					var imageKey = GetImageKey(item.Image);

					// A workaround for a bug in the 
					// swf treeview. If a null key is specified,
					// it uses the first image, unless an
					// image index greater than the number of 
					// images in the image list is specified
					// http://stackoverflow.com/questions/261660
					if (imageKey == null)
					{
						node.ImageIndex = Int32.MaxValue;
						node.SelectedImageIndex = Int32.MaxValue;
					}
					else
					{
						node.ImageKey = imageKey;
						node.SelectedImageKey = imageKey;
					}
				}
			}
		}

		public void RefreshData()
		{
			Control.BeginUpdate();
			Control.ImageList = null;
			images.Clear();
			PopulateNodes(Control.Nodes, top);
			Control.EndUpdate();
		}

		public void RefreshItem(ITreeItem item)
		{
			var node = GetTreeNode(item);
			if (node != null)
			{
				Control.BeginUpdate();
				var selected = SelectedItem;
				node.Text = item.Text;
				SetImage(item, node);
				PopulateNodes(node.Nodes, item);
				if (node.IsExpanded != item.Expanded)
				{
					if (item.Expanded)
						node.Expand();
					else
						node.Collapse();
				}
				SelectedItem = selected;
				Control.EndUpdate();
			}
			else
				RefreshData();

		}
	}
}

