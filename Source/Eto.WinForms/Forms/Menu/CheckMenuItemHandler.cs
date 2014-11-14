using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Menu
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class CheckMenuItemHandler : MenuHandler<SWF.ToolStripMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
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

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public string ToolTip
		{
			get
			{
				return Control.ToolTipText;
			}
			set
			{
				Control.ToolTipText = value;
			}
		}

		public Keys Shortcut
		{
			get { return Control.ShortcutKeys.ToEto(); }
			set
			{
				var key = value.ToSWF();
				if (SWF.ToolStripManager.IsValidShortcut(key)) Control.ShortcutKeys = key;
			}
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}
	}
}
