using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;
using System.Collections;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeViewHandler : WpfControl<swc.TreeView, TreeView>, ITreeView
	{
		ITreeItem topNode;

		class MyTreeNode
		{
			public ITreeItem Item { get; set; }

			public MyTreeNode (ITreeItem item)
			{
				this.Item = item;
			}

			public string Text
			{
				get { return Item.Text; }
			}

			public bool IsExpanded
			{
				get { return Item.Expanded; }
				set { Item.Expanded = value; }
			}

			public IEnumerable Children
			{
				get
				{
					if (Item == null)
						yield break;
					for (int i = 0; i < Item.Count; i++) {
						yield return new MyTreeNode (Item.GetChild (i));
					}
				}
			}

			public override string ToString ()
			{
				return Item.Text;
			}

		}

		public TreeViewHandler ()
		{
			Control = new swc.TreeView ();
			var template = new sw.HierarchicalDataTemplate (typeof (MyTreeNode));
			var labelFactory = new sw.FrameworkElementFactory (typeof (swc.TextBlock));
			labelFactory.SetBinding (swc.TextBlock.TextProperty, new swd.Binding ("Text"));
			template.VisualTree = labelFactory;
			template.ItemsSource = new swd.Binding ("Children");
			Control.ItemTemplate = template;

			var style = new sw.Style (typeof (swc.TreeViewItem));
			style.Setters.Add (new sw.Setter (swc.TreeViewItem.IsExpandedProperty, new swd.Binding ("IsExpanded") { Mode = swd.BindingMode.TwoWay }));
			Control.ItemContainerStyle = style;

			Control.SelectedItemChanged += delegate {
				Widget.OnSelectionChanged (EventArgs.Empty);
			};
		}

		public ITreeItem TopNode
		{
			get { return topNode; }
			set
			{
				topNode = value;
				Control.ItemsSource = new MyTreeNode (value).Children;
			}
		}

		static private bool SetSelected (swc.ItemsControl parent, object child)
		{

			if (parent == null || child == null) {
				return false;
			}

			var childNode = parent.ItemContainerGenerator.ContainerFromItem (child) as swc.TreeViewItem;

			if (childNode != null) {
				childNode.Focus ();
				return childNode.IsSelected = true;
			}

			if (parent.Items.Count > 0) {
				foreach (object childItem in parent.Items) {
					var childControl = parent.ItemContainerGenerator.ContainerFromItem (childItem) as swc.ItemsControl;

					if (SetSelected (childControl, child)) {
						return true;
					}
				}
			}

			return false;
		}

		public ITreeItem SelectedItem
		{
			get
			{
				var node = Control.SelectedItem as MyTreeNode;
				return node.Item;
			}
			set
			{
				SetSelected (Control, value);
			}
		}
	}
}
