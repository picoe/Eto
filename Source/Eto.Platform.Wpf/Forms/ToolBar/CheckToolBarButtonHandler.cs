using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class CheckToolBarButtonHandler : ToolBarItemHandler<swc.Primitives.ToggleButton, CheckToolBarButton>, ICheckToolBarButton
	{
		Icon icon;
		swc.Image image;
		swc.TextBlock label;
		public CheckToolBarButtonHandler ()
		{
			Control = new swc.Primitives.ToggleButton {
				IsThreeState = false
			};
			image = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock ();
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add (image);
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
