using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class Quit : Command
	{
		public Quit()
		{
			ID = "quit";
			MenuText = "&Quit";
			ToolBarText = "Quit";
			ToolTip = "Close the application";
			Shortcut = Keys.Q | Application.Instance.CommonModifier;
			Image = TestIcons.TestImage;
		}

		public override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			Application.Instance.Quit();
		}
	}
}

