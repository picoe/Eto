using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class DockLayoutHandler : WidgetHandler<System.Windows.Controls.DockPanel, DockLayout>, IDockLayout
	{
		public DockLayoutHandler ()
		{
			Control = new System.Windows.Controls.DockPanel ();
		}

		public Eto.Drawing.Padding Padding
		{
			get { return Generator.Convert (Control.Margin); }
			set { Control.Margin = Generator.Convert (value); }
		}

		public void Add (Control control)
		{
			Control.Children.Clear ();
			Control.Children.Add((System.Windows.UIElement)control.ControlObject);
		}

		public void Remove (Control control)
		{
			Control.Children.Clear ();
		}
	}
}
