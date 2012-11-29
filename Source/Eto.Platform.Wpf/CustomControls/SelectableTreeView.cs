using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Eto.Platform.Wpf.CustomControls
{
	public class SelectableTreeView : TreeView
	{
		public static DependencyProperty CurrentItemProperty = DependencyProperty.RegisterAttached (
			"CurrentItem", typeof (object), typeof (SelectableTreeView),
			new PropertyMetadata (new object (), OnCurrentItemChanged));

		public static RoutedEvent CurrentItemChangedEvent;

		static SelectableTreeView ()
		{
			SelectableTreeView.CurrentItemChangedEvent = EventManager.RegisterRoutedEvent ("CurrentItemChangedEvent", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<object>), typeof (SelectableTreeView));
		}

		public SelectableTreeView ()
		{
			this.SelectedItemChanged += TreeViewItemChanged;
		}

		public event RoutedPropertyChangedEventHandler<object> CurrentItemChanged
		{
			add { base.AddHandler (SelectableTreeView.CurrentItemChangedEvent, value); }
			remove { base.RemoveHandler (SelectableTreeView.CurrentItemChangedEvent, value); }
		}

		protected virtual void OnCurrentItemChanged (RoutedPropertyChangedEventArgs<object> e)
		{
			base.RaiseEvent (e);
		}

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

		bool refreshing;
		public void RefreshData ()
		{
			var selectedItem = this.CurrentItem;
			refreshing = true;
			Items.Refresh ();
			refreshing = false;
			FindItem (selectedItem, this).ContinueWith (t => {
				if (t.Result != null)
				{
					Dispatcher.BeginInvoke(new Action(() => {
						t.Result.IsSelected = true;
					}));
				}
			});
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
			if (treeViewItem != null)
				treeViewItem.IsSelected = true;
			treeView.SelectedItemChanged += TreeViewItemChanged;
			treeView.OnCurrentItemChanged (new RoutedPropertyChangedEventArgs<object>(args.OldValue, args.NewValue, CurrentItemChangedEvent));
		}

		static void TreeViewItemChanged (object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var treeView = (SelectableTreeView)sender;
			if (!treeView.refreshing)
				treeView.SetValue (CurrentItemProperty, e.NewValue);
		}

		class Helper
		{
			Dictionary<ItemContainerGenerator, ItemsControl> items = new Dictionary<ItemContainerGenerator, ItemsControl> ();
			TaskCompletionSource<TreeViewItem> completion = new TaskCompletionSource<TreeViewItem> ();
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
				if (generator.Status == GeneratorStatus.ContainersGenerated)
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
							Complete (container as TreeViewItem);
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
