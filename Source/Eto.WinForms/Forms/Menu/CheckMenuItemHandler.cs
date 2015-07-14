using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Menu
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class CheckMenuItemHandler : MenuItemHandler<SWF.ToolStripMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
		public CheckMenuItemHandler()
		{
			Control = new SWF.ToolStripMenuItem();
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			Callback.OnClick(Widget, e);
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}
	}
}
