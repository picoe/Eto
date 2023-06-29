namespace Eto.Wpf.Forms.ToolBar
{
	public class ButtonToolItemHandler : ToolItemHandler<swc.Button, ButtonToolItem>, ButtonToolItem.IHandler
	{
		public ButtonToolItemHandler ()
		{
			Control = new swc.Button();
			Control.Click += Control_Click;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Content = CreateContent();
		}

		private void Control_Click(object sender, sw.RoutedEventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public override string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

	}
}
