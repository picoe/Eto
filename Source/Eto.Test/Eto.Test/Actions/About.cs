using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class About
	{
		public const string ActionID = "about";
		private Image Image { get { return TestIcons.TestIcon; } }
		private Keys Accelerator { get { return Keys.F11; } }

		public MenuItem CreateMenuItem()
		{
			return new ImageMenuItem
			{
				ID = ActionID,
				Text = "About Test Application",
				Image = this.Image,
				Accelerator = this.Accelerator,			
			};
		}

		public ToolBarItem CreateToolbarItem()
		{
			return new ToolBarActionItem
			{
				ID = ActionID,
				Text = "About",
				Image = this.Image,
				Accelerator = this.Accelerator,
			};
		}
				
		public void Handle()
		{
			// show the about dialog
			var about = new Dialogs.About();
			about.ShowDialog (Application.Instance.MainForm);
		}
	}
}

