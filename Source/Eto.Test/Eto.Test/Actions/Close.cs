using System;
using Eto.Forms;

namespace Eto.Test.Actions
{
	public class Close : ButtonAction
	{
		public static string ActionID = "close";
		
		public Close ()
		{
			this.ID = ActionID;	
			this.MenuText = "Close";
			this.ToolBarText = "Close";
			this.Accelerator = Application.Instance.CommonModifier | Keys.W;
		}
		
		protected override void OnActivated (EventArgs e)
		{
			base.OnActivated (e);
			
			Application.Instance.MainForm.Close ();
		}
	}
}

