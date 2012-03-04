using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Forms;
using System.Collections;
using Eto.Platform.Wpf.Drawing;
using Eto.Platform.Wpf.Forms.Menu;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeViewHandler : WpfControl<swc.TreeView, TreeView>, ITreeView
	{
		ContextMenu contextMenu;
		ITreeStore<ITreeItem> topNode;

		public TreeViewHandler ()
		{
			Control = new swc.TreeView ();
			var template = new sw.HierarchicalDataTemplate (typeof (ITreeItem));
			template.VisualTree = WpfListItemHelper.ItemTemplate ();
			template.ItemsSource = new swd.Binding { Converter = new WpfTreeItemHelper.ChildrenConverter() };
			Control.ItemTemplate = template;

			var style = new sw.Style (typeof (swc.TreeViewItem));
			style.Setters.Add (new sw.Setter (swc.TreeViewItem.IsExpandedProperty, new swd.Binding { Converter = new WpfTreeItemHelper.IsExpandedConverter (), Mode = swd.BindingMode.OneWay }));
			Control.ItemContainerStyle = style;

			Control.SelectedItemChanged += delegate {
				Widget.OnSelectionChanged (EventArgs.Empty);
			};

			Control.KeyDown += (sender, e) => {
				if (e.Key == sw.Input.Key.Enter) {
					if (SelectedItem != null)
						Widget.OnActivated (new TreeViewItemEventArgs (this.SelectedItem));
				}
			};
			Control.MouseDoubleClick += delegate {
				if (SelectedItem != null) {
					Widget.OnActivated (new TreeViewItemEventArgs (this.SelectedItem));
				}
			};
		}


		public ITreeStore<ITreeItem> DataStore
		{
			get { return topNode; }
			set
			{
				topNode = value;
				Control.ItemsSource = WpfTreeItemHelper.GetChildren(topNode); //.OfType<ITreeItem>().ToArray();
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
				return Control.SelectedItem as ITreeItem;
			}
			set
			{
				SetSelected (Control, value);
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				if (contextMenu != null)
					Control.ContextMenu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					Control.ContextMenu = null;
			}
		}


		public bool ShowHeader
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}
	}
}
