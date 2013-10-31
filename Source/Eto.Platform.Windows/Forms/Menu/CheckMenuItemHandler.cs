using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class CheckMenuItemHandler : MenuHandler<SWF.ToolStripMenuItem, CheckMenuItem>, ICheckMenuItem, IMenu
	{
		public CheckMenuItemHandler()
		{
			Control = new SWF.ToolStripMenuItem();
			Control.Click += control_Click;
		}

		private void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(e);
		}
		#region IMenuItem Members

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		public string Text
		{
			get	{ return Control.Text; }
			set { Control.Text = value; }
		}
		
		public string ToolTip {
			get {
				return Control.ToolTipText;
			}
			set {
				Control.ToolTipText = value;
			}
		}

		public Key Shortcut
		{
			get { return this.Control.ShortcutKeys.ToEto (); }
			set 
			{
				var key = value.ToSWF ();
				if (SWF.ToolStripManager.IsValidShortcut(key)) Control.ShortcutKeys = key;
			}
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}

		#endregion

		#region IMenu Members

		public void AddMenu(int index, MenuItem item)
		{
			Control.DropDownItems.Insert(index, (SWF.ToolStripItem)item.ControlObject);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.DropDownItems.Remove((SWF.ToolStripItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.DropDownItems.Clear();
		}

		#endregion

	}
}
