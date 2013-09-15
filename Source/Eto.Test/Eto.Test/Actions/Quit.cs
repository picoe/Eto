using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Actions
{
	public class Quit : ButtonAction
	{
		public const string ActionID = "quit";
		
		public Quit ()
		{
			this.ID = ActionID;
			this.MenuText = "&Quit";
			this.ToolBarText = "Quit";
			this.TooltipText = "Close the application";
			this.Accelerator = Key.Q | Application.Instance.CommonModifier;
			this.Image = TestIcons.TestImage;
		}
		
		protected override void OnActivated (EventArgs e)
		{
			base.OnActivated (e);
			
			Application.Instance.Quit();
		}
	}
}

