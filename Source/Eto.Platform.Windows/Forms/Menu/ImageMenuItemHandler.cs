using System;
using System.Reflection;
using System.Linq;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class ImageMenuItemHandler : MenuHandler<SWF.ToolStripMenuItem, ImageMenuItem>, IImageMenuItem, IMenu
	{
		Icon icon;
		bool openedHandled;

		public ImageMenuItemHandler()
		{
			Control = new SWF.ToolStripMenuItem();
			Control.Click += control_Click;
		}

		void HandleDropDownOpened (object sender, EventArgs e)
		{
			foreach (var item in Widget.MenuItems.OfType<MenuActionItem>()) {
				item.OnValidate (EventArgs.Empty);
			}
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
				return this.Control.ToolTipText;
			}
			set {
				this.Control.ToolTipText = ToolTip;
			}
		}

		public Key Shortcut
		{
			get { return KeyMap.Convert(this.Control.ShortcutKeys); }
			set 
			{ 
				var key = KeyMap.Convert(value);
				if (SWF.ToolStripManager.IsValidShortcut(key)) Control.ShortcutKeys = key;
				else this.Control.ShortcutKeyDisplayString = KeyMap.KeyToString(value);
			}
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				if (icon != null) Control.Image = ((IconHandler)icon.Handler).GetIconClosestToSize(16).ToBitmap();
				else Control.Image = null;
			}
		}

		#endregion


		#region IMenu Members

		public void AddMenu(int index, MenuItem item)
		{
			Control.DropDownItems.Insert(index, (SWF.ToolStripItem)item.ControlObject);
			if (!openedHandled) {
				Control.DropDownOpening += HandleDropDownOpened;
			}
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
