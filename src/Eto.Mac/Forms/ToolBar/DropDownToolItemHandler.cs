using System;
using Eto;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Forms.ToolBar;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;

namespace Eto.Mac.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<NSToolbarItem, DropDownToolItem>, DropDownToolItem.IHandler
	{
		ContextMenu contextMenu;

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);

			var ctxMenu = contextMenu.ControlObject as NSMenu;
			if (ctxMenu != null)
			{
				ctxMenu.PopUpMenu(null, Button.Frame.Location, Button.Superview);
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}

		public new string Text
		{
			get { return Button.Title; }
			set { Button.Title = value; }
		}
	}
}
