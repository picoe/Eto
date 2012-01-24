using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class TabControlSection : SectionBase
	{
		public TabControlSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.Add (DefaultTabs ());
			
		}
		
		Control DefaultTabs ()
		{
			var control = new TabControl ();
			LogEvents (control);
			
			var page = new TabPage { Text = "Tab 1" };
			page.AddDockedControl (TabOne ());
			control.TabPages.Add (page);
			
			LogEvents (page);
			
			page = new TabPage { 
				Text = "Tab 2", 
				Image = new Icon(null, "Eto.Test.Interface.TestIcon.ico") 
			};
			LogEvents (page);
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
		
		void LogEvents (TabControl control)
		{
			control.SelectedIndexChanged += delegate {
				Log (control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);	
			};
		}
		
		void LogEvents (TabPage control)
		{
			control.Click += delegate {
				Log (control, "Click, Item: {0}", control.Text);
			};
		}
	}
}

