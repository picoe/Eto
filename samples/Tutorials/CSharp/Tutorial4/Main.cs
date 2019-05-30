using System;
using Eto.Forms;
using Eto.Drawing;
using Eto;
using System.Diagnostics;

namespace Tutorial4
{
	public class MyObject
	{
		string textProperty;
		public string TextProperty
		{
			get { return textProperty; }
			set
			{
				textProperty = value;
				Debug.WriteLine(string.Format("Set TextProperty to {0}", value));
			}
		}
	}

	public class MyForm : Form
	{
		public MyForm()
		{
			ClientSize = new Size(600, 400);
			Title = "Table Layout";


			Content = new TableLayout(
				new TableRow(new Label { Text = "DataContext Binding" }, DataContextBinding()),
				new TableRow(new Label { Text = "Object Binding" }, ObjectBinding()),
				new TableRow(new Label { Text = "Direct Binding" }, DirectBinding()),
				null // same as creating a row with ScaleHeight = true
			) { Spacing = new Size(5, 5), Padding = new Padding(10) };

			// Set data context so it propegates to all child controls
			DataContext = new MyObject { TextProperty = "Initial value 1" };

			Menu = new MenuBar
			{
				QuitItem = new Command((sender, e) => Application.Instance.Quit())
				{ 
					MenuText = "Quit",
					Shortcut = Application.Instance.CommonModifier | Keys.Q
				}
			};
		}

		TextBox DataContextBinding()
		{
			var textBox = new TextBox();
			// bind to the text property using delegates
			textBox.TextBinding.BindDataContext<MyObject>(r => r.TextProperty, (r, val) => r.TextProperty = val);
			// You can also bind using reflection
			//textBox.TextBinding.BindDataContext<MyObject>(r => r.TextProperty);
			// or, if the data context type is unknown
			//textBox.TextBinding.BindDataContext(new PropertyBinding<string>("TextProperty"));
			return textBox;
		}

		TextBox ObjectBinding()
		{
			// object instance we want to bind to
			var obj = new MyObject { TextProperty = "Initial value 2" };

			var textBox = new TextBox();
			// bind to the text property of a specific object instance using reflection
			textBox.TextBinding.Bind(obj, r => r.TextProperty);
			return textBox;
		}
	
		TextBox DirectBinding()
		{
			var textBox = new TextBox();

			// bind to the text property using delegates
			textBox.TextBinding.Bind(() => "some value", val => Debug.WriteLine(string.Format("Set value to {0} directly", val)));
			return textBox;
		}
	}

	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application().Run(new MyForm());
		}
	}
}
