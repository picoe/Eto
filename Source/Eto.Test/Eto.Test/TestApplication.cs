using System;
using Eto.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Eto.Test
{
	public class TestApplication : Application
	{
		public TestApplication(Platform platform)
			: base(platform)
		{
			this.Name = "Test Application";
			this.Style = "application";
		}

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm();

			base.OnInitialized(e);

			/*
			int count = 100000;
			var start = DateTime.Now;
			for (int i = 0; i < count; i++)
			{
				new Button();
			}
			var end = DateTime.Now;
			Debug.WriteLine("Time: {0}", end - start);
			*/

			// show the main form
			MainForm.Show();
		}

		public override void OnTerminating(CancelEventArgs e)
		{
			base.OnTerminating(e);
			Log.Write(this, "Terminating");
			
			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
	}
}

