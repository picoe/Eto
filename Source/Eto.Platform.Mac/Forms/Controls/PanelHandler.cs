using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class PanelHandler : MacContainer<NSView, Panel>, IPanel
	{
		
		public PanelHandler()
		{
			Enabled = true;
			Control = new MacEventView{ Handler = this };
		}
		
		public override bool Enabled { get; set; }

		public override object ContainerObject {
			get {
				return Control;
			}
		}
		
		
	}
}
