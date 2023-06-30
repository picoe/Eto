namespace Eto.Wpf.Forms.Controls
{
	public class PanelHandler : WpfPanel<swc.Border, Panel, Panel.ICallback>, Panel.IHandler
	{
		public PanelHandler ()
		{
			Control = new EtoBorder
			{
				Handler = this,
				Focusable = false,
				Background = swm.Brushes.Transparent // to get mouse events
			};
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			Control.Child = content;
		}
	}
}
