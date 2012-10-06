using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Test.Sections.Serialization.Xaml
{
#if XAML
	public class Test : Panel
	{
		protected CheckBox myCheckBox;
		protected TextArea myTextArea;

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
#endif
}
