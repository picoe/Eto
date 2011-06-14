using System;
using Eto.Forms;

namespace Eto.Test.Interface.Actions
{
	public class About : ButtonAction
	{
		public const string ActionID = "about";
		
		public About ()
		{
			this.ID = ActionID;
			this.MenuText = "About Test Application";
			this.ToolBarText = "About";
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

