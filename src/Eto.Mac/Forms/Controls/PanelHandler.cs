using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class PanelHandler : MacPanel<NSView, Panel, Panel.ICallback>, Panel.IHandler
	{
		public class EtoPanelView : MacPanelView
		{
		}
		
		protected override NSView CreateControl() => new EtoPanelView();
		
		public override NSView ContainerControl => Control;
	}
}
