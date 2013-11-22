using System;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class About : ButtonAction
	{
		public const string ActionID = "about";
		
		public About ()
		{
			this.ID = ActionID;
			this.MenuText = "About Test Application";
			this.ToolBarText = "About";
			this.Image = TestIcons.TestIcon;
			this.Accelerator = Keys.F11;
		}
		
		protected override void OnActivated (EventArgs e)
		{
			base.OnActivated (e);
			
			// show the about dialog
			var about = new Dialogs.About();
			about.ShowDialog (Application.Instance.MainForm);
		}
	}
}

