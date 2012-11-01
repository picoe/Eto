using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WC = System.Windows.Controls;
using W = System.Windows;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class PixelLayoutHandler : WpfLayout<WC.Canvas, PixelLayout>, IPixelLayout
	{
		public PixelLayoutHandler ()
		{
			Control = new WC.Canvas {
				SnapsToDevicePixels = true
			};
		}

		public override void OnLoad ()
		{
			base.OnLoad ();
			var size = new W.Size ();
			foreach (var c in Control.Children.OfType<W.FrameworkElement>()) {
				var left = WC.Canvas.GetLeft(c) + c.Width;
				var top = WC.Canvas.GetTop (c) + c.Height;
				if (size.Width < left) size.Width = left;
				if (size.Height < top) size.Height = top;
			}
			this.Control.Width = size.Width;
			this.Control.Height = size.Height;
		}

		public void Add (Control child, int x, int y)
		{
			var element = child.GetContainerControl ();
			WC.Canvas.SetLeft (element, x);
			WC.Canvas.SetTop (element, y);
			/*Control.Width = Math.Max (x + child.Size.Width, Control.Width);
			Control.Height = Math.Max (y + child.Size.Height, Control.Height);*/
			Control.Children.Add (element);
		}

		public void Move (Control child, int x, int y)
		{
			var element = child.GetContainerControl ();
			WC.Canvas.SetLeft (element, x);
			WC.Canvas.SetTop (element, y);
			Control.Width = Math.Max (x + child.Size.Width, Control.Width);
			Control.Height = Math.Max (y + child.Size.Height, Control.Height);
		}

		public void Remove (Control child)
		{
			var element = child.GetContainerControl ();
			Control.Children.Remove (element);
		}
	}
}
