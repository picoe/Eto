using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms
{
	/// <summary>
	/// Pixel layout handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PixelLayoutHandler : WpfLayout<swc.Canvas, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			var size = new wf.Size();
			foreach (var control in Widget.Controls)
			{
				var container = control.GetContainerControl();
				var preferredSize = control.GetPreferredSize(constraint);
				var left = swc.Canvas.GetLeft(container) + preferredSize.Width;
				var top = swc.Canvas.GetTop(container) + preferredSize.Height;
				if (size.Width < left) size.Width = left;
				if (size.Height < top) size.Height = top;
			}
			return size;
		}

		public PixelLayoutHandler()
		{
			Control = new swc.Canvas
			{
#if TODO_XAML
				SnapsToDevicePixels = true
#endif
			};
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public void Add(Control child, int x, int y)
		{
			var element = child.GetContainerControl();
			swc.Canvas.SetLeft(element, x);
			swc.Canvas.SetTop(element, y);
			Control.Children.Add(element);
			UpdatePreferredSize();
		}

		public void Move(Control child, int x, int y)
		{
			var element = child.GetContainerControl();
			swc.Canvas.SetLeft(element, x);
			swc.Canvas.SetTop(element, y);
			UpdatePreferredSize();
		}

		public void Remove(Control child)
		{
			var element = child.GetContainerControl();
			Control.Children.Remove(element);
			UpdatePreferredSize();
		}

		public override void Remove(sw.FrameworkElement child)
		{
			Control.Children.Remove(child);
			UpdatePreferredSize();
		}
	}
}
