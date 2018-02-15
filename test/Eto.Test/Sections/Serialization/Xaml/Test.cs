using System;
using Eto.Forms;
using Eto.Serialization.Xaml;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;


#if NET40
using ICommand = Eto.Forms.ICommand;
#endif

namespace Eto.Test.Sections.Serialization.Xaml
{
	public class MyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return System.Convert.ToString(value) + " (converted)";
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return System.Convert.ToString(value) + " (converted back)";
		}
	}

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
					Log.Write(this, string.Format("Changed to {0}", someText));
				}
			}

			public ICommand ClickMe
			{
				get { return new Command((sender, e) => MessageBox.Show("Clicked!")); }
			}
		}

		protected CheckBox MyCheckBox { get; set; }
		protected TextArea MyTextArea { get; set; }
		protected DropDown MyDropDown { get; set; }

		class MyItem
		{
			public string MyText { get; set; }
		}


		public Test()
		{
			var sw = new Stopwatch();
			sw.Start();

			XamlReader.Load(this);

			var items = new List<MyItem>();
			items.Add(new MyItem { MyText = "My Item 1" });
			items.Add(new MyItem { MyText = "My Item 2" });
			items.Add(new MyItem { MyText = "My Item 3" });
			MyDropDown.DataStore = items;
			sw.Stop();
			Log.Write(this, "loaded in {0} seconds", sw.Elapsed.TotalSeconds);

			MyCheckBox.Checked = true;
			MyTextArea.Text = "This form was created via xaml!";

			DataContext = new MyModel { SomeText = "Text from data model" };
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