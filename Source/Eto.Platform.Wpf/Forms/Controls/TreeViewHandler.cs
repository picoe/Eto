using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using swk = System.Windows.Markup;
using swi = System.Windows.Input;
using Eto.Forms;
using System.Collections;
using Eto.Platform.Wpf.Drawing;
using Eto.Platform.Wpf.Forms.Menu;
using Eto.Drawing;
using System.ComponentModel;
using System.Reflection;
using Eto.Platform.Wpf.CustomControls;
using System.Threading.Tasks;
using System.IO;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TreeViewHandler : WpfControl<SelectableTreeView, TreeView>, ITreeView
	{
		ContextMenu contextMenu;
		ITreeStore topNode;
		ITreeItem selectedItem;
		bool labelEdit;
		// use two templates to refresh individual items by changing its template (hack? yes, fast? yes)
		sw.HierarchicalDataTemplate template1;
		sw.HierarchicalDataTemplate template2;

		public class EtoTreeViewItem : swc.TreeViewItem, INotifyPropertyChanged
		{
			public static readonly sw.RoutedEvent CollapsingEvent =
					sw.EventManager.RegisterRoutedEvent("Collapsing",
					sw.RoutingStrategy.Bubble, typeof(sw.RoutedEventHandler),
					typeof(EtoTreeViewItem));
			public static readonly sw.RoutedEvent ExpandingEvent =
					sw.EventManager.RegisterRoutedEvent("Expanding",
					sw.RoutingStrategy.Bubble, typeof(sw.RoutedEventHandler),
					typeof(EtoTreeViewItem));

			public event sw.RoutedEventHandler Collapsing
			{
				add { AddHandler(CollapsingEvent, value); }
				remove { RemoveHandler(CollapsingEvent, value); }
			}
			public event sw.RoutedEventHandler Expanding
			{
				add { AddHandler(ExpandingEvent, value); }
				remove { RemoveHandler(ExpandingEvent, value); }
			}

			public TreeViewHandler Handler
			{
				get { return this.GetParent<EtoTreeView>().Handler; }
			}

			public string Text
			{
				get
				{
					var item = DataContext as ITreeItem;
					if (item != null)
						return item.Text;
					return null;
				}
				set
				{
					var item = DataContext as ITreeItem;
					if (item != null)
						item.Text = value;
				}
			}

			bool isInEditMode;

			public bool IsInEditMode
			{
				get { return isInEditMode; }
				set
				{
					if (isInEditMode != value)
					{
						isInEditMode = value;
						OnPropertyChanged("IsInEditMode");
					}
				}
			}

			protected void OnPropertyChanged(string name)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(name));
			}

			bool cancelEvents;

			protected override void OnExpanded(sw.RoutedEventArgs e)
			{
				if (cancelEvents) return;
				var args = new sw.RoutedEventArgs(ExpandingEvent, this);
				OnExpanding(args);
				if (!args.Handled)
					base.OnExpanded(e);
				else
				{
					cancelEvents = true;
					this.IsExpanded = false;
					cancelEvents = false;
				}
			}

			protected override void OnCollapsed(sw.RoutedEventArgs e)
			{
				if (cancelEvents) return;
				var args = new sw.RoutedEventArgs(CollapsingEvent, this);
				OnCollapsing(args);
				if (!args.Handled)
					base.OnCollapsed(e);
				else
				{
					cancelEvents = true;
					this.IsExpanded = true;
					cancelEvents = false;
				}
			}

			protected virtual void OnCollapsing(sw.RoutedEventArgs e) { RaiseEvent(e); }

			protected virtual void OnExpanding(sw.RoutedEventArgs e) { RaiseEvent(e); }

			protected override sw.DependencyObject GetContainerForItemOverride()
			{
				return new EtoTreeViewItem();
			}

			protected override bool IsItemItsOwnContainerOverride(object item)
			{
				return item is EtoTreeViewItem;
			}

			protected override void OnKeyDown(swi.KeyEventArgs e)
			{
				base.OnKeyDown(e);

				if (e.Key == swi.Key.F2)
				{
					var etb = this.FindChild<EditableTextBlock>();
					if (etb != null && etb.IsEditable)
					{
						etb.IsInEditMode = true;
						e.Handled = true;
					}
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public class EtoTreeView : SelectableTreeView
		{
			public TreeViewHandler Handler { get; set; }

			protected override sw.DependencyObject GetContainerForItemOverride()
			{
				return new EtoTreeViewItem();
			}

			protected override bool IsItemItsOwnContainerOverride(object item)
			{
				return item is EtoTreeViewItem;
			}
		}

		static sw.PropertyPath expandedProperty = PropertyPathHelper.Create("(Eto.Forms.ITreeItem`1,Eto<Eto.Forms.ITreeItem,Eto>.Expanded)");

		public TreeViewHandler()
		{
			Control = new EtoTreeView { Handler = this };
			SetTemplate();

			var style = new sw.Style(typeof(swc.TreeViewItem));
			//style.Setters.Add (new sw.Setter (swc.TreeViewItem.IsExpandedProperty, new swd.Binding { Converter = new WpfTreeItemHelper.IsExpandedConverter (), Mode = swd.BindingMode.OneWay }));
			style.Setters.Add(new sw.Setter(swc.TreeViewItem.IsExpandedProperty, new swd.Binding { Path = expandedProperty, Mode = swd.BindingMode.OneTime }));
			Control.ItemContainerStyle = style;
		}

		void SetTemplate()
		{
			var source = new swd.RelativeSource(swd.RelativeSourceMode.FindAncestor, typeof(swc.TreeViewItem), 1);
			template1 = new sw.HierarchicalDataTemplate(typeof(ITreeItem));
			template1.VisualTree = WpfListItemHelper.ItemTemplate(LabelEdit, source);
			template1.ItemsSource = new swd.Binding { Converter = new WpfTreeItemHelper.ChildrenConverter() };
			Control.ItemTemplate = template1;

			template2 = new sw.HierarchicalDataTemplate(typeof(ITreeItem));
			template2.VisualTree = WpfListItemHelper.ItemTemplate(LabelEdit, source);
			template2.ItemsSource = new swd.Binding { Converter = new WpfTreeItemHelper.ChildrenConverter() };
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(TreeView.ExpandedEvent);
			HandleEvent(TreeView.CollapsedEvent);
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case TreeView.ExpandedEvent:
					Control.AddHandler(swc.TreeViewItem.ExpandedEvent, new sw.RoutedEventHandler((sender, e) =>
					{
						if (Control.Refreshing)
							return;
						var treeItem = e.OriginalSource as swc.TreeViewItem;
						var item = treeItem.DataContext as ITreeItem;
						if (item != null && item.Expandable && !item.Expanded)
						{
							item.Expanded = true;
							Widget.OnExpanded(new TreeViewItemEventArgs(item));
						}
					}));
					break;
				case TreeView.ExpandingEvent:
					Control.AddHandler(EtoTreeViewItem.ExpandingEvent, new sw.RoutedEventHandler((sender, e) =>
					{
						if (Control.Refreshing)
							return;
						var treeItem = e.OriginalSource as swc.TreeViewItem;
						var item = treeItem.DataContext as ITreeItem;
						if (item != null && item.Expandable && !item.Expanded)
						{
							var args = new TreeViewItemCancelEventArgs(item);
							Widget.OnExpanding(args);
							e.Handled = args.Cancel;
						}
					}));
					break;
				case TreeView.CollapsedEvent:
					Control.AddHandler(swc.TreeViewItem.CollapsedEvent, new sw.RoutedEventHandler((sender, e) =>
					{
						if (Control.Refreshing)
							return;
						var treeItem = e.OriginalSource as swc.TreeViewItem;
						var item = treeItem.DataContext as ITreeItem;
						if (item != null && item.Expandable && item.Expanded)
						{
							item.Expanded = false;
							Widget.OnCollapsed(new TreeViewItemEventArgs(item));
						}
					}));
					break;
				case TreeView.CollapsingEvent:
					Control.AddHandler(EtoTreeViewItem.CollapsingEvent, new sw.RoutedEventHandler((sender, e) =>
					{
						if (Control.Refreshing)
							return;
						var treeItem = e.OriginalSource as swc.TreeViewItem;
						var item = treeItem.DataContext as ITreeItem;
						if (item != null && item.Expandable && item.Expanded)
						{
							var args = new TreeViewItemCancelEventArgs(item);
							Widget.OnCollapsing(args);
							e.Handled = args.Cancel;
						}
					}));
					break;
				case TreeView.ActivatedEvent:
					Control.PreviewKeyDown += (sender, e) =>
					{
						if (!LabelEdit && e.Key == sw.Input.Key.Enter && SelectedItem != null)
						{
							Widget.OnActivated(new TreeViewItemEventArgs(this.SelectedItem));
							e.Handled = true;
						}
					};
					Control.PreviewMouseDoubleClick += (sender, e) =>
					{
						if (!LabelEdit && SelectedItem != null)
						{
							Widget.OnActivated(new TreeViewItemEventArgs(this.SelectedItem));
							e.Handled = true;
						}
					};
					break;
				case TreeView.SelectionChangedEvent:
					ITreeItem oldSelectedItem = null;
					Control.CurrentItemChanged += (sender, e) =>
					{
						Control.Dispatcher.BeginInvoke(new Action(() =>
						{
							selectedItem = null;
							var newSelected = this.SelectedItem;
							if (!object.ReferenceEquals(oldSelectedItem, newSelected))
							{
								Widget.OnSelectionChanged(EventArgs.Empty);
								RefreshItem(Control.CurrentTreeViewItem);
								oldSelectedItem = newSelected;
							}
						}));
					};
					break;
				case TreeView.BeforeLabelEditEvent:
					break;
				case TreeView.AfterLabelEditEvent:
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		void RefreshItem(swc.TreeViewItem item)
		{
			if (item == null)
				return;
			var old = item.DataContext;
			item.DataContext = null;
			item.DataContext = old;

			item.InvalidateProperty(EtoTreeViewItem.IsExpandedProperty);
			item.HeaderTemplate = item.HeaderTemplate == template1 ? template2 : template1;
		}

		public ITreeStore DataStore
		{
			get { return topNode; }
			set
			{
				topNode = value;
				var source = WpfTreeItemHelper.GetChildren(topNode);
				if (Control.ItemsSource == source)
					Control.ItemsSource = null; // force a refresh
				Control.ItemsSource = source;
			}
		}

		public ITreeItem SelectedItem
		{
			get { return selectedItem ?? Control.CurrentItem as ITreeItem; }
			set
			{
				if (!Control.IsLoaded)
				{
					if (selectedItem == null && value != null)
						Control.Loaded += HandleSelectedItemLoad;
					selectedItem = value;
				}
				else
					Control.CurrentItem = value;
			}
		}

		public void HandleSelectedItemLoad(object sender, sw.RoutedEventArgs e)
		{
			Control.CurrentItem = selectedItem;
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

		public void RefreshData()
		{
			Control.RefreshData();
		}

		public void RefreshItem(ITreeItem item)
		{
			Control.FindTreeViewItem(item).ContinueWith(r =>
				{
					if (r.IsCompleted)
					{
						var sel = this.SelectedItem;
						RefreshItem(r.Result);
						this.SelectedItem = sel;
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		public ITreeItem GetNodeAt(PointF point)
		{
			var item = Control.InputHitTest(point.ToWpf()) as sw.DependencyObject;
			if (item != null)
			{
				var tvi = item.GetParent<swc.TreeViewItem>();
				if (tvi != null)
				{
					return tvi.DataContext as ITreeItem;
				}
			}
			return null;
		}

		public bool LabelEdit
		{
			get { return labelEdit; }
			set
			{
				labelEdit = value;
				if (Control.IsLoaded)
				{
					var sel = SelectedItem;
					SetTemplate();
					SelectedItem = sel;
				}
				else
					SetTemplate();
			}
		}
	}
}
