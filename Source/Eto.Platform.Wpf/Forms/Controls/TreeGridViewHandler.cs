﻿using System;
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
using Eto.Platform.CustomControls;
using Eto.Platform.Wpf.CustomControls.TreeGridView;
using System.Collections.ObjectModel;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<swc.DataGrid, TreeGridView>, ITreeGridView
	{
		TreeController controller;

		protected override IGridItem GetItemAtRow (int row)
		{
			if (controller == null) return null;
			return controller[row];
		}

		public override void Initialize ()
		{
			base.Initialize ();
			Control.Background = sw.SystemColors.WindowBrush;
			Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.None;
			Control.KeyDown += (sender, e) => {
				if (e.Key == sw.Input.Key.Enter) {
					if (SelectedItem != null)
						Widget.OnActivated (new TreeGridViewItemEventArgs (this.SelectedItem));
				}
			};
			Control.MouseDoubleClick += delegate {
				if (SelectedItem != null) {
					Widget.OnActivated (new TreeGridViewItemEventArgs (this.SelectedItem));
				}
			};
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeGridView.ExpandingEvent:
				controller.Expanding += (sender, e) => {
					Widget.OnExpanding (e);
				};
				break;
			case TreeGridView.ExpandedEvent:
				controller.Expanded += (sender, e) => {
					Widget.OnExpanded (e);
				};
				break;
			case TreeGridView.CollapsingEvent:
				controller.Collapsing += (sender, e) => {
					Widget.OnCollapsing (e);
				};
				break;
			case TreeGridView.CollapsedEvent:
				controller.Collapsed += (sender, e) => {
					Widget.OnCollapsed (e);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore
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

		public ITreeGridItem SelectedItem
		{
			get { return Control.SelectedItem as ITreeGridItem; }
			set { SetSelected (Control, value); }
		}

		public override sw.FrameworkElement SetupCell (IGridColumnHandler column, sw.FrameworkElement defaultContent)
		{
			if (object.ReferenceEquals (column, Columns.DataStore[0].Handler))
				return TreeToggleButton.Create (defaultContent, controller);
			else
				return defaultContent;
		}
	}
}
