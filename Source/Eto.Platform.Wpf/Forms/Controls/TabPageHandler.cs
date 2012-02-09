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
	public class TabPageHandler : WpfControl<swc.TabItem, TabPage>, ITabPage
	{
		Eto.Drawing.Image image;
		swc.DockPanel content;
		swc.Image headerImage;
		swc.TextBlock headerText;

		public TabPageHandler ()
		{
			Control = new swc.TabItem ();
			var header = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			headerImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			headerText = new swc.TextBlock ();
			header.Children.Add (headerImage);
			header.Children.Add (headerText);
			Control.Header = header;

			Control.Content = content = new swc.DockPanel { };
		}

		public string Text
		{
			get { return headerText.Text; }
			set { headerText.Text = value; }
		}

		public Eto.Drawing.Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (image != null)
					headerImage.Source = ((IWpfImage)image.Handler).GetIconClosestToSize (16);
				else
					headerImage.Source = null;
			}
		}

		public Eto.Drawing.Size ClientSize
		{
			get {
				return new Eto.Drawing.Size ((int)content.Width, (int)content.Height);
			}
			set {
				content.Width = value.Width;
				content.Height = value.Height;
			}
		}

		public object ContainerObject
		{
			get { return Control.Content; }
		}

		public void SetLayout (Layout layout)
		{
			content.Children.Clear ();
			content.Children.Add ((sw.UIElement)layout.ControlObject);
		}

		public Size? MinimumSize
		{
			get
			{
				if (Control.MinWidth > 0 && Control.MinHeight > 0)
					return new Size ((int)Control.MinWidth, (int)Control.MinHeight);
				else
					return null;
			}
			set
			{
				if (value != null) {
					Control.MinWidth = value.Value.Width;
					Control.MinHeight = value.Value.Height;
				}
				else {
					Control.MinHeight = 0;
					Control.MinWidth = 0;
				}
			}
		}
	}
}
