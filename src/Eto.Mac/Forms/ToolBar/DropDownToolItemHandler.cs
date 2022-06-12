using System;
using Eto;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Forms.ToolBar;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<NSToolbarItem, DropDownToolItem>, DropDownToolItem.IHandler
	{
		ContextMenu contextMenu;

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);

			var ctxMenu = contextMenu.ControlObject as NSMenu /* ??? */;
			if (ctxMenu != null)
			{
				// https://github.com/picoe/Eto/blob/1b4821b375827b8348969bf1684216e0a70131d1/src/Eto.Mac/Forms/Menu/ContextMenuHandler.cs
				// ???
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}
	}
}
