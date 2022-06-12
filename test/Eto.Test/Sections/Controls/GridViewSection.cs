using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(GridView))]
	public class GridViewSection : GridViewSection<GridView>
	{
		protected SelectableFilterCollection<MyGridItem> filteredCollection;

		protected override string GetCellInfo(GridView grid, PointF location)
		{
			var cell = grid.GetCellAt(location);
			return $"Row: {cell?.RowIndex}, Column: {cell?.ColumnIndex} ({cell?.Column?.HeaderText}), Type: {cell?.Type}, Item: {cell?.Item}";
		}

		protected override int GetRowCount(GridView grid) => ((ICollection)grid.DataStore).Count;

		protected override void ReloadData(GridView grid) => grid.ReloadData(grid.SelectedRows);

		protected override void SetContextMenu(GridView grid, ContextMenu menu) => grid.ContextMenu = menu;

		protected override void SetDataStore(GridView grid)
		{
			filteredCollection = new SelectableFilterCollection<MyGridItem>(grid, CreateItems());
			grid.DataStore = filteredCollection;
		}

		List<MyGridItem> CreateItems(int count = 10000)
		{
			var list = new List<MyGridItem>(count);

			for (int i = 0; i < count; i++)
			{
				list.Add(new MyGridItem(i, $"Row {i}"));
			}
			return list;
		}

		SearchBox CreateSearchBox()
		{
			var filterText = new SearchBox { PlaceholderText = "Filter" };
			filterText.TextChanged += (s, e) =>
			{
				var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				if (filterItems.Length == 0)
					filteredCollection.Filter = null;
				else
					filteredCollection.Filter = i =>
				{
					// Every item in the split filter string should be within the Text property
					for (int i1 = 0; i1 < filterItems.Length; i1++)
					{
						string filterItem = filterItems[i1];
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

		Button AddItemButton()
		{
			var control = new Button { Text = "Add Item" };
			control.Click += (sender, e) => filteredCollection.Add(new MyGridItem(filteredCollection.Count + 1, $"New Row {filteredCollection.Count + 1}"));
			return control;
		}

		protected override void AddExtraControls(DynamicLayout layout)
		{
			layout.AddSeparateRow(null, AddItemButton(), null);
			layout.AddSeparateRow(CreateSearchBox());
			base.AddExtraControls(layout);
		}

		protected override void AddExtraContextMenuItems(ContextMenu menu)
		{
			// Delete menu item: deletes the item from the store, the UI updates via the binding.
			var deleteItem = new ButtonMenuItem { Text = "Delete Item" };
			deleteItem.Click += (s, e) =>
			{
				var i = grid.SelectedItems.First() as MyGridItem;
				if (i != null)
					filteredCollection.Remove(i);
			};
			menu.Items.Add(deleteItem);

			// Insert item: inserts an item into the store, the UI updates via the binding.
			var insertItem = new ButtonMenuItem { Text = "Insert Item" };
			insertItem.Click += (s, e) =>
			{
				var i = grid.SelectedItems.First() as MyGridItem;
				if (i != null)
				{
					filteredCollection.Insert(filteredCollection.IndexOf(i), new MyGridItem(0, ""));
				}
			};
			menu.Items.Add(insertItem);
			base.AddExtraContextMenuItems(menu);
		}

		protected override int GetColumnAt(PointF location)
		{
			return grid.GetCellAt(location).ColumnIndex;
		}
	}

	public abstract class GridViewSection<T> : Panel
		where T : Grid, new()
	{
		static Icon image1 = new Icon(1, TestIcons.TestImage).WithSize(16, 16);
		static Icon image2 = TestIcons.TestIcon.WithSize(16, 16);
		protected T grid;

		protected abstract void SetContextMenu(T grid, ContextMenu menu);
		protected abstract int GetRowCount(T grid);
		protected abstract void SetDataStore(T grid);
		protected abstract void ReloadData(T grid);
		protected abstract string GetCellInfo(T grid, PointF location);
		protected abstract int GetColumnAt(PointF location);

		public GridViewSection()
		{
			Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);

			CreateGrid();

			SetDataStore(grid);

			if (Platform.Supports<ContextMenu>())
				SetContextMenu(grid, CreateContextMenu(grid));

			Content = new TableLayout
			{
				Padding = new Padding(5),
				Spacing = new Size(2, 2),
				Rows =
				{
					CreateOptions(grid),
					new TableRow(grid) { ScaleHeight = true },
					CreatePositionLabel(grid)
				}
			};
		}

		Label CreatePositionLabel(T grid)
		{
			var label = new Label();
			grid.MouseMove += (sender, e) =>
			{
				label.Text = GetCellInfo(grid, e.Location);
			};
			return label;
		}

		Control CreateOptions(T grid)
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(2, 2) };

			layout.AddSeparateRow(null,
						EnabledCheckBox(grid),
						EditableCheckBox(grid),
						ShowHeaderCheckBox(grid),
						GridLinesDropDown(grid),
						"Border",
						CreateBorderType(grid),
						null);
			layout.AddSeparateRow(null,
						AllowMultiSelectCheckBox(grid),
						AllowEmptySelectionCheckBox(grid),
						null);
			layout.AddSeparateRow(null,
						AllowColumnReorderingCheckBox(grid),
						SaveColumnDisplayOrderCheckBox(grid),
						SetAllVisibleButton(grid),
						null
					);
			layout.AddSeparateRow(null,
						CreateScrollToRow(grid),
						CreateBeginEditButton(grid),
						null
					);
			layout.AddSeparateRow(null,
						ReloadDataButton(grid),
						SetDataButton(grid),
						SetTextBoxWidth(grid),
						null
					);
			layout.AddSeparateRow(null,
						"TextBoxCell:",
						"TextAlignment", TextAlignmentDropDown(grid),
						"VerticalAlignment", VerticalAlignmentDropDown(grid),
						null
					);
			layout.AddSeparateRow(null,
						"AutoSelectMode", AutoSelectModeDropDown(grid),
						null
					);
			AddExtraControls(layout);
			return layout;
		}

		protected virtual void AddExtraControls(DynamicLayout layout)
		{
		}

		Control SetTextBoxWidth(T grid)
		{
			var button = new Button { Text = "Set TextBoxCell.Width" };
			var textBoxCell = grid.Columns.FirstOrDefault(r => r.DataCell is TextBoxCell);
			button.Click += (sender, e) => textBoxCell.Width = 100;
			return button;
		}

		Control TextAlignmentDropDown(T grid)
		{
			var control = new EnumDropDown<TextAlignment>();

			var textBoxCell = grid.Columns.Select(r => r.DataCell).OfType<TextBoxCell>().FirstOrDefault();
			if (textBoxCell != null)
				control.SelectedValueBinding.Bind(textBoxCell, c => c.TextAlignment);

			var imageTextCell = grid.Columns.Select(r => r.DataCell).OfType<ImageTextCell>().FirstOrDefault();
			if (imageTextCell != null)
				control.SelectedValueBinding.Bind(imageTextCell, c => c.TextAlignment);
			return control;
		}

		Control VerticalAlignmentDropDown(T grid)
		{
			var control = new EnumDropDown<VerticalAlignment>();

			var textBoxCell = grid.Columns.Select(r => r.DataCell).OfType<TextBoxCell>().FirstOrDefault();
			if (textBoxCell != null)
				control.SelectedValueBinding.Bind(textBoxCell, c => c.VerticalAlignment);

			var imageTextCell = grid.Columns.Select(r => r.DataCell).OfType<ImageTextCell>().FirstOrDefault();
			if (imageTextCell != null)
				control.SelectedValueBinding.Bind(imageTextCell, c => c.VerticalAlignment);
			return control;
		}

		Control AutoSelectModeDropDown(T grid)
		{
			var control = new EnumDropDown<AutoSelectMode>();

			var textBoxCell = grid.Columns.Select(r => r.DataCell).OfType<TextBoxCell>().FirstOrDefault();
			if (textBoxCell != null)
				control.SelectedValueBinding.Bind(textBoxCell, c => c.AutoSelectMode);

			var imageTextCell = grid.Columns.Select(r => r.DataCell).OfType<ImageTextCell>().FirstOrDefault();
			if (imageTextCell != null)
				control.SelectedValueBinding.Bind(imageTextCell, c => c.AutoSelectMode);
			return control;
		}

		Control AllowEmptySelectionCheckBox(T grid)
		{
			var control = new CheckBox { Text = "AllowEmptySelection" };
			control.CheckedBinding.Bind(grid, g => g.AllowEmptySelection);
			return control;
		}


		Control AllowColumnReorderingCheckBox(T grid)
		{
			var control = new CheckBox { Text = "AllowColumnReordering" };
			control.CheckedBinding.Bind(grid, g => g.AllowColumnReordering);
			return control;
		}

		Control SaveColumnDisplayOrderCheckBox(T grid)
		{
			var control = new CheckBox { Text = "Save Order and Visible" };
			control.CheckedBinding.Bind(TestApplication.Settings, s => s.GridViewSection_SaveColumnDisplayIndexes);
			control.CheckedChanged += (sender, e) =>
			{
				SaveColumnOrder();
				SaveColumnVisibility();
				SaveColumnWidths();
			};

			return control;
		}
		
		Control SetAllVisibleButton(T grid)
		{
			var control = new Button { Text = "All Column.Visible=true" };
			control.Click += (sender, e) =>
			{
				foreach (var col in grid.Columns)
				{
					col.Visible = true;
				}
				SaveColumnVisibility();
			};
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

		Control AllowMultiSelectCheckBox(T grid)
		{
			var control = new CheckBox { Text = "AllowMultipleSelection" };
			control.CheckedBinding.Bind(grid, r => r.AllowMultipleSelection);
			return control;
		}

		Control ShowHeaderCheckBox(T grid)
		{
			var control = new CheckBox { Text = "ShowHeader" };
			control.CheckedBinding.Bind(grid, r => r.ShowHeader);
			return control;
		}

		Control ReloadDataButton(T grid)
		{
			var control = new Button { Text = "ReloadData" };
			control.Click += (sender, e) => ReloadData(grid);
			return control;
		}


		Control SetDataButton(T grid)
		{
			var control = new Button { Text = "SetDataStore" };
			control.Click += (sender, e) => SetDataStore(grid);
			return control;
		}

		Control GridLinesDropDown(T grid)
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

		Control EnabledCheckBox(T grid)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(grid, r => r.Enabled);
			return control;
		}

		Control EditableCheckBox(T grid)
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

		Control CreateBorderType(T grid)
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
				return new Panel { Content = control, Padding = 2 };
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

		GridColumn SetColumnState(int index, GridColumn column)
		{
			// only set the indexes if we are saving/restoring to ensure it is not required
			// also, we want to support setting the indexes during creation, without having to
			// set them all at the end after added to the columns collection
			if (!TestApplication.Settings.GridViewSection_SaveColumnDisplayIndexes)
				return column;
			var indexes = TestApplication.Settings.GridViewSection_DisplayIndexes;
			if (indexes != null && index < indexes.Count)
			{
				column.DisplayIndex = indexes[index];
			}
			var visibleIndexes = TestApplication.Settings.GridViewSection_VisibleIndexes;
			if (visibleIndexes != null && index < visibleIndexes.Count)
			{
				column.Visible = visibleIndexes[index];
			}
			var widths = TestApplication.Settings.GridViewSection_ColumnWidths;
			if (widths != null && index < widths.Count)
			{
				column.Width = widths[index];
			}
			var autoSize = TestApplication.Settings.GridViewSection_ColumnAutoSize;
			if (autoSize != null && index < autoSize.Count)
			{
				column.AutoSize = autoSize[index];
			}

			return column;
		}


		void CreateGrid()
		{
			grid = new T();
			LogEvents(grid);

			var dropDown = MyDropDown("DropDownKey");
			grid.Columns.Add(SetColumnState(0, new GridColumn { HeaderText = "ImageText", DataCell = new ImageTextCell("Image", "Text") }));
			grid.Columns.Add(SetColumnState(1, new GridColumn { DataCell = new CheckBoxCell("Check"), AutoSize = true, Resizable = false }));
			grid.Columns.Add(SetColumnState(2, new GridColumn { HeaderText = "Image", DataCell = new ImageViewCell("Image"), Resizable = false }));
			grid.Columns.Add(SetColumnState(3, new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell("Text"), Sortable = true }));
			grid.Columns.Add(SetColumnState(4, new GridColumn { HeaderText = "Progress", DataCell = new ProgressCell("Progress") }));
			grid.Columns.Add(SetColumnState(5, new GridColumn { HeaderText = "Drop Down", DataCell = dropDown, Sortable = true }));
			if (Platform.Supports<CustomCell>())
			{
				var col = SetColumnState(6, new GridColumn { HeaderText = "Custom", Sortable = true, DataCell = new MyCustomCell() });
				grid.Columns.Add(col);
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
				grid.Columns.Add(SetColumnState(7, new GridColumn
				{
					HeaderText = "Drawable",
					DataCell = drawableCell
				}));
			}

		}

		ContextMenu CreateContextMenu(T grid)
		{
			// Context menu
			var menu = new ContextMenu();
			int currentColumn = -1;

			var columnVisibleItem = new CheckMenuItem { Text = "Column.Visible" };
			columnVisibleItem.CheckedChanged += (sender, e) =>
			{
				if (currentColumn >= 0)
				{
					grid.Columns[currentColumn].Visible = !grid.Columns[currentColumn].Visible;
					SaveColumnVisibility();
				}
			};
			menu.Items.Add(columnVisibleItem);

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
				var column = GetColumnAt(grid.PointFromScreen(Mouse.Position));
				currentColumn = -1;
				columnVisibleItem.Enabled = column >= 0;
				columnVisibleItem.Checked = column >= 0 ? grid.Columns[column].Visible : false;
				currentColumn = column;
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

			AddExtraContextMenuItems(menu);

			var subMenu = menu.Items.GetSubmenu("Sub Menu");
			item = new ButtonMenuItem { Text = "Item 5" };
			item.Click += (s, e) => Log.Write(item, "Clicked");
			subMenu.Items.Add(item);
			item = new ButtonMenuItem { Text = "Item 6" };
			item.Click += (s, e) => Log.Write(item, "Clicked");
			subMenu.Items.Add(item);

			return menu;
		}

		private void SaveColumnWidths()
		{
			if (!TestApplication.Settings.GridViewSection_SaveColumnDisplayIndexes)
				return;
			TestApplication.Settings.GridViewSection_ColumnWidths = grid.Columns.Select(r => r.Width).ToList();			
			TestApplication.Settings.GridViewSection_ColumnAutoSize = grid.Columns.Select(r => r.AutoSize).ToList();			
		}
		
		private void SaveColumnOrder()
		{
			if (!TestApplication.Settings.GridViewSection_SaveColumnDisplayIndexes)
				return;
			TestApplication.Settings.GridViewSection_DisplayIndexes = grid.Columns.Select(r => r.DisplayIndex).ToList();			
		}

		private void SaveColumnVisibility()
		{
			if (!TestApplication.Settings.GridViewSection_SaveColumnDisplayIndexes)
				return;
			TestApplication.Settings.GridViewSection_VisibleIndexes = grid.Columns.Select(r => r.Visible).ToList();
		}

		protected virtual void AddExtraContextMenuItems(ContextMenu menu)
		{
		}

		string GetDisplayIndexString(T control)
		{
			return string.Join(",", control.Columns.Select(r => r.DisplayIndex.ToString()));
		}

		protected virtual void LogEvents(T control)
		{
			control.CellEditing += (sender, e) => Log.Write(control, $"CellEditing, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.CellEdited += (sender, e) => Log.Write(control, $"CellEdited, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.SelectionChanged += (sender, e) => Log.Write(control, $"SelectionChanged, SelectedRows: {SelectedRowsString(control)}");
			control.SelectedItemsChanged += (sender, e) => Log.Write(control, $"SelectedItemsChanged, SelectedItems: {SelectedItemsString(control)}");
			control.ColumnHeaderClick += (sender, e) => Log.Write(control, $"ColumnHeaderClick: {e.Column.HeaderText}");
			control.CellClick += (sender, e) => Log.Write(control, $"CellClick, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.CellDoubleClick += (sender, e) => Log.Write(control, $"CellDoubleClick, Row: {e.Row}, Column: {e.Column}, Item: {e.Item}, GridColumn: {e.GridColumn}, IsEditing: {control.IsEditing}");
			control.ColumnOrderChanged += (sender, e) =>
			{
				SaveColumnOrder();
				Log.Write(grid, $"ColumnDisplayIndexChanged, Column: {e.Column}, DisplayIndex: {e.Column.DisplayIndex}, Indexes: {GetDisplayIndexString(grid)}");
			};
			control.ColumnWidthChanged += (sender, e) =>
			{
				SaveColumnWidths();
				Log.Write(control, $"ColumnWidthChanged, Column: {e.Column.HeaderText}, Width: {e.Column.Width}, AutoSize: {e.Column.AutoSize}");				
			};

			control.MouseDown += (sender, e) => Log.Write(control, $"MouseDown, Buttons: {e.Buttons}, Location: {e.Location}");
			control.MouseUp += (sender, e) => Log.Write(control, $"MouseUp, Buttons: {e.Buttons}, Location: {e.Location}");
			control.MouseDoubleClick += (sender, e) => Log.Write(control, $"MouseDoubleClick, Buttons: {e.Buttons}, Location: {e.Location}");
		}

		static string SelectedRowsString(T grid)
		{
			return string.Join(",", grid.SelectedRows.OrderBy(r => r).Select(r => r.ToString()));
		}

		static string SelectedItemsString(T grid)
		{
			return string.Join(",", grid.SelectedItems.OfType<MyGridItem>().Select(r => r.Text));
		}

		Button CreateBeginEditButton(T grid)
		{
			var control = new Button { Text = "Begin Edit Row:1, Column:0" };
			control.Click += (sender, e) => grid.BeginEdit(1, 0);
			return control;
		}

		Button CreateScrollToRow(T grid)
		{
			var control = new Button { Text = "ScrollToRow" };
			control.Click += (sender, e) =>
			{
				var row = new Random().Next(GetRowCount(grid) - 1);
				Log.Write(grid, "ScrollToRow: {0}", row);
				grid.ScrollToRow(row);
			};
			return control;
		}

		/// <summary>
		/// POCO (Plain Old CLR Object) to test property bindings
		/// </summary>
		protected class MyGridItem : INotifyPropertyChanged
		{
			bool? check;
			string text;
			string dropDownKey;
			float? progress;
			Color color;
			Image image;
			Command command;

			public event PropertyChangedEventHandler PropertyChanged;

			void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

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
					OnPropertyChanged();
					Log("Check", value);
				}
			}

			public string Text
			{
				get { return text; }
				set
				{
					text = value;
					OnPropertyChanged();
					Log("Text", value);
				}
			}

			public Image Image
			{
				get { return image; }
				set
				{
					image = value;
					OnPropertyChanged();
				}
			}

			public string DropDownKey
			{
				get { return dropDownKey; }
				set
				{
					dropDownKey = value;
					Log("DropDownKey", value);
					OnPropertyChanged();
				}
			}

			// used for drawable cell
			public Color Color
			{
				get { return color; }
				set
				{
					color = value;
					Log("Color", value);
					OnPropertyChanged();
				}
			}

			public float? Progress
			{
				get { return progress; }
				set
				{
					progress = value;
					Log("Progress", value);
					OnPropertyChanged();
				}
			}

			public Command Command
			{
				get
				{
					return command ?? (command = new Command((sender, e) => Test.Log.Write(null, "Command Executed for item: {0}", this)));
				}
			}

			public MyGridItem(int row, string name)
			{
				// initialize to random values
				this.Row = row;
				var val = row % 3;
				check = val == 0 ? (bool?)false : val == 1 ? (bool?)true : null;

				val = row % 2;
				image = val == 0 ? image1 : val == 1 ? (Image)image2 : null;

				text = $"Col 1 {name}";

				color = Color.FromElementId(row);

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

