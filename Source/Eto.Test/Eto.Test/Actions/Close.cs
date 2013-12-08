using System;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class Close
	{
		public static string ActionID = "close";
		private Keys Accelerator { get { return Application.Instance.CommonModifier | Keys.W; } }
		
		public MenuItem CreateMenuItem()
		{
			return new ImageMenuItem
			{
				ID = ActionID,
				Text = "Close",
				Accelerator = this.Accelerator,
			};
		}

		public ToolBarItem CreateToolbarItem()
		{
			return new ToolBarActionItem
			{
				ID = ActionID,
				Text = "Close",
				Accelerator = this.Accelerator,
			};
		}


		public void Handle()
		{
			Application.Instance.MainForm.Close ();
		}
	}
}

