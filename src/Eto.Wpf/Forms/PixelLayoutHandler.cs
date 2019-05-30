using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class PixelLayoutHandler : WpfLayout<swc.Canvas, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public class EtoCanvas : swc.Canvas
		{
			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				var size = new sw.Size();
				
				foreach (sw.UIElement control in Children)
				{
					control.Measure(constraint);
					var preferredSize = control.DesiredSize;
					var left = GetLeft(control) + preferredSize.Width;
					var top = GetTop(control) + preferredSize.Height;
					if (size.Width < left) size.Width = left;
					if (size.Height < top) size.Height = top;
				}
				return size;
			}
		}

		public PixelLayoutHandler()
		{
			Control = new EtoCanvas
			{
				SnapsToDevicePixels = true
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

		int suspended;
		public override void SuspendLayout()
		{
			suspended++;
			base.SuspendLayout();
		}

		public override void ResumeLayout()
		{
			if (suspended > 0)
			{
				suspended--;
				UpdatePreferredSize();
			}
			base.ResumeLayout();
		}
		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			UpdatePreferredSize();
		}

		public override void UpdatePreferredSize()
		{
			Control.InvalidateMeasure();
			if (suspended == 0 && Widget.Loaded)
				base.UpdatePreferredSize();
		}
	}
}
