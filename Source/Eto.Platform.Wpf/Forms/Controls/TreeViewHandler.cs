using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using swk = System.Windows.Markup;
using Eto.Forms;
using System.Collections;
using Eto.Platform.Wpf.Drawing;
using Eto.Platform.Wpf.Forms.Menu;
using Eto.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeViewHandler : WpfControl<swc.TreeView, TreeView>, ITreeView
	{
		ContextMenu contextMenu;
		ITreeStore topNode;
        ITreeItem selectedItem;

		public class EtoTreeViewItem : swc.TreeViewItem
		{
			public static readonly sw.RoutedEvent CollapsingEvent =
					sw.EventManager.RegisterRoutedEvent ("Collapsing",
					sw.RoutingStrategy.Bubble, typeof (sw.RoutedEventHandler),
					typeof (EtoTreeViewItem));
			public static readonly sw.RoutedEvent ExpandingEvent =
					sw.EventManager.RegisterRoutedEvent ("Expanding",
					sw.RoutingStrategy.Bubble, typeof (sw.RoutedEventHandler),
					typeof (EtoTreeViewItem));

			public event sw.RoutedEventHandler Collapsing
			{
				add { AddHandler (CollapsingEvent, value); }
				remove { RemoveHandler (CollapsingEvent, value); }
			}
			public event sw.RoutedEventHandler Expanding
			{
				add { AddHandler (ExpandingEvent, value); }
				remove { RemoveHandler (ExpandingEvent, value); }
			}

			bool cancelEvents;

			protected override void OnExpanded (sw.RoutedEventArgs e)
			{
				if (cancelEvents) return;
				var args = new sw.RoutedEventArgs (ExpandingEvent, this);
				OnExpanding (args);
				if (!args.Handled)
					base.OnExpanded (e);
				else {
					cancelEvents = true;
					this.IsExpanded = false;
					cancelEvents = false;
				}
			}
			protected override void OnCollapsed (sw.RoutedEventArgs e)
			{
				if (cancelEvents) return;
				var args = new sw.RoutedEventArgs (CollapsingEvent, this);
				OnCollapsing (args);
				if (!args.Handled)
					base.OnCollapsed (e);
				else {
					cancelEvents = true;
					this.IsExpanded = true;
					cancelEvents = false;
				}
			}

			protected virtual void OnCollapsing (sw.RoutedEventArgs e) { RaiseEvent (e); }

			protected virtual void OnExpanding (sw.RoutedEventArgs e) { RaiseEvent (e); }

			protected override sw.DependencyObject GetContainerForItemOverride ()
			{
				return new EtoTreeViewItem ();
			}

			protected override bool IsItemItsOwnContainerOverride (object item)
			{
				return item is EtoTreeViewItem;
			}
		}

		public class EtoTreeView : swc.TreeView
		{
			protected override sw.DependencyObject GetContainerForItemOverride ()
			{
				return new EtoTreeViewItem ();
			}

			protected override bool IsItemItsOwnContainerOverride (object item)
			{
				return item is EtoTreeViewItem;
			}
		}

        static sw.PropertyPath expandedProperty = PropertyPathHelper.Create("(Eto.Forms.ITreeItem`1<Eto.Forms.ITreeItem,Eto>,Eto.Expanded)");

		public TreeViewHandler ()
		{
			Control = new EtoTreeView ();
			var template = new sw.HierarchicalDataTemplate (typeof (ITreeItem));
			template.VisualTree = WpfListItemHelper.ItemTemplate ();
			template.ItemsSource = new swd.Binding { Converter = new WpfTreeItemHelper.ChildrenConverter() };
			Control.ItemTemplate = template;

            var style = new sw.Style (typeof (swc.TreeViewItem));
			//style.Setters.Add (new sw.Setter (swc.TreeViewItem.IsExpandedProperty, new swd.Binding { Converter = new WpfTreeItemHelper.IsExpandedConverter (), Mode = swd.BindingMode.OneWay }));
			style.Setters.Add (new sw.Setter (swc.TreeViewItem.IsExpandedProperty, new swd.Binding { Path = expandedProperty, Mode = swd.BindingMode.TwoWay }));
			Control.ItemContainerStyle = style;


			Control.SelectedItemChanged += (sender, e) => {
				Control.Dispatcher.BeginInvoke (new Action(() => {
					Widget.OnSelectionChanged (EventArgs.Empty);
				}));
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
			case TreeView.ExpandedEvent:
				Control.AddHandler (swc.TreeViewItem.ExpandedEvent, new sw.RoutedEventHandler ((sender, e) => {
					var treeItem = e.OriginalSource as swc.TreeViewItem;
					var item = treeItem.DataContext as ITreeItem;
					if (item != null) {
						Widget.OnExpanded (new TreeViewItemEventArgs (item));
					}
				}));
				break;
			case TreeView.ExpandingEvent:
				Control.AddHandler (EtoTreeViewItem.ExpandingEvent, new sw.RoutedEventHandler ((sender, e) => {
					var treeItem = e.OriginalSource as swc.TreeViewItem;
					var item = treeItem.DataContext as ITreeItem;
					if (item != null) {
						var args = new TreeViewItemCancelEventArgs (item);
						Widget.OnExpanding (args);
						e.Handled = args.Cancel;
					}
				}));
				break;
			case TreeView.CollapsedEvent:
				Control.AddHandler (swc.TreeViewItem.CollapsedEvent, new sw.RoutedEventHandler ((sender, e) => {
					var treeItem = e.OriginalSource as swc.TreeViewItem;
					var item = treeItem.DataContext as ITreeItem;
					if (item != null) {
						Widget.OnCollapsed (new TreeViewItemEventArgs (item));
					}
				}));
				break;
			case TreeView.CollapsingEvent:
				Control.AddHandler (EtoTreeViewItem.CollapsingEvent, new sw.RoutedEventHandler ((sender, e) => {
					var treeItem = e.OriginalSource as swc.TreeViewItem;
					var item = treeItem.DataContext as ITreeItem;
					if (item != null) {
						var args = new TreeViewItemCancelEventArgs (item);
						Widget.OnCollapsing (args);
						e.Handled = args.Cancel;
					}
				}));
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public ITreeStore DataStore
		{
			get { return topNode; }
			set
			{
				topNode = value;
				var source = WpfTreeItemHelper.GetChildren (topNode);
				if (Control.ItemsSource == source)
					Control.ItemsSource = null; // force a refresh
				Control.ItemsSource = source;
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
                if (!Control.IsLoaded)
                {
                    if (selectedItem == null && value != null)
                        Control.Loaded += HandleSelectedItemLoad;
                    selectedItem = value;
                }
				else SetSelected (Control, value);
			}
		}

        public void HandleSelectedItemLoad(object sender, sw.RoutedEventArgs e)
        {
            SetSelected(Control, selectedItem);
            selectedItem = null;
            Control.Loaded -= HandleSelectedItemLoad;
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

		public void RefreshData ()
		{
			Control.Items.Refresh ();
		}

		public void RefreshItem (ITreeItem item)
		{
			Control.Items.Refresh ();
		}
	}
}
