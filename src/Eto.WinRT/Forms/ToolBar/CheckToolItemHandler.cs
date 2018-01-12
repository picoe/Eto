using System;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms
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
			get { return label.Text.ToEtoMneumonic(); }
			set { label.Text = value.ToWpfMneumonic(); }
		}

		public override string ToolTip
		{
#if TODO_XAML
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
#else
			get; set;
#endif
		}

		public override Image Image
		{
			get { return image; }
			set
			{
				image = value;
				swcImage.Source = image.ToWpf ((int)swcImage.MaxWidth);
			}
		}

		public override bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}
	}
}
