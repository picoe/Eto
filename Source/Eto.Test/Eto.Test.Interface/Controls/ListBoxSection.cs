using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class ListBoxSection : Panel
	{
		public ListBoxSection ()
		{
			var layout = new TableLayout(this, 2, 4);
			
			int row = 0;
			layout.Add(new Label{ Text = "Default"}, 0, row);
			layout.Add(Default (), 1, row++);

			layout.Add(new Label{ Text = "With Icons"}, 0, row);
			layout.Add(WithIcons (), 1, row++);

			layout.Add(new Label{ Text = "Context Menu"}, 0, row);
			layout.Add(WithContextMenu (), 1, row++);
			
			layout.SetRowScale(row);
		}
		
		Control Default ()
		{
			var control = new ListBox {
				Size = new Size(100, 150)
			};

			for (int i = 0; i < 10; i++) {
				control.Items.Add (new ListItem{ Text = "Item " + i });
			}
			return control;
		}
		
		Control WithIcons ()
		{
			var control = new ListBox {
				Size = new Size(100, 150)
			};
			
			var image = new Icon(null, "Eto.Test.Interface.TestIcon.ico");
			for (int i = 0; i < 10; i++) {
				control.Items.Add (new ImageListItem{ Text = "Item " + i, Image = image });
			}
			return control;
		}

		Control WithContextMenu ()
		{
			var control = new ListBox {
				Size = new Size(100, 150)
			};

			for (int i = 0; i < 10; i++) {
				control.Items.Add (new ListItem{ Text = "Item " + i });
			}
			
			var menu = new ContextMenu ();
			var item = new ImageMenuItem{ Text = "Click Me!"};
			item.Click += delegate {
				if (control.SelectedValue != null)
					MessageBox.Show (this, string.Format("You clicked me on {0}", control.SelectedValue.Text));
				else 
					MessageBox.Show (this, "You clicked me without any selection");
			};
			menu.MenuItems.Add (item);
			
			control.ContextMenu = menu;
			return control;
		}
		
	}
}

