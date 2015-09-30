using System;
using Eto.Forms;
using Eto.Serialization.Xaml;
using System.Windows.Input;
using System.Diagnostics;
#if NET40
using ICommand = Eto.Forms.ICommand;
#endif

namespace Eto.Test.Sections.Serialization.Xaml
{
	public class Test : Panel
	{
		public class MyModel
		{
			string someText;
			public string SomeText
			{
				get { return someText; }
				set
				{
					someText = value;
					Debug.WriteLine(string.Format("Changed to {0}", someText));
				}
			}

			public ICommand ClickMe
			{
				get { return new Command((sender, e) => MessageBox.Show("Clicked!")); }
			}
		}

		protected CheckBox MyCheckBox { get; set; }
		protected TextArea MyTextArea { get; set; }

		public Test()
		{
			XamlReader.Load(this);

			MyCheckBox.Checked = true;
			MyTextArea.Text = "This form was created via xaml!";

			DataContext = new MyModel { SomeText = "Woerijoweijr " };
		}

		protected void HandleButtonClick(object sender, EventArgs e)
		{
			MessageBox.Show(this, "I was clicked from Xaml!");
		}

		public void HandleTextChanged(object sender, EventArgs e)
		{
			Log.Write(sender, "Text was changed: {0}", ((TextBox)sender).Text);
		}
	}
}