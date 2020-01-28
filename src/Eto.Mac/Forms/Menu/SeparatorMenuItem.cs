using System;
using Eto.Forms;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

namespace Eto.Mac.Forms.Menu
{
	public class SeparatorMenuItemHandler : MenuHandler<NSMenuItem, SeparatorMenuItem, SeparatorMenuItem.ICallback>, SeparatorMenuItem.IHandler
	{
		protected override NSMenuItem CreateControl()
		{
			return NSMenuItem.SeparatorItem;
		}

		protected override bool DisposeControl { get { return false; } }

		public string Text {
			get { return string.Empty; }
			set { throw new NotSupportedException (); }
		}
		
		public Keys Shortcut {
			get { return Keys.None; }
			set { throw new NotSupportedException (); }
		}

		public string ToolTip {
			get { return string.Empty; }
			set { throw new NotSupportedException (); }
		}

	}
}
