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

		public void HandleTextChanged(object sender, EventArgs e)
		{
			Log.Write(sender, "Text was changed: {0}", ((TextBox)sender).Text);
		}

		public void HandleKeyDown(object sender, KeyEventArgs e)
		{
			Log.Write(sender, "Key was pressed: {0}", e.Key);
		}
	}
}
