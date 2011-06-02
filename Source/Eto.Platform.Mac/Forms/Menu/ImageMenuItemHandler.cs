using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class ImageMenuItemHandler : MenuHandler<NSMenuItem, ImageMenuItem>, IImageMenuItem
	{
		Icon icon;

		public ImageMenuItemHandler()
		{
			Control = new NSMenuItem();
			Control.Enabled = true;
			Control.Activated += control_Click;
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
			get	{ return Control.Title; }
			set 
			{ 
				Control.SetTitleWithMnemonic(value);
				if (Control.HasSubmenu) Control.Submenu.Title = Control.Title;
			}
		}
		
		public string ToolTip {
			get {
				return Control.ToolTip;
			}
			set {
				Control.ToolTip = value;
			}
		}

		public Key Shortcut
		{
			get { return KeyMap.Convert(Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set { 
				this.Control.KeyEquivalent = KeyMap.KeyEquivalent(value);
				this.Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask(value);
			}
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				/*if (icon != null) Control.Image = ((NSImage)icon.ControlObject);
				else Control.Image = null;*/
			}
		}

		#endregion
		
		public override void AddMenu (int index, MenuItem item)
		{
			base.AddMenu (index, item);
			//if (Control.HasSubmenu) Control.Submenu.Title = Control.Title;
		}


	}
}
