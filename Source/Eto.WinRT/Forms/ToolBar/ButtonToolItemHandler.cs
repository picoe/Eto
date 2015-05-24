using System;
using Eto.Drawing;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;

namespace Eto.WinRT.Forms.ToolBar
{
	public class ButtonToolItemHandler : ToolItemHandler<swc.AppBarButton, ButtonToolItem>, ButtonToolItem.IHandler
	{
		Image image;
		readonly swc.Image swcImage;
		readonly swc.TextBlock label;

		public ButtonToolItemHandler()
		{
			Control = new swc.AppBarButton();
			swcImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock();
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add(swcImage);
			panel.Children.Add(label);
			Control.Content = panel;
			Control.Click += delegate
			{
				Widget.OnClick(EventArgs.Empty);
			};
		}

		public override string Text
		{
			get { return label.Text.ToEtoMneumonic(); }
			set { label.Text = value.ToWpfMneumonic(); }
		}

		public override string ToolTip
		{
#if TODO_XAML
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
#else
			get;
			set;
#endif
		}

		public override Image Image
		{
			get { return image; }
			set
			{
				image = value;
				swcImage.Source = image.ToWpf((int)swcImage.MaxWidth);
			}
		}

		public override bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}
	}
}
