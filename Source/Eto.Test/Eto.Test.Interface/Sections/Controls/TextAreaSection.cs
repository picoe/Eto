using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class TextAreaSection : SectionBase
	{
		public TextAreaSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (new Label{ Text = "Default" }, Default ());
			layout.AddRow (new Label{ Text = "Different Size" }, DifferentSize ());
			layout.AddRow (new Label{ Text = "Read Only" }, ReadOnly ());
			layout.AddRow (new Label{ Text = "Disabled" }, Disabled ());
			layout.AddRow (new Label{ Text = "No Wrap" }, NoWrap ());
			layout.AddRow (new Label{ Text = "Wrap" }, Wrap ());
			
			// growing space at end is blank!
			layout.Add (null);
		}
		
		Control Default ()
		{
			var control = new TextArea { Text = "Some Text" };
			LogEvents (control);
			return control;
		}
		
		Control DifferentSize ()
		{
			var control = new TextArea{ Text = "Some Text", Size = new Size (100, 50) };
			LogEvents (control);
			return control;
		}
		
		Control ReadOnly ()
		{
			var control = new TextArea{ Text = "Read only text", ReadOnly = true, Size = new Size (100, 50) };
			LogEvents (control);
			return control;
		}

		Control Disabled ()
		{
			var control = DifferentSize ();
			control.Enabled = false;
			return control;
		}

		Control Wrap ()
		{
			var control = new TextArea{ 
				Text = "Some very long text that should wrap. Some very long text that should wrap. Some very long text that should wrap. Some very long text that should wrap.\nSecond Line",
				Size = new Size (100, 50),
				Wrap = true
			};
			LogEvents (control);
			return control;
		}
		
		Control NoWrap ()
		{
			var control = new TextArea{ 
				Text = "Some very long text that should not wrap. Some very long text that should not wrap. Some very long text that should not wrap. Some very long text that should not wrap.\nSecond Line",
				Size = new Size (100, 50),
				Wrap = false
			};
			LogEvents (control);
			return control;
		}
		
		void LogEvents (TextArea control)
		{
			control.TextChanged += delegate {
				Log (control, "TextChanged, Text: {0}", control.Text);
			};
		}
	}
}

