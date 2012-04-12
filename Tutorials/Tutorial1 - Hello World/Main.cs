using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial1
{
	public class MyForm : Form
	{
		public MyForm()
		{
			this.ClientSize = new Size(600, 400);
			
			this.Text = "Hello, Eto.Forms";
		}
	}
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			var app = new Application ();
			
			app.Initialized += delegate {
				app.MainForm = new MyForm ();
				app.MainForm.Show ();
			};
			app.Run (args);
		}
	}
}
