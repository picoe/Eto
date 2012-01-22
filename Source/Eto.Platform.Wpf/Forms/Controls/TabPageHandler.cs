using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TabPageHandler : WpfControl<swc.TabItem, TabPage>, ITabPage
	{
		Eto.Drawing.Image image;
		swc.DockPanel content;

		public TabPageHandler ()
		{
			Control = new swc.TabItem ();
			Control.Content = content = new swc.DockPanel {
			};
		}

		public string Text
		{
			get { return Control.Header as string; }
			set { Control.Header = value; }
		}

		public Eto.Drawing.Image Image
		{
			get { return image; }
			set
			{
				image = value;
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
