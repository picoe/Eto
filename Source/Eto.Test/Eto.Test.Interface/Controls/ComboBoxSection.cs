using System;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class ComboBoxSection : Panel
	{
		public ComboBoxSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label{ Text = "Default"}, Default (), null);
			
			layout.AddRow (new Label{ Text = "Items"}, Items ());

			layout.AddRow (new Label{ Text = "Disabled"}, Disabled ());
			
			layout.AddRow (new Label{ Text = "Events"}, Events ());
			
			layout.AddRow (new Label{ Text = "EnumComboBox<Key>"}, EnumCombo ());
			
			layout.Add (null, null, true);
		}
		
		Control Default ()
		{
			return new ComboBox ();
		}
		
		ComboBox Items ()
		{
			var control = new ComboBox ();
			for (int i = 0; i < 20; i++) {
				control.Items.Add (new ListItem{ Text = "Item " + i});
			}
			return control;
		}
		
		ComboBox Disabled ()
		{
			var control = Items ();
			control.Enabled = false;
			return control;
		}
		
		ComboBox Events ()
		{
			var control = Items ();
			control.SelectedKey = "Item 8";
			control.SelectedIndexChanged += delegate {
				MessageBox.Show (this, string.Format ("Combo Box changed to {0}", control.SelectedValue.Text));
			};
			return control;
		}
		
		Control EnumCombo ()
		{
			var control = new EnumComboBox<Key> ();
			control.SelectedKey = ((int)Key.E).ToString ();
			return control;
		}
	}
}

