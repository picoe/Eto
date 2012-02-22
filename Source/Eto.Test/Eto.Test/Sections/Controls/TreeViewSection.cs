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
			
			layout.AddRow (new Label{ Text = "With Images" }, Images ());
			layout.AddRow (new Label{ Text = "Disabled" }, Disabled ());
			
			layout.Add (null, false, true);
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
			LogEvents (control);
			control.DataStore = CreateTreeItem (0, "Item", null);
			return control;
		}

		Control Images ()
		{
			var control = new TreeView {
				Size = new Size(100, 150)
			};
			LogEvents (control);
			control.DataStore = CreateTreeItem (0, "Item", Image);
			return control;
		}
		
		Control Disabled ()
		{
			var control = Images ();
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
		}
	}
}

