using Eto.Serialization.Json;
using Eto.Serialization.Xaml;

namespace Eto.Test.Sections.Serialization.Json
{
	public partial class Test : Panel
	{
		protected CheckBox MyCheckBox { get; set; }

		protected TextArea MyTextArea { get; set; }

		protected DropDown MyDropDown { get; set; }

		class MyItem
		{
			public string MyText { get; set; }
		}

		public Test()
			: this(t => JsonReader.Load(t))
		{
		}

		/// <summary>Loads the .jeto another way, e.g. with Eto.Serialization.Xaml to compare.</summary>
		internal Test(Action<Test> load)
		{
			var sw = new Stopwatch();
			sw.Start();

			load(this);

			sw.Stop();
			Log.Write(this, "loaded in {0} seconds", sw.Elapsed.TotalSeconds);

			MyDropDown.DataStore = new List<MyItem>
			{
				new MyItem { MyText = "My Item 1" },
				new MyItem { MyText = "My Item 2" },
				new MyItem { MyText = "My Item 3" }
			};

			MyCheckBox.Checked = true;
			MyTextArea.Text = "This form was created via json!";

			DataContext = new Xaml.Test.MyModel { SomeText = "Text from data model" };
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
