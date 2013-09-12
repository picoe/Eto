using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using Eto.Test.Sections.Layouts.ScrollingLayouts;
using Eto.Test.Sections.Controls;
using Eto.Test.Sections.Behaviors;

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
			base.MainForm = new MainForm();
			HandleEvent(Application.TerminatingEvent);
			
			base.OnInitialized(e);
			
			// show the main form
			base.MainForm.Show();
		}
		#if DESKTOP
		public override void OnTerminating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating(e);
			Log.Write(this, "Terminating");
			
			var result = MessageBox.Show(base.MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
		#endif
	}
}

