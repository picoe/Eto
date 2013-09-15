using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TabPageHandler : WpfDockContainer<swc.TabItem, TabPage>, ITabPage
	{
		Eto.Drawing.Image image;
		swc.DockPanel content;
		swc.Image headerImage;
		swc.TextBlock headerText;

		public TabPageHandler()
		{
			Control = new swc.TabItem();
			var header = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			headerImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			headerText = new swc.TextBlock();
			header.Children.Add(headerImage);
			header.Children.Add(headerText);
			Control.Header = header;

			Control.Content = content = new swc.DockPanel { };
		}

		public string Text
		{
			get { return headerText.Text; }
			set { headerText.Text = value; }
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public Eto.Drawing.Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (image != null)
					headerImage.Source = ((IWpfImage)image.Handler).GetImageClosestToSize(16);
				else
					headerImage.Source = null;
			}
		}

		public override Size ClientSize
		{
			get { return new Size((int)content.Width, (int)content.Height); }
			set
			{
				content.Width = value.Width;
				content.Height = value.Height;
			}
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Children.Clear();
			this.content.Children.Add(content);
		}
	}
}
