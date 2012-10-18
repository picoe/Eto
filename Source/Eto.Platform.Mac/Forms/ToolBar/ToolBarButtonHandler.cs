using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{

	public class ToolBarButtonHandler : ToolBarItemHandler<NSToolbarItem, ToolBarButton>, IToolBarButton
	{
		
		public ToolBarButtonHandler()
		{
		}

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
