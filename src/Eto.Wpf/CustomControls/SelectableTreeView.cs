using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Eto.Wpf.CustomControls
{
	public class SelectableTreeView : TreeView
	{

		static SelectableTreeView ()
		{
		}

		public SelectableTreeView ()
		{
			this.SelectedItemChanged += TreeViewItemChanged;
		}

		public static DependencyProperty CurrentTreeViewItemProperty = DependencyProperty.RegisterAttached(
			"CurrentTreeViewItem", typeof(TreeViewItem), typeof(SelectableTreeView),
			new PropertyMetadata());

		public static TreeViewItem GetCurrentTreeViewItem(TreeView treeView)
		{
			return (TreeViewItem)treeView.GetValue(CurrentTreeViewItemProperty);
		}

		public static void SetCurrentTreeViewItem(TreeView treeView, TreeViewItem value)
		{
			treeView.SetValue(CurrentTreeViewItemProperty, value);
		}

		public TreeViewItem CurrentTreeViewItem
		{
			get { return GetCurrentTreeViewItem(this); }
			set { SetCurrentTreeViewItem(this, value); }
		}


		public static RoutedEvent CurrentItemChangedEvent = EventManager.RegisterRoutedEvent (
			"CurrentItemChangedEvent", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<object>), typeof (SelectableTreeView)
			);

		public event RoutedPropertyChangedEventHandler<object> CurrentItemChanged
		{
			add { AddHandler (SelectableTreeView.CurrentItemChangedEvent, value); }
			remove { RemoveHandler(SelectableTreeView.CurrentItemChangedEvent, value); }
		}

		protected virtual void OnCurrentItemChanged (RoutedPropertyChangedEventArgs<object> e)
		{
			RaiseEvent(e);
		}

		public static DependencyProperty CurrentItemProperty = DependencyProperty.RegisterAttached(
			"CurrentItem", typeof(object), typeof(SelectableTreeView),
			new PropertyMetadata(new object(), OnCurrentItemChanged));

		public static object GetCurrentItem (TreeView treeView)
		{
			return treeView.GetValue (CurrentItemProperty);
		}

		public static void SetCurrentItem (TreeView treeView, object value)
		{
			treeView.SetValue (CurrentItemProperty, value);
		}

		public object CurrentItem
		{
			get { return GetCurrentItem (this); }
			set { SetCurrentItem (this, value); }
		}

        public bool Refreshing
        {
            get { return refreshing; }
        }

		bool refreshing;
		public void RefreshData ()
		{
			var selectedItem = CurrentItem;
			refreshing = true;
			Items.Refresh ();
			refreshing = false;
			if (IsLoaded)
			{
				SetSelected(selectedItem);
			}
			else
			{
				Loaded += (sender, e) => SetSelected(selectedItem);
			}
            //this.CurrentItem = selectedItem;
		}

		void SetSelected(object selectedItem)
		{
			FindItem(selectedItem, this).ContinueWith(t =>
			{
				if (t.Result != null)
				{
					t.Result.IsSelected = true;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		protected override void OnItemsChanged (System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (!refreshing)
				base.OnItemsChanged (e);
		}

		static void OnCurrentItemChanged (DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			var treeView = d as SelectableTreeView;
			if (treeView == null)
				return;
			treeView.SelectedItemChanged -= TreeViewItemChanged;
			var treeViewItem = SelectTreeViewItemForBinding (args.NewValue, treeView);
			var newValue = args.NewValue;
			if (treeViewItem != null)
			{
				treeViewItem.IsSelected = true;
				treeView.CurrentTreeViewItem = treeViewItem;
			}
			else
			{
				newValue = treeView.SelectedItem;
				SetCurrentItem(treeView, newValue);
			}
			treeView.SelectedItemChanged += TreeViewItemChanged;
			treeView.OnCurrentItemChanged (new RoutedPropertyChangedEventArgs<object>(args.OldValue, newValue, CurrentItemChangedEvent));
		}

		static void TreeViewItemChanged (object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var treeView = (SelectableTreeView)sender;
			if (!treeView.refreshing)
				treeView.SetValue (CurrentItemProperty, e.NewValue);
		}

		class Helper
		{
			readonly Dictionary<ItemContainerGenerator, ItemsControl> items = new Dictionary<ItemContainerGenerator, ItemsControl> ();
			readonly TaskCompletionSource<TreeViewItem> completion = new TaskCompletionSource<TreeViewItem> ();
			bool completed;

			public object SelectedItem { get; set; }

			public Task<TreeViewItem> Task { get { return completion.Task; } }

			public TaskCompletionSource<TreeViewItem> Completion { get { return completion; } }

			void AddGenerator (ItemsControl ic)
			{
				items.Add (ic.ItemContainerGenerator, ic);
				ic.ItemContainerGenerator.StatusChanged += HandleStatusChanged;
			}

			void HandleStatusChanged (object sender, EventArgs e)
			{
				var generator = sender as ItemContainerGenerator;
				if (generator != null && generator.Status == GeneratorStatus.ContainersGenerated)
				{
					generator.StatusChanged -= HandleStatusChanged;
					ItemsControl ic;
					if (items.TryGetValue (generator, out ic))
					{
						Seek (ic);
						items.Remove (generator);
					}
					if (items.Count == 0)
						Complete (null);
				}
			}

			void Complete (TreeViewItem item)
			{
				if (!completed)
				{
					completed = true;
					Completion.SetResult (item);
					Unwind ();
				}
			}

			void Unwind ()
			{
				foreach (var generator in items.Keys)
				{
					generator.StatusChanged -= HandleStatusChanged;
				}
				items.Clear ();
			}

			public void Find (ItemsControl ic)
			{
				Seek (ic);
				if (items.Count == 0)
					Complete (null);
			}

			bool Seek (ItemsControl ic)
			{
				if (ic == null || !ic.HasItems)
					return false;
				var generator = ic.ItemContainerGenerator;
				if (generator.Status == GeneratorStatus.ContainersGenerated)
				{
					foreach (var item in ic.Items)
					{
						var container = generator.ContainerFromItem (item) as TreeViewItem;
						if (item == SelectedItem && container != null)
						{
							Complete (container);
							return true;
						}
						if (Seek (container))
							return true;
					}
				}
				else
				{
					AddGenerator (ic);
				}
				return false;
			}
		}

		public Task<TreeViewItem> FindTreeViewItem(object dataItem)
		{
			return FindItem(dataItem, this);
		}

		static Task<TreeViewItem> FindItem (object dataItem, ItemsControl ic)
		{
			var helper = new Helper { SelectedItem = dataItem };

			helper.Find (ic);

			return helper.Task;
		}

		static TreeViewItem SelectTreeViewItemForBinding (Helper helper, ItemsControl ic, object dataItem)
		{
			if (ic == null || dataItem == null || !ic.HasItems)
				return null;
			IItemContainerGenerator generator = ic.ItemContainerGenerator;
			if (ic.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				foreach (var t in ic.Items)
				{
					var tvi = ic.ItemContainerGenerator.ContainerFromItem (t);
					if (t == dataItem)
						return tvi as TreeViewItem;

					var tmp = SelectTreeViewItemForBinding (dataItem, tvi as ItemsControl);
					if (tmp != null)
						return tmp;
				}
			}
			else
				using (generator.StartAt (generator.GeneratorPositionFromIndex (-1), GeneratorDirection.Forward))
				{
					foreach (var t in ic.Items)
					{
						bool isNewlyRealized;
						var tvi = generator.GenerateNext (out isNewlyRealized);
						if (isNewlyRealized)
						{
							generator.PrepareItemContainer (tvi);
						}
						if (t == dataItem)
							return tvi as TreeViewItem;

						var tmp = SelectTreeViewItemForBinding (dataItem, tvi as ItemsControl);
						if (tmp != null)
							return tmp;
					}
				}
			return null;
		}

		public TreeViewItem GetTreeViewItemForItem(object dataItem)
		{
			return SelectTreeViewItemForBinding(dataItem, this);
		}

		static TreeViewItem SelectTreeViewItemForBinding (object dataItem, ItemsControl ic)
		{
			if (ic == null || dataItem == null || !ic.HasItems)
				return null;
			IItemContainerGenerator generator = ic.ItemContainerGenerator;
			if (ic.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				foreach (var t in ic.Items)
				{
					var tvi = ic.ItemContainerGenerator.ContainerFromItem(t);
					if (t == dataItem)
						return tvi as TreeViewItem;

					var tmp = SelectTreeViewItemForBinding (dataItem, tvi as ItemsControl);
					if (tmp != null)
						return tmp;
				}
			}
			else 
			using (generator.StartAt (generator.GeneratorPositionFromIndex (-1), GeneratorDirection.Forward))
			{
				foreach (var t in ic.Items)
				{
					bool isNewlyRealized;
					var tvi = generator.GenerateNext (out isNewlyRealized);
					if (isNewlyRealized)
					{
						generator.PrepareItemContainer (tvi);
					}
					if (t == dataItem)
						return tvi as TreeViewItem;

					var tmp = SelectTreeViewItemForBinding (dataItem, tvi as ItemsControl);
					if (tmp != null)
						return tmp;
				}
			}
			return null;
		}
	}
}
