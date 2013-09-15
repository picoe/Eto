using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class ToolBarButtonHandler : ToolBarItemHandler<swc.Button, ToolBarButton>, IToolBarButton
	{
		Image image;
		swc.Image swcImage;
		swc.TextBlock label;
		public ToolBarButtonHandler ()
		{
			Control = new swc.Button ();
			swcImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock ();
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add (swcImage);
			panel.Children.Add (label);
			Control.Content = panel;
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}

		public string Text
		{
			get { return label.Text.ToEtoMneumonic(); }
			set { label.Text = value.ToWpfMneumonic(); }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				swcImage.Source = image.ToWpf ((int)swcImage.MaxWidth);
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}
	}
}
