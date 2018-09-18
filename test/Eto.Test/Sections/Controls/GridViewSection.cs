using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		static Icon image1 = new Icon(1, TestIcons.TestImage).WithSize(16, 16);
		static Icon image2 = TestIcons.TestIcon.WithSize(16, 16);

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
							new TableRow(gridView) { ScaleHeight = true },
                            CreatePositionLabel(gridView)
						)
					) { ScaleHeight = true },
					new TableRow("Selected Items", selectionGridView)
				}
			};
		}

		Label CreatePositionLabel(GridView grid)
		{
			var label = new Label();
			grid.MouseMove += (sender, e) =>
			{
				var cell = grid.GetCellAt(e.Location);
				label.Text = $"Row: {cell?.RowIndex}, Column: {cell?.ColumnIndex} ({cell?.Column?.HeaderText}), Item: {cell?.Item}";
			};
			return label;
		}

		StackLayout CreateOptions(GridView grid, SelectableFilterCollection<MyGridItem> filtered)
		{
			return new StackLayout
			{
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					TableLayout.Horizontal(
						5,
						null,
						EnabledCheckBox(grid),
						EditableCheckBox(grid),
						AllowMultiSelectCheckBox(grid),
						ShowHeaderCheckBox(grid),
						GridLinesDropDown(grid),
						null
					),
					TableLayout.Horizontal(
						5,
						null,
						AddItemButton(filtered),
						CreateScrollToRow(grid),
						CreateBeginEditButton(grid),
						"Border",
						CreateBorderType(grid),
						null
					),
					TableLayout.Horizontal(
						5,
						null,
						ReloadDataButton(grid),
						null
					),
					TableLayout.Horizontal(
						5,
						null,
						"TextBoxCell:",
						"TextAlignment", TextAlignmentDropDown(grid),
						"VerticalAlignment", VerticalAlignmentDropDown(grid),
						null
					),
					TableLayout.Horizontal(
						5,
						null,
						"AutoSelectMode", AutoSelectModeDropDown(grid),
						null
					),
					CreateSearchBox(filtered)
				}
			};
		}

		Control TextAlignmentDropDown(GridView grid)
		{
			var control = new EnumDropDown<TextAlignment>();

			var textBoxCell = grid.Columns.Select(r => r.DataCell).OfType<TextBoxCell>().First();
			control.SelectedValueBinding.Bind(textBoxCell, c => c.TextAlignment);

			var imageTextCell = grid.Columns.Select(r => r.DataCell).OfType<ImageTextCell>().First();
			control.SelectedValueBinding.Bind(imageTextCell, c => c.TextAlignment);
			return control;
		}

		Control VerticalAlignmentDropDown(GridView grid)
		{
			var control = new EnumDropDown<VerticalAlignment>();

			var textBoxCell = grid.Columns.Select(r => r.DataCell).OfType<TextBoxCell>().First();
			control.SelectedValueBinding.Bind(textBoxCell, c => c.VerticalAlignment);

			var imageTextCell = grid.Columns.Select(r => r.DataCell).OfType<ImageTextCell>().First();
			control.SelectedValueBinding.Bind(imageTextCell, c => c.VerticalAlignment);
			return control;
		}

		Control AutoSelectModeDropDown(GridView grid)
		{
			var control = new EnumDropDown<AutoSelectMode>();

			var textBoxCell = grid.Columns.Select(r => r.DataCell).OfType<TextBoxCell>().First();
			control.SelectedValueBinding.Bind(textBoxCell, c => c.AutoSelectMode);

			var imageTextCell = grid.Columns.Select(r => r.DataCell).OfType<ImageTextCell>().First();
			control.SelectedValueBinding.Bind(imageTextCell, c => c.AutoSelectMode);
			return control;
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

		Control ReloadDataButton(GridView grid)
		{
			var control = new Button { Text = "ReloadData" };
			control.Click += (sender, e) => grid.ReloadData(grid.SelectedRows);
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

		Control CreateBorderType(GridView grid)
		{
			var borderType = new EnumDropDown<BorderType>();
			borderType.SelectedValueBinding.Bind(grid, g => g.Border);
			return borderType;
		}

		class MyCustomCell : CustomCell
		{
			protected override Control OnCreateCell(CellEventArgs args)
			{
				//Log.Write(this, "OnCreateCell: Row: {1}, CellState: {2}, Item: {0}", args.Item, args.Row, args.CellState);
				//var control = new Label();
				var control = new Button();
				control.TextBinding.BindDataContext((MyGridItem m) => m.Text);
				control.Bind(c => c.TextColor, args, a => a.CellTextColor);
				control.BindDataContext(c => c.Command, (MyGridItem m) => m.Command);
				//control.Click += (sender, e) => Log.Write(sender, "Clicked row button {0}", ((Button)sender).Text);
				return control;
			}

			protected override void OnConfigureCell(CellEventArgs args, Control control)
			{
				// if you don't use binding or have other scenarios, you can use ConfigureCell:
				/**
				var item = (MyGridItem)args.Item;
				var button = (Button)control;
				button.BackgroundColor = args.CellState.HasFlag(CellStates.Selected) ? Colors.Blue : Colors.White;
				//button.Text = item.Text;
				//button.Enabled = ((GridColumn)Parent).Editable;
				/**/
				base.OnConfigureCell(args, control);

				//Log.Write(this, "OnConfigureCell: Control: {0}, Row: {2}, CellState: {3}, Item: {1}", control, args.Item, args.Row, args.CellState);
			}

			protected override void OnPaint(CellPaintEventArgs args)
			{
				var item = (MyGridItem)args.Item;
				if (!args.IsEditing)
					args.Graphics.DrawText(SystemFonts.Default(), Colors.Black, args.ClipRectangle.Location, item.Text);
				//args.Graphics.DrawLine(Colors.Blue, args.ClipRectangle.TopLeft, args.ClipRectangle.BottomRight);
			}
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
			control.Columns.Add(new GridColumn { HeaderText = "Image", DataCell = new ImageViewCell("Image"), Resizable = false });
			control.Columns.Add(new GridColumn { HeaderText = "ImageText", DataCell = new ImageTextCell("Image", "Text") });
			control.Columns.Add(new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell("Text"), Sortable = true });
			control.Columns.Add(new GridColumn { HeaderText = "Progress", DataCell = new ProgressCell("Progress") });
			control.Columns.Add(new GridColumn { HeaderText = "Drop Down", DataCell = dropDown, Sortable = true });
			if (Platform.Supports<CustomCell>())
			{
				//control.ReloadSelectedCells = true;
				//control.SelectedRowsChanged += (sender, e) => control.ReloadData(control.SelectedRows);
				var col = new GridColumn { HeaderText = "Custom", Sortable = true, DataCell = new MyCustomCell() };
				control.Columns.Add(col);
			}

			if (Platform.Supports<DrawableCell>())
			{
				var drawableCell = new DrawableCell();
				drawableCell.Paint += (sender, e) =>
				{
					var m = e.Item as MyGridItem;
					if (m != null)
					{
						if (e.CellState.HasFlag(CellStates.Selected))
							e.Graphics.FillRectangle(Colors.Blue, e.ClipRectangle);
						else
							e.Graphics.FillRectangle(Brushes.Cached(m.Color), e.ClipRectangle);
						var rect = e.ClipRectangle;
						rect.Inflate(-5, -5);

						var color = e.CellState.HasFlag(CellStates.Editing) ? Colors.Black : Colors.White;
						e.Graphics.DrawRectangle(color, rect);
						e.Graphics.DrawLine(color, rect.Left, rect.Bottom, rect.MiddleX, rect.Top);
						e.Graphics.DrawLine(color, rect.Right, rect.Bottom, rect.MiddleX, rect.Top);
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

			var commitEditItem = new ButtonMenuItem { Text = "CommitEdit" };
			commitEditItem.Click += (s, e) =>
			{
				var result = grid.CommitEdit();
				Log.Write(grid, $"CommitEdit, Result: {result}, IsEditing: {grid.IsEditing}");
			};
			menu.Items.Add(commitEditItem);

			var abortEditItem = new ButtonMenuItem { Text = "CancelEdit" };
			abortEditItem.Click += (s, e) =>
			{
				var result = grid.CancelEdit();
				Log.Write(grid, $"CancelEdit, Result: {result}, IsEditing: {grid.IsEditing}");
			};
			menu.Items.Add(abortEditItem);

			menu.Opening += (sender, e) =>
			{
				commitEditItem.Enabled = grid.IsEditing;
				abortEditItem.Enabled = grid.IsEditing;
			};

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
			control.CellEditing += (sender, e) => Log.Write(control, $"BeginCellEdit, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.CellEdited += (sender, e) => Log.Write(control, $"EndCellEdit, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.SelectionChanged += (sender, e) => Log.Write(control, $"Selection Changed, Rows: {SelectedRowsString(control)}");
			control.ColumnHeaderClick += (sender, e) => Log.Write(control, $"Column Header Clicked: {e.Column.HeaderText}");
			control.CellClick += (sender, e) => Log.Write(control, $"Cell Clicked, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.CellDoubleClick += (sender, e) => Log.Write(control, $"Cell Double Clicked, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
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

		Button AddItemButton(SelectableFilterCollection<MyGridItem> filtered)
		{
			var control = new Button { Text = "Add Item" };
			control.Click += (sender, e) => filtered.Add(new MyGridItem(new Random(), filtered.Count + 1));
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
		protected class MyGridItem
		{
			bool? check;
			string text;
			string dropDownKey;
			float? progress;
			Command command;

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

			public Command Command
			{
				get
				{
					return command ?? (command = new Command((sender, e) => Test.Log.Write(null, "Command Executed for item: {0}", this)));
				}
			}

			public MyGridItem(Random rand, int row)
			{
				// initialize to random values
				this.Row = row;
				var val = row % 3;
				check = val == 0 ? (bool?)false : val == 1 ? (bool?)true : null;

				val = row % 2;
				Image = val == 0 ? image1 : val == 1 ? (Image)image2 : null;

				text = string.Format("Col 1 Row {0}", row);

				Color = Color.FromElementId(row);

				val = row % 5;
				if (val < 4)
					dropDownKey = "Item " + Convert.ToString(val + 1);

				val = row % 12;
				if (val <= 10)
					progress = (float)Math.Round(val / 10f, 1);
			}

			public override string ToString()
			{
				return string.Format("[MyGridItem: Row={0}, Check={1}, Text={2}, Image={3}, DropDownKey={4}, Color={5}, Progress={6}]", Row, Check, Text, Image, DropDownKey, Color, Progress);
			}
		}
	}
}

