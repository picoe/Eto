using System;
using Eto.Forms;

namespace Eto.Test.Interface
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
			
			base.OnInitialized (e);
			
			// show the main form
			this.MainForm.Show ();
		}
	}
}

