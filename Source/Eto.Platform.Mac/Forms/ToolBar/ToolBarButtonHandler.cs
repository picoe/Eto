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

		void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}


		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
		
	}


}
