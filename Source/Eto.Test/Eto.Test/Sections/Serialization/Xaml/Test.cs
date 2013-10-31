#if XAML

using System;
using Eto.Forms;
using Eto.Xaml;

namespace Eto.Test.Sections.Serialization.Xaml
{
	public class Test : Panel
	{
		protected CheckBox myCheckBox { get; set; }
		protected TextArea myTextArea { get; set; }

		public Test ()
		{
			// NOTE: Only works in MS.NET or Mono 2.11 at the moment
			XamlReader.Load (this);
			
			myCheckBox.Checked = true;
			myTextArea.Text = "This form was created via xaml!";
		}

		protected void HandleButtonClick (object sender, EventArgs e)
		{
			MessageBox.Show (this, "I was clicked from Xaml!");
		}

	}
}

#endif
