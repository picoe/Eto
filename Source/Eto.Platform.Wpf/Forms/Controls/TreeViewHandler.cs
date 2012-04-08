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
using Eto.Platform.Wpf.CustomControls.TreeGridView;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeViewHandler : WpfControl<swc.DataGrid, TreeView>, ITreeView
	{
		ContextMenu contextMenu;
		ITreeStore<ITreeItem> topNode;
		ColumnCollection columns;
		TreeController controller;

		public TreeViewHandler ()
		{
			Control = new swc.DataGrid {
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				SelectionMode = swc.DataGridSelectionMode.Single
			};
			controller = new TreeController ();
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

		public override void Initialize ()
		{
			base.Initialize ();
			columns = new ColumnCollection { Handler = this };
			columns.Register (Widget.Columns);
		}

		class ColumnCollection : EnumerableChangedHandler<TreeColumn, TreeColumnCollection>
		{
			public TreeViewHandler Handler { get; set; }

			public override void AddItem (TreeColumn item)
			{
				var colhandler = (TreeColumnHandler)item.Handler;
				Handler.Control.Columns.Add(colhandler.Control);
			}

			public override void InsertItem (int index, TreeColumn item)
			{
				var colhandler = (TreeColumnHandler)item.Handler;
				Handler.Control.Columns.Insert(index, colhandler.Control);
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Columns.RemoveAt(index);
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Columns.Clear ();
			}
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
				return Control.HeadersVisibility.HasFlag (swc.DataGridHeadersVisibility.Column);
			}
			set
			{
				Control.HeadersVisibility = value ? swc.DataGridHeadersVisibility.Column : swc.DataGridHeadersVisibility.None;
			}
		}
	}
}
