namespace Eto.Mac.Forms.Controls
{
	public class PanelHandler : MacPanel<NSView, Panel, Panel.ICallback>, Panel.IHandler
	{
		public class EtoPanelView : MacPanelView
		{
			public EtoPanelView()
			{
			}
			public EtoPanelView(NativeHandle handle) : base(handle)
			{
			}
		}
		
		protected override NSView CreateControl() => new EtoPanelView();
		
		public override NSView ContainerControl => Control;
	}
}
