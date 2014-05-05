using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial1
{
	public class MyForm : Form
	{
		public MyForm()
		{
			ClientSize = new Size(600, 400);
			
			Title = "Hello, Eto.Forms";

			Content = new Label { Text = "Some content" };
		}
	}

	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var app = new Application();
			
			app.Initialized += delegate
			{
				app.MainForm = new MyForm();
				app.MainForm.Show();
			};
			app.Run(args);
		}
	}
}
