using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class PixelLayoutHandler : WpfLayout<swc.Canvas, PixelLayout>, IPixelLayout
	{
		public override sw.Size GetPreferredSize (sw.Size? constraint)
		{
			var size = new sw.Size ();
			foreach (var control in Widget.Controls) {
				var container = control.GetContainerControl ();
				var preferredSize = control.GetPreferredSize (constraint);
				var left = swc.Canvas.GetLeft (container) + preferredSize.Width;
				var top = swc.Canvas.GetTop (container) + preferredSize.Height;
				if (size.Width < left) size.Width = left;
				if (size.Height < top) size.Height = top;
			}
			return size;
		}

		public PixelLayoutHandler ()
		{
			Control = new swc.Canvas {
				SnapsToDevicePixels = true
			};
		}

		public void Add (Control child, int x, int y)
		{
			var element = child.GetContainerControl ();
			swc.Canvas.SetLeft (element, x);
			swc.Canvas.SetTop (element, y);
			Control.Children.Add (element);
		}

		public void Move (Control child, int x, int y)
		{
			var element = child.GetContainerControl ();
			swc.Canvas.SetLeft (element, x);
			swc.Canvas.SetTop (element, y);
		}

		public void Remove (Control child)
		{
			var element = child.GetContainerControl ();
			Control.Children.Remove (element);
		}
	}
}
