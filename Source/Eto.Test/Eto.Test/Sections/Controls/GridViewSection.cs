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
		readonly SearchBox filterText;
		GridView defaultGridView;

		public GridViewSection()
		{
			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Default" }, Default(CreateItems().ToArray()));
			layout.AddRow(new Label { Text = "No Header,\nNon-Editable" }, NoHeader());
			if (Platform.Supports<ContextMenu>())
			{
				layout.BeginHorizontal();
				layout.Add(new Label { Text = "Context Menu\n&& Multi-Select\n&& Filter" });
				layout.BeginVertical();
				layout.Add(filterText = new SearchBox { PlaceholderText = "Filter" });
				layout.Add(BeginEdit());
				var withContextMenuAndFilter = WithContextMenuAndFilter();
				layout.Add(withContextMenuAndFilter);
				layout.EndVertical();
				layout.EndHorizontal();

				var selectionGridView = Default(null);
				layout.AddRow(new Label { Text = "Selected Items" }, selectionGridView);

				// hook up selection of main grid to the selection grid
				withContextMenuAndFilter.SelectionChanged += (s, e) =>
				{
					selectionGridView.DataStore = withContextMenuAndFilter.SelectedItems.ToArray();
				};
			}

			Content = layout;
		}

		/// <summary>
		/// POCO (Plain Old CLR Object) to test property bindings
		/// </summary>
		class MyGridItem
		{
			bool? check;
			string text;
			string dropDownKey;

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

			public Color Color { get; set; }
			// used for owner-drawn cells
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
			}
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

		GridView Default(IEnumerable<object> items)
		{
			var control = new GridView
			{
				Size = new Size(300, 100),
				DataStore = items
			};
			LogEvents(control);

			var dropDown = MyDropDown("DropDownKey");
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell("Check"), Editable = true, AutoSize = true, Resizable = false });
			control.Columns.Add(new GridColumn { HeaderText = "Image", DataCell = new ImageViewCell("Image") });
			control.Columns.Add(new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell("Text"), Editable = true, Sortable = true });
			control.Columns.Add(new GridColumn { HeaderText = "Drop Down", DataCell = dropDown, Editable = true, Sortable = true });

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

		GridView NoHeader()
		{
			var control = Default(CreateItems().ToArray());
			foreach (var col in control.Columns)
			{
				col.Editable = false;
			}
			control.ShowHeader = false;
			return control;
		}

		GridView WithContextMenuAndFilter()
		{
			var control = defaultGridView = Default(null);
			var filtered = new SelectableFilterCollection<MyGridItem>(control, CreateItems().ToArray());
			control.DataStore = filtered;
			control.AllowMultipleSelection = true;

			// Filter
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
							if (i.Text.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
							{
								return false;
							}
						}
						return true;
					};
			};

			// Context menu
			var menu = new ContextMenu();
			var item = new ButtonMenuItem { Text = "Click Me!" };
			item.Click += delegate
			{
				if (control.SelectedRows.Any())
					Log.Write(item, "Click, Rows: {0}", SelectedRowsString(control));
				else
					Log.Write(item, "Click, no item selected");
			};
			menu.Items.Add(item);

			// Delete menu item: deletes the item from the store, the UI updates via the binding.
			var deleteItem = new ButtonMenuItem { Text = "Delete Item" };
			deleteItem.Click += (s, e) =>
			{
				var i = control.SelectedItems.First() as MyGridItem;
				if (i != null)
					filtered.Remove(i);
			};
			menu.Items.Add(deleteItem);

			// Insert item: inserts an item into the store, the UI updates via the binding.
			var insertItem = new ButtonMenuItem { Text = "Insert Item at the start of the list" };
			insertItem.Click += (s, e) =>
			{
				var i = control.SelectedItems.First() as MyGridItem;
				if (i != null)
					filtered.Insert(0, new MyGridItem(new Random(), 0));
			};
			menu.Items.Add(insertItem);

			control.ContextMenu = menu;
			return control;
		}

		protected virtual void LogEvents(GridView control)
		{
			control.CellEditing += (sender, e) =>
			{
				Log.Write(control, "BeginCellEdit, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			};
			control.CellEdited += (sender, e) =>
			{
				Log.Write(control, "EndCellEdit, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			};
			control.SelectionChanged += delegate
			{
				Log.Write(control, "Selection Changed, Rows: {0}", SelectedRowsString(control));
			};
			control.ColumnHeaderClick += delegate(object sender, GridColumnEventArgs e)
			{
				Log.Write(control, "Column Header Clicked: {0}", e.Column.HeaderText);
			};
		}

		static string SelectedRowsString(GridView control)
		{
			return string.Join(",", control.SelectedRows.Select(r => r.ToString()).OrderBy(r => r));
		}

		Button BeginEdit()
		{
			var control = new Button {Text = "Begin Edit Row:1, Column:2"};
			control.Click += delegate
			{
				defaultGridView.BeginEdit(1, 2);
			};
			return control;
		}
	}
}

