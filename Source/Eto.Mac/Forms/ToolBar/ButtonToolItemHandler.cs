using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac
{

	public class ButtonToolItemHandler : ToolItemHandler<NSToolbarItem, ButtonToolItem>, IButtonToolItem
	{
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
