using System;
using Eto.Forms;
using sw = System.Windows;
using sd = System.Drawing;
using swm = System.Windows.Media;
using swf = System.Windows.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, Screen.IHandler
	{
		float realScale;

		public ScreenHandler(sw.Window window)
		{
			var source = sw.PresentationSource.FromVisual(window);
			Control = GetCurrentScreen(window);
			realScale = (float)source.CompositionTarget.TransformToDevice.M22;
		}

		public ScreenHandler(swf.Screen screen)
		{
			Control = screen;
			using (var form = new System.Windows.Forms.Form { Bounds = screen.Bounds })
			using (var graphics = form.CreateGraphics())
			{
				realScale = graphics.DpiY / 96f;
			}
		}

		static swf.Screen GetCurrentScreen(sw.Window window)
		{
			var centerPoint = new sd.Point((int)(window.Left + window.ActualWidth / 2), (int)(window.Top + window.ActualHeight / 2));
			foreach (var s in swf.Screen.AllScreens)
			{
				if (s.Bounds.Contains(centerPoint))
					return s;
			}
			return swf.Screen.PrimaryScreen;
		}

		public float RealScale
		{
			get { return realScale * Scale; }
		}

		public float Scale
		{
			get { return 96f / 72f; }
		}

		public RectangleF Bounds
		{
			get { return (RectangleF)Control.Bounds.ToEto() / realScale; }
		}

		public RectangleF WorkingArea
		{
			get { return (RectangleF)Control.WorkingArea.ToEto() / realScale; }
		}

		public int BitsPerPixel
		{
			get { return Control.BitsPerPixel; }
		}

		public bool IsPrimary
		{
			get { return Control.Primary; }
		}
	}
}