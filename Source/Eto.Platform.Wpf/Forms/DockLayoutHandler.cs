using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class DockLayoutHandler : WpfLayout<swc.Border, DockLayout>, IDockLayout
	{
		Control content;

		public override sw.Size PreferredSize
		{
			get { 
				var preferredSize = content.GetPreferredSize ();
				return new sw.Size (preferredSize.Width + Padding.Horizontal, preferredSize.Height + Padding.Vertical);
			}
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

	}
}
