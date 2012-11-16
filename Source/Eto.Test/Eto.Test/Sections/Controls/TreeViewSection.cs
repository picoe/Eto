using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	public class TreeViewSection : Panel
	{
		int expanded;
		CheckBox allowCollapsing;
		CheckBox allowExpanding;

		static Image Image = Icon.FromResource ("Eto.Test.TestIcon.ico");
		
		public TreeViewSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.BeginHorizontal ();
			layout.Add (new Label { });
			layout.BeginVertical ();
			layout.BeginHorizontal ();
			layout.Add (null);
			layout.Add (allowExpanding = new CheckBox{ Text = "Allow Expanding", Checked = true });
			layout.Add (allowCollapsing = new CheckBox{ Text = "Allow Collapsing", Checked = true });
			layout.Add (RefreshButton ());
			layout.Add (null);
			layout.EndHorizontal ();
			layout.EndVertical ();
			layout.EndHorizontal ();

			layout.AddRow (new Label{ Text = "Simple" }, Default ());
			
			layout.AddRow (new Label{ Text = "With Images\n&& Context Menu" }, ImagesAndMenu ());
            layout.AddRow(new Label { Text = "Dynamic via the Context Menu" }, Dynamic());
            layout.AddRow(new Label { Text = "Disabled" }, Disabled());
			
			layout.Add (null, false, true);
		}

		Control RefreshButton ()
		{
			var control = new Button { Text = "Refresh" };
			control.Click += (sender, e) => {
				foreach (var tree in this.Children.OfType<TreeView>())
				{
					tree.RefreshData ();
				}
			};
			return control;
		}
		
		TreeItem CreateTreeItem (int level, string name, Image image)
		{
			var item = new TreeItem {
				Text = name,
				Expanded = expanded++ % 2 == 0,
				Image = image
			};
			if (level < 4) {
				for (int i = 0; i < 4; i++) {
					item.Children.Add (CreateTreeItem (level + 1, name + " " + i, image));
				}
			}
			return item;
		}
		
		Control Default ()
		{
			var control = new TreeView {
				Size = new Size(100, 150)
			};
			control.DataStore = CreateTreeItem (0, "Item", null);
			LogEvents (control);
			return control;
		}

		Control ImagesAndMenu ()
		{
			var control = new TreeView {
				Size = new Size(100, 150)
			};

#if DESKTOP
			var menu = new ContextMenu ();
			var item = new ImageMenuItem{ Text = "Click Me!"};
			item.Click += delegate {
				if (control.SelectedItem != null)
					Log.Write (item, "Click, Rows: {0}", control.SelectedItem.Text);
				else
					Log.Write (item, "Click, no item selected");
			};
			menu.MenuItems.Add (item);
			
			control.ContextMenu = menu;
#endif

			control.DataStore = CreateTreeItem (0, "Item", Image);
			LogEvents (control);
			return control;
		}

        Control Dynamic()
        {
            var control = new TreeView
            {
                Size = new Size(100, 150)
            };

            var menu = new ContextMenu();
            var item = new ImageMenuItem { Text = "Click Me!" };
            item.Click += (s, e) =>
            {
                if (control.SelectedItem != null)
                {
                    var treeItem = 
                        control.SelectedItem
                            as TreeItem;

                    if (treeItem != null)
                        treeItem.Children.Add(
                            new TreeItem
                            {
                                Text = "New Item"
                            });

                    Log.Write(item, "Click, Rows: {0}", control.SelectedItem.Text);
                }
                else
                    Log.Write(item, "Click, no item selected");
            };
            menu.MenuItems.Add(item);

            control.ContextMenu = menu;

            LogEvents(control);

            if (false)
                control.DataStore = new TreeItem
                {
                    Text = "Root",
                    Expanded = true
                };
            else
                control.DataStore =
                    CreateTreeItem(2, "Item", Image);

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
				Log.Write (control, "Activated, Item: {0}", e.Item.Text);
			};
			control.SelectionChanged += delegate {
				Log.Write (control, "SelectionChanged, Item: {0}", control.SelectedItem != null ? control.SelectedItem.Text : "<none selected>");
			};
			control.Expanding += (sender, e) => {
				Log.Write (control, "Expanding, Item: {0}", e.Item);
				e.Cancel = !(allowExpanding.Checked ?? true);
			};
			control.Expanded += (sender, e) => {
				Log.Write (control, "Expanded, Item: {0}", e.Item);
			};
			control.Collapsing += (sender, e) => {
				Log.Write (control, "Collapsing, Item: {0}", e.Item);
				e.Cancel = !(allowCollapsing.Checked ?? true);
			};
			control.Collapsed += (sender, e) => {
				Log.Write (control, "Collapsed, Item: {0}", e.Item);
			};
		}
	}
}

