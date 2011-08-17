using System;
using System.Collections;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class SeparatorMenuItemHandler : MenuHandler<NSMenuItem, SeparatorMenuItem>, ISeparatorMenuItem
	{
		public SeparatorMenuItemHandler ()
		{
			Control = NSMenuItem.SeparatorItem;
		}
		
		public bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public string Text {
			get { return string.Empty; }
			set { throw new NotSupportedException (); }
		}
		
		public Key Shortcut {
			get { return Key.None; }
			set { throw new NotSupportedException (); }
		}
	}
}
