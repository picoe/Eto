using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Drawing;
using System.Diagnostics;

namespace Eto.Platform.Wpf.Forms
{
	public class DockLayoutHandler : WpfLayout<swc.Border, DockLayout>, IDockLayout
	{
		Control content;

		public override sw.Size GetPreferredSize (sw.Size? constraint)
		{
			var size = constraint ?? new sw.Size (double.PositiveInfinity, double.PositiveInfinity);
			size = new sw.Size (Math.Max (0, size.Width - Padding.Horizontal), Math.Max (0, size.Height - Padding.Vertical));
			var preferredSize = content.GetPreferredSize (size);
			return new sw.Size (Math.Max(0, preferredSize.Width + Padding.Horizontal), Math.Max(0, preferredSize.Height + Padding.Vertical));
		}

		public DockLayoutHandler ()
		{
			Control = new swc.Border { 
				SnapsToDevicePixels = true
			};
			Control.SizeChanged += (sender, e) => {
				if (content != null) {
					var element = (sw.FrameworkElement)content.ControlObject;
					if (!double.IsNaN (element.Width)) element.Width = Math.Max (0, e.NewSize.Width - Padding.Horizontal);
					if (!double.IsNaN (element.Height)) element.Height = Math.Max (0, e.NewSize.Height - Padding.Vertical);
				}
			};
		}

		public Padding Padding
		{
			get { return Control.Padding.ToEto (); }
			set { Control.Padding = value.ToWpf (); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				content = value;
				if (content != null) {
					var element = content.GetContainerControl();
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					Control.Child = element;
				}
				else
					Control.Child = null;
			}
		}

		public override void Remove (sw.FrameworkElement child)
		{
			if (Control.Child == child) {
				Control.Child = null;
				content = null;
			}
		}
	}
}
