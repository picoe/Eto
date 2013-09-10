using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using Eto.Test.Sections.Layouts.ScrollingLayouts;
using Eto.Test.Sections.Controls;

namespace Eto.Test
{
	public class TestApplication : Application
	{
		public TestApplication (Generator generator)
			: base(generator)
		{
			this.Name = "Test Application";
			this.Style = "application";
		}
#if bla
		/**/
		public class MainForm : Form
		{
			public MainForm()
			{
				this.Title = "Hello";
				this.Size = new Size(400, 400);
				//this.Content = new DrawableSection();
				this.Content = new ThemedTabControlSection();
				//this.Content = mainlayout.Parent;

				/*var layout = new DynamicLayout(this);
				layout.Add(new TextBox { Text = "Hello" });
				layout.Add(new TextBox { Text = "There" });
				layout.Add(new TextBox { Text = "My" });
				layout.Add(new TextBox { Text = "Friend" });
				*/

				/**
				var layout = new TableLayout(this, 5, 5);

				layout.Add(new TextBox { Text = "Hello" }, 0, 0);
				layout.Add(new TextBox { Text = "There" }, 0, 1);
				layout.Add(new TextBox { Text = "My" }, 0, 2);
				layout.Add(new TextBox { Text = "Friend" }, 0, 3);
				/**/

			}
		}
		/**/
#endif

		public override void OnInitialized (EventArgs e)
		{
			base.MainForm = new MainForm ();
			HandleEvent (Application.TerminatingEvent);
			
			base.OnInitialized (e);
			
			// show the main form
			base.MainForm.Show ();
		}
		
#if DESKTOP
		public override void OnTerminating (System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating (e);
			Log.Write (this, "Terminating");
			
			var result = MessageBox.Show (base.MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No) e.Cancel = true;
		}
#endif
	}
}

