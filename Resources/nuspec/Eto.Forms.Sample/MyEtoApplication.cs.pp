using System;
using Eto.Forms;
using Eto.Drawing;

namespace $rootnamespace$
{
	/// <summary>
	/// Set this class as your startup object
	/// </summary>
	public class MyEtoApplication : Application
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var app = new MyEtoApplication();
			app.Run(args);
		}

		/// <summary>
		/// Handles when the application is initialized and running
		/// </summary>
		public override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			MainForm = new MyEtoForm();
			MainForm.Show();
		}
	}
}
