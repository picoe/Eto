using System;
using Eto.Forms;
using Eto.Test.Controls;

namespace Eto.Test.Sections.Controls
{
	public class RadioButtonSection : SectionBase
	{
		public RadioButtonSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label{ Text = "Default" }, Default (), null);

			layout.AddRow (new Label{ Text = "Set Initial Value" }, SetInitialValue (), null);

			layout.AddRow (new Label{ Text = "Disabled" }, Disabled (), null);
			
			layout.Add (null, null, true);
		}
		
		Control Default ()
		{
			var layout = new DynamicLayout (new Panel ());
			
			var first = new RadioButton { Text = "First" };
			var second = new RadioButton (first) { Text = "Second"  };
			var third = new RadioButton (first) { Text = "Third" };
			layout.AddRow (first, second, third);
			LogEvents (first);
			LogEvents (second);
			LogEvents (third);
			
			return layout.Container;
		}
		
		Control SetInitialValue ()
		{
			var layout = new DynamicLayout (new Panel ());
			
			layout.BeginHorizontal ();
			RadioButton controller = null;
			for (int i = 0; i < 5; i++) {
				var item = new RadioButton(controller) { Text = "Item " + i, Checked = i == 2 };
				if (controller == null) controller = item;
				LogEvents (item);
				layout.Add (item);
			}
			layout.EndHorizontal ();
			
			return layout.Container;
		}
		
		Control Disabled ()
		{
			var layout = new DynamicLayout (new Panel ());
			
			layout.BeginHorizontal ();
			RadioButton controller = null;
			for (int i = 0; i < 5; i++) {
				var item = new RadioButton(controller) { Text = "Item " + i, Checked = i == 2, Enabled = false };
				if (controller == null) controller = item;
				LogEvents (item);
				layout.Add (item);
			}
			layout.EndHorizontal ();
			
			return layout.Container;
		}
		
		void LogEvents (RadioButton control)
		{
			control.CheckedChanged += delegate {
				Log.Write (control, "CheckedChanged, Value: {0}, Checked: {1}", control.Text, control.Checked);
			};
		}
		
	}
}

