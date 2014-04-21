using System;
using Eto.Forms;

namespace Eto.Test.Sections.Serialization.Json
{
	public class Test : Panel
	{
		protected CheckBox MyCheckBox { get; set; }
		protected TextArea MyTextArea { get; set; }

		public Test ()
		{
#if !PCL
			Eto.Json.JsonReader.Load (this);
#endif

			MyCheckBox.Checked = true;
			MyTextArea.Text = "This form was created via json!";
		}

		protected void HandleButtonClick (object sender, EventArgs e)
		{
			MessageBox.Show (this, "I was clicked from Json!");
		}

	}
}
