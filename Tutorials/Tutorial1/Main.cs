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
			
			this.Title = "Hello, Eto.Forms";
		}
	}
	
	class MainClass
	{
		[STAThread]
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
