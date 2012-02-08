using System;
using Eto.Forms;

namespace Eto.Test
{
	public class TestApplication : Application
	{
		public TestApplication (Generator generator)
			: base(generator)
		{
		}
		
		public override void OnInitialized (EventArgs e)
		{
			this.MainForm = new MainForm();
            HandleEvent (Application.TerminatingEvent);
			
			base.OnInitialized (e);
			
			// show the main form
			this.MainForm.Show ();
		}
		
		public override void OnTerminating (System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating (e);
			Log.Write (this, "Terminating");
			
			var result = MessageBox.Show (this.MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No) e.Cancel = true;
		}
	}
}

