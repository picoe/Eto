using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class DockLayoutHandler : WpfLayout<System.Windows.Controls.DockPanel, DockLayout>, IDockLayout
	{
		Control content;

		public DockLayoutHandler ()
		{
			Control = new System.Windows.Controls.DockPanel { 
				LastChildFill = true,
				SnapsToDevicePixels = true
			};
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Generator.Convert (Control.Margin); }
			set { Control.Margin = Generator.Convert (value); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				Control.Children.Clear ();
				content = value;
				if (content != null) {
					var element = (System.Windows.FrameworkElement)content.ControlObject;
					//element.Height = double.MaxValue;
					System.Windows.Controls.DockPanel.SetDock (element, System.Windows.Controls.Dock.Top);
					element.Height = double.NaN;
					element.Width = double.NaN;
					/*element.Width = element.Height = double.NaN;
					 * */
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					Control.Children.Add (element);
				}
			}
		}
	}
}
