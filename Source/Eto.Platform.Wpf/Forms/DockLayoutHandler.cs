using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class DockLayoutHandler : WpfLayout<swc.DockPanel, DockLayout>, IDockLayout
	{
		Control content;

		public DockLayoutHandler ()
		{
			Control = new swc.DockPanel { 
				SnapsToDevicePixels = true,
				LastChildFill = true
			};
			Control.SizeChanged += (sender, e) => {
				if (content != null) {
					var element = (sw.FrameworkElement)content.ControlObject;
					if (!double.IsNaN (element.Width)) element.Width = Math.Max (0, e.NewSize.Width - Padding.Horizontal);
					if (!double.IsNaN (element.Height)) element.Height = Math.Max (0, e.NewSize.Height - Padding.Vertical);
				}
			};
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Control.Margin.ToEto (); }
			set { Control.Margin = value.ToWpf (); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				Control.Children.Clear ();
				content = value;
				if (content != null) {
					var element = (sw.FrameworkElement)content.ControlObject;
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					Control.Children.Add (element);
				}
			}
		}

	}
}
