using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Test.Interface.Xaml
{
	public class Test : Panel
	{
		protected CheckBox someOption;
		protected TextArea someTextArea;

		public Test ()
		{
			XamlReader.Load (Resources.GetResource ("Eto.Test.Interface.Xaml.Test.xaml"), this);

			someOption.Checked = true;
			someTextArea.Text = "This form was created via xaml!";
		}

		protected void HandleButtonClick (object sender, EventArgs e)
		{
			MessageBox.Show (this, "I was clicked from Xaml!");
		}

	}
}
