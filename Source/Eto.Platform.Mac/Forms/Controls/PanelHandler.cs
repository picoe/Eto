using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class PanelHandler : MacDockContainer<NSView, Panel>, IPanel
	{
		public PanelHandler()
		{
			Control = new MacEventView{ Handler = this };
		}
		
		public override NSView ContainerControl { get { return Control; } }
	}
}
