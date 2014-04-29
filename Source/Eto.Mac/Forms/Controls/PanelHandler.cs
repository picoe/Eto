using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.Controls
{
	public class PanelHandler : MacPanel<NSView, Panel>, IPanel
	{
		public PanelHandler()
		{
			Control = new MacEventView{ Handler = this };
		}
		
		public override NSView ContainerControl { get { return Control; } }
	}
}
