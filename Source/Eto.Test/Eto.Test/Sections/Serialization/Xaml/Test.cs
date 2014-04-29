#if XAML

using System;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace Eto.Test.Sections.Serialization.Xaml
{
	public class Test : Panel
	{
		protected CheckBox MyCheckBox { get; set; }
		protected TextArea MyTextArea { get; set; }

		public Test ()
		{
			// NOTE: Only works in MS.NET or Mono 2.11 at the moment
			XamlReader.Load (this);
			
			MyCheckBox.Checked = true;
			MyTextArea.Text = "This form was created via xaml!";
		}

		protected void HandleButtonClick (object sender, EventArgs e)
		{
			MessageBox.Show (this, "I was clicked from Xaml!");
		}

	}
}

#endif
