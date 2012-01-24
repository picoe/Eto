using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class ListBoxSection : SectionBase
	{
		public ListBoxSection ()
		{
			var layout = new TableLayout (this, 2, 4);
			
			int row = 0;
			layout.Add (new Label{ Text = "Default"}, 0, row);
			layout.Add (Default (), 1, row++);

			layout.Add (new Label{ Text = "With Icons"}, 0, row);
			layout.Add (WithIcons (), 1, row++);

			layout.Add (new Label{ Text = "Context Menu"}, 0, row);
			layout.Add (WithContextMenu (), 1, row++);
			
			layout.SetRowScale (row);
		}
		
		Control Default ()
		{
			var control = new ListBox {
				Size = new Size (100, 150)
			};
			LogEvents (control);

			for (int i = 0; i < 10; i++) {
				control.Items.Add (new ListItem{ Text = "Item " + i });
			}
			return control;
		}
		
		Control WithIcons ()
		{
			var control = new ListBox {
				Size = new Size (100, 150)
			};
			LogEvents (control);
			
			var image = new Icon (null, "Eto.Test.Interface.TestIcon.ico");
			var items = new List<IListItem> ();
			for (int i = 0; i < 1000; i++) {
				items.Add (new ImageListItem{ Text = "Item " + i, Image = image });
			}
			// use addrange for speed!
			control.Items.AddRange (items);
			return control;
		}

		Control WithContextMenu ()
		{
			var control = new ListBox {
				Size = new Size (100, 150)
			};
			LogEvents (control);

			for (int i = 0; i < 10; i++) {
				control.Items.Add (new ListItem{ Text = "Item " + i });
			}
			
			var menu = new ContextMenu ();
			var item = new ImageMenuItem{ Text = "Click Me!"};
			item.Click += delegate {
				if (control.SelectedValue != null)
					Log (item, "Click, Item: {0}", control.SelectedValue.Text);
				else 
					Log (item, "Click, no item selected");
			};
			menu.MenuItems.Add (item);
			
			control.ContextMenu = menu;
			return control;
		}
		
		void LogEvents (ListBox control)
		{
			control.SelectedIndexChanged += delegate {
				Log (control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);
			};
			control.Activated += delegate {
				Log (control, "Activated, Index: {0}", control.SelectedIndex);
			};
		}
		
	}
}

