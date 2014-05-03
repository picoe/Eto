using System;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test
{
	public class TestApplication : Application
	{
		public TestApplication(Generator generator)
			: base(generator)
		{
			this.Name = "Test Application";
			this.Style = "application";
		}

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm();

			base.OnInitialized(e);
			
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

