using System;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class Close
	{
		public static string ActionID = "close";
		private Keys Shortcut { get { return Application.Instance.CommonModifier | Keys.W; } }
		
		public MenuItem CreateMenuItem()
		{
			var result = new ImageMenuItem
			{
				ID = ActionID,
				Text = "Close",
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
				Text = "Close",
				Shortcut = this.Shortcut,
			};
			result.Clicked += (s, e) => Handle();
			return result;
		}


		public void Handle()
		{
			Application.Instance.MainForm.Close ();
		}
	}
}

