using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	class LogGridItem : GridItem
	{
		public int Row { get; set; }

		public LogGridItem(params object[] values)
			: base (values)
		{
		}

		public override void SetValue(int column, object value)
		{
			base.SetValue(column, value);
			Log.Write(this, "SetValue, Row: {0}, Column: {1}, Value: {2}", Row, column, value);
		}
	}

	public class GridViewSection : Panel
	{
		static Image image1 = TestIcons.TestImage;
		static Image image2 = TestIcons.TestIcon;
		SearchBox filterText = null;

		public GridViewSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "No Header,\nNon-Editable" }, NoHeader());
#if DESKTOP
			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Context Menu\n&& Multi-Select\n&& Filter" });
			layout.BeginVertical();
			layout.Add(filterText = new SearchBox { PlaceholderText = "Filter" });
			var withContextMenuAndFilter = WithContextMenuAndFilter();
			layout.Add(withContextMenuAndFilter);
			var selectionGridView = Default(addItems: false);
			layout.EndVertical();
			layout.EndHorizontal();
			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Selected Items" });
			layout.Add(selectionGridView);
			withContextMenuAndFilter.SelectionChanged += (s, e) => {
				var items = new GridItemCollection();
				items.AddRange(withContextMenuAndFilter.SelectedItems);
				selectionGridView.DataStore = items;
			};
			layout.EndHorizontal();
#endif

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
			public MyGridItem(Random rand, int row, ComboBoxCell dropDown)
			{
				// initialize to random values
				this.Row = row;
				var val = rand.Next(3);
				check = val == 0 ? (bool?)false : val == 1 ? (bool?)true : null;

				val = rand.Next(3);
				Image = val == 0 ? (Image)image1 : val == 1 ? (Image)image2 : null;

				text = string.Format("Col 1 Row {0}", row);

				Color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));

				if (dropDown != null)
				{
					val = rand.Next(dropDown.DataStore.Count + 1);
					dropDownKey = val == 0 ? null : dropDown.DataStore[val - 1].Key;
				}
			}
		}

		ComboBoxCell MyDropDown(string bindingProperty)
		{
			var combo = new ComboBoxCell(bindingProperty);
			var items = new ListItemCollection();
			items.Add(new ListItem { Text = "Item 1" });
			items.Add(new ListItem { Text = "Item 2" });
			items.Add(new ListItem { Text = "Item 3" });
			items.Add(new ListItem { Text = "Item 4" });
			combo.DataStore = items;
			return combo;
		}

		GridView Default(bool addItems = true)
		{
			var control = new GridView
			{
				Size = new Size (300, 100)
			};
			LogEvents(control);
			
			var dropDown = MyDropDown("DropDownKey");
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell ("Check"), Editable = true, AutoSize = true, Resizable = false });
			control.Columns.Add(new GridColumn { HeaderText = "Image", DataCell = new ImageViewCell ("Image") });
			control.Columns.Add(new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell ("Text"), Editable = true, Sortable = true });
			control.Columns.Add(new GridColumn { HeaderText = "Drop Down", DataCell = dropDown, Editable = true, Sortable = true });

#if Windows // Drawable cells - need to implement on other platforms.
			var drawableCell = new DrawableCell
			{
				PaintHandler = args => {
					var m = args.Item as MyGridItem;
					if (m != null)
						args.Graphics.FillRectangle(Brushes.Cached(m.Color) as SolidBrush, args.CellBounds);
				}
			};
			control.Columns.Add(new GridColumn { HeaderText = "Owner drawn", DataCell = drawableCell });
#endif

			if (addItems)
			{
				var items = new GridItemCollection();
				var rand = new Random();
				for (int i = 0; i < 10000; i++)
				{
					items.Add(new MyGridItem(rand, i, dropDown));
				}
				control.DataStore = items;
			}

			return control;
		}

		GridView NoHeader()
		{
			var control = Default();
			foreach (var col in control.Columns)
			{
				col.Editable = false;
			}
			control.ShowHeader = false;
			return control;
		}
		#if DESKTOP
		GridView WithContextMenuAndFilter()
		{
			var control = Default();
			control.AllowMultipleSelection = true;
			var items = control.DataStore as GridItemCollection;

			// Filter
			filterText.TextChanged += (s, e) => {
				var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				
				// Set the filter delegate on the GridView
				control.Filter = (filterItems.Length == 0) 
				? (Func<object, bool>)null
				: o => {
					var i = o as MyGridItem;					
					var matches = true;

					// Every item in the split filter string should be within the Text property
					foreach (var filterItem in filterItems)
						if (!i.Text.Contains(filterItem))
						{
							matches = false;
							break;
						}
					;

					return matches;
				};
			};
			
			// Context menu
			var menu = new ContextMenu();
			var item = new ImageMenuItem { Text = "Click Me!" };
			item.Click += delegate
			{
				if (control.SelectedRows.Count() > 0)
					Log.Write(item, "Click, Rows: {0}", SelectedRowsString(control));
				else
					Log.Write(item, "Click, no item selected");
			};
			menu.MenuItems.Add(item);

			// Delete menu item: deletes the item from the store, the UI updates via the binding.
			var deleteItem = new ImageMenuItem { Text = "Delete Item" };
			deleteItem.Click += (s, e) =>
			{
				var i = control.SelectedItems.First() as MyGridItem;
				if (i != null)
					items.Remove(i);
			};
			menu.MenuItems.Add(deleteItem);

			// Insert item: inserts an item into the store, the UI updates via the binding.
			var insertItem = new ImageMenuItem { Text = "Insert Item at the start of the list" };
			insertItem.Click += (s, e) => {
				var i = control.SelectedItems.First() as MyGridItem;
				if (i != null)
					items.Insert(0, new MyGridItem(new Random(), 0, null));
			};
			menu.MenuItems.Add(insertItem);
			
			control.ContextMenu = menu;
			return control;
		}
		#endif
		protected virtual void LogEvents(GridView control)
		{
			control.BeginCellEdit += (sender, e) => {
				Log.Write(control, "BeginCellEdit, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			};
			control.EndCellEdit += (sender, e) => {
				Log.Write(control, "EndCellEdit, Row: {0}, Column: {1}, Item: {2}, ColInfo: {3}", e.Row, e.Column, e.Item, e.GridColumn);
			};
			control.SelectionChanged += delegate
			{
				Log.Write(control, "Selection Changed, Rows: {0}", SelectedRowsString(control));
			};
			control.ColumnHeaderClick += delegate(object sender, GridColumnEventArgs e)
			{
				Log.Write(control, "Column Header Clicked: {0}", e.Column);
			};
		}

		string SelectedRowsString(GridView control)
		{
			return string.Join(",", control.SelectedRows.Select(r => r.ToString()).OrderBy(r => r));
		}
	}
}

