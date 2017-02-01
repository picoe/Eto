using System;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.ToolBar
{
	public class CheckToolItemHandler : ToolItemHandler<swc.Primitives.ToggleButton, CheckToolItem>, CheckToolItem.IHandler
	{
        Image image;
		readonly swc.Image swcImage;
		readonly swc.TextBlock label;
		public CheckToolItemHandler ()
		{
			Control = new swc.Primitives.ToggleButton {
				IsThreeState = false
			};
			swcImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock ();
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add (swcImage);
			panel.Children.Add (label);
			Control.Content = panel;

			Control.Checked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
			Control.Unchecked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}

		public bool Checked
		{
			get { return Control.IsChecked ?? false; }
			set { Control.IsChecked = value; }
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
			set { Control.IsEnabled = value; }
		}
	}
}
