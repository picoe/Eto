using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class About
	{
		public const string ActionID = "about";
		private Image Image { get { return TestIcons.TestIcon; } }
		private Keys Shortcut { get { return Keys.F11; } }

		public MenuItem CreateMenuItem()
		{
			var result = new ImageMenuItem
			{
				ID = ActionID,
				Text = "About Test Application",
				Image = this.Image,
				Shortcut = this.Shortcut,			
			};
			result.Clicked += (s, e) => Handle();
			return result;
		}

		public ToolBarItem CreateToolBarItem()
		{
			var result = new ToolBarButton
			{
				ID = ActionID,
				Text = "About",
				Image = this.Image,
				Shortcut = this.Shortcut,
			};
			result.Clicked += (s, e) => Handle();
			return result;
		}
				
		public void Handle()
		{
			// show the about dialog
			var about = new Dialogs.About();
			about.ShowDialog (Application.Instance.MainForm);
		}
	}
}

