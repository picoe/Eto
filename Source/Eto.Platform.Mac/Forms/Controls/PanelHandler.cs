using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class PanelHandler : MacContainer<NSView, Panel>, IPanel
	{
		
		public PanelHandler()
		{
			Control = new MacEventView{ Handler = this };
		}

		public override object ContainerObject {
			get {
				return Control;
			}
		}
		
		
	}
}
