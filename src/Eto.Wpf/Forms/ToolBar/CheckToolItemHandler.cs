using System.Windows;

namespace Eto.Wpf.Forms.ToolBar
{
	public class CheckToolItemHandler : ToolItemHandler<swc.Primitives.ToggleButton, CheckToolItem>, CheckToolItem.IHandler
	{
		public CheckToolItemHandler ()
		{
			Control = new swc.Primitives.ToggleButton {
				IsThreeState = false
			};

			Control.Checked += Control_CheckedChanged;
			Control.Unchecked += Control_CheckedChanged;
			Control.Click += Control_Click;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Content = CreateContent();
		}

		private void Control_Click(object sender, RoutedEventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		private void Control_CheckedChanged(object sender, RoutedEventArgs e)
		{
			Widget.OnCheckedChanged(EventArgs.Empty);
		}

		public bool Checked
		{
			get { return Control.IsChecked ?? false; }
			set { Control.IsChecked = value; }
		}
		public override string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}
	}
}
