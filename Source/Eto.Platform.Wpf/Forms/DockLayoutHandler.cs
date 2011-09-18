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
				LastChildFill = true
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
					var element = (System.Windows.UIElement)content.ControlObject;
					System.Windows.Controls.DockPanel.SetDock (element, System.Windows.Controls.Dock.Top);
					Control.Children.Add (element);
				}
			}
		}
	}
}
