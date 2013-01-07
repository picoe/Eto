using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class TreeGridViewSection : Scrollable
	{
		int expanded;
		CheckBox allowCollapsing;
		CheckBox allowExpanding;
		static Image Image = Icon.FromResource ("Eto.Test.TestIcon.ico");
		
		public TreeGridViewSection ()
		{
			var layout = new DynamicLayout (this);
		
			layout.BeginHorizontal ();
			layout.Add (new Label { });
			layout.BeginVertical ();
			layout.BeginHorizontal ();
			layout.Add (null);
			layout.Add (allowExpanding = new CheckBox{ Text = "Allow Expanding", Checked = true });
			layout.Add (allowCollapsing = new CheckBox{ Text = "Allow Collapsing", Checked = true });
			layout.Add (null);
			layout.EndHorizontal ();
			layout.EndVertical ();
			layout.EndHorizontal ();
			
			layout.AddRow (new Label{ Text = "Simple" }, Default ());
			
			layout.AddRow (new Label{ Text = "With Images\n&& Context Menu" }, ImagesAndMenu ());
			layout.AddRow (new Label{ Text = "Disabled" }, Disabled ());
			
			layout.Add (null, false, true);
		}
		
		TreeGridItem CreateSimpleTreeItem (int level, string name)
		{
			var item = new TreeGridItem {
				Expanded = expanded++ % 2 == 0
			};
			item.Values = new object[] { "col 0 - " + name };
			if (level < 4) {
				for (int i = 0; i < 4; i++) {
					item.Children.Add (CreateSimpleTreeItem (level + 1, name + " " + i));
				}
			}
			return item;
		}
		
		Control Default ()
		{
			var control = new TreeGridView {
				Size = new Size (100, 150),
				ShowHeader = false
			};
			control.Columns.Add (new GridColumn{ DataCell = new TextBoxCell (0) });
			control.DataStore = CreateSimpleTreeItem (0, "");
			LogEvents (control);
			return control;
		}

		TreeGridItem CreateComplexTreeItem (int level, string name, Image image)
		{
			var item = new TreeGridItem {
				Expanded = expanded++ % 2 == 0
			};
			item.Values = new object[] { image, "col 0 - " + name, "col 1 - " + name };
			if (level < 4) {
				for (int i = 0; i < 4; i++) {
					item.Children.Add (CreateComplexTreeItem (level + 1, name + " " + i, image));
				}
			}
			return item;
		}

		Control ImagesAndMenu ()
		{
			var control = new TreeGridView {
				Size = new Size (100, 150)
			};

			control.Columns.Add (new GridColumn { DataCell = new ImageTextCell (0, 1), HeaderText = "Image and Text", AutoSize = true, Resizable = true, Editable = true });
			control.Columns.Add (new GridColumn { DataCell = new TextBoxCell (2), HeaderText = "Text", AutoSize = true, Width = 150, Resizable = true, Editable = true });
		
#if DESKTOP
			var menu = new ContextMenu ();
			var item = new ImageMenuItem{ Text = "Click Me!"};
			item.Click += delegate {
				if (control.SelectedItem != null)
					Log.Write (item, "Click, Rows: {0}", control.SelectedItem);
				else
					Log.Write (item, "Click, no item selected");
			};
			menu.MenuItems.Add (item);
			
			control.ContextMenu = menu;
#endif

			control.DataStore = CreateComplexTreeItem (0, "", Image);
			LogEvents (control);
			return control;
		}
		
		Control Disabled ()
		{
			var control = ImagesAndMenu ();
			control.Enabled = false;
			return control;
		}


		string GetDescription (ITreeGridItem item)
		{
			var treeItem = item as TreeGridItem;
			if (treeItem != null)
				return Convert.ToString (string.Join (", ", treeItem.Values.Select (r => Convert.ToString (r))));
			else
				return Convert.ToString (item);
		}

		void LogEvents (TreeGridView control)
		{
			control.Activated += (sender, e) => {
				Log.Write (control, "Activated, Item: {0}", GetDescription (e.Item));
			};
			control.SelectionChanged += delegate {
				Log.Write (control, "SelectionChanged, Rows: {0}", string.Join(", ", control.SelectedRows.Select(r => r.ToString())));
			};
			control.SelectedItemChanged += delegate {
				Log.Write (control, "SelectedItemChanged, Item: {0}", control.SelectedItem != null ? GetDescription (control.SelectedItem) : "<none selected>");
			};

			control.Expanding += (sender, e) => {
				Log.Write (control, "Expanding, Item: {0}", GetDescription (e.Item));
				e.Cancel = !(allowExpanding.Checked ?? true);
			};
			control.Expanded += (sender, e) => {
				Log.Write (control, "Expanded, Item: {0}", GetDescription (e.Item));
			};
			control.Collapsing += (sender, e) => {
				Log.Write (control, "Collapsing, Item: {0}", GetDescription (e.Item));
				e.Cancel = !(allowCollapsing.Checked ?? true);
			};
			control.Collapsed += (sender, e) => {
				Log.Write (control, "Collapsed, Item: {0}", GetDescription (e.Item));
			};
			control.ColumnHeaderClick += delegate(object sender, GridColumnEventArgs e) {
				Log.Write (control, "Column Header Clicked: {0}", e.Column);
			};
		}
	}
}

