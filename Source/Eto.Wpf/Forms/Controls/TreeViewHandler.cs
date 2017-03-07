using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using swk = System.Windows.Markup;
using swi = System.Windows.Input;
using Eto.Forms;
using System.Collections;
using Eto.Wpf.Forms.Menu;
using Eto.Drawing;
using System.ComponentModel;
using Eto.Wpf.CustomControls;
using System.Threading.Tasks;

namespace Eto.Wpf.Forms.Controls
{
	[Obsolete("Since 2.4. TreeView is deprecated, please use TreeGridView instead.")]
	public class TreeViewHandler : WpfControl<TreeViewHandler.EtoTreeView, TreeView, TreeView.ICallback>, TreeView.IHandler
	{
		ContextMenu contextMenu;
		ITreeStore topNode;
		ITreeItem selectedItem;
		sw.Setter foreground;

		bool labelEdit;
		// use two templates to refresh individual items by changing its template (hack? yes, fast? yes)
		sw.HierarchicalDataTemplate template1;
		sw.HierarchicalDataTemplate template2;

		protected override sw.Size DefaultSize => new sw.Size(100, 100);

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
			public static readonly sw.RoutedEvent LabelEditingEvent =
					sw.EventManager.RegisterRoutedEvent("LabelEditing",
					sw.RoutingStrategy.Direct, typeof(sw.RoutedEventHandler),
					typeof(EtoTreeViewItem));
			public static readonly sw.RoutedEvent LabelEditedEvent =
					sw.EventManager.RegisterRoutedEvent("LabelEdited",
					sw.RoutingStrategy.Direct, typeof(sw.RoutedEventHandler),
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

			public event sw.RoutedEventHandler LabelEditing
			{
				add { AddHandler(LabelEditingEvent, value); }
				remove { RemoveHandler(LabelEditingEvent, value); }
			}

			public event sw.RoutedEventHandler LabelEdited
			{
				add { AddHandler(LabelEditedEvent, value); }
				remove { RemoveHandler(LabelEditedEvent, value); }
			}

			public TreeViewHandler Handler
			{
				get { return this.GetVisualParent<EtoTreeView>().Handler; }
			}

			public EtoTreeViewItem()
			{
				LabelEditing += HandleLabelEditing;
				LabelEdited += HandleLabelEdited;
			}

			void HandleLabelEdited(object sender, sw.RoutedEventArgs e)
			{
				var args = new TreeViewItemEditEventArgs(DataContext as ITreeItem, text);
				Handler.Callback.OnLabelEdited(Handler.Widget, args);
				if (!args.Cancel)
				{
					var item = DataContext as ITreeItem;
					if (item != null)
					{
						item.Text = text;
					}
				}
				OnPropertyChanged("Text");
                e.Handled = args.Cancel;
			}

			void HandleLabelEditing(object sender, sw.RoutedEventArgs e)
			{
				var args = new TreeViewItemCancelEventArgs(DataContext as ITreeItem);
				Handler.Callback.OnLabelEditing(Handler.Widget, args);
				e.Handled = args.Cancel;
			}

			string text;

			public string Text
			{
				get
				{
					var item = DataContext as ITreeItem;
					return item != null ? item.Text : null;
				}
				set
				{
					text = value;
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

			protected override sw.Size MeasureOverride(sw.Size availableSize)
			{
				if (IsLoaded)
					availableSize = availableSize.IfInfinity(Handler.UserPreferredSize.IfNaN(Handler.DefaultSize));
				return Handler?.MeasureOverride(availableSize, base.MeasureOverride) ?? base.MeasureOverride(availableSize);
			}
		}

		static readonly sw.PropertyPath expandedProperty = PropertyPathHelper.Create("(Eto.Forms.ITreeItem`1,Eto<Eto.Forms.ITreeItem,Eto>.Expanded)");

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

		public override void AttachEvent(string id)
		{
			switch (id)
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
							Callback.OnExpanded(Widget, new TreeViewItemEventArgs(item));
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
							Callback.OnExpanding(Widget, args);
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
							Callback.OnCollapsed(Widget, new TreeViewItemEventArgs(item));
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
							Callback.OnCollapsing(Widget, args);
							e.Handled = args.Cancel;
						}
					}));
					break;
				case TreeView.ActivatedEvent:
					Control.PreviewKeyDown += (sender, e) =>
					{
						if (!LabelEdit && e.Key == sw.Input.Key.Enter && SelectedItem != null)
						{
							Callback.OnActivated(Widget, new TreeViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					Control.PreviewMouseDoubleClick += (sender, e) =>
					{
						if (!LabelEdit && SelectedItem != null)
						{
							Callback.OnActivated(Widget, new TreeViewItemEventArgs(SelectedItem));
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
							Callback.OnSelectionChanged(Widget, EventArgs.Empty);
							RefreshItem(Control.CurrentTreeViewItem);
							oldSelectedItem = newSelected;
						}
					}));
					break;
				case TreeView.LabelEditingEvent:
					break;
				case TreeView.LabelEditedEvent:
					break;
				default:
					base.AttachEvent(id);
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
						var sel = SelectedItem;
						RefreshItem(r.Result);
						SelectedItem = sel;
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		public ITreeItem GetNodeAt(PointF point)
		{
			var item = Control.InputHitTest(point.ToWpf()) as sw.DependencyObject;
			if (item != null)
			{
				var tvi = item.GetVisualParent<swc.TreeViewItem>();
				if (tvi != null)
				{
					return tvi.DataContext as ITreeItem;
				}
			}
			return null;
		}

		public override Color TextColor
		{
			get
			{
				if (foreground != null)
					return ((swm.Brush)foreground.Value).ToEtoColor();
				return sw.SystemColors.ControlTextColor.ToEto();
			}
			set
			{
				if (foreground == null)
				{
					foreground = new sw.Setter(swc.TreeViewItem.ForegroundProperty, value.ToWpfBrush());
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
