using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class ToolBarButtonHandler : ToolBarItemHandler<swc.Button, ToolBarButton>, IToolBarButton
	{
		Icon icon;
		swc.Image image;
		swc.TextBlock label;
		public ToolBarButtonHandler ()
		{
			Control = new swc.Button ();
			image = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock ();
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add (image);
			panel.Children.Add (label);
			Control.Content = panel;
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}

		public string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				if (icon != null)
					image.Source = icon.ControlObject as swm.ImageSource;
				else
					image.Source = null;
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}
	}
}
