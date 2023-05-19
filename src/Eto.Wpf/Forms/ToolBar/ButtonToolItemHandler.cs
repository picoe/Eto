using System;
using Eto.Drawing;
using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using System.Windows;

namespace Eto.Wpf.Forms.ToolBar
{
	public class ButtonToolItemHandler : ToolItemHandler<swc.Button, ButtonToolItem>, ButtonToolItem.IHandler
	{
		Image image;
		readonly swc.Image swcImage;
		readonly swc.TextBlock label;

		public ButtonToolItemHandler ()
		{
			Control = new swc.Button();
			swcImage = new swc.Image();
			label = new swc.TextBlock();
			label.VerticalAlignment = sw.VerticalAlignment.Center;
			label.Margin = new Thickness(2, 0, 2, 0);
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add(swcImage);
			panel.Children.Add(label);
			Control.Content = panel;
			Control.Click += Control_Click;
			sw.Automation.AutomationProperties.SetLabeledBy(Control, label);
		}

		protected override void OnImageSizeChanged()
		{
			base.OnImageSizeChanged();
			var size = ImageSize;
			swcImage.MaxHeight = size?.Height ?? double.PositiveInfinity;
			swcImage.MaxWidth = size?.Width ?? double.PositiveInfinity;
		}

		private void Control_Click(object sender, RoutedEventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public override string Text
		{
			get { return label.Text.ToEtoMnemonic(); }
			set { label.Text = value.ToPlatformMnemonic(); }
		}

		public override string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public override Image Image
		{
			get { return image; }
			set
			{
				image = value;
				swcImage.Source = image.ToWpf(Screen.PrimaryScreen.LogicalPixelSize, swcImage.GetMaxSize().ToEtoSize());
			}
		}

		public override bool Enabled
		{
			get { return Control.IsEnabled; }
			set
			{
				Control.IsEnabled = value;
				swcImage.IsEnabled = value;
			}
		}
	}
}
