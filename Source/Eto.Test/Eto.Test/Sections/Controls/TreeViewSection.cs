using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class TreeViewSection : Panel
	{
		int expanded;

		static Image Image = Icon.FromResource ("Eto.Test.TestIcon.ico");
		
		public TreeViewSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label{ Text = "Simple" }, Default ());
			
			layout.AddRow (new Label{ Text = "With Images\n& Context Menu" }, ImagesAndMenu ());
			layout.AddRow (new Label{ Text = "Disabled" }, Disabled ());
			
			layout.Add (null, false, true);
		}
		

		TreeItem CreateSimpleTreeItem (int level, string name)
		{
			var item = new TreeItem {
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
			var control = new TreeView {
				Size = new Size(100, 150),
				ShowHeader = false
			};
			control.Columns.Add (new TreeColumn{ DataCell = new TextCell(0) });
			LogEvents (control);
			control.DataStore = CreateSimpleTreeItem (0, "");
			return control;
		}

		TreeItem CreateComplexTreeItem (int level, string name, Image image)
		{
			var item = new TreeItem {
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
			var control = new TreeView {
				Size = new Size(100, 150)
			};
			
			control.Columns.Add (new TreeColumn{ DataCell = new ImageTextCell(0, 1), HeaderText = "Outline", AutoSize = true, Resizable = true, Editable = true });
			control.Columns.Add (new TreeColumn{ DataCell = new TextCell(2), HeaderText = "Hello!", AutoSize = false, Resizable = true, Editable = true });
			
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

			LogEvents (control);
			control.DataStore = CreateComplexTreeItem (0, "", Image);
			return control;
		}
		
		Control Disabled ()
		{
			var control = ImagesAndMenu ();
			control.Enabled = false;
			return control;
		}
		
		void LogEvents (TreeView control)
		{
			control.Activated += delegate(object sender, TreeViewItemEventArgs e) {
				Log.Write (control, "Activated, Item: {0}", e.Item);
			};
			control.SelectionChanged += delegate {
				Log.Write (control, "SelectionChanged, Item: {0}", control.SelectedItem != null ? Convert.ToString (control.SelectedItem) : "<none selected>");
			};
		}
	}
}

