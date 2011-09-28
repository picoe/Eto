using System;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TreeViewHandler : WindowsControl<SWF.TreeView, TreeView>, ITreeView
	{
		ITreeItem top;
		Dictionary<Image, string> images = new Dictionary<Image, string> ();
		
		public TreeViewHandler ()
		{
			this.Control = new SWF.TreeView ();
			
			this.Control.BeforeExpand += delegate(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
				var item = e.Node.Tag as ITreeItem;
				if (e.Node.Nodes.Count == 1 && e.Node.Nodes [0].Name == "empty") {
					e.Node.Nodes.Clear ();
					PopulateNodes (e.Node.Nodes, item);
				}
			};
			this.Control.AfterSelect += delegate(object sender, System.Windows.Forms.TreeViewEventArgs e) {
				Widget.OnSelectionChanged (EventArgs.Empty);
			};
		}

		public ITreeItem TopNode {
			get { return top; }
			set {
				top = value;
				this.Control.ImageList = null;
				images.Clear ();
				PopulateNodes (this.Control.Nodes, top);
			}
		}
		
		void PopulateNodes (System.Windows.Forms.TreeNodeCollection nodes, ITreeItem item)
		{
			var count = item.Count;
			for (int i=0; i<count; i++) {
				var child = item.GetChild (i);
				var node = nodes.Add (child.Key, child.Text, GetImageKey (child.Image));
				node.Tag = child;
				
				if (child.Expandable) {
					if (child.Expanded) {
						PopulateNodes (node.Nodes, child);
						node.Expand ();
					} else {
						node.Nodes.Add ("empty", string.Empty);
					}
				}
			}
		}
		
		string GetImageKey (Image image)
		{
			if (image == null)
				return null;
			
			if (this.Control.ImageList == null)
				this.Control.ImageList = new System.Windows.Forms.ImageList{ ColorDepth = SWF.ColorDepth.Depth32Bit };
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
			set {
				// TODO: finish this
				var nodes = this.Control.Nodes.Find (value.Key, true);
				if (nodes.Length > 0)
					this.Control.SelectedNode = nodes [0];
			}
		}
	}
}

