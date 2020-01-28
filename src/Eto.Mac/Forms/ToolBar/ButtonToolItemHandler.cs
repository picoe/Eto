using System;
using Eto.Forms;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

namespace Eto.Mac.Forms.ToolBar
{

	public class ButtonToolItemHandler : ToolItemHandler<NSToolbarItem, ButtonToolItem>, ButtonToolItem.IHandler
	{
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
