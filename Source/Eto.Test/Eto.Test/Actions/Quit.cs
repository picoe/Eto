using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class Quit
	{
		public const string ActionID = "quit";
		string TooltipText { get { return "Close the application"; } }
		Keys Accelerator { get { return Keys.Q | Application.Instance.CommonModifier; } }
		Image Image { get { return TestIcons.TestImage; } }

		public MenuItem CreateMenuItem()
		{
			return new ImageMenuItem
			{
				ID = ActionID,
				Text = "&Quit",
				Image = this.Image,
				Accelerator = this.Accelerator,
			};
		}

		public ToolBarItem CreateToolBarItem()
		{
			return new ToolBarButton
			{
				ID = ActionID,
				Text = "Quit",
				Image = this.Image,
				Accelerator = this.Accelerator,				
			};			
		}
	
		public void Handle()
		{
			Application.Instance.Quit();
		}
	}
}

