using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{

	public class ToolBarButtonHandler : ToolBarItemHandler<NSToolbarItem, ToolBarButton>, IToolBarButton
	{
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
