using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TreeGridView))]
	public class TreeGridViewSection : Scrollable
	{
		int expanded;
		readonly CheckBox allowCollapsing;
		readonly CheckBox allowExpanding;
		readonly TreeGridView grid;
		int newItemCount;
		static readonly Image Image = TestIcons.TestIcon.WithSize(16, 16);
		Label hoverNodeLabel;
		bool cancelLabelEdit;

		public TreeGridViewSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			grid = ImagesAndMenu();

			layout.AddSeparateRow(
				null,
				allowExpanding = new CheckBox { Text = "Allow Expanding", Checked = true },
				allowCollapsing = new CheckBox { Text = "Allow Collapsing", Checked = true },
				ShowHeaderCheckBox(grid),
				null
			);
			layout.AddSeparateRow(null, InsertButton(), AddChildButton(), RemoveButton(), ExpandButton(), CollapseButton(), null);
			layout.AddSeparateRow(null, EnabledCheck(), AllowMultipleSelect(), AllowEmptySelectionCheckBox(), "Border", CreateBorderType(grid), null);
			layout.AddSeparateRow(null, ReloadDataButton(grid), SetDataButton(grid), null);

			layout.Add(grid, yscale: true);
			layout.Add(HoverNodeLabel());

			Content = layout;
		}

		Control ShowHeaderCheckBox(TreeGridView grid)
		{
			var control = new CheckBox { Text = "ShowHeader" };
			control.CheckedBinding.Bind(grid, g => g.ShowHeader);
			return control;
		}

		Control ReloadDataButton(TreeGridView grid)
		{
			var control = new Button { Text = "ReloadData" };
			control.Click += (sender, e) => grid.ReloadData();
			return control;
		}

		Control SetDataButton(TreeGridView grid)
		{
			var control = new Button { Text = "SetDataStore" };
			control.Click += (sender, e) => SetDataStore(grid);
			return control;
		}

		Control CreateBorderType(TreeGridView grid)
		{
			var borderType = new EnumDropDown<BorderType>();
			borderType.SelectedValueBinding.Bind(grid, g => g.Border);
			return borderType;
		}

		Control HoverNodeLabel()
		{
			hoverNodeLabel = new Label();

			grid.MouseMove += (sender, e) =>
			{
				var cell = grid.GetCellAt(e.Location);
				if (cell != null)
					hoverNodeLabel.Text = $"Item under mouse: {((TreeGridItem)cell.Item)?.Values[1] ?? "(no item)"}, Column: {cell.Column?.HeaderText ?? "(no column)"}";
			};

			return hoverNodeLabel;
		}

		Control InsertButton()
		{
			var control = new Button { Text = "Insert" };
			control.Click += (sender, e) =>
			{
				var item = grid.SelectedItem as TreeGridItem;
				var parent = (item?.Parent ?? (ITreeGridItem)grid.DataStore) as TreeGridItem;
				if (parent != null)
				{
					var index = item != null ? parent.Children.IndexOf(item) : 0;
					parent.Children.Insert(index, CreateComplexTreeItem(0, "New Item " + newItemCount++, null));
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
				var item = grid.SelectedItem as TreeGridItem;
				if (item != null)
				{
					item.Children.Add(CreateComplexTreeItem(0, "New Item " + newItemCount++, null));
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
					foreach (var item in grid.SelectedItems.OfType<ITreeGridItem>().ToList())
					{
						var parent = item.Parent as TreeGridItem;
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
					var item = grid.SelectedItem as TreeGridItem;
					if (item != null)
					{
						var parent = item.Parent as TreeGridItem;
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

		Control CancelLabelEdit()
		{
			var control = new CheckBox { Text = "Cancel Edit" };
			control.CheckedChanged += (sender, e) => cancelLabelEdit = control.Checked ?? false;
			return control;
		}

		Control EnabledCheck()
		{
			var control = new CheckBox { Text = "Enabled", Checked = grid.Enabled };
			control.CheckedChanged += (sender, e) => grid.Enabled = control.Checked ?? false;
			return control;
		}

		Control AllowMultipleSelect()
		{
			var control = new CheckBox { Text = "AllowMultipleSelection" };
			control.CheckedBinding.Bind(grid, t => t.AllowMultipleSelection);
			return control;
		}

		Control AllowEmptySelectionCheckBox()
		{
			var control = new CheckBox { Text = "AllowEmptySelection" };
			control.CheckedBinding.Bind(grid, g => g.AllowEmptySelection);
			return control;
		}


		TreeGridItem CreateComplexTreeItem(int level, string name, Image image)
		{
			var item = new TreeGridItem
			{
				Expanded = expanded++ % 2 == 0
			};
			item.Values = new object[] { image, "col 0 - " + name, "col 1 - " + name };
			if (level < 4)
			{
				for (int i = 0; i < 4; i++)
				{
					item.Children.Add(CreateComplexTreeItem(level + 1, name + " " + i, image));
				}
			}
			return item;
		}

		TreeGridView ImagesAndMenu()
		{
			var control = new TreeGridView
			{
				Size = new Size(100, 150)
			};

			control.Columns.Add(new GridColumn { DataCell = new ImageTextCell(0, 1), HeaderText = "Image and Text", AutoSize = true, Resizable = true, Editable = true });
			control.Columns.Add(new GridColumn { DataCell = new TextBoxCell(2), HeaderText = "Text", AutoSize = true, Width = 150, Resizable = true, Editable = true });

			if (Platform.Supports<ContextMenu>())
			{
				var menu = new ContextMenu();
				var item = new ButtonMenuItem { Text = "Click Me!" };
				item.Click += delegate
				{
					if (grid.SelectedItems.Any())
						Log.Write(item, "Click, Items: {0}, Rows: {1}", SelectedItemsString(grid), SelectedRowsString(grid));
					else
						Log.Write(item, "Click, no item selected");
				};
				menu.Items.Add(item);

				control.ContextMenu = menu;
			}

			SetDataStore(control);
			LogEvents(control);
			return control;
		}

		private void SetDataStore(TreeGridView control)
		{
			control.DataStore = CreateComplexTreeItem(0, "", Image);
		}

		static string SelectedRowsString(TreeGridView grid)
		{
			return string.Join(",", grid.SelectedRows.Select(r => r.ToString()).OrderBy(r => r));
		}

		static string SelectedItemsString(TreeGridView grid)
		{
			return string.Join(",", grid.SelectedItems.Cast<ITreeGridItem>().Select(GetDescription).OrderBy(r => r));
		}

		static string GetDescription(ITreeGridItem item)
		{
			var treeItem = item as TreeGridItem;
			if (treeItem != null)
				return Convert.ToString(treeItem.Values[1]);
				//return Convert.ToString(string.Join(", ", treeItem.Values.Select(r => Convert.ToString(r))));
			return Convert.ToString(item);
		}

		void LogEvents(TreeGridView control)
		{
			control.Activated += (sender, e) =>
			{
				Log.Write(control, "Activated, Item: {0}", GetDescription(e.Item));
			};
			control.SelectionChanged += delegate
			{
				Log.Write(control, "SelectionChanged, Rows: {0}", string.Join(", ", control.SelectedRows.Select(r => r.ToString())));
				Log.Write(control, "\t Items: {0}", string.Join(", ", control.SelectedItems.OfType<TreeGridItem>().Select(r => GetDescription(r))));
			};
			control.SelectedItemChanged += delegate
			{
				Log.Write(control, "SelectedItemChanged, Item: {0}", control.SelectedItem != null ? GetDescription(control.SelectedItem) : "<none selected>");
			};

			control.Expanding += (sender, e) =>
			{
				Log.Write(control, "Expanding, Item: {0}", GetDescription(e.Item));
				e.Cancel = !(allowExpanding.Checked ?? true);
			};
			control.Expanded += (sender, e) =>
			{
				Log.Write(control, "Expanded, Item: {0}", GetDescription(e.Item));
			};
			control.Collapsing += (sender, e) =>
			{
				Log.Write(control, "Collapsing, Item: {0}", GetDescription(e.Item));
				e.Cancel = !(allowCollapsing.Checked ?? true);
			};
			control.Collapsed += (sender, e) =>
			{
				Log.Write(control, "Collapsed, Item: {0}", GetDescription(e.Item));
			};
			control.ColumnHeaderClick += delegate (object sender, GridColumnEventArgs e)
			{
				Log.Write(control, "ColumnHeaderClick: {0}", e.Column);
			};

			control.CellClick += (sender, e) =>
			{
				Log.Write(control, "CellClick, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			};

			control.CellDoubleClick += (sender, e) =>
			{
				Log.Write(control, "CellDoubleClick, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			};

			control.MouseDown += (sender, e) =>
			{
				var cell = control.GetCellAt(e.Location);
				Log.Write(control, $"MouseDown, Cell Column: {cell.Column?.HeaderText}, Item: {GetDescription(cell.Item as ITreeGridItem)}");
			};

			control.MouseUp += (sender, e) =>
			{
				var cell = control.GetCellAt(e.Location);
				Log.Write(control, $"MouseUp, Cell Column: {cell.Column?.HeaderText}, Item: {GetDescription(cell.Item as ITreeGridItem)}");
			};

			control.MouseDoubleClick += (sender, e) =>
			{
				var cell = control.GetCellAt(e.Location);
				Log.Write(control, $"MouseDoubleClick, Cell Column: {cell.Column?.HeaderText}, Item: {GetDescription(cell.Item as ITreeGridItem)}");
			};
		}
	}
}

