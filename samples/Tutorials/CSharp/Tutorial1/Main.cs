using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial1
{
	public class MyForm : Form
	{
		public MyForm()
		{
			// Set ClientSize instead of Size, as each platform has different window border sizes
			ClientSize = new Size(600, 400);

			// Title to show in the title bar
			Title = "Hello, Eto.Forms";

			// Content of the form
			Content = new Label { Text = "Some content", VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
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
