using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Menu
{
	public class SeparatorMenuItemHandler : MenuHandler<SWF.ToolStripSeparator, SeparatorMenuItem, SeparatorMenuItem.ICallback>, SeparatorMenuItem.IHandler
	{
		
		public SeparatorMenuItemHandler()
		{
			Control = new SWF.ToolStripSeparator();
		}

		public string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public Keys Shortcut
		{
			get { return Keys.None; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}
	}
}
