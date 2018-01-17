using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using swd = Windows.UI.Xaml.Data;
using swm = Windows.UI.Xaml.Media;
using swk = Windows.UI.Xaml.Markup;
using swi = Windows.UI.Xaml.Input;
using wuc = Windows.UI.Core;
using Eto.Forms;
using System.Collections;
//using Eto.WinRT.Forms.Menu;
using Eto.Drawing;
using System.ComponentModel;
//using Eto.WinRT.CustomControls;
using System.Threading.Tasks;
using mwc = WinRTXamlToolkit.Controls;
using Windows.System;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Tree view handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>	
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TreeViewHandler : WpfControl<mwc.TreeView, TreeView, TreeView.ICallback>, TreeView.IHandler
	{
		ContextMenu contextMenu;
		ITreeStore topNode;
		ITreeItem selectedItem;
		sw.Setter foreground;

		bool labelEdit;
		// use two templates to refresh individual items by changing its template (hack? yes, fast? yes)
		//mwc.Data.HierarchicalDataTemplate template1;
		//mwc.Data.HierarchicalDataTemplate template2;

		public class EtoTreeViewItem : mwc.TreeViewItem, INotifyPropertyChanged
		{
#if TODO_XAML
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
#endif

			public TreeViewHandler Handler
			{
				get { return this.GetParent<EtoTreeView>().Handler; }
			}

			public string Text
			{
				get
				{
					var item = DataContext as ITreeItem;
					return item != null ? item.Text : null;
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


#if TODO_XAML
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
					IsExpanded = false;
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
					IsExpanded = true;
					cancelEvents = false;
				}
			}

			protected virtual void OnCollapsing(sw.RoutedEventArgs e) { RaiseEvent(e); }

			protected virtual void OnExpanding(sw.RoutedEventArgs e) { RaiseEvent(e); }
#endif
			protected override sw.DependencyObject GetContainerForItemOverride()
			{
				return new EtoTreeViewItem();
			}

			protected override bool IsItemItsOwnContainerOverride(object item)
			{
				return item is EtoTreeViewItem;
			}

			protected override void OnKeyDown(swi.KeyRoutedEventArgs e)
			{
				base.OnKeyDown(e);

				if (e.Key == VirtualKey.F2)
				{
#if TODO_XAML
					var etb = this.FindChild<EditableTextBlock>();
					if (etb != null && etb.IsEditable)
					{
						etb.IsInEditMode = true;
						e.Handled = true;
					}
#endif
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public class EtoTreeView : mwc.TreeView
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

#if TODO_XAML
		static readonly sw.PropertyPath expandedProperty = PropertyPathHelper.Create("(Eto.Forms.ITreeItem`1,Eto<Eto.Forms.ITreeItem,Eto>.Expanded)");
#endif

		public TreeViewHandler()
		{
			Control = new EtoTreeView { Handler = this };
			SetTemplate();

#if TODO_XAML
			var style = new sw.Style(typeof(mwc.TreeViewItem));
			//style.Setters.Add (new sw.Setter (mwc.TreeViewItem.IsExpandedProperty, new swd.Binding { Converter = new WpfTreeItemHelper.IsExpandedConverter (), Mode = swd.BindingMode.OneWay }));
			style.Setters.Add(new sw.Setter(mwc.TreeViewItem.IsExpandedProperty, new swd.Binding { Path = expandedProperty, Mode = swd.BindingMode.OneTime }));
			Control.ItemContainerStyle = style;
#endif
		}

		void SetTemplate()
		{
#if TODO_XAML
			var source = new swd.RelativeSource(swd.RelativeSourceMode.FindAncestor, typeof(mwc.TreeViewItem), 1);
			template1 = new mwc.Data.HierarchicalDataTemplate(typeof(ITreeItem));
			template1.VisualTree = WpfListItemHelper.ItemTemplate(LabelEdit, source);
			template1.ItemsSource = new swd.Binding { Converter = new WpfTreeItemHelper.ChildrenConverter() };
			Control.ItemTemplate = template1;


			template2 = new mwc.Data.HierarchicalDataTemplate(typeof(ITreeItem));
			template2.VisualTree = WpfListItemHelper.ItemTemplate(LabelEdit, source);
			template2.ItemsSource = new swd.Binding { Converter = new WpfTreeItemHelper.ChildrenConverter() };
#endif
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(TreeView.ExpandedEvent);
			HandleEvent(TreeView.CollapsedEvent);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
#if TODO_XAML
				case TreeView.ExpandedEvent:
					Control.AddHandler(mwc.TreeViewItem.ExpandedEvent, new sw.RoutedEventHandler((sender, e) =>
					{
						if (Control.Refreshing)
							return;
						var treeItem = e.OriginalSource as mwc.TreeViewItem;
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
						var treeItem = e.OriginalSource as mwc.TreeViewItem;
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
					Control.AddHandler(mwc.TreeViewItem.CollapsedEvent, new sw.RoutedEventHandler((sender, e) =>
					{
						if (Control.Refreshing)
							return;
						var treeItem = e.OriginalSource as mwc.TreeViewItem;
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
						var treeItem = e.OriginalSource as mwc.TreeViewItem;
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
							Widget.OnActivated(new TreeViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					Control.PreviewMouseDoubleClick += (sender, e) =>
					{
						if (!LabelEdit && SelectedItem != null)
						{
							Widget.OnActivated(new TreeViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					break;
				case TreeView.SelectionChangedEvent:
					ITreeItem oldSelectedItem = null;
					Control.CurrentItemChanged += (sender, e) => Control.Dispatcher.BeginInvoke(new Action(() =>
					{
						selectedItem = null;
						var newSelected = SelectedItem;
						if (!object.ReferenceEquals(oldSelectedItem, newSelected))
						{
							Widget.OnSelectionChanged(EventArgs.Empty);
							RefreshItem(Control.CurrentTreeViewItem);
							oldSelectedItem = newSelected;
						}
					}));
					break;
#endif
				case TreeView.LabelEditingEvent:
					break;
				case TreeView.LabelEditedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void RefreshItem(mwc.TreeViewItem item)
		{
			if (item == null)
				return;
			var old = item.DataContext;
			item.DataContext = null;
			item.DataContext = old;
#if TODO_XAML
			item.InvalidateProperty(EtoTreeViewItem.IsExpandedProperty);
			item.HeaderTemplate = item.HeaderTemplate == template1 ? template2 : template1;
#endif
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
			get { return selectedItem ?? Control.SelectedItem as ITreeItem; }
			set
			{
#if TODO_XAML // this section is probably not needed in WinRT
				if (!Control.IsLoaded)
				{
					if (selectedItem == null && value != null)
						Control.Loaded += HandleSelectedItemLoad;
					selectedItem = value;
				}
				else
					Control.CurrentItem = value;

#else
				// TODO: how to select a tree item in WinRT?
				// Control.SelectedItem = value;
#endif
			}
		}

		public void HandleSelectedItemLoad(object sender, sw.RoutedEventArgs e)
		{
#if TODO_XAML
			Control.SelectedItem = selectedItem;
#endif
			selectedItem = null;
			Control.Loaded -= HandleSelectedItemLoad;
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
#if TODO_XAML
				if (contextMenu != null)
					Control.ContextMenu = ((ContextMenuHandler)contextMenu.Handler).Control;
				else
					Control.ContextMenu = null;
#endif
			}
		}

		public void RefreshData()
		{
#if TODO_XAML
			Control.RefreshData();
#endif
		}

		public void RefreshItem(ITreeItem item)
		{
#if TODO_XAML
			Control.FindTreeViewItem(item).ContinueWith(r =>
				{
					if (r.IsCompleted)
					{
						var sel = SelectedItem;
						RefreshItem(r.Result);
						SelectedItem = sel;
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
#endif
		}


		public ITreeItem GetNodeAt(PointF point)
		{
#if TODO_XAML
			var item = Control.InputHitTest(point.ToWpf()) as sw.DependencyObject;
			if (item != null)
			{
				var tvi = item.GetParent<mwc.TreeViewItem>();
				if (tvi != null)
				{
					return tvi.DataContext as ITreeItem;
				}
			}
#endif
			return null;
		}

		public Color TextColor
		{
			get
			{
				if (foreground != null)
					return ((swm.Brush)foreground.Value).ToEtoColor();
#if TODO_XAML
				return sw.SystemColors.ControlTextColor.ToEto();
#else
				throw new NotImplementedException();
#endif
			}
			set
			{
				if (foreground == null)
				{
					foreground = new sw.Setter(mwc.TreeViewItem.ForegroundProperty, value.ToWpfBrush());
					Control.ItemContainerStyle.Setters.Add(foreground);
				}
				else
					foreground.Value = value.ToWpfBrush(foreground.Value as swm.Brush);
			}
		}

		public bool LabelEdit
		{
			get { return labelEdit; }
			set
			{
				labelEdit = value;
#if TODO_XAML
				if (Control.IsLoaded)
				{
					var sel = SelectedItem;
					SetTemplate();
					SelectedItem = sel;
				}
				else
#endif
					SetTemplate();
			}
		}
	}
}
