using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TreeGridView))]
	public class TreeGridViewSection : GridViewSection<TreeGridView>
	{
		int newItemCount;
		CheckBox allowExpanding;
		CheckBox allowCollapsing;

		protected override string GetCellInfo(TreeGridView grid, PointF location)
		{
			var cell = grid.GetCellAt(location);
			return $"Column: {cell?.ColumnIndex} ({cell?.Column?.HeaderText}), Item: {cell?.Item}";
		}

		protected override int GetRowCount(TreeGridView grid) => grid.DataStore.Count;

		protected override void ReloadData(TreeGridView grid) => grid.ReloadData();

		protected override void SetContextMenu(TreeGridView grid, ContextMenu menu) => grid.ContextMenu = menu;

		protected override void SetDataStore(TreeGridView grid)
		{
			var item = CreateItem(4, "Item", 0, 100);
			// var item = await Task.Run(() => CreateItem(0, "Item", 0, 1000));
			grid.DataStore = item;
		}

		MyTreeGridItem CreateItem(int level, string name, int row, int childCount)
		{
			var item = new MyTreeGridItem(row, name);
			if (level > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					var child = CreateItem(level - 1, name + " " + i, i, i % 4 == 3 ? 0 : 4);
					item.Children.Add(child);
				}
				item.Expanded = row % 2 == 0;
			}
			return item;
		}

		protected override void AddExtraControls(DynamicLayout layout)
		{
			layout.AddSeparateRow(
				null,
				allowExpanding = new CheckBox { Text = "Allow Expanding", Checked = true },
				allowCollapsing = new CheckBox { Text = "Allow Collapsing", Checked = true },
				null
			);
			layout.AddSeparateRow(null, InsertButton(), AddChildButton(), RemoveButton(), ExpandButton(), CollapseButton(), null);
			
			base.AddExtraControls(layout);
		}

		protected override void LogEvents(TreeGridView control)
		{
			base.LogEvents(control);
			
			control.Expanding += (sender, e) =>
			{
				Log.Write(control, $"Expanding, Item: {e.Item}");
				e.Cancel = allowExpanding?.Checked == false;
			};
			control.Expanded += (sender, e) =>
			{
				Log.Write(control, $"Expanded, Item: {e.Item}");
			};
			control.Collapsing += (sender, e) =>
			{
				Log.Write(control, $"Collapsing, Item: {e.Item}");
				e.Cancel = allowCollapsing?.Checked == false;
			};
			control.Collapsed += (sender, e) =>
			{
				Log.Write(control, $"Collapsed, Item: {e.Item}");
			};
		}

		Control InsertButton()
		{
			var control = new Button { Text = "Insert" };
			control.Click += (sender, e) =>
			{
				var item = grid.SelectedItem as MyTreeGridItem;
				var parent = (item?.Parent ?? (ITreeGridItem)grid.DataStore) as MyTreeGridItem;
				if (parent != null)
				{
					var index = item != null ? parent.Children.IndexOf(item) : 0;
					parent.Children.Insert(index, CreateItem(newItemCount % 2, "New Item " + newItemCount, newItemCount, 10));
					newItemCount++;
					if (item != null)
						grid.ReloadItem(parent);
					else
						grid.ReloadData();
				}
			};
			return control;
		}

		Control AddChildButton()
		{
			var control = new Button { Text = "Add Child" };
			control.Click += (sender, e) =>
			{
				var item = grid.SelectedItem as MyTreeGridItem;
				if (item != null)
				{
					item.Children.Add(CreateItem(newItemCount % 2, "New Item " + newItemCount, newItemCount, 10));
					newItemCount++;
					grid.ReloadItem(item);
				}
			};
			return control;
		}

		Control RemoveButton()
		{
			var control = new Button { Text = "Remove" };
			control.Click += (sender, e) =>
			{
				if (grid.AllowMultipleSelection)
				{
					var parents = new List<ITreeGridItem>();
					bool reloadData = false;
					foreach (var item in grid.SelectedItems.OfType<MyTreeGridItem>().ToList())
					{
						var parent = item.Parent as MyTreeGridItem;
						parent.Children.Remove(item);
						if (parent.Parent == null)
							reloadData = true;
						if (!parents.Contains(parent))
							parents.Add(parent);
					}
					if (reloadData)
						grid.ReloadData();
					else
					{
						foreach (var parent in parents)
						{
							grid.ReloadItem(parent);
						}
					}
				}
				else
				{
					var item = grid.SelectedItem as MyTreeGridItem;
					if (item != null)
					{
						var parent = item.Parent as MyTreeGridItem;
						parent.Children.Remove(item);
						if (parent.Parent == null)
							grid.ReloadData();
						else
							grid.ReloadItem(parent);
					}
				}
			};
			return control;
		}

		Control ExpandButton()
		{
			var control = new Button { Text = "Expand" };
			control.Click += (sender, e) =>
			{
				var item = grid.SelectedItem;
				if (item != null)
				{
					item.Expanded = true;
					grid.ReloadItem(item);
				}
			};
			return control;
		}

		Control CollapseButton()
		{
			var control = new Button { Text = "Collapse" };
			control.Click += (sender, e) =>
			{
				var item = grid.SelectedItem;
				if (item != null)
				{
					item.Expanded = false;
					grid.ReloadItem(item);
				}
			};
			return control;
		}

		protected override int GetColumnAt(PointF location)
		{
			return grid.GetCellAt(location).ColumnIndex;
		}

		class MyTreeGridItem : MyGridItem, ITreeGridStore<MyTreeGridItem>, ITreeGridItem
		{
			ObservableCollection<MyTreeGridItem> children;

			public MyTreeGridItem(int row, string name) : base(row, name)
			{
			}

			public IList<MyTreeGridItem> Children
			{
				get
				{
					if (children == null)
					{
						children = new ObservableCollection<MyTreeGridItem>();
						children.CollectionChanged += Children_CollectionChanged;
					}
					return children;
				}
			}

			private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Reset:
						foreach (var item in children)
						{
							item.Parent = this;
						}
						break;
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Replace:
						foreach (ITreeGridItem item in e.NewItems)
						{
							item.Parent = this;
						}
						break;
				}
			}

			public MyTreeGridItem this[int index] => children[index];

			public int Count => children?.Count ?? 0;

			public bool Expanded { get; set; }

			public bool Expandable => children?.Count > 0;

			public ITreeGridItem Parent { get; set; }
		}
	}
}

