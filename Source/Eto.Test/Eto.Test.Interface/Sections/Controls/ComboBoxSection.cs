using System;
using Eto.Forms;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class ComboBoxSection : SectionBase
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
			var control = new ComboBox ();
			LogEvents (control);
			return control;
		}
		
		ComboBox Items ()
		{
			var control = new ComboBox ();
			LogEvents (control);
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
			return control;
		}
		
		Control EnumCombo ()
		{
			var control = new EnumComboBox<Key> ();
			LogEvents (control);
			control.SelectedKey = ((int)Key.E).ToString ();
			return control;
		}
		
		void LogEvents (ComboBox control)
		{
			control.SelectedIndexChanged += delegate {
				Log.Write (control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);
			};
		}
		
	}
}

