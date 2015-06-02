using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Test.Sections.Controls
{
	class LogGridItem : GridItem
	{
		public int Row { get; set; }

		public LogGridItem(params object[] values)
			: base(values)
		{
		}

		public override void SetValue(int column, object value)
		{
			base.SetValue(column, value);
			Log.Write(this, "SetValue, Row: {0}, Column: {1}, Value: {2}", Row, column, value);
		}
	}

	[Section("Controls", typeof(GridView))]
	public class GridViewSection : Panel
	{
		static readonly Image image1 = TestIcons.TestImage;
		static readonly Image image2 = TestIcons.TestIcon;

		public GridViewSection()
		{
			var gridView = CreateGrid();
			var selectionGridView = CreateGrid();

			// hook up selection of main grid to the selection grid
			gridView.SelectionChanged += (s, e) => selectionGridView.DataStore = gridView.SelectedItems.ToList();

			var filteredCollection = new SelectableFilterCollection<MyGridItem>(gridView, CreateItems().ToList());
			gridView.DataStore = filteredCollection;

			if (Platform.Supports<ContextMenu>())
				gridView.ContextMenu = CreateContextMenu(gridView, filteredCollection);

			Content = new TableLayout
			{
				Padding = new Padding(10),
				Spacing = new Size(5, 5),
				Rows =
				{
					new TableRow(
						"Grid", 
						new TableLayout(
							CreateOptions(gridView, filteredCollection),
							gridView
						)
					) { ScaleHeight = true },
					new TableRow("Selected Items", selectionGridView)
				}
			};
		}

		StackLayout CreateOptions(GridView grid, SelectableFilterCollection<MyGridItem> filtered)
		{
			return new StackLayout
			{
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					new StackLayout
					{
						Orientation = Orientation.Horizontal,
						Spacing = 5,
						Items =
						{
							null,
							EnabledCheckBox(grid),
							EditableCheckBox(grid),
							AllowMultiSelectCheckBox(grid),
							ShowHeaderCheckBox(grid),
							GridLinesDropDown(grid),
							null
						}
					},
					new StackLayout 
					{
						Orientation = Orientation.Horizontal,
						Spacing = 5,
						Items =
						{
							null,
							CreateScrollToRow(grid),
							CreateBeginEditButton(grid),
							null
						}
					},
					CreateSearchBox(filtered)
				}
			};
		}

		ComboBoxCell MyDropDown(string bindingProperty)
		{
			var combo = new ComboBoxCell(bindingProperty);
			combo.DataStore = new ListItemCollection
			{
				new ListItem { Text = "Item 1" },
				new ListItem { Text = "Item 2" },
				new ListItem { Text = "Item 3" },
				new ListItem { Text = "Item 4" }
			};
			return combo;
		}

		Control AllowMultiSelectCheckBox(GridView grid)
		{
			var control = new CheckBox { Text = "AllowMultipleSelection" };
			control.CheckedBinding.Bind(grid, r => r.AllowMultipleSelection);
			return control;
		}

		Control ShowHeaderCheckBox(GridView grid)
		{
			var control = new CheckBox { Text = "ShowHeader" };
			control.CheckedBinding.Bind(grid, r => r.ShowHeader);
			return control;
		}

		Control GridLinesDropDown(GridView grid)
		{
			var control = new EnumDropDown<GridLines>();
			control.SelectedValueBinding.Bind(grid, r => r.GridLines);
			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items = { "GridLines", control }
			};
		}

		Control EnabledCheckBox(GridView grid)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(grid, r => r.Enabled);
			return control;
		}

		Control EditableCheckBox(GridView grid)
		{
			var control = new CheckBox { Text = "Editable" };
			control.CheckedBinding.Bind(() => grid.Columns.First().Editable, v =>
			{
				foreach (var col in grid.Columns)
				{
					col.Editable = v ?? false;
				}
			});
			return control;
		}

		SearchBox CreateSearchBox(SelectableFilterCollection<MyGridItem> filtered)
		{
			var filterText = new SearchBox { PlaceholderText = "Filter" };
			filterText.TextChanged += (s, e) =>
			{
				var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				if (filterItems.Length == 0)
					filtered.Filter = null;
				else
					filtered.Filter = i =>
				{
					// Every item in the split filter string should be within the Text property
					foreach (var filterItem in filterItems)
					{
						if (i.Text.IndexOf(filterItem, StringComparison.OrdinalIgnoreCase) == -1)
						{
							return false;
						}
					}
					return true;
				};
			};
			return filterText;
		}

		GridView CreateGrid()
		{
			var control = new GridView
			{
				Size = new Size(300, 100)
			};
			LogEvents(control);

			var dropDown = MyDropDown("DropDownKey");
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell("Check"), AutoSize = true, Resizable = false });
			control.Columns.Add(new GridColumn { HeaderText = "Image", DataCell = new ImageViewCell("Image") });
			control.Columns.Add(new GridColumn { HeaderText = "ImageText", DataCell = new ImageTextCell("Image", "Text") });
			control.Columns.Add(new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell("Text"), Sortable = true });
			control.Columns.Add(new GridColumn { HeaderText = "Progress", DataCell = new ProgressCell("Progress") });
			control.Columns.Add(new GridColumn { HeaderText = "Drop Down", DataCell = dropDown, Sortable = true });

			if (Platform.Supports<DrawableCell>())
			{
				var drawableCell = new DrawableCell();
				drawableCell.Paint += (sender, e) =>
				{
					var m = e.Item as MyGridItem;
					if (m != null)
					{
						if (e.CellState.HasFlag(DrawableCellStates.Selected))
							e.Graphics.FillRectangle(Colors.Blue, e.ClipRectangle);
						else
							e.Graphics.FillRectangle(Brushes.Cached(m.Color), e.ClipRectangle);
						var rect = e.ClipRectangle;
						rect.Inflate(-5, -5);
						e.Graphics.DrawRectangle(Colors.White, rect);
						e.Graphics.DrawLine(Colors.White, rect.Left, rect.Bottom, rect.MiddleX, rect.Top);
						e.Graphics.DrawLine(Colors.White, rect.Right, rect.Bottom, rect.MiddleX, rect.Top);
					}
				};
				control.Columns.Add(new GridColumn
				{
					HeaderText = "Owner drawn",
					DataCell = drawableCell
				});
			}

			return control;
		}

		IEnumerable<MyGridItem> CreateItems()
		{
			var rand = new Random();
			for (int i = 0; i < 10000; i++)
			{
				yield return new MyGridItem(rand, i);
			}
		}

		ContextMenu CreateContextMenu(GridView grid, SelectableFilterCollection<MyGridItem> filtered)
		{
			// Context menu
			var menu = new ContextMenu();
			var item = new ButtonMenuItem { Text = "Click Me!" };
			item.Click += (sender, e) =>
			{
				if (grid.SelectedRows.Any())
					Log.Write(item, "Click, Rows: {0}", SelectedRowsString(grid));
				else
					Log.Write(item, "Click, no item selected");
			};
			menu.Items.Add(item);

			// Delete menu item: deletes the item from the store, the UI updates via the binding.
			var deleteItem = new ButtonMenuItem { Text = "Delete Item" };
			deleteItem.Click += (s, e) =>
			{
				var i = grid.SelectedItems.First() as MyGridItem;
				if (i != null)
					filtered.Remove(i);
			};
			menu.Items.Add(deleteItem);

			// Insert item: inserts an item into the store, the UI updates via the binding.
			var insertItem = new ButtonMenuItem { Text = "Insert Item at the start of the list" };
			insertItem.Click += (s, e) =>
			{
				var i = grid.SelectedItems.First() as MyGridItem;
				if (i != null)
					filtered.Insert(0, new MyGridItem(new Random(), 0));
			};
			menu.Items.Add(insertItem);

			var subMenu = menu.Items.GetSubmenu("Sub Menu");
			item = new ButtonMenuItem { Text = "Item 5" };
			item.Click += (s, e) => Log.Write(item, "Clicked");
			subMenu.Items.Add(item);
			item = new ButtonMenuItem { Text = "Item 6" };
			item.Click += (s, e) => Log.Write(item, "Clicked");
			subMenu.Items.Add(item);

			return menu;
		}

		protected virtual void LogEvents(GridView control)
		{
			control.CellEditing += (sender, e) => Log.Write(control, "BeginCellEdit, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			control.CellEdited += (sender, e) => Log.Write(control, "EndCellEdit, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			control.SelectionChanged += (sender, e) => Log.Write(control, "Selection Changed, Rows: {0}", SelectedRowsString(control));
			control.ColumnHeaderClick += (sender, e) => Log.Write(control, "Column Header Clicked: {0}", e.Column.HeaderText);
			control.CellClick += (sender, e) => Log.Write(control, "Cell Clicked, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			control.CellDoubleClick += (sender, e) => Log.Write(control, "Cell Double Clicked, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
		}

		static string SelectedRowsString(GridView grid)
		{
			return string.Join(",", grid.SelectedRows.Select(r => r.ToString()).OrderBy(r => r));
		}

		Button CreateBeginEditButton(GridView grid)
		{
			var control = new Button { Text = "Begin Edit Row:1, Column:2" };
			control.Click += (sender, e) => grid.BeginEdit(1, 2);
			return control;
		}

		Button CreateScrollToRow(GridView grid)
		{
			var control = new Button { Text = "ScrollToRow" };
			control.Click += (sender, e) =>
			{
				var row = new Random().Next(((ICollection)grid.DataStore).Count - 1);
				Log.Write(grid, "ScrollToRow: {0}", row);
				grid.ScrollToRow(row);
			};
			return control;
		}

		/// <summary>
		/// POCO (Plain Old CLR Object) to test property bindings
		/// </summary>
		class MyGridItem
		{
			bool? check;
			string text;
			string dropDownKey;
			float? progress;

			public int Row { get; set; }

			void Log(string property, object value)
			{
				Eto.Test.Log.Write(this, "SetValue, Row: {0}, Column: {1}, Value: {2}", Row, property, value);
			}

			public bool? Check
			{
				get { return check; }
				set
				{
					check = value;
					Log("Check", value);
				}
			}

			public string Text
			{
				get { return text; }
				set
				{
					text = value;
					Log("Text", value);
				}
			}

			public Image Image { get; set; }

			public string DropDownKey
			{
				get { return dropDownKey; }
				set
				{
					dropDownKey = value;
					Log("DropDownKey", value);
				}
			}

			// used for drawable cell
			public Color Color { get; set; }

			public float? Progress
			{
				get { return progress; }
				set
				{
					progress = value;
					Log("Progress", value);
				}
			}

			public MyGridItem(Random rand, int row)
			{
				// initialize to random values
				this.Row = row;
				var val = rand.Next(3);
				check = val == 0 ? (bool?)false : val == 1 ? (bool?)true : null;

				val = rand.Next(3);
				Image = val == 0 ? image1 : val == 1 ? (Image)image2 : null;

				text = string.Format("Col 1 Row {0}", row);

				Color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));

				dropDownKey = "Item " + Convert.ToString(rand.Next(4) + 1);

				progress = rand.Next() % 10 != 0 ? (float?)rand.NextDouble() : null;
			}
		}
	}
}

