using System;
using Eto.Forms;

namespace Eto.Test.Sections.Serialization.Json
{
	public class Test : Panel
	{
		protected CheckBox MyCheckBox { get; set; }

		protected TextArea MyTextArea { get; set; }

		public Test()
		{
			Eto.Serialization.Json.JsonReader.Load(this);

			MyCheckBox.Checked = true;
			MyTextArea.Text = "This form was created via json!";
		}

		public void HandleButtonClick(object sender, EventArgs e)
		{
			MessageBox.Show(this, "I was clicked from Json!");
		}
	}
}
