using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class ListBoxSection : Panel
	{
		public ListBoxSection ()
		{
			this.AddDockedControl (Default (), new Padding (20));
		}
		
		Control Default ()
		{
			var control = new ListBox ();

			for (int i = 0; i < 10; i++) {
				control.Items.Add (new ListItem{ Text = "Item " + i, Key = i.ToString()});
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

