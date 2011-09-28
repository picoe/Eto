using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class TabControlSection : Panel
	{
		public TabControlSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.Add (DefaultTabs ());
			
		}
		
		Control DefaultTabs ()
		{
			var control = new TabControl ();
			
			var page = new TabPage { Text = "Tab 1" };
			page.AddDockedControl (TabOne ());
			control.TabPages.Add (page);
			
			page = new TabPage { 
				Text = "Tab 2", 
				Image = new Icon(null, "Eto.Test.Interface.TestIcon.ico") 
			};
			page.AddDockedControl (TabTwo ());
			control.TabPages.Add (page);
			
			return control;
			
		}
		
		Control TabOne ()
		{
			var control = new Panel ();
			
			control.AddDockedControl (new LabelSection());
			
			return control;
		}

		Control TabTwo ()
		{
			var control = new Panel ();
			
			control.AddDockedControl (new TextAreaSection());
			
			return control;
		}
	}
}

