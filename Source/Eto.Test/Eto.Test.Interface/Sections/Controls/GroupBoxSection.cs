using System;
using Eto.Forms;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class GroupBoxSection : SectionBase
	{
		public GroupBoxSection ()
		{
			var layout = new DynamicLayout (this);
			
			
			layout.AddRow (new Label{ Text = "Default"}, Default ());
			
			layout.AddRow (new Label { Text = "With Header" }, Header ());
			
			layout.Add (null, null, true);
		}
		
		Control Default ()
		{
			var control = new GroupBox ();
			
			control.AddDockedControl (new CheckBoxSection ());
			return control;
			
		}
		
		Control Header ()
		{
			var control = new GroupBox{ Text = "Some Header" };
			
			control.AddDockedControl (new LabelSection ());
			return control;
		}
	}
}

