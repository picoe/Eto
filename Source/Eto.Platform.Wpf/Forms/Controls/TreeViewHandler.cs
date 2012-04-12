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
using System.Collections.ObjectModel;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeViewHandler : WpfControl<swc.DataGrid, TreeView>, ITreeView, IDataGridHandler
	{
		ContextMenu contextMenu;
		ColumnCollection columns;
		TreeController controller;

		public TreeViewHandler ()
		{
			Control = new swc.DataGrid {
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				GridLinesVisibility = swc.DataGridGridLinesVisibility.None,
				Background = sw.SystemColors.WindowBrush,
				SelectionMode = swc.DataGridSelectionMode.Single
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

		public override void AttachEvent (string handler)
		{
			switch (handler) {
/*			case GridView.BeginCellEditEvent:
				Control.PreparingCellForEdit += (sender, e) => {
					var row = e.Row.GetIndex ();
					var item = store[row];
					var gridColumn = Widget.Columns[e.Column.DisplayIndex];
					Widget.OnBeginCellEdit (new GridViewCellArgs (gridColumn, row, e.Column.DisplayIndex, item));
				};
				break;
			case GridView.EndCellEditEvent:
				Control.CellEditEnding += (sender, e) => {
					var row = e.Row.GetIndex ();
					var item = store[row];
					var gridColumn = Widget.Columns[e.Column.DisplayIndex];
					Widget.OnEndCellEdit (new GridViewCellArgs (gridColumn, row, e.Column.DisplayIndex, item));
				};
				break;*/
			case TreeView.SelectionChangedEvent:
				Control.SelectedCellsChanged += (sender, e) => {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
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
				colhandler.Setup (Handler);
				Handler.Control.Columns.Add(colhandler.Control);
			}

			public override void InsertItem (int index, TreeColumn item)
			{
				var colhandler = (TreeColumnHandler)item.Handler;
				colhandler.Setup (Handler);
				Handler.Control.Columns.Insert (index, colhandler.Control);
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
			get { return controller != null ? controller.Store : null; }
			set
			{
				controller = new TreeController{ Store = value };
				controller.InitializeItems();
				Control.ItemsSource = controller;
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
			get { return Control.SelectedItem as ITreeItem; }
			set { SetSelected (Control, value); }
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
			get { return Control.HeadersVisibility.HasFlag (swc.DataGridHeadersVisibility.Column); }
			set { Control.HeadersVisibility = value ? swc.DataGridHeadersVisibility.Column : swc.DataGridHeadersVisibility.None; }
		}

		public sw.FrameworkElement SetupCell (IDataColumnHandler column, sw.FrameworkElement defaultContent)
		{
			if (object.ReferenceEquals (column, columns.DataStore[0].Handler))
				return TreeToggleButton.Create (defaultContent, controller);
			else
				return defaultContent;
		}
	}
}
